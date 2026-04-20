#include "InventoryEditor.h"
#include "MonoAPI.h"
#include "MinHook.h"
#include <deque>
#include <iostream>
#include <mutex>
#include <string>
#include <windows.h>

namespace InventoryEditorModule {
    struct Vector3 {
        float x;
        float y;
        float z;
    };

    typedef void(__fastcall* CharacterItems_Update_t)(void* _this);
    typedef bool(__fastcall* Character_get_IsLocal_t)(void* _this);

    CharacterItems_Update_t oCharacterItems_Update = nullptr;
    Character_get_IsLocal_t oCharacter_get_IsLocal = nullptr;

    MonoMethod* spawnItemInHandMethod = nullptr;
    MonoMethod* dropItemFromSlotMethod = nullptr;
    MonoClassField* characterField = nullptr;

    std::mutex localItemsMutex;
    void* localCharacterItems = nullptr;
    ULONGLONG localItemsLastSeenMs = 0;
    constexpr ULONGLONG kLocalItemsFreshMs = 1500;

    enum class ActionType {
        AddItem,
        RemoveSlot
    };

    struct PendingAction {
        ActionType type;
        std::string itemName;
        int slotIndex;
    };

    std::mutex actionMutex;
    std::deque<PendingAction> pendingActions;
    constexpr size_t kMaxQueuedActions = 32;

    void ProcessPendingActions(void* characterItems) {
        PendingAction action{};
        {
            std::lock_guard<std::mutex> lock(actionMutex);
            if (pendingActions.empty()) {
                return;
            }
            action = std::move(pendingActions.front());
            pendingActions.pop_front();
        }

        if (!MonoAPI::AttachCurrentThread()) {
            return;
        }

        if (action.type == ActionType::AddItem) {
            if (!spawnItemInHandMethod || action.itemName.empty()) {
                return;
            }

            MonoString* itemNameMono = MonoAPI::CreateString(action.itemName.c_str());
            if (!itemNameMono) {
                return;
            }

            void* args[1] = { itemNameMono };
            MonoObject* invokeExc = nullptr;
            MonoAPI::mono_runtime_invoke(spawnItemInHandMethod, characterItems, args, reinterpret_cast<void**>(&invokeExc));
            if (invokeExc) {
                std::cout << "[ENI-Inventory] Add item failed for '" << action.itemName
                          << "': " << MonoAPI::GetExceptionString(invokeExc) << std::endl;
            } else {
                std::cout << "[ENI-Inventory] Add item request queued for '" << action.itemName << "'." << std::endl;
            }
            return;
        }

        if (action.type == ActionType::RemoveSlot) {
            if (!dropItemFromSlotMethod || action.slotIndex < 0 || action.slotIndex > 3) {
                return;
            }

            unsigned char slot = static_cast<unsigned char>(action.slotIndex);
            Vector3 removePos{0.0f, -5000.0f, 0.0f};
            void* args[2] = { &slot, &removePos };
            MonoObject* invokeExc = nullptr;
            MonoAPI::mono_runtime_invoke(dropItemFromSlotMethod, characterItems, args, reinterpret_cast<void**>(&invokeExc));
            if (invokeExc) {
                std::cout << "[ENI-Inventory] Remove slot failed for slot " << action.slotIndex
                          << ": " << MonoAPI::GetExceptionString(invokeExc) << std::endl;
            }
        }
    }

    void __fastcall Hook_CharacterItems_Update(void* _this) {
        if (_this && characterField && MonoAPI::mono_field_get_value && oCharacter_get_IsLocal) {
            void* characterObj = nullptr;
            MonoAPI::mono_field_get_value(reinterpret_cast<MonoObject*>(_this), characterField, &characterObj);
            if (characterObj && oCharacter_get_IsLocal(characterObj)) {
                std::lock_guard<std::mutex> lock(localItemsMutex);
                localCharacterItems = _this;
                localItemsLastSeenMs = GetTickCount64();
                ProcessPendingActions(_this);
            }
        }

        oCharacterItems_Update(_this);
    }

    void* GetLocalCharacterItems() {
        std::lock_guard<std::mutex> lock(localItemsMutex);
        if (!localCharacterItems) {
            return nullptr;
        }

        const ULONGLONG now = GetTickCount64();
        if (now - localItemsLastSeenMs > kLocalItemsFreshMs) {
            localCharacterItems = nullptr;
            return nullptr;
        }

        return localCharacterItems;
    }
}

void InventoryEditor::Initialize() {
    std::cout << "[ENI-Inventory] Initializing Sandbox..." << std::endl;

    void* pCharacterItemsUpdate = MonoAPI::GetMethodAddress("CharacterItems", "Update", 0);
    void* pCharacterIsLocal = MonoAPI::GetMethodAddress("Character", "get_IsLocal", 0);

    if (!MonoAPI::mono_class_from_name || !MonoAPI::mono_class_get_method_from_name || !MonoAPI::mono_class_get_field_from_name) {
        std::cout << "[ENI-Inventory] Missing Mono metadata exports. Inventory editor disabled." << std::endl;
        return;
    }

    if (!pCharacterItemsUpdate || !pCharacterIsLocal) {
        std::cout << "[ENI-Inventory] Required methods missing. Inventory editor disabled." << std::endl;
        return;
    }

    MonoClass* characterItemsMetaClass = MonoAPI::GetClass("CharacterItems");
    if (!characterItemsMetaClass) {
        std::cout << "[ENI-Inventory] Failed to resolve CharacterItems/Character metadata." << std::endl;
        return;
    }

    InventoryEditorModule::spawnItemInHandMethod = MonoAPI::GetMethod("CharacterItems", "SpawnItemInHand", 1);
    InventoryEditorModule::dropItemFromSlotMethod = MonoAPI::GetMethod("CharacterItems", "DropItemFromSlotRPC", 2);
    InventoryEditorModule::characterField = MonoAPI::mono_class_get_field_from_name(characterItemsMetaClass, "character");

    if (!InventoryEditorModule::spawnItemInHandMethod || !InventoryEditorModule::dropItemFromSlotMethod || !InventoryEditorModule::characterField) {
        std::cout << "[ENI-Inventory] Failed resolving inventory methods/fields." << std::endl;
        return;
    }

    InventoryEditorModule::oCharacter_get_IsLocal = reinterpret_cast<InventoryEditorModule::Character_get_IsLocal_t>(pCharacterIsLocal);
    MH_CreateHook(pCharacterItemsUpdate, InventoryEditorModule::Hook_CharacterItems_Update, reinterpret_cast<void**>(&InventoryEditorModule::oCharacterItems_Update));
    MH_EnableHook(pCharacterItemsUpdate);

    std::cout << "[ENI-Inventory] -> System Secured." << std::endl;
}

bool InventoryEditor::IsReady() {
    return InventoryEditorModule::GetLocalCharacterItems() != nullptr &&
           InventoryEditorModule::spawnItemInHandMethod != nullptr &&
           InventoryEditorModule::dropItemFromSlotMethod != nullptr &&
           MonoAPI::mono_runtime_invoke != nullptr;
}

bool InventoryEditor::AddItemByName(const char* itemName) {
    if (!itemName || !itemName[0] || !IsReady()) {
        return false;
    }

    std::lock_guard<std::mutex> lock(InventoryEditorModule::actionMutex);
    if (InventoryEditorModule::pendingActions.size() >= InventoryEditorModule::kMaxQueuedActions) {
        InventoryEditorModule::pendingActions.pop_front();
    }
    InventoryEditorModule::pendingActions.push_back({
        InventoryEditorModule::ActionType::AddItem,
        std::string(itemName),
        -1
    });
    return true;
}

bool InventoryEditor::RemoveSlot(int slotIndex) {
    if (!IsReady() || slotIndex < 0 || slotIndex > 3) {
        return false;
    }

    std::lock_guard<std::mutex> lock(InventoryEditorModule::actionMutex);
    if (InventoryEditorModule::pendingActions.size() >= InventoryEditorModule::kMaxQueuedActions) {
        InventoryEditorModule::pendingActions.pop_front();
    }
    InventoryEditorModule::pendingActions.push_back({
        InventoryEditorModule::ActionType::RemoveSlot,
        "",
        slotIndex
    });
    return true;
}

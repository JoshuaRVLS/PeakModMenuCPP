#include "NetworkSafety.h"
#include "MonoAPI.h"
#include "MinHook.h"
#include "../UI/Menu.h"
#include <iostream>

namespace NetworkSafetyModule {
    typedef void(__fastcall* Player_RPCRemoveItemFromSlot_t)(void* _this, unsigned char slotID);
    Player_RPCRemoveItemFromSlot_t oPlayer_RPCRemoveItemFromSlot = nullptr;

    typedef void*(__fastcall* Player_get_character_t)(void* _this);
    Player_get_character_t oPlayer_get_character = nullptr;

    typedef bool(__fastcall* Character_get_IsLocal_t)(void* _this);
    Character_get_IsLocal_t oCharacter_get_IsLocal = nullptr;

    bool IsLocalPlayerInstance(void* playerInstance) {
        if (!playerInstance || !oPlayer_get_character || !oCharacter_get_IsLocal) {
            return false;
        }

        void* character = oPlayer_get_character(playerInstance);
        if (!character) {
            return false;
        }

        return oCharacter_get_IsLocal(character);
    }

    void __fastcall Hook_Player_RPCRemoveItemFromSlot(void* _this, unsigned char slotID) {
        if (!_this || !oPlayer_RPCRemoveItemFromSlot) {
            return;
        }

        // Only protect local inventory; never interfere with other Player objects.
        if (Menu::bAntiInventoryStrip && IsLocalPlayerInstance(_this)) {
            return;
        }
        oPlayer_RPCRemoveItemFromSlot(_this, slotID);
    }
}

void NetworkSafety::Initialize() {
    std::cout << "[ENI-NetworkSafety] Initializing Sandbox..." << std::endl;

    // NOTE: RPC_GetKicked(PhotonMessageInfo) is intentionally not hooked here.
    // Its complex value-type signature can be ABI-fragile and caused host loading crashes.
    std::cout << "[ENI-NetworkSafety] AntiKick hook disabled in stability mode." << std::endl;

    void* pRpcRemoveItem = MonoAPI::GetMethodAddress("Player", "RPCRemoveItemFromSlot", 1);
    if (pRpcRemoveItem) {
        MH_CreateHook(pRpcRemoveItem, NetworkSafetyModule::Hook_Player_RPCRemoveItemFromSlot, reinterpret_cast<void**>(&NetworkSafetyModule::oPlayer_RPCRemoveItemFromSlot));
        MH_EnableHook(pRpcRemoveItem);
    }

    void* pPlayerGetCharacter = MonoAPI::GetMethodAddress("Player", "get_character", 0);
    void* pCharacterGetIsLocal = MonoAPI::GetMethodAddress("Character", "get_IsLocal", 0);
    if (pPlayerGetCharacter && pCharacterGetIsLocal) {
        NetworkSafetyModule::oPlayer_get_character = reinterpret_cast<NetworkSafetyModule::Player_get_character_t>(pPlayerGetCharacter);
        NetworkSafetyModule::oCharacter_get_IsLocal = reinterpret_cast<NetworkSafetyModule::Character_get_IsLocal_t>(pCharacterGetIsLocal);
    } else {
        std::cout << "[ENI-NetworkSafety] Local-player resolver unavailable; safety hooks running in pass-through mode." << std::endl;
    }

    std::cout << "[ENI-NetworkSafety] -> System Secured." << std::endl;
}

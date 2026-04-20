#include "NoHunger.h"
#include "MonoAPI.h"
#include "MinHook.h"
#include "../UI/Menu.h"
#include <iostream>

namespace NoHungerModule {
    constexpr int kStatusHunger = 1;

    typedef bool(__fastcall* CharacterAfflictions_AddStatus_t)(void* _this, int statusType, float amount, bool fromRPC, bool playEffects, bool notify);
    CharacterAfflictions_AddStatus_t oCharacterAfflictions_AddStatus = nullptr;

    bool __fastcall Hook_CharacterAfflictions_AddStatus(void* _this, int statusType, float amount, bool fromRPC, bool playEffects, bool notify) {
        if (Menu::bNoHunger && statusType == kStatusHunger && amount > 0.0f) {
            return false;
        }
        return oCharacterAfflictions_AddStatus(_this, statusType, amount, fromRPC, playEffects, notify);
    }
}

void NoHunger::Initialize() {
    std::cout << "[ENI-NoHunger] Initializing Sandbox..." << std::endl;

    void* pAddStatus = MonoAPI::GetMethodAddress("CharacterAfflictions", "AddStatus", 5);
    if (pAddStatus) {
        MH_CreateHook(pAddStatus, NoHungerModule::Hook_CharacterAfflictions_AddStatus, reinterpret_cast<void**>(&NoHungerModule::oCharacterAfflictions_AddStatus));
        MH_EnableHook(pAddStatus);
        std::cout << "[ENI-NoHunger] -> System Secured." << std::endl;
    }
}

#include "GodMode.h"
#include "MonoAPI.h"
#include "MinHook.h"
#include "../UI/Menu.h"
#include <iostream>

namespace GodModeModule {
    constexpr int kStatusHunger = 1;
    constexpr int kStatusCold = 2;
    constexpr int kStatusPoison = 3;
    constexpr int kStatusCurse = 5;
    constexpr int kStatusDrowsy = 6;
    constexpr int kStatusHot = 8;

    typedef void(__fastcall* ActionDie_RunAction_t)(void* _this);
    ActionDie_RunAction_t oActionDie_RunAction = nullptr;

    typedef void(__fastcall* Character_DieInstantly_t)(void* _this);
    Character_DieInstantly_t oCharacter_DieInstantly = nullptr;

    typedef bool(__fastcall* CharacterAfflictions_AddStatus_t)(void* _this, int statusType, float amount, bool fromRPC, bool playEffects, bool notify);
    CharacterAfflictions_AddStatus_t oCharacterAfflictions_AddStatus = nullptr;

    bool __fastcall Hook_CharacterAfflictions_AddStatus(void* _this, int statusType, float amount, bool fromRPC, bool playEffects, bool notify) {
        if (amount > 0.0f) {
            if (Menu::bGodMode) {
                return false;
            }

            if ((Menu::bNoHunger && statusType == kStatusHunger) ||
                (Menu::bNoPoison && statusType == kStatusPoison) ||
                (Menu::bNoCold && statusType == kStatusCold) ||
                (Menu::bNoHot && statusType == kStatusHot) ||
                (Menu::bNoDrowsy && statusType == kStatusDrowsy) ||
                (Menu::bNoCurse && statusType == kStatusCurse)) {
                return false;
            }
        }

        return oCharacterAfflictions_AddStatus(_this, statusType, amount, fromRPC, playEffects, notify);
    }

    void __fastcall Hook_Character_DieInstantly(void* _this) {
        if (Menu::bGodMode) return;
        return oCharacter_DieInstantly(_this);
    }

    void __fastcall Hook_ActionDie_RunAction(void* _this) {
        if (Menu::bGodMode) return;
        return oActionDie_RunAction(_this);
    }
}

void GodMode::Initialize() {
    std::cout << "[ENI-GodMode] Initializing Sandbox..." << std::endl;

    void* pActionDie = MonoAPI::GetMethodAddress("Action_Die", "RunAction", 0);
    if (pActionDie) {
        MH_CreateHook(pActionDie, GodModeModule::Hook_ActionDie_RunAction, reinterpret_cast<void**>(&GodModeModule::oActionDie_RunAction));
        MH_EnableHook(pActionDie);
    }

    void* pCharDie = MonoAPI::GetMethodAddress("Character", "DieInstantly", 0);
    if (pCharDie) {
        MH_CreateHook(pCharDie, GodModeModule::Hook_Character_DieInstantly, reinterpret_cast<void**>(&GodModeModule::oCharacter_DieInstantly));
        MH_EnableHook(pCharDie);
    }

    void* pAddStatus = MonoAPI::GetMethodAddress("CharacterAfflictions", "AddStatus", 5);
    if (pAddStatus) {
        MH_CreateHook(pAddStatus, GodModeModule::Hook_CharacterAfflictions_AddStatus, reinterpret_cast<void**>(&GodModeModule::oCharacterAfflictions_AddStatus));
        MH_EnableHook(pAddStatus);
        std::cout << "[ENI-GodMode] -> System Secured." << std::endl;
    }
}

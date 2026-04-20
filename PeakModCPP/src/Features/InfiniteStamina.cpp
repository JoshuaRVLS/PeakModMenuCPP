#include "InfiniteStamina.h"
#include "MonoAPI.h"
#include "MinHook.h"
#include "../UI/Menu.h"
#include <iostream>

namespace StaminaModule {
    typedef void(__fastcall* Character_UseStamina_t)(void* _this, float usage, bool useBonusStamina);
    Character_UseStamina_t oCharacter_UseStamina = nullptr;

    void __fastcall Hook_Character_UseStamina(void* _this, float usage, bool useBonusStamina) {
        if (Menu::bInfiniteStamina) {
            return oCharacter_UseStamina(_this, 0.0f, useBonusStamina);
        }
        return oCharacter_UseStamina(_this, usage, useBonusStamina);
    }
}

void InfiniteStamina::Initialize() {
    std::cout << "[ENI-Stamina] Initializing Sandbox..." << std::endl;

    void* pUseStamina = MonoAPI::GetMethodAddress("Character", "UseStamina", 2);
    if (pUseStamina) {
        MH_CreateHook(pUseStamina, StaminaModule::Hook_Character_UseStamina, reinterpret_cast<void**>(&StaminaModule::oCharacter_UseStamina));
        MH_EnableHook(pUseStamina);
        std::cout << "[ENI-Stamina] -> System Secured." << std::endl;
    }
}

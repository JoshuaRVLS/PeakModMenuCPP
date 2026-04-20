#include "NoFallDamage.h"
#include "MonoAPI.h"
#include "MinHook.h"
#include "../UI/Menu.h"
#include <iostream>

namespace NoFallDamageModule {
    typedef void(__fastcall* Character_Fall_t)(void* _this, float seconds, float screenShake);
    Character_Fall_t oCharacter_Fall = nullptr;

    void __fastcall Hook_Character_Fall(void* _this, float seconds, float screenShake) {
        if (Menu::bNoFallDamage) {
            return oCharacter_Fall(_this, 0.0f, 0.0f);
        }
        return oCharacter_Fall(_this, seconds, screenShake);
    }
}

void NoFallDamage::Initialize() {
    std::cout << "[ENI-NoFallDamage] Initializing Sandbox..." << std::endl;

    void* pFall = MonoAPI::GetMethodAddress("Character", "Fall", 2);
    if (pFall) {
        MH_CreateHook(pFall, NoFallDamageModule::Hook_Character_Fall, reinterpret_cast<void**>(&NoFallDamageModule::oCharacter_Fall));
        MH_EnableHook(pFall);
        std::cout << "[ENI-NoFallDamage] -> System Secured." << std::endl;
    }
}

#include "NoAnimationCooldown.h"
#include "MonoAPI.h"
#include "MinHook.h"
#include "../UI/Menu.h"
#include <iostream>

namespace NoAnimationCooldownModule {
    typedef void(__fastcall* CharacterAnimations_PlaySpecificAnimation_t)(void* _this, void* animationName);
    CharacterAnimations_PlaySpecificAnimation_t oCharacterAnimations_PlaySpecificAnimation = nullptr;

    typedef void(__fastcall* Item_ContinueUsePrimary_t)(void* _this);
    Item_ContinueUsePrimary_t oItem_ContinueUsePrimary = nullptr;

    typedef void(__fastcall* Item_ContinueUseSecondary_t)(void* _this);
    Item_ContinueUseSecondary_t oItem_ContinueUseSecondary = nullptr;

    typedef void(__fastcall* Item_FinishCastPrimary_t)(void* _this);
    Item_FinishCastPrimary_t oItem_FinishCastPrimary = nullptr;

    typedef void(__fastcall* Item_FinishCastSecondary_t)(void* _this);
    Item_FinishCastSecondary_t oItem_FinishCastSecondary = nullptr;

    void __fastcall Hook_CharacterAnimations_PlaySpecificAnimation(void* _this, void* animationName) {
        if (Menu::bNoAnimation) {
            return;
        }
        oCharacterAnimations_PlaySpecificAnimation(_this, animationName);
    }

    void __fastcall Hook_Item_ContinueUsePrimary(void* _this) {
        oItem_ContinueUsePrimary(_this);
        if (Menu::bNoCooldown && oItem_FinishCastPrimary) {
            oItem_FinishCastPrimary(_this);
        }
    }

    void __fastcall Hook_Item_ContinueUseSecondary(void* _this) {
        oItem_ContinueUseSecondary(_this);
        if (Menu::bNoCooldown && oItem_FinishCastSecondary) {
            oItem_FinishCastSecondary(_this);
        }
    }
}

void NoAnimationCooldown::Initialize() {
    std::cout << "[ENI-NoAnimCooldown] Initializing Sandbox..." << std::endl;

    void* pPlaySpecificAnimation = MonoAPI::GetMethodAddress("CharacterAnimations", "PlaySpecificAnimation", 1);
    if (pPlaySpecificAnimation) {
        MH_CreateHook(
            pPlaySpecificAnimation,
            NoAnimationCooldownModule::Hook_CharacterAnimations_PlaySpecificAnimation,
            reinterpret_cast<void**>(&NoAnimationCooldownModule::oCharacterAnimations_PlaySpecificAnimation));
        MH_EnableHook(pPlaySpecificAnimation);
    }

    void* pContinueUsePrimary = MonoAPI::GetMethodAddress("Item", "ContinueUsePrimary", 0);
    if (pContinueUsePrimary) {
        MH_CreateHook(
            pContinueUsePrimary,
            NoAnimationCooldownModule::Hook_Item_ContinueUsePrimary,
            reinterpret_cast<void**>(&NoAnimationCooldownModule::oItem_ContinueUsePrimary));
        MH_EnableHook(pContinueUsePrimary);
    }

    void* pContinueUseSecondary = MonoAPI::GetMethodAddress("Item", "ContinueUseSecondary", 0);
    if (pContinueUseSecondary) {
        MH_CreateHook(
            pContinueUseSecondary,
            NoAnimationCooldownModule::Hook_Item_ContinueUseSecondary,
            reinterpret_cast<void**>(&NoAnimationCooldownModule::oItem_ContinueUseSecondary));
        MH_EnableHook(pContinueUseSecondary);
    }

    void* pFinishCastPrimary = MonoAPI::GetMethodAddress("Item", "FinishCastPrimary", 0);
    void* pFinishCastSecondary = MonoAPI::GetMethodAddress("Item", "FinishCastSecondary", 0);
    NoAnimationCooldownModule::oItem_FinishCastPrimary = reinterpret_cast<NoAnimationCooldownModule::Item_FinishCastPrimary_t>(pFinishCastPrimary);
    NoAnimationCooldownModule::oItem_FinishCastSecondary = reinterpret_cast<NoAnimationCooldownModule::Item_FinishCastSecondary_t>(pFinishCastSecondary);

    std::cout << "[ENI-NoAnimCooldown] -> System Secured." << std::endl;
}

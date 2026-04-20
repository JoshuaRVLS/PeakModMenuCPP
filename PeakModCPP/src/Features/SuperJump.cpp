#include "SuperJump.h"
#include "MonoAPI.h"
#include "MinHook.h"
#include "../UI/Menu.h"
#include <iostream>

namespace SuperJumpModule {
    typedef void(__fastcall* CharacterMovement_JumpRpc_t)(void* _this, bool isPalJump);
    CharacterMovement_JumpRpc_t oCharacterMovement_JumpRpc = nullptr;

    void __fastcall Hook_CharacterMovement_JumpRpc(void* _this, bool isPalJump) {
        if (Menu::bSuperJump) {
            return oCharacterMovement_JumpRpc(_this, true);
        }
        return oCharacterMovement_JumpRpc(_this, isPalJump);
    }
}

void SuperJump::Initialize() {
    std::cout << "[ENI-SuperJump] Initializing Sandbox..." << std::endl;

    void* pJumpRpc = MonoAPI::GetMethodAddress("CharacterMovement", "JumpRpc", 1);
    if (pJumpRpc) {
        MH_CreateHook(pJumpRpc, SuperJumpModule::Hook_CharacterMovement_JumpRpc, reinterpret_cast<void**>(&SuperJumpModule::oCharacterMovement_JumpRpc));
        MH_EnableHook(pJumpRpc);
        std::cout << "[ENI-SuperJump] -> System Secured." << std::endl;
    }
}

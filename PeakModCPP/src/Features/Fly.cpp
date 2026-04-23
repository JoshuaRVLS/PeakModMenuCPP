#include "Fly.h"
#include "MonoAPI.h"
#include "MinHook.h"
#include "../UI/Menu.h"
#include <windows.h>
#include <iostream>

namespace FlyModule {
    struct Vector3 {
        float x;
        float y;
        float z;
    };

    typedef Vector3(__fastcall* CharacterMovement_GetGravityForce_t)(void* _this);
    CharacterMovement_GetGravityForce_t oCharacterMovement_GetGravityForce = nullptr;

    Vector3 __fastcall Hook_CharacterMovement_GetGravityForce(void* _this) {
        if (!oCharacterMovement_GetGravityForce) {
            return { 0.0f, 0.0f, 0.0f };
        }

        if (!Menu::bFly) {
            return oCharacterMovement_GetGravityForce(_this);
        }

        Vector3 gravity = oCharacterMovement_GetGravityForce(_this);
        gravity.x = 0.0f;
        gravity.z = 0.0f;
        gravity.y = 0.0f;

        const bool up = (GetAsyncKeyState(VK_SPACE) & 0x8000) != 0;
        const bool down = (GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0;
        constexpr float flyForce = 35.0f;

        if (up && !down) {
            gravity.y = flyForce;
        } else if (down && !up) {
            gravity.y = -flyForce;
        }

        return gravity;
    }
}

void Fly::Initialize() {
    std::cout << "[ENI-Fly] Initializing Sandbox..." << std::endl;

    void* pGetGravityForce = MonoAPI::GetMethodAddress("CharacterMovement", "GetGravityForce", 0);
    if (!pGetGravityForce) {
        std::cout << "[ENI-Fly] Failed to resolve CharacterMovement::GetGravityForce." << std::endl;
        return;
    }

    MH_CreateHook(
        pGetGravityForce,
        FlyModule::Hook_CharacterMovement_GetGravityForce,
        reinterpret_cast<void**>(&FlyModule::oCharacterMovement_GetGravityForce));
    MH_EnableHook(pGetGravityForce);

    std::cout << "[ENI-Fly] -> System Secured." << std::endl;
}

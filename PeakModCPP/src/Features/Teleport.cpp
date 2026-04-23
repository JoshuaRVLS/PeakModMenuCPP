#include "Teleport.h"
#include "MonoAPI.h"
#include "MinHook.h"
#include "../UI/Menu.h"
#include <windows.h>
#include <iostream>

namespace TeleportModule {
    typedef void(__fastcall* Character_Update_t)(void* _this);
    typedef bool(__fastcall* Character_get_IsLocal_t)(void* _this);
    typedef void(__fastcall* Character_WarpToSpawn_t)();

    Character_Update_t oCharacter_Update = nullptr;
    Character_get_IsLocal_t oCharacter_get_IsLocal = nullptr;
    Character_WarpToSpawn_t oCharacter_WarpToSpawn = nullptr;

    bool prevF7 = false;
    ULONGLONG lastTeleportMs = 0;
    constexpr ULONGLONG kTeleportCooldownMs = 200;

    void TryHandleTeleportHotkeys(void* character) {
        if (!Menu::bTeleport || !character) {
            return;
        }

        const bool f7Down = (GetAsyncKeyState(VK_F7) & 0x8000) != 0;
        const bool f7Pressed = f7Down && !prevF7;

        prevF7 = f7Down;

        const ULONGLONG now = GetTickCount64();
        if (now - lastTeleportMs < kTeleportCooldownMs) {
            return;
        }

        if (f7Pressed) {
            if (oCharacter_WarpToSpawn) {
                oCharacter_WarpToSpawn();
                std::cout << "[ENI-Teleport] WarpToSpawn requested." << std::endl;
            }
            lastTeleportMs = now;
        }
    }

    void __fastcall Hook_Character_Update(void* _this) {
        if (_this && oCharacter_get_IsLocal) {
            if (Menu::bTeleport) {
                if (oCharacter_get_IsLocal(_this)) {
                    TryHandleTeleportHotkeys(_this);
                }
            }
        }
        oCharacter_Update(_this);
    }
}

void Teleport::Initialize() {
    std::cout << "[ENI-Teleport] Initializing Sandbox..." << std::endl;

    void* pUpdate = MonoAPI::GetMethodAddress("Character", "Update", 0);
    void* pGetIsLocal = MonoAPI::GetMethodAddress("Character", "get_IsLocal", 0);
    void* pWarpToSpawn = MonoAPI::GetMethodAddress("Character", "WarpToSpawn", 0);

    if (!pUpdate || !pGetIsLocal || !pWarpToSpawn) {
        std::cout << "[ENI-Teleport] Missing Character methods. Teleport disabled." << std::endl;
        return;
    }

    TeleportModule::oCharacter_get_IsLocal = reinterpret_cast<TeleportModule::Character_get_IsLocal_t>(pGetIsLocal);
    TeleportModule::oCharacter_WarpToSpawn = reinterpret_cast<TeleportModule::Character_WarpToSpawn_t>(pWarpToSpawn);

    MH_CreateHook(pUpdate, TeleportModule::Hook_Character_Update, reinterpret_cast<void**>(&TeleportModule::oCharacter_Update));
    MH_EnableHook(pUpdate);
    std::cout << "[ENI-Teleport] -> System Secured. (F7 spawn)" << std::endl;
}

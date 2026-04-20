#include "CursorBypass.h"
#include "MonoAPI.h"
#include "MinHook.h"
#include "../UI/Menu.h"
#include <iostream>

namespace CursorModule {
    typedef void(__fastcall* CursorHandler_Update_t)(void* _this);
    CursorHandler_Update_t oCursorHandler_Update = nullptr;

    void __fastcall Hook_CursorHandler_Update(void* _this) {
        if (Menu::bShowMenu) {
            MonoAPI::ApplyCursorState(true, 0);
            return;
        }
        return oCursorHandler_Update(_this);
    }
}

void CursorBypass::Initialize() {
    std::cout << "[ENI-Cursor] Initializing Sandbox..." << std::endl;

    void* pCursorUpdate = MonoAPI::GetMethodAddress("CursorHandler", "Update", 0);
    if (pCursorUpdate) {
        MH_CreateHook(pCursorUpdate, CursorModule::Hook_CursorHandler_Update, reinterpret_cast<void**>(&CursorModule::oCursorHandler_Update));
        MH_EnableHook(pCursorUpdate);
        std::cout << "[ENI-Cursor] -> System Secured." << std::endl;
    }
}

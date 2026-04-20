#include <windows.h>
#include <iostream>
#include "Hooks/Hooks.h"
#include "Features/FeatureManager.h"
#include "Features/GodMode.h"
#include "Features/InfiniteStamina.h"
#include "Features/CursorBypass.h"
#include "Features/MonoAPI.h"

DWORD WINAPI MainThread(LPVOID lpParam) {
    AllocConsole();
    FILE* f;
    freopen_s(&f, "CONOUT$", "w", stdout);

    std::cout << "[ENI] PEAK+ Master DLL Injected." << std::endl;
    
    // Initialize Hooking Backend
    Hooks::Init();

    // Initialize Mono C# Engine Hook
    if (MonoAPI::Initialize()) {
        std::cout << "[ENI-Core] Assembling Modular Sandboxes..." << std::endl;
        FeatureManager& manager = FeatureManager::GetInstance();
        
        manager.RegisterFeature(new GodMode());
        manager.RegisterFeature(new InfiniteStamina());
        manager.RegisterFeature(new CursorBypass());

        manager.InitializeAll();
    } else {
        std::cout << "[ENI-Core] Mono Injection Failed! C# hooks disabled." << std::endl;
    }

    // Main cheat thread loop
    while (!GetAsyncKeyState(VK_END)) {
        Sleep(10);
    }

    std::cout << "[ENI] Ejecting gracefully..." << std::endl;
    Hooks::Shutdown();

    fclose(f);
    FreeConsole();
    FreeLibraryAndExitThread((HMODULE)lpParam, 0);
    return 0;
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved) {
    if (ul_reason_for_call == DLL_PROCESS_ATTACH) {
        DisableThreadLibraryCalls(hModule);
        CreateThread(0, 0, MainThread, hModule, 0, 0);
    }
    return TRUE;
}

#include "Hooks.h"
#include "UI/Menu.h"
#include <d3d11.h>
#include <dxgi.h>
#include "MinHook.h"
#include <imgui.h>
#include <imgui_impl_win32.h>
#include <imgui_impl_dx11.h>
#include <Windows.h>
#include <iostream>
#include "Features/MonoAPI.h"

extern IMGUI_IMPL_API LRESULT ImGui_ImplWin32_WndProcHandler(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam);

namespace Hooks {
    bool init = false;
    WNDPROC oWndProc = NULL;
    HWND window = NULL;
    ID3D11Device* pDevice = nullptr;
    ID3D11DeviceContext* pContext = nullptr;
    ID3D11RenderTargetView* mainRenderTargetView = nullptr;

    typedef HRESULT(__stdcall* Present_t)(IDXGISwapChain* pSwapChain, UINT SyncInterval, UINT Flags);
    Present_t oPresent = nullptr;

    LRESULT __stdcall WndProc(const HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam) {
        if (uMsg == WM_KEYDOWN && wParam == VK_INSERT) {
            Menu::bShowMenu = !Menu::bShowMenu;
            if (Menu::bShowMenu) {
                MonoAPI::ApplyCursorState(true, 0); // 0 = CursorLockMode.None
            } else {
                MonoAPI::ApplyCursorState(false, 1); // 1 = CursorLockMode.Locked
            }
            return 1;
        }

        if (Menu::bShowMenu) {
            if (ImGui_ImplWin32_WndProcHandler(hWnd, uMsg, wParam, lParam))
                return true;
            
            // Block all mouse/camera inputs to the game while menu is open
            if (uMsg >= WM_MOUSEFIRST && uMsg <= WM_MOUSELAST) return true;
            if (uMsg == WM_INPUT) return true;
            // Let all other messages (keyboard, paint) fall through to the game
        }

        return CallWindowProc(oWndProc, hWnd, uMsg, wParam, lParam);
    }

    HRESULT __stdcall hookD3D11Present(IDXGISwapChain* pSwapChain, UINT SyncInterval, UINT Flags) {
        if (!init) {
            if (SUCCEEDED(pSwapChain->GetDevice(__uuidof(ID3D11Device), (void**)&pDevice))) {
                pDevice->GetImmediateContext(&pContext);
                DXGI_SWAP_CHAIN_DESC sd;
                pSwapChain->GetDesc(&sd);
                window = sd.OutputWindow;
                ID3D11Texture2D* pBackBuffer;
                pSwapChain->GetBuffer(0, __uuidof(ID3D11Texture2D), (LPVOID*)&pBackBuffer);
                pDevice->CreateRenderTargetView(pBackBuffer, NULL, &mainRenderTargetView);
                pBackBuffer->Release();
                oWndProc = (WNDPROC)SetWindowLongPtr(window, GWLP_WNDPROC, (LONG_PTR)WndProc);
                
                ImGui::CreateContext();
                ImGuiIO& io = ImGui::GetIO();
                io.ConfigFlags = ImGuiConfigFlags_NoMouseCursorChange;
                ImGui_ImplWin32_Init(window);
                ImGui_ImplDX11_Init(pDevice, pContext);
                init = true;
            } else {
                return oPresent(pSwapChain, SyncInterval, Flags);
            }
        }

        if (Menu::bShowMenu) {
            ImGui::GetIO().MouseDrawCursor = true;
            ImGui_ImplDX11_NewFrame();
            ImGui_ImplWin32_NewFrame();
            ImGui::NewFrame();
            
            // Draw the menu using the UI logic
            Menu::Render();

            ImGui::Render();
            pContext->OMSetRenderTargets(1, &mainRenderTargetView, NULL);
            ImGui_ImplDX11_RenderDrawData(ImGui::GetDrawData());
        } else {
            ImGui::GetIO().MouseDrawCursor = false;
        }

        return oPresent(pSwapChain, SyncInterval, Flags);
    }

    void Init() {
        std::cout << "[ENI] Abandoning Kiero. Executing raw D3D11 SwapChain Interception..." << std::endl;
        
        // Wait just a moment to let the game boot its window peacefully before we attack
        Sleep(2000); 

        WNDCLASSEXA wc{sizeof(WNDCLASSEXA), CS_CLASSDC, DefWindowProc, 0, 0, GetModuleHandle(nullptr), nullptr, nullptr, nullptr, nullptr, "DummyClass", nullptr};
        RegisterClassExA(&wc);
        HWND hWnd = CreateWindowA("DummyClass", "DummyWindow", WS_OVERLAPPEDWINDOW, 0, 0, 100, 100, nullptr, nullptr, wc.hInstance, nullptr);

        D3D_FEATURE_LEVEL featureLevel;
        const D3D_FEATURE_LEVEL featureLevels[] = { D3D_FEATURE_LEVEL_11_0, D3D_FEATURE_LEVEL_10_1, D3D_FEATURE_LEVEL_10_0 };
        DXGI_SWAP_CHAIN_DESC sd;
        ZeroMemory(&sd, sizeof(sd));
        sd.BufferCount = 1;
        sd.BufferDesc.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
        sd.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
        sd.OutputWindow = hWnd;
        sd.SampleDesc.Count = 1;
        sd.Windowed = TRUE;
        sd.SwapEffect = DXGI_SWAP_EFFECT_DISCARD;

        IDXGISwapChain* dummySwapChain = nullptr;
        ID3D11Device* dummyDevice = nullptr;
        ID3D11DeviceContext* dummyContext = nullptr;

        HRESULT hr = D3D11CreateDeviceAndSwapChain(nullptr, D3D_DRIVER_TYPE_HARDWARE, nullptr, 0, featureLevels, 3, D3D11_SDK_VERSION, &sd, &dummySwapChain, &dummyDevice, &featureLevel, &dummyContext);
        
        if (FAILED(hr)) {
            std::cout << "[ENI] Raw DX11 dummy creation failed. The engine might be completely blocking D3D11." << std::endl;
            DestroyWindow(hWnd);
            UnregisterClassA(wc.lpszClassName, wc.hInstance);
            return;
        }

        void** pVTable = *reinterpret_cast<void***>(dummySwapChain);
        void* presentAddress = pVTable[8];

        std::cout << "[ENI] Present Address located perfectly at: " << presentAddress << std::endl;

        MH_Initialize();
        MH_CreateHook(presentAddress, hookD3D11Present, reinterpret_cast<void**>(&oPresent));
        MH_EnableHook(MH_ALL_HOOKS);

        std::cout << "[ENI] Raw DX11 Hook succeeded. Menu active! (Press INSERT)" << std::endl;

        dummySwapChain->Release();
        dummyContext->Release();
        dummyDevice->Release();
        DestroyWindow(hWnd);
        UnregisterClassA(wc.lpszClassName, wc.hInstance);
    }

    void Shutdown() {
        if (init) {
            ImGui_ImplDX11_Shutdown();
            ImGui_ImplWin32_Shutdown();
            ImGui::DestroyContext();
        }
        if (oWndProc && window) {
            SetWindowLongPtr(window, GWLP_WNDPROC, (LONG_PTR)oWndProc);
        }
        MH_DisableHook(MH_ALL_HOOKS);
        MH_Uninitialize();
    }
}

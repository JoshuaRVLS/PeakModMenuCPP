#include "Hooks.h"
#include "UI/Menu.h"
#include "UI/Theme.h"
#include "Features/MonoAPI.h"
#include "Features/PlayerESP.h"
#include "MinHook.h"
#include <Windows.h>
#include <d3d11.h>
#include <d3d12.h>
#include <dxgi.h>
#include <dxgi1_4.h>
#include <imgui.h>
#include <imgui_impl_win32.h>
#include <imgui_impl_dx11.h>
#include <imgui_impl_dx12.h>
#include <iostream>

extern IMGUI_IMPL_API LRESULT ImGui_ImplWin32_WndProcHandler(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam);

namespace Hooks {
    enum class RenderBackend {
        None,
        DX11,
        DX12
    };

    template <typename T>
    static void SafeRelease(T*& ptr) {
        if (ptr) {
            ptr->Release();
            ptr = nullptr;
        }
    }

    struct DX12FrameContext {
        ID3D12CommandAllocator* CommandAllocator = nullptr;
        ID3D12Resource* RenderTarget = nullptr;
        D3D12_CPU_DESCRIPTOR_HANDLE RtvHandle{};
        UINT64 FenceValue = 0;
    };

    static bool init = false;
    static bool imguiInit = false;
    static RenderBackend backend = RenderBackend::None;
    static WNDPROC oWndProc = nullptr;
    static HWND window = nullptr;

    // Shared present hook
    typedef HRESULT(__stdcall* Present_t)(IDXGISwapChain* pSwapChain, UINT SyncInterval, UINT Flags);
    static Present_t oPresent = nullptr;
    typedef HRESULT(__stdcall* Present1_t)(IDXGISwapChain1* pSwapChain, UINT SyncInterval, UINT PresentFlags, const DXGI_PRESENT_PARAMETERS* pPresentParameters);
    static Present1_t oPresent1 = nullptr;

    // DX11 state
    static ID3D11Device* d3d11Device = nullptr;
    static ID3D11DeviceContext* d3d11Context = nullptr;
    static ID3D11RenderTargetView* d3d11Rtv = nullptr;

    // DX12 state
    typedef void(__stdcall* ExecuteCommandLists_t)(ID3D12CommandQueue* pQueue, UINT NumCommandLists, ID3D12CommandList* const* ppCommandLists);
    static ExecuteCommandLists_t oExecuteCommandLists = nullptr;

    static ID3D12Device* d3d12Device = nullptr;
    static ID3D12CommandQueue* d3d12Queue = nullptr;
    static ID3D12GraphicsCommandList* d3d12CommandList = nullptr;
    static ID3D12DescriptorHeap* d3d12RtvHeap = nullptr;
    static ID3D12DescriptorHeap* d3d12SrvHeap = nullptr;
    static ID3D12Fence* d3d12Fence = nullptr;
    static HANDLE d3d12FenceEvent = nullptr;
    static DX12FrameContext* d3d12Frames = nullptr;
    static UINT d3d12FrameCount = 0;
    static UINT d3d12RtvDescriptorSize = 0;
    static UINT64 d3d12FenceLastSignaledValue = 0;
    static bool dx12WaitingForQueueLogged = false;
    static bool presentCallbackSeenLogged = false;
    static bool dx12IgnoredNonDirectQueueLogged = false;

    static void WaitForFenceValue(UINT64 fenceValue) {
        if (!d3d12Fence || d3d12Fence->GetCompletedValue() >= fenceValue) {
            return;
        }

        d3d12Fence->SetEventOnCompletion(fenceValue, d3d12FenceEvent);
        WaitForSingleObject(d3d12FenceEvent, INFINITE);
    }

    static void SignalFrameFence(DX12FrameContext& frame) {
        if (!d3d12Queue || !d3d12Fence) {
            return;
        }

        d3d12FenceLastSignaledValue++;
        d3d12Queue->Signal(d3d12Fence, d3d12FenceLastSignaledValue);
        frame.FenceValue = d3d12FenceLastSignaledValue;
    }

    static LRESULT __stdcall WndProc(const HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam) {
        if (uMsg == WM_KEYDOWN && wParam == VK_INSERT) {
            Menu::bShowMenu = !Menu::bShowMenu;
            if (Menu::bShowMenu) {
                MonoAPI::ApplyCursorState(true, 0); // CursorLockMode.None
            } else {
                MonoAPI::ApplyCursorState(false, 1); // CursorLockMode.Locked
            }
            return 1;
        }

        if (Menu::bShowMenu) {
            if (ImGui_ImplWin32_WndProcHandler(hWnd, uMsg, wParam, lParam)) {
                return true;
            }

            // Block all mouse/camera inputs to the game while menu is open
            if (uMsg >= WM_MOUSEFIRST && uMsg <= WM_MOUSELAST) return true;
            if (uMsg == WM_INPUT) return true;
        }

        return CallWindowProc(oWndProc, hWnd, uMsg, wParam, lParam);
    }

    static bool InitImGuiBase(HWND targetWindow) {
        if (imguiInit) {
            return true;
        }

        if (!ImGui::GetCurrentContext()) {
            ImGui::CreateContext();
        }
        ImGuiIO& io = ImGui::GetIO();
        io.ConfigFlags = ImGuiConfigFlags_NoMouseCursorChange;

        Theme::Apply();

        if (!ImGui_ImplWin32_Init(targetWindow)) {
            return false;
        }

        imguiInit = true;
        return true;
    }

    static bool InitDX11(IDXGISwapChain* pSwapChain) {
        if (backend == RenderBackend::DX11) {
            return true;
        }

        if (FAILED(pSwapChain->GetDevice(__uuidof(ID3D11Device), reinterpret_cast<void**>(&d3d11Device)))) {
            return false;
        }

        d3d11Device->GetImmediateContext(&d3d11Context);

        DXGI_SWAP_CHAIN_DESC sd{};
        pSwapChain->GetDesc(&sd);
        window = sd.OutputWindow;
        oWndProc = reinterpret_cast<WNDPROC>(SetWindowLongPtr(window, GWLP_WNDPROC, reinterpret_cast<LONG_PTR>(WndProc)));

        ID3D11Texture2D* backBuffer = nullptr;
        if (FAILED(pSwapChain->GetBuffer(0, __uuidof(ID3D11Texture2D), reinterpret_cast<void**>(&backBuffer)))) {
            return false;
        }
        d3d11Device->CreateRenderTargetView(backBuffer, nullptr, &d3d11Rtv);
        backBuffer->Release();

        if (!InitImGuiBase(window)) {
            return false;
        }
        if (!ImGui_ImplDX11_Init(d3d11Device, d3d11Context)) {
            return false;
        }

        backend = RenderBackend::DX11;
        std::cout << "[ENI] Renderer backend selected: DirectX 11" << std::endl;
        return true;
    }

    static bool InitDX12(IDXGISwapChain* pSwapChain) {
        if (backend == RenderBackend::DX12) {
            return true;
        }

        if (!d3d12Queue) {
            // Queue will be captured via ExecuteCommandLists hook.
            if (!dx12WaitingForQueueLogged) {
                std::cout << "[ENI] DX12 swapchain detected. Waiting for command queue capture..." << std::endl;
                dx12WaitingForQueueLogged = true;
            }
            return false;
        }

        D3D12_COMMAND_QUEUE_DESC capturedQueueDesc = d3d12Queue->GetDesc();
        if (capturedQueueDesc.Type != D3D12_COMMAND_LIST_TYPE_DIRECT) {
            std::cout << "[ENI] Captured DX12 queue is not DIRECT. Waiting for DIRECT queue..." << std::endl;
            return false;
        }

        if (FAILED(pSwapChain->GetDevice(__uuidof(ID3D12Device), reinterpret_cast<void**>(&d3d12Device)))) {
            std::cout << "[ENI] DX12 init failed: swapchain GetDevice(ID3D12Device) failed." << std::endl;
            return false;
        }

        DXGI_SWAP_CHAIN_DESC sd{};
        pSwapChain->GetDesc(&sd);
        window = sd.OutputWindow;
        d3d12FrameCount = sd.BufferCount > 0 ? sd.BufferCount : 2;
        oWndProc = reinterpret_cast<WNDPROC>(SetWindowLongPtr(window, GWLP_WNDPROC, reinterpret_cast<LONG_PTR>(WndProc)));

        d3d12Frames = new DX12FrameContext[d3d12FrameCount];

        D3D12_DESCRIPTOR_HEAP_DESC rtvDesc{};
        rtvDesc.Type = D3D12_DESCRIPTOR_HEAP_TYPE_RTV;
        rtvDesc.NumDescriptors = d3d12FrameCount;
        rtvDesc.Flags = D3D12_DESCRIPTOR_HEAP_FLAG_NONE;
        if (FAILED(d3d12Device->CreateDescriptorHeap(&rtvDesc, IID_PPV_ARGS(&d3d12RtvHeap)))) {
            std::cout << "[ENI] DX12 init failed: CreateDescriptorHeap RTV failed." << std::endl;
            return false;
        }
        d3d12RtvDescriptorSize = d3d12Device->GetDescriptorHandleIncrementSize(D3D12_DESCRIPTOR_HEAP_TYPE_RTV);

        D3D12_DESCRIPTOR_HEAP_DESC srvDesc{};
        srvDesc.Type = D3D12_DESCRIPTOR_HEAP_TYPE_CBV_SRV_UAV;
        srvDesc.NumDescriptors = 1;
        srvDesc.Flags = D3D12_DESCRIPTOR_HEAP_FLAG_SHADER_VISIBLE;
        if (FAILED(d3d12Device->CreateDescriptorHeap(&srvDesc, IID_PPV_ARGS(&d3d12SrvHeap)))) {
            std::cout << "[ENI] DX12 init failed: CreateDescriptorHeap SRV failed." << std::endl;
            return false;
        }

        D3D12_CPU_DESCRIPTOR_HANDLE rtvHandle = d3d12RtvHeap->GetCPUDescriptorHandleForHeapStart();
        for (UINT i = 0; i < d3d12FrameCount; ++i) {
            d3d12Frames[i].RtvHandle = rtvHandle;
            pSwapChain->GetBuffer(i, IID_PPV_ARGS(&d3d12Frames[i].RenderTarget));
            if (!d3d12Frames[i].RenderTarget) {
                std::cout << "[ENI] DX12 init failed: swapchain GetBuffer returned null RT at index " << i << std::endl;
                return false;
            }
            d3d12Device->CreateRenderTargetView(d3d12Frames[i].RenderTarget, nullptr, d3d12Frames[i].RtvHandle);

            if (FAILED(d3d12Device->CreateCommandAllocator(D3D12_COMMAND_LIST_TYPE_DIRECT, IID_PPV_ARGS(&d3d12Frames[i].CommandAllocator)))) {
                std::cout << "[ENI] DX12 init failed: CreateCommandAllocator failed." << std::endl;
                return false;
            }

            rtvHandle.ptr += d3d12RtvDescriptorSize;
        }

        if (FAILED(d3d12Device->CreateCommandList(0, D3D12_COMMAND_LIST_TYPE_DIRECT, d3d12Frames[0].CommandAllocator, nullptr, IID_PPV_ARGS(&d3d12CommandList)))) {
            std::cout << "[ENI] DX12 init failed: CreateCommandList failed." << std::endl;
            return false;
        }
        d3d12CommandList->Close();

        if (FAILED(d3d12Device->CreateFence(0, D3D12_FENCE_FLAG_NONE, IID_PPV_ARGS(&d3d12Fence)))) {
            std::cout << "[ENI] DX12 init failed: CreateFence failed." << std::endl;
            return false;
        }
        d3d12FenceEvent = CreateEvent(nullptr, FALSE, FALSE, nullptr);
        if (!d3d12FenceEvent) {
            std::cout << "[ENI] DX12 init failed: CreateEvent for fence failed." << std::endl;
            return false;
        }

        if (!InitImGuiBase(window)) {
            std::cout << "[ENI] DX12 init failed: InitImGuiBase failed." << std::endl;
            return false;
        }

        ImGui_ImplDX12_InitInfo initInfo{};
        initInfo.Device = d3d12Device;
        initInfo.CommandQueue = d3d12Queue;
        initInfo.NumFramesInFlight = static_cast<int>(d3d12FrameCount);
        initInfo.RTVFormat = sd.BufferDesc.Format;
        initInfo.DSVFormat = DXGI_FORMAT_UNKNOWN;
        initInfo.SrvDescriptorHeap = d3d12SrvHeap;
        initInfo.LegacySingleSrvCpuDescriptor = d3d12SrvHeap->GetCPUDescriptorHandleForHeapStart();
        initInfo.LegacySingleSrvGpuDescriptor = d3d12SrvHeap->GetGPUDescriptorHandleForHeapStart();

        if (!ImGui_ImplDX12_Init(&initInfo)) {
            std::cout << "[ENI] DX12 init failed: ImGui_ImplDX12_Init failed." << std::endl;
            return false;
        }

        backend = RenderBackend::DX12;
        dx12WaitingForQueueLogged = false;
        std::cout << "[ENI] Renderer backend selected: DirectX 12" << std::endl;
        return true;
    }

    static void __stdcall Hook_ExecuteCommandLists(ID3D12CommandQueue* pQueue, UINT NumCommandLists, ID3D12CommandList* const* ppCommandLists) {
        if (!d3d12Queue && pQueue) {
            D3D12_COMMAND_QUEUE_DESC desc = pQueue->GetDesc();
            if (desc.Type == D3D12_COMMAND_LIST_TYPE_DIRECT) {
                d3d12Queue = pQueue;
                d3d12Queue->AddRef();
                dx12IgnoredNonDirectQueueLogged = false;
                std::cout << "[ENI] Captured DirectX12 DIRECT command queue." << std::endl;
            } else if (!dx12IgnoredNonDirectQueueLogged) {
                dx12IgnoredNonDirectQueueLogged = true;
                std::cout << "[ENI] Ignored non-DIRECT DX12 queue (type " << static_cast<int>(desc.Type) << ")." << std::endl;
            }
        }
        oExecuteCommandLists(pQueue, NumCommandLists, ppCommandLists);
    }

    static void RenderFrame(IDXGISwapChain* pSwapChain) {
        if (!presentCallbackSeenLogged) {
            std::cout << "[ENI] SwapChain present callback reached." << std::endl;
            presentCallbackSeenLogged = true;
        }

        if (!init) {
            ID3D12Device* testDx12 = nullptr;
            if (SUCCEEDED(pSwapChain->GetDevice(__uuidof(ID3D12Device), reinterpret_cast<void**>(&testDx12)))) {
                SafeRelease(testDx12);
                InitDX12(pSwapChain);
            } else {
                InitDX11(pSwapChain);
            }
            init = (backend != RenderBackend::None);
        } else if (backend == RenderBackend::None) {
            ID3D12Device* testDx12 = nullptr;
            if (SUCCEEDED(pSwapChain->GetDevice(__uuidof(ID3D12Device), reinterpret_cast<void**>(&testDx12)))) {
                SafeRelease(testDx12);
                InitDX12(pSwapChain);
            } else {
                InitDX11(pSwapChain);
            }
        }

        if (backend == RenderBackend::DX11) {
            ImGui_ImplDX11_NewFrame();
            ImGui_ImplWin32_NewFrame();
            ImGui::NewFrame();

            ImGui::GetIO().MouseDrawCursor = Menu::bShowMenu;
            if (Menu::bShowMenu) {
                Menu::Render();
            }
            PlayerESP::RenderOverlay();

            ImGui::Render();
            d3d11Context->OMSetRenderTargets(1, &d3d11Rtv, nullptr);
            ImGui_ImplDX11_RenderDrawData(ImGui::GetDrawData());
        } else if (backend == RenderBackend::DX12 && d3d12Frames && d3d12CommandList && d3d12Queue) {
            IDXGISwapChain3* swapChain3 = nullptr;
            if (FAILED(pSwapChain->QueryInterface(IID_PPV_ARGS(&swapChain3)))) {
                return;
            }
            UINT backBufferIndex = swapChain3->GetCurrentBackBufferIndex();
            swapChain3->Release();
            DX12FrameContext& frame = d3d12Frames[backBufferIndex];
            WaitForFenceValue(frame.FenceValue);

            frame.CommandAllocator->Reset();
            d3d12CommandList->Reset(frame.CommandAllocator, nullptr);

            D3D12_RESOURCE_BARRIER barrierToRtv{};
            barrierToRtv.Type = D3D12_RESOURCE_BARRIER_TYPE_TRANSITION;
            barrierToRtv.Flags = D3D12_RESOURCE_BARRIER_FLAG_NONE;
            barrierToRtv.Transition.pResource = frame.RenderTarget;
            barrierToRtv.Transition.Subresource = D3D12_RESOURCE_BARRIER_ALL_SUBRESOURCES;
            barrierToRtv.Transition.StateBefore = D3D12_RESOURCE_STATE_PRESENT;
            barrierToRtv.Transition.StateAfter = D3D12_RESOURCE_STATE_RENDER_TARGET;
            d3d12CommandList->ResourceBarrier(1, &barrierToRtv);

            d3d12CommandList->OMSetRenderTargets(1, &frame.RtvHandle, FALSE, nullptr);
            ID3D12DescriptorHeap* descriptorHeaps[] = { d3d12SrvHeap };
            d3d12CommandList->SetDescriptorHeaps(1, descriptorHeaps);

            ImGui_ImplDX12_NewFrame();
            ImGui_ImplWin32_NewFrame();
            ImGui::NewFrame();

            ImGui::GetIO().MouseDrawCursor = Menu::bShowMenu;
            if (Menu::bShowMenu) {
                Menu::Render();
            }
            PlayerESP::RenderOverlay();

            ImGui::Render();
            ImGui_ImplDX12_RenderDrawData(ImGui::GetDrawData(), d3d12CommandList);

            D3D12_RESOURCE_BARRIER barrierToPresent{};
            barrierToPresent.Type = D3D12_RESOURCE_BARRIER_TYPE_TRANSITION;
            barrierToPresent.Flags = D3D12_RESOURCE_BARRIER_FLAG_NONE;
            barrierToPresent.Transition.pResource = frame.RenderTarget;
            barrierToPresent.Transition.Subresource = D3D12_RESOURCE_BARRIER_ALL_SUBRESOURCES;
            barrierToPresent.Transition.StateBefore = D3D12_RESOURCE_STATE_RENDER_TARGET;
            barrierToPresent.Transition.StateAfter = D3D12_RESOURCE_STATE_PRESENT;
            d3d12CommandList->ResourceBarrier(1, &barrierToPresent);

            d3d12CommandList->Close();
            ID3D12CommandList* commandLists[] = { d3d12CommandList };
            d3d12Queue->ExecuteCommandLists(1, commandLists);
            SignalFrameFence(frame);
        }
    }

    static HRESULT __stdcall Hook_Present(IDXGISwapChain* pSwapChain, UINT SyncInterval, UINT Flags) {
        RenderFrame(pSwapChain);
        return oPresent(pSwapChain, SyncInterval, Flags);
    }

    static HRESULT __stdcall Hook_Present1(IDXGISwapChain1* pSwapChain, UINT SyncInterval, UINT PresentFlags, const DXGI_PRESENT_PARAMETERS* pPresentParameters) {
        RenderFrame(reinterpret_cast<IDXGISwapChain*>(pSwapChain));
        return oPresent1(pSwapChain, SyncInterval, PresentFlags, pPresentParameters);
    }

    void Init() {
        std::cout << "[ENI] Initializing renderer hooks (DX11 + DX12)..." << std::endl;
        Sleep(2000);

        MH_Initialize();

        WNDCLASSEXA wc{ sizeof(WNDCLASSEXA), CS_CLASSDC, DefWindowProc, 0, 0, GetModuleHandle(nullptr), nullptr, nullptr, nullptr, nullptr, "DummyClass", nullptr };
        RegisterClassExA(&wc);
        HWND hWnd = CreateWindowA("DummyClass", "DummyWindow", WS_OVERLAPPEDWINDOW, 0, 0, 100, 100, nullptr, nullptr, wc.hInstance, nullptr);

        // Hook IDXGISwapChain::Present (works for both DX11 and DX12 swapchains).
        {
            D3D_FEATURE_LEVEL featureLevel;
            const D3D_FEATURE_LEVEL featureLevels[] = { D3D_FEATURE_LEVEL_11_0, D3D_FEATURE_LEVEL_10_1, D3D_FEATURE_LEVEL_10_0 };
            DXGI_SWAP_CHAIN_DESC sd{};
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
            HRESULT hr = D3D11CreateDeviceAndSwapChain(
                nullptr,
                D3D_DRIVER_TYPE_HARDWARE,
                nullptr,
                0,
                featureLevels,
                3,
                D3D11_SDK_VERSION,
                &sd,
                &dummySwapChain,
                &dummyDevice,
                &featureLevel,
                &dummyContext);

            if (SUCCEEDED(hr) && dummySwapChain) {
                void** vtable = *reinterpret_cast<void***>(dummySwapChain);
                void* presentAddress = vtable[8];
                MH_CreateHook(presentAddress, Hook_Present, reinterpret_cast<void**>(&oPresent));
                std::cout << "[ENI] Hooked IDXGISwapChain::Present at " << presentAddress << std::endl;

                IDXGISwapChain1* dummySwapChain1 = nullptr;
                if (SUCCEEDED(dummySwapChain->QueryInterface(IID_PPV_ARGS(&dummySwapChain1))) && dummySwapChain1) {
                    void** vtable1 = *reinterpret_cast<void***>(dummySwapChain1);
                    void* present1Address = vtable1[22];
                    MH_CreateHook(present1Address, Hook_Present1, reinterpret_cast<void**>(&oPresent1));
                    std::cout << "[ENI] Hooked IDXGISwapChain1::Present1 at " << present1Address << std::endl;
                    dummySwapChain1->Release();
                } else {
                    std::cout << "[ENI] Failed to get IDXGISwapChain1 for Present1 hook." << std::endl;
                }
            } else {
                std::cout << "[ENI] Failed to create dummy DX11 swapchain for Present hook." << std::endl;
            }

            SafeRelease(dummySwapChain);
            SafeRelease(dummyContext);
            SafeRelease(dummyDevice);
        }

        // Hook ID3D12CommandQueue::ExecuteCommandLists to capture runtime queue.
        {
            IDXGIFactory4* factory = nullptr;
            IDXGIAdapter1* adapter = nullptr;
            ID3D12Device* dummyDevice12 = nullptr;
            ID3D12CommandQueue* dummyQueue = nullptr;

            if (SUCCEEDED(CreateDXGIFactory1(IID_PPV_ARGS(&factory)))) {
                factory->EnumAdapters1(0, &adapter);
            }

            if (adapter && SUCCEEDED(D3D12CreateDevice(adapter, D3D_FEATURE_LEVEL_11_0, IID_PPV_ARGS(&dummyDevice12)))) {
                D3D12_COMMAND_QUEUE_DESC queueDesc{};
                queueDesc.Type = D3D12_COMMAND_LIST_TYPE_DIRECT;
                queueDesc.Flags = D3D12_COMMAND_QUEUE_FLAG_NONE;
                if (SUCCEEDED(dummyDevice12->CreateCommandQueue(&queueDesc, IID_PPV_ARGS(&dummyQueue))) && dummyQueue) {
                    void** vtable = *reinterpret_cast<void***>(dummyQueue);
                    // ID3D12CommandQueue::ExecuteCommandLists is vtable slot 10.
                    void* executeAddress = vtable[10];
                    MH_CreateHook(executeAddress, Hook_ExecuteCommandLists, reinterpret_cast<void**>(&oExecuteCommandLists));
                    std::cout << "[ENI] Hooked ID3D12CommandQueue::ExecuteCommandLists at " << executeAddress << std::endl;
                }
            }

            SafeRelease(dummyQueue);
            SafeRelease(dummyDevice12);
            SafeRelease(adapter);
            SafeRelease(factory);
        }

        MH_EnableHook(MH_ALL_HOOKS);
        std::cout << "[ENI] Renderer hooks armed. Press INSERT for menu." << std::endl;

        DestroyWindow(hWnd);
        UnregisterClassA(wc.lpszClassName, wc.hInstance);
    }

    void Shutdown() {
        if (backend == RenderBackend::DX11) {
            ImGui_ImplDX11_Shutdown();
        } else if (backend == RenderBackend::DX12) {
            ImGui_ImplDX12_Shutdown();
        }

        if (imguiInit) {
            ImGui_ImplWin32_Shutdown();
            ImGui::DestroyContext();
        }

        if (oWndProc && window) {
            SetWindowLongPtr(window, GWLP_WNDPROC, reinterpret_cast<LONG_PTR>(oWndProc));
        }

        if (d3d12Frames) {
            for (UINT i = 0; i < d3d12FrameCount; ++i) {
                WaitForFenceValue(d3d12Frames[i].FenceValue);
                SafeRelease(d3d12Frames[i].CommandAllocator);
                SafeRelease(d3d12Frames[i].RenderTarget);
            }
            delete[] d3d12Frames;
            d3d12Frames = nullptr;
        }

        if (d3d12FenceEvent) {
            CloseHandle(d3d12FenceEvent);
            d3d12FenceEvent = nullptr;
        }

        SafeRelease(d3d11Rtv);
        SafeRelease(d3d11Context);
        SafeRelease(d3d11Device);

        SafeRelease(d3d12CommandList);
        SafeRelease(d3d12RtvHeap);
        SafeRelease(d3d12SrvHeap);
        SafeRelease(d3d12Fence);
        SafeRelease(d3d12Queue);
        SafeRelease(d3d12Device);

        MH_DisableHook(MH_ALL_HOOKS);
        MH_Uninitialize();
    }
}

# PEAK+ Universal Mod

A highly modular, unmanaged C++ external DLL modification for the Unity-based game **PEAK**. 
PEAK+ bypasses standard engine limitations by injecting directly into the `mono-2.0-bdwgc.dll` runtime, ripping JIT-compiled C# method pointers from memory, and detouring them natively in C++ via `MinHook`. 

## Features
- **Raw DirectX 11/12 Interception**: Hooks `IDXGISwapChain::Present` and auto-selects a DX11 or DX12 ImGui renderer backend at runtime.
- **True God Mode**: Blocks physical damage, debuffs, hunger, and environmental hazards by hijacking the game's internal `CharacterAfflictions` engine.
- **Infinite Stamina**: Nullifies stamina drain callbacks statically.
- **No Hunger**: Prevents any positive hunger buildup from `CharacterAfflictions::AddStatus`.
- **Status Filters**: Independently block `Poison`, `Cold`, `Hot`, `Drowsy`, and `Curse` buildup from the same status pipeline.
- **No Fall Damage**: Neutralizes all fall applications by forcing `Character::Fall` values to zero.
- **Super Jump**: Forces jumps through the "pal jump" path (`CharacterMovement::JumpRpc`) for amplified jump force.
- **Player ESP**: Tracks live `Character` instances and draws an on-screen player overlay with local/remote labels and names.
- **Inventory Editor**: Adds menu tools to spawn items by prefab name and remove items directly from slots `0-3`.
- **Network Safety**: Adds `Anti-Kick` and `Anti-Inventory Strip` toggles by filtering hostile network RPC paths.
- **Dynamic Cursor Bypass**: Suppresses Unity's intrinsic `CursorHandler` script and recursively injects `mono_runtime_invoke` commands to safely decouple the hardware cursor for overlay control.

## Architecture Highlights
- **Direct Mono Method Extraction**: Dynamically scrapes `Assembly-CSharp.dll` and `UnityEngine.CoreModule` natively from process RAM.
- **Modular Component Design**: Completely sandboxed features orchestrated by a central `FeatureManager` loop.
- **Zero-Disk Reliance**: Compiles entirely into a singular, untraceable `.dll` payload injected via Xenos.

## Build Requirements
- Visual Studio 2022 (MSVC)
- CMake 3.20+
- (ImGui, MinHook, Volk, and Vulkan-Headers are automatically fetched via CMake)

```bash
mkdir build
cmake -S . -B build
cmake --build build --config Release
```

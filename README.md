# PEAK+ Universal Mod

A highly modular, unmanaged C++ external DLL modification for the Unity-based game **PEAK**. 
PEAK+ bypasses standard engine limitations by injecting directly into the `mono-2.0-bdwgc.dll` runtime, ripping JIT-compiled C# method pointers from memory, and detouring them natively in C++ via `MinHook`. 

## Features
- **Raw DirectX 11 Interception**: Overrides the native graphics layer to render a custom `ImGui` overlay cleanly bypassing anti-capture.
- **True God Mode**: Blocks physical damage, debuffs, hunger, and environmental hazards by hijacking the game's internal `CharacterAfflictions` engine.
- **Infinite Stamina**: Nullifies stamina drain callbacks statically.
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

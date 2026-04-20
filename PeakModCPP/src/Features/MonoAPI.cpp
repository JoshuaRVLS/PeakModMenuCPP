#include "MonoAPI.h"
#include <iostream>

namespace MonoAPI {
    bool initialized = false;

    mono_get_root_domain_t mono_get_root_domain = nullptr;
    mono_thread_attach_t mono_thread_attach = nullptr;
    mono_domain_assembly_open_t mono_domain_assembly_open = nullptr;
    mono_assembly_get_image_t mono_assembly_get_image = nullptr;
    mono_class_from_name_t mono_class_from_name = nullptr;
    mono_class_get_method_from_name_t mono_class_get_method_from_name = nullptr;
    mono_compile_method_t mono_compile_method = nullptr;
    mono_assembly_foreach_t mono_assembly_foreach = nullptr;
    mono_image_get_name_t mono_image_get_name = nullptr;
    mono_runtime_invoke_t mono_runtime_invoke = nullptr;

    MonoDomain* root_domain = nullptr;
    MonoImage* main_assembly_image = nullptr;
    MonoImage* core_assembly_image = nullptr;

    void AssemblyIteratorCallback(MonoAssembly* assembly, void* user_data) {
        MonoImage* image = mono_assembly_get_image(assembly);
        if (image) {
            const char* name = mono_image_get_name(image);
            if (name) {
                std::cout << "[ENI-Mono] Scraped Assembly: " << name << std::endl;
                if (strstr(name, "Assembly-CSharp") && !strstr(name, "-firstpass") && !main_assembly_image) {
                    main_assembly_image = image;
                    std::cout << "[ENI-Mono] ^^^ Locked onto Target Interface ^^^" << std::endl;
                }
                if (strstr(name, "UnityEngine.CoreModule") && !core_assembly_image) {
                    core_assembly_image = image;
                    std::cout << "[ENI-Mono] ^^^ Locked onto Unity CoreModule ^^^" << std::endl;
                }
            }
        }
    }

    bool Initialize() {
        if (initialized) return true;

        HMODULE hMono = GetModuleHandleA("mono-2.0-bdwgc.dll");
        if (!hMono) {
            std::cout << "[ENI-Mono] Failed to locate mono-2.0-bdwgc.dll in process memory!" << std::endl;
            return false;
        }

        std::cout << "[ENI-Mono] Mono DLL successfully mapped. Resolving internal function exports..." << std::endl;

        mono_get_root_domain = (mono_get_root_domain_t)GetProcAddress(hMono, "mono_get_root_domain");
        mono_thread_attach = (mono_thread_attach_t)GetProcAddress(hMono, "mono_thread_attach");
        mono_domain_assembly_open = (mono_domain_assembly_open_t)GetProcAddress(hMono, "mono_domain_assembly_open");
        mono_assembly_get_image = (mono_assembly_get_image_t)GetProcAddress(hMono, "mono_assembly_get_image");
        mono_class_from_name = (mono_class_from_name_t)GetProcAddress(hMono, "mono_class_from_name");
        mono_class_get_method_from_name = (mono_class_get_method_from_name_t)GetProcAddress(hMono, "mono_class_get_method_from_name");
        mono_compile_method = (mono_compile_method_t)GetProcAddress(hMono, "mono_compile_method");
        mono_assembly_foreach = (mono_assembly_foreach_t)GetProcAddress(hMono, "mono_assembly_foreach");
        mono_image_get_name = (mono_image_get_name_t)GetProcAddress(hMono, "mono_image_get_name");
        mono_runtime_invoke = (mono_runtime_invoke_t)GetProcAddress(hMono, "mono_runtime_invoke");

        if (!mono_get_root_domain || !mono_thread_attach || !mono_class_from_name || !mono_compile_method) {
            std::cout << "[ENI-Mono] CRITICAL FAILURE: Could not bind core Mono exported functions!" << std::endl;
            return false;
        }

        root_domain = mono_get_root_domain();
        if (!root_domain) return false;

        mono_thread_attach(root_domain);

        std::cout << "[ENI-Mono] Thread successfully attached to Main Mono Domain. Iterating AppDomain for target assembly..." << std::endl;

        mono_assembly_foreach((void*)AssemblyIteratorCallback, nullptr);

        if (!main_assembly_image) {
            std::cout << "[ENI-Mono] Failed to find Assembly-CSharp.dll loaded in the game's memory space!" << std::endl;
            return false;
        }

        std::cout << "[ENI-Mono] Target Assembly mapped successfully from live memory!" << std::endl;

        initialized = true;
        return true;
    }

    void* GetMethodAddress(const char* className, const char* methodName, int paramCount) {
        if (!initialized || !main_assembly_image) return nullptr;

        MonoClass* klass = mono_class_from_name(main_assembly_image, "", className);
        if (!klass) {
            std::cout << "[ENI-Mono] Failed to find class: " << className << std::endl;
            return nullptr;
        }

        MonoMethod* method = mono_class_get_method_from_name(klass, methodName, paramCount);
        if (!method) {
            std::cout << "[ENI-Mono] Failed to find method: " << className << "::" << methodName << std::endl;
            return nullptr;
        }

        // JIT compile the method into native x64 memory space so MinHook can attach to it
        void* nativeAddress = mono_compile_method(method);
        
        std::cout << "[ENI-Mono] Resolved " << className << "::" << methodName << " -> Memory Address: " << nativeAddress << std::endl;

        return nativeAddress;
    }

    void ApplyCursorState(bool visible, int lockState) {
        if (!initialized || !core_assembly_image || !mono_runtime_invoke) return;

        MonoClass* cursorClass = mono_class_from_name(core_assembly_image, "UnityEngine", "Cursor");
        if (!cursorClass) return;

        MonoMethod* setVisibleMethod = mono_class_get_method_from_name(cursorClass, "set_visible", 1);
        MonoMethod* setLockStateMethod = mono_class_get_method_from_name(cursorClass, "set_lockState", 1);

        if (setVisibleMethod) {
            void* args[1] = { &visible };
            mono_runtime_invoke(setVisibleMethod, nullptr, args, nullptr);
        }

        if (setLockStateMethod) {
            void* args[1] = { &lockState };
            mono_runtime_invoke(setLockStateMethod, nullptr, args, nullptr);
        }
    }
}

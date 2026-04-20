#include "MonoAPI.h"
#include <iostream>
#include <string>

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
    mono_string_to_utf8_t mono_string_to_utf8 = nullptr;
    mono_free_t mono_free = nullptr;
    mono_object_to_string_t mono_object_to_string = nullptr;
    mono_string_new_t mono_string_new = nullptr;
    mono_class_get_field_from_name_t mono_class_get_field_from_name = nullptr;
    mono_field_get_value_t mono_field_get_value = nullptr;

    MonoDomain* root_domain = nullptr;
    MonoImage* main_assembly_image = nullptr;
    MonoImage* core_assembly_image = nullptr;
    std::string lastExceptionString;

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
        mono_string_to_utf8 = (mono_string_to_utf8_t)GetProcAddress(hMono, "mono_string_to_utf8");
        mono_free = (mono_free_t)GetProcAddress(hMono, "mono_free");
        mono_object_to_string = (mono_object_to_string_t)GetProcAddress(hMono, "mono_object_to_string");
        mono_string_new = (mono_string_new_t)GetProcAddress(hMono, "mono_string_new");
        mono_class_get_field_from_name = (mono_class_get_field_from_name_t)GetProcAddress(hMono, "mono_class_get_field_from_name");
        mono_field_get_value = (mono_field_get_value_t)GetProcAddress(hMono, "mono_field_get_value");

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

    bool AttachCurrentThread() {
        if (!initialized || !mono_get_root_domain || !mono_thread_attach) {
            return false;
        }

        MonoDomain* domain = mono_get_root_domain();
        if (!domain) {
            return false;
        }

        mono_thread_attach(domain);
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

    MonoClass* GetClass(const char* className, const char* nameSpace) {
        if (!initialized || !main_assembly_image || !mono_class_from_name) {
            return nullptr;
        }
        return mono_class_from_name(main_assembly_image, nameSpace ? nameSpace : "", className);
    }

    MonoMethod* GetMethod(const char* className, const char* methodName, int paramCount, const char* nameSpace) {
        MonoClass* klass = GetClass(className, nameSpace);
        if (!klass || !mono_class_get_method_from_name) {
            return nullptr;
        }
        return mono_class_get_method_from_name(klass, methodName, paramCount);
    }

    void ApplyCursorState(bool visible, int lockState) {
        if (!initialized || !core_assembly_image || !mono_runtime_invoke) return;
        if (!AttachCurrentThread()) return;

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

    MonoString* CreateString(const char* text) {
        if (!initialized || !mono_string_new) {
            return nullptr;
        }
        if (!AttachCurrentThread()) {
            return nullptr;
        }

        MonoDomain* domain = mono_get_root_domain ? mono_get_root_domain() : nullptr;
        if (!domain) {
            return nullptr;
        }

        return mono_string_new(domain, text ? text : "");
    }

    const char* GetExceptionString(MonoObject* exceptionObject) {
        if (!exceptionObject) {
            return "";
        }
        if (!mono_object_to_string || !mono_string_to_utf8) {
            lastExceptionString = "<exception raised, but mono_object_to_string unavailable>";
            return lastExceptionString.c_str();
        }

        MonoObject* toStringExc = nullptr;
        MonoString* exceptionText = mono_object_to_string(exceptionObject, &toStringExc);
        if (!exceptionText || toStringExc) {
            lastExceptionString = "<exception raised, failed to stringify>";
            return lastExceptionString.c_str();
        }

        char* utf8 = mono_string_to_utf8(exceptionText);
        if (!utf8) {
            lastExceptionString = "<exception raised, failed UTF8 conversion>";
            return lastExceptionString.c_str();
        }

        lastExceptionString = utf8;
        if (mono_free) {
            mono_free(utf8);
        }
        return lastExceptionString.c_str();
    }
}

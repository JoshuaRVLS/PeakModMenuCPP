#pragma once
#include <windows.h>

typedef void* MonoDomain;
typedef void* MonoAssembly;
typedef void* MonoImage;
typedef void* MonoClass;
typedef void* MonoMethod;
typedef void* MonoObject;

namespace MonoAPI {
    extern bool initialized;
    
    // Function Pointers to exported Mono DLL methods
    typedef MonoDomain* (__cdecl* mono_get_root_domain_t)();
    typedef void* (__cdecl* mono_thread_attach_t)(MonoDomain* domain);
    typedef MonoAssembly* (__cdecl* mono_domain_assembly_open_t)(MonoDomain* domain, const char* name);
    typedef MonoImage* (__cdecl* mono_assembly_get_image_t)(MonoAssembly* assembly);
    typedef MonoClass* (__cdecl* mono_class_from_name_t)(MonoImage* image, const char* name_space, const char* name);
    typedef MonoMethod* (__cdecl* mono_class_get_method_from_name_t)(MonoClass* klass, const char* name, int param_count);
    typedef void* (__cdecl* mono_compile_method_t)(MonoMethod* method);
    typedef void (__cdecl* mono_assembly_foreach_t)(void* func, void* user_data);
    typedef const char* (__cdecl* mono_image_get_name_t)(MonoImage* image);
    typedef void* (__cdecl* mono_runtime_invoke_t)(MonoMethod* method, void* obj, void** params, void** exc);
    
    extern mono_get_root_domain_t mono_get_root_domain;
    extern mono_thread_attach_t mono_thread_attach;
    extern mono_domain_assembly_open_t mono_domain_assembly_open;
    extern mono_assembly_get_image_t mono_assembly_get_image;
    extern mono_class_from_name_t mono_class_from_name;
    extern mono_class_get_method_from_name_t mono_class_get_method_from_name;
    extern mono_compile_method_t mono_compile_method;
    extern mono_assembly_foreach_t mono_assembly_foreach;
    extern mono_image_get_name_t mono_image_get_name;
    extern mono_runtime_invoke_t mono_runtime_invoke;

    bool Initialize();
    void* GetMethodAddress(const char* className, const char* methodName, int paramCount = -1);
    void ApplyCursorState(bool visible, int lockState);
}

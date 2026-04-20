cmake_minimum_required(VERSION 3.14)
project(PeakModCPP)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED True)

include(FetchContent)

# --- MinHook ---
FetchContent_Declare(minhook GIT_REPOSITORY https://github.com/TsudaKageyu/minhook.git GIT_TAG master)
FetchContent_MakeAvailable(minhook)

# --- ImGui ---
FetchContent_Declare(imgui GIT_REPOSITORY https://github.com/ocornut/imgui.git GIT_TAG master)
FetchContent_GetProperties(imgui)
if(NOT imgui_POPULATED)
    FetchContent_Populate(imgui)
endif()

file(GLOB IMGUI_SOURCES
    ${imgui_SOURCE_DIR}/*.cpp
    ${imgui_SOURCE_DIR}/backends/imgui_impl_win32.cpp
    ${imgui_SOURCE_DIR}/backends/imgui_impl_dx11.cpp
    ${imgui_SOURCE_DIR}/backends/imgui_impl_dx12.cpp
    ${imgui_SOURCE_DIR}/backends/imgui_impl_vulkan.cpp
)

# --- Kiero ---
FetchContent_Declare(kiero GIT_REPOSITORY https://github.com/Rebzzel/kiero.git GIT_TAG master)
FetchContent_GetProperties(kiero)
if(NOT kiero_POPULATED)
    FetchContent_Populate(kiero)
endif()

file(GLOB KIERO_SOURCES
    ${kiero_SOURCE_DIR}/kiero.cpp
)

# --- Vulkan Headers (required for compilation without SDK) ---
FetchContent_Declare(VulkanHeaders GIT_REPOSITORY https://github.com/KhronosGroup/Vulkan-Headers.git GIT_TAG main)
FetchContent_MakeAvailable(VulkanHeaders)

# --- Volk (Dynamic Vulkan Loader) ---
FetchContent_Declare(volk GIT_REPOSITORY https://github.com/zeux/volk.git GIT_TAG master)
FetchContent_MakeAvailable(volk)

# --- Project Source Files ---
# explicitly GLOBing source files to capture the newly created MonoAPI interface
file(GLOB_RECURSE SRC_SOURCES src/*.cpp)

set(SOURCES
    ${SRC_SOURCES}
    ${IMGUI_SOURCES}
    ${KIERO_SOURCES}
)

# Instruct ImGui to use Volk instead of relying on statically linking vulkan-1.lib
add_definitions(-DKIERO_INCLUDE_D3D11 -DKIERO_INCLUDE_D3D12 -DKIERO_INCLUDE_VULKAN -DKIERO_USE_MINHOOK -DIMGUI_IMPL_VULKAN_USE_VOLK)

add_library(PeakModCPP SHARED ${SOURCES})

target_include_directories(PeakModCPP PRIVATE 
    ${imgui_SOURCE_DIR}
    ${imgui_SOURCE_DIR}/backends
    ${kiero_SOURCE_DIR}
    ${vulkanheaders_SOURCE_DIR}/include
    ${volk_SOURCE_DIR}
    src
)

# Link volk alongside our default windows libraries.
target_link_libraries(PeakModCPP PRIVATE minhook volk user32.lib kernel32.lib d3d11.lib d3d12.lib dxguid.lib dxgi.lib)

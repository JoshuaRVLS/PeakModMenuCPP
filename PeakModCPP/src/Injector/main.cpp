#include <windows.h>
#include <tlhelp32.h>
#include <iostream>
#include <string>

DWORD GetTargetProcessId(const char* processName) {
    DWORD pid = 0;
    HANDLE snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
    if (snapshot == INVALID_HANDLE_VALUE) return 0;

    PROCESSENTRY32 entry{};
    entry.dwSize = sizeof(entry);

    if (Process32First(snapshot, &entry)) {
        do {
            if (_stricmp(entry.szExeFile, processName) == 0) {
                pid = entry.th32ProcessID;
                break;
            }
        } while (Process32Next(snapshot, &entry));
    }

    CloseHandle(snapshot);
    return pid;
}

bool InjectDLL(DWORD pid, const std::string& dllPath) {
    HANDLE hProcess = OpenProcess(PROCESS_ALL_ACCESS, FALSE, pid);
    if (!hProcess) {
        std::cerr << "[ENI-Inject] ERROR: Failed to open process. Are you running as Administrator?" << std::endl;
        return false;
    }

    // Allocate memory inside the remote process
    size_t pathSize = dllPath.size() + 1;
    LPVOID pRemotePath = VirtualAllocEx(hProcess, nullptr, pathSize, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
    if (!pRemotePath) {
        std::cerr << "[ENI-Inject] ERROR: Failed to allocate remote memory." << std::endl;
        CloseHandle(hProcess);
        return false;
    }

    // Write the DLL path string into that memory
    if (!WriteProcessMemory(hProcess, pRemotePath, dllPath.c_str(), pathSize, nullptr)) {
        std::cerr << "[ENI-Inject] ERROR: Failed to write DLL path to process memory." << std::endl;
        VirtualFreeEx(hProcess, pRemotePath, 0, MEM_RELEASE);
        CloseHandle(hProcess);
        return false;
    }

    // Grab the real LoadLibraryA address from kernel32 (identical in all processes)
    LPTHREAD_START_ROUTINE pLoadLibrary = reinterpret_cast<LPTHREAD_START_ROUTINE>(
        GetProcAddress(GetModuleHandleA("kernel32.dll"), "LoadLibraryA")
    );

    if (!pLoadLibrary) {
        std::cerr << "[ENI-Inject] ERROR: Failed to resolve LoadLibraryA." << std::endl;
        VirtualFreeEx(hProcess, pRemotePath, 0, MEM_RELEASE);
        CloseHandle(hProcess);
        return false;
    }

    // Spin a remote thread inside PEAK.exe pointing at LoadLibraryA
    HANDLE hThread = CreateRemoteThread(hProcess, nullptr, 0, pLoadLibrary, pRemotePath, 0, nullptr);
    if (!hThread) {
        std::cerr << "[ENI-Inject] ERROR: Failed to create remote thread." << std::endl;
        VirtualFreeEx(hProcess, pRemotePath, 0, MEM_RELEASE);
        CloseHandle(hProcess);
        return false;
    }

    // Wait for LoadLibrary to finish executing inside the game
    WaitForSingleObject(hThread, 8000);

    CloseHandle(hThread);
    VirtualFreeEx(hProcess, pRemotePath, 0, MEM_RELEASE);
    CloseHandle(hProcess);
    return true;
}

int main() {
    std::cout << R"(
  ____  _____    _    _  __  
 |  _ \| ____|  / \  | |/ /  
 | |_) |  _|   / _ \ | ' /   
 |  __/| |___ / ___ \| . \   
 |_|   |_____/_/   \_\_|\_\  
  PEAK+ Internal Injector v1.0
)" << std::endl;

    const char* targetProcess = "PEAK.exe";
    
    // Build absolute path to DLL sitting next to this injector EXE
    char exePath[MAX_PATH];
    GetModuleFileNameA(nullptr, exePath, MAX_PATH);
    std::string dllPath = std::string(exePath);
    dllPath = dllPath.substr(0, dllPath.find_last_of("\\/") + 1) + "PeakModCPP.dll";

    std::cout << "[ENI-Inject] DLL Path: " << dllPath << std::endl;
    std::cout << "[ENI-Inject] Scanning for " << targetProcess << "..." << std::endl;

    DWORD pid = 0;
    while (!pid) {
        pid = GetTargetProcessId(targetProcess);
        if (!pid) {
            std::cout << "[ENI-Inject] Process not found. Retrying in 2s..." << std::endl;
            Sleep(2000);
        }
    }

    std::cout << "[ENI-Inject] Target anchored! PID: " << pid << std::endl;
    std::cout << "[ENI-Inject] Deploying payload..." << std::endl;

    if (InjectDLL(pid, dllPath)) {
        std::cout << "[ENI-Inject] Payload successfully injected. Press ENTER to exit." << std::endl;
    } else {
        std::cerr << "[ENI-Inject] Injection FAILED. Check permissions." << std::endl;
    }

    std::cin.get();
    return 0;
}

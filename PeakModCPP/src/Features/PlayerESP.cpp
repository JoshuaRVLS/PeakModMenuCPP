#include "PlayerESP.h"
#include "MonoAPI.h"
#include "MinHook.h"
#include "../UI/Menu.h"
#include <imgui.h>
#include <windows.h>
#include <algorithm>
#include <iostream>
#include <mutex>
#include <string>
#include <vector>

namespace PlayerESPModule {
    struct TrackedPlayer {
        void* character;
        bool isLocal;
        std::string name;
        ULONGLONG lastSeenMs;
    };

    typedef void(__fastcall* Character_Update_t)(void* _this);
    typedef bool(__fastcall* Character_get_IsLocal_t)(void* _this);
    typedef MonoString*(__fastcall* Character_get_characterName_t)(void* _this);

    Character_Update_t oCharacter_Update = nullptr;
    Character_get_IsLocal_t oCharacter_get_IsLocal = nullptr;
    Character_get_characterName_t oCharacter_get_characterName = nullptr;

    std::mutex gPlayersMutex;
    std::vector<TrackedPlayer> gPlayers;

    std::string ToUtf8(MonoString* monoString) {
        if (!monoString || !MonoAPI::mono_string_to_utf8) {
            return "Unknown";
        }

        char* utf8 = MonoAPI::mono_string_to_utf8(monoString);
        if (!utf8) {
            return "Unknown";
        }

        std::string out(utf8);
        if (MonoAPI::mono_free) {
            MonoAPI::mono_free(utf8);
        }
        return out;
    }

    void UpdateTrackedPlayer(void* character) {
        if (!character || !oCharacter_get_IsLocal || !oCharacter_get_characterName) {
            return;
        }

        TrackedPlayer incoming{};
        incoming.character = character;
        incoming.isLocal = oCharacter_get_IsLocal(character);
        incoming.name = ToUtf8(oCharacter_get_characterName(character));
        incoming.lastSeenMs = GetTickCount64();

        std::lock_guard<std::mutex> lock(gPlayersMutex);
        auto it = std::find_if(gPlayers.begin(), gPlayers.end(), [character](const TrackedPlayer& player) {
            return player.character == character;
        });

        if (it == gPlayers.end()) {
            gPlayers.push_back(std::move(incoming));
        } else {
            it->isLocal = incoming.isLocal;
            it->name = std::move(incoming.name);
            it->lastSeenMs = incoming.lastSeenMs;
        }

        const ULONGLONG now = GetTickCount64();
        gPlayers.erase(
            std::remove_if(gPlayers.begin(), gPlayers.end(), [now](const TrackedPlayer& player) {
                return now - player.lastSeenMs > 5000;
            }),
            gPlayers.end());
    }

    void __fastcall Hook_Character_Update(void* _this) {
        if (Menu::bPlayerESP) {
            UpdateTrackedPlayer(_this);
        }
        oCharacter_Update(_this);
    }
}

void PlayerESP::Initialize() {
    std::cout << "[ENI-PlayerESP] Initializing Sandbox..." << std::endl;

    void* pUpdate = MonoAPI::GetMethodAddress("Character", "Update", 0);
    void* pGetIsLocal = MonoAPI::GetMethodAddress("Character", "get_IsLocal", 0);
    void* pGetCharacterName = MonoAPI::GetMethodAddress("Character", "get_characterName", 0);

    if (!pUpdate || !pGetIsLocal || !pGetCharacterName) {
        std::cout << "[ENI-PlayerESP] Missing one or more Character methods. ESP disabled." << std::endl;
        return;
    }

    PlayerESPModule::oCharacter_get_IsLocal = reinterpret_cast<PlayerESPModule::Character_get_IsLocal_t>(pGetIsLocal);
    PlayerESPModule::oCharacter_get_characterName = reinterpret_cast<PlayerESPModule::Character_get_characterName_t>(pGetCharacterName);

    MH_CreateHook(pUpdate, PlayerESPModule::Hook_Character_Update, reinterpret_cast<void**>(&PlayerESPModule::oCharacter_Update));
    MH_EnableHook(pUpdate);
    std::cout << "[ENI-PlayerESP] -> System Secured." << std::endl;
}

void PlayerESP::RenderOverlay() {
    if (!Menu::bPlayerESP) {
        return;
    }

    std::vector<PlayerESPModule::TrackedPlayer> snapshot;
    {
        std::lock_guard<std::mutex> lock(PlayerESPModule::gPlayersMutex);
        snapshot = PlayerESPModule::gPlayers;
    }

    ImGui::SetNextWindowPos(ImVec2(20.0f, 20.0f), ImGuiCond_Always);
    ImGui::SetNextWindowBgAlpha(0.35f);
    constexpr ImGuiWindowFlags flags =
        ImGuiWindowFlags_NoTitleBar |
        ImGuiWindowFlags_AlwaysAutoResize |
        ImGuiWindowFlags_NoMove |
        ImGuiWindowFlags_NoSavedSettings |
        ImGuiWindowFlags_NoInputs;

    ImGui::Begin("Player ESP Overlay", nullptr, flags);
    ImGui::Text("PEAK+ Player ESP");
    ImGui::Separator();
    ImGui::Text("Tracked: %d", static_cast<int>(snapshot.size()));

    for (const auto& player : snapshot) {
        ImVec4 color = player.isLocal ? ImVec4(0.4f, 1.0f, 0.4f, 1.0f) : ImVec4(1.0f, 0.85f, 0.3f, 1.0f);
        ImGui::TextColored(color, "%s %s", player.isLocal ? "[LOCAL]" : "[REMOTE]", player.name.c_str());
    }

    ImGui::End();
}

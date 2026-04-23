#include "PlayerESP.h"
#include "MonoAPI.h"
#include "MinHook.h"
#include "../UI/Menu.h"
#include <imgui.h>
#include <windows.h>
#include <algorithm>
#include <cmath>
#include <iostream>
#include <mutex>
#include <string>
#include <vector>

namespace PlayerESPModule {
    struct Vector3 {
        float x;
        float y;
        float z;
    };

    struct TrackedPlayer {
        void* character;
        bool isLocal;
        std::string name;
        bool hasProjection;
        float headX;
        float headY;
        float headZ;
        float feetX;
        float feetY;
        float feetZ;
        ULONGLONG lastSeenMs;
    };

    typedef void(__fastcall* Character_Update_t)(void* _this);
    typedef bool(__fastcall* Character_get_IsLocal_t)(void* _this);
    typedef MonoString*(__fastcall* Character_get_characterName_t)(void* _this);

    Character_Update_t oCharacter_Update = nullptr;
    Character_get_IsLocal_t oCharacter_get_IsLocal = nullptr;
    Character_get_characterName_t oCharacter_get_characterName = nullptr;
    MonoMethod* mCharacterGetHead = nullptr;
    MonoMethod* mCharacterGetCenter = nullptr;
    MonoMethod* mCameraGetMain = nullptr;
    MonoMethod* mCameraWorldToScreen = nullptr;

    std::mutex gPlayersMutex;
    std::vector<TrackedPlayer> gPlayers;
    constexpr ULONGLONG kPlayerFreshMs = 2000;

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

    bool InvokeVector3(MonoMethod* method, void* obj, Vector3& outVec) {
        if (!method || !MonoAPI::mono_runtime_invoke || !MonoAPI::mono_object_unbox) {
            return false;
        }

        MonoObject* exc = nullptr;
        MonoObject* result = reinterpret_cast<MonoObject*>(
            MonoAPI::mono_runtime_invoke(method, obj, nullptr, reinterpret_cast<void**>(&exc)));
        if (exc || !result) {
            return false;
        }

        Vector3* ptr = reinterpret_cast<Vector3*>(MonoAPI::mono_object_unbox(result));
        if (!ptr) {
            return false;
        }

        outVec = *ptr;
        return true;
    }

    bool WorldToScreen(void* cameraObj, const Vector3& world, Vector3& outScreen) {
        if (!cameraObj || !mCameraWorldToScreen || !MonoAPI::mono_runtime_invoke || !MonoAPI::mono_object_unbox) {
            return false;
        }

        void* args[1] = { const_cast<Vector3*>(&world) };
        MonoObject* exc = nullptr;
        MonoObject* result = reinterpret_cast<MonoObject*>(
            MonoAPI::mono_runtime_invoke(mCameraWorldToScreen, cameraObj, args, reinterpret_cast<void**>(&exc)));
        if (exc || !result) {
            return false;
        }

        Vector3* ptr = reinterpret_cast<Vector3*>(MonoAPI::mono_object_unbox(result));
        if (!ptr) {
            return false;
        }

        outScreen = *ptr;
        return true;
    }

    void UpdateTrackedPlayer(void* character) {
        if (!character || !oCharacter_get_IsLocal || !oCharacter_get_characterName) {
            return;
        }

        TrackedPlayer incoming{};
        incoming.character = character;
        incoming.isLocal = oCharacter_get_IsLocal(character);
        incoming.name = ToUtf8(oCharacter_get_characterName(character));
        incoming.hasProjection = false;
        incoming.headX = 0.0f;
        incoming.headY = 0.0f;
        incoming.headZ = -1.0f;
        incoming.feetX = 0.0f;
        incoming.feetY = 0.0f;
        incoming.feetZ = -1.0f;
        incoming.lastSeenMs = GetTickCount64();

        if (mCharacterGetHead && mCharacterGetCenter && mCameraGetMain && mCameraWorldToScreen && MonoAPI::AttachCurrentThread()) {
            MonoObject* camExc = nullptr;
            void* cameraObj = MonoAPI::mono_runtime_invoke(mCameraGetMain, nullptr, nullptr, reinterpret_cast<void**>(&camExc));
            if (!camExc && cameraObj) {
                Vector3 headWorld{};
                Vector3 centerWorld{};
                if (InvokeVector3(mCharacterGetHead, character, headWorld) && InvokeVector3(mCharacterGetCenter, character, centerWorld)) {
                    Vector3 feetWorld{};
                    feetWorld.x = centerWorld.x;
                    feetWorld.y = centerWorld.y - std::fabs(headWorld.y - centerWorld.y) * 1.25f;
                    feetWorld.z = centerWorld.z;

                    Vector3 headScreen{};
                    Vector3 feetScreen{};
                    if (WorldToScreen(cameraObj, headWorld, headScreen) && WorldToScreen(cameraObj, feetWorld, feetScreen)) {
                        incoming.hasProjection = (headScreen.z > 0.05f && feetScreen.z > 0.05f);
                        incoming.headX = headScreen.x;
                        incoming.headY = headScreen.y;
                        incoming.headZ = headScreen.z;
                        incoming.feetX = feetScreen.x;
                        incoming.feetY = feetScreen.y;
                        incoming.feetZ = feetScreen.z;
                    }
                }
            }
        }

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
                return now - player.lastSeenMs > kPlayerFreshMs;
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
    PlayerESPModule::mCharacterGetHead = MonoAPI::GetMethod("Character", "get_Head", 0);
    PlayerESPModule::mCharacterGetCenter = MonoAPI::GetMethod("Character", "get_Center", 0);
    PlayerESPModule::mCameraGetMain = MonoAPI::GetCoreMethod("Camera", "get_main", 0);
    PlayerESPModule::mCameraWorldToScreen = MonoAPI::GetCoreMethod("Camera", "WorldToScreenPoint", 1);

    if (!pUpdate || !pGetIsLocal || !pGetCharacterName) {
        std::cout << "[ENI-PlayerESP] Missing Character methods. ESP disabled." << std::endl;
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

    const ULONGLONG now = GetTickCount64();
    snapshot.erase(
        std::remove_if(snapshot.begin(), snapshot.end(), [now](const PlayerESPModule::TrackedPlayer& player) {
            return now - player.lastSeenMs > PlayerESPModule::kPlayerFreshMs;
        }),
        snapshot.end());

    const bool hasLocal = std::any_of(snapshot.begin(), snapshot.end(), [](const PlayerESPModule::TrackedPlayer& player) {
        return player.isLocal;
    });

    std::sort(snapshot.begin(), snapshot.end(), [](const PlayerESPModule::TrackedPlayer& a, const PlayerESPModule::TrackedPlayer& b) {
        if (a.isLocal != b.isLocal) {
            return a.isLocal;
        }
        return a.name < b.name;
    });
    const int remoteCount = static_cast<int>(std::count_if(snapshot.begin(), snapshot.end(), [](const PlayerESPModule::TrackedPlayer& p) {
        return !p.isLocal;
    }));

    std::vector<std::pair<std::string, ImU32>> lines;
    lines.reserve(snapshot.size() + 2);
    lines.emplace_back("PEAK+ ESP Overlay", IM_COL32(255, 80, 80, 255));
    lines.emplace_back(
        "Tracked: " + std::to_string(snapshot.size()) + " | Remote: " + std::to_string(remoteCount),
        IM_COL32(210, 210, 210, 255));

    if (!hasLocal) {
        lines.emplace_back("Local player not resolved yet.", IM_COL32(255, 200, 80, 255));
    }

    for (const auto& player : snapshot) {
        const std::string label = std::string(player.isLocal ? "[LOCAL] " : "[REMOTE] ") + player.name;
        const ImU32 color = player.isLocal ? IM_COL32(110, 255, 130, 255) : IM_COL32(255, 220, 110, 255);
        lines.emplace_back(label, color);
    }

    ImDrawList* draw = ImGui::GetForegroundDrawList();
    ImGuiIO& io = ImGui::GetIO();

    const float padding = 10.0f;
    const float lineGap = 4.0f;
    float maxWidth = 0.0f;
    float totalHeight = padding * 2.0f;
    for (const auto& line : lines) {
        const ImVec2 sz = ImGui::CalcTextSize(line.first.c_str());
        if (sz.x > maxWidth) {
            maxWidth = sz.x;
        }
        totalHeight += sz.y + lineGap;
    }
    if (!lines.empty()) {
        totalHeight -= lineGap;
    }

    const ImVec2 panelMin(io.DisplaySize.x - maxWidth - padding * 2.0f - 20.0f, 20.0f);
    const ImVec2 panelMax(panelMin.x + maxWidth + padding * 2.0f, panelMin.y + totalHeight);

    draw->AddRectFilled(panelMin, panelMax, IM_COL32(15, 15, 20, 165), 8.0f);
    draw->AddRect(panelMin, panelMax, IM_COL32(255, 90, 90, 180), 8.0f, 0, 1.2f);

    float y = panelMin.y + padding;
    for (const auto& line : lines) {
        const ImVec2 pos(panelMin.x + padding, y);
        draw->AddText(ImVec2(pos.x + 1.0f, pos.y + 1.0f), IM_COL32(0, 0, 0, 220), line.first.c_str());
        draw->AddText(pos, line.second, line.first.c_str());
        y += ImGui::CalcTextSize(line.first.c_str()).y + lineGap;
    }

    const ImU32 boxColor = IM_COL32(255, 80, 80, 220);
    const ImU32 boxOutline = IM_COL32(0, 0, 0, 220);
    const ImU32 nameColor = IM_COL32(255, 230, 160, 255);
    const float screenH = io.DisplaySize.y;

    for (const auto& player : snapshot) {
        if (player.isLocal || !player.hasProjection) {
            continue;
        }

        float headY = screenH - player.headY;
        float feetY = screenH - player.feetY;
        if (feetY < headY) {
            std::swap(feetY, headY);
        }

        const float boxH = feetY - headY;
        if (boxH < 10.0f) {
            continue;
        }
        const float boxW = boxH * 0.46f;
        const float x1 = player.headX - boxW * 0.5f;
        const float x2 = player.headX + boxW * 0.5f;
        const float y1 = headY;
        const float y2 = feetY;

        draw->AddRect(ImVec2(x1 - 1.0f, y1 - 1.0f), ImVec2(x2 + 1.0f, y2 + 1.0f), boxOutline, 0.0f, 0, 2.0f);
        draw->AddRect(ImVec2(x1, y1), ImVec2(x2, y2), boxColor, 0.0f, 0, 1.2f);

        const ImVec2 nameSize = ImGui::CalcTextSize(player.name.c_str());
        const ImVec2 namePos(player.headX - nameSize.x * 0.5f, y1 - nameSize.y - 4.0f);
        draw->AddText(ImVec2(namePos.x + 1.0f, namePos.y + 1.0f), boxOutline, player.name.c_str());
        draw->AddText(namePos, nameColor, player.name.c_str());
    }
}

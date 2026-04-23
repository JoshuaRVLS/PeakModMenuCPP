#include "Menu.h"
#include <imgui.h>
#include "../Features/InventoryEditor.h"

namespace Menu {
    bool bShowMenu = true;
    
    // Player State
    bool bGodMode = false;
    bool bInfiniteStamina = false;
    bool bNoHunger = false;
    bool bNoPoison = false;
    bool bNoCold = false;
    bool bNoHot = false;
    bool bNoDrowsy = false;
    bool bNoCurse = false;
    bool bNoFallDamage = false;
    bool bSuperJump = false;
    bool bNoAnimation = false;
    bool bNoCooldown = false;
    bool bFly = false;
    bool bTeleport = false;
    bool bPlayerESP = false;
    bool bAntiKick = false;
    bool bAntiInventoryStrip = false;
    bool bInventoryEditor = false;

    void Render() {
        if (!bShowMenu) return;

        static char itemNameBuf[64] = "RopeShooter";
        static int removeSlot = 0;
        static bool lastOpSuccess = false;
        static const char* lastOpMessage = "No action yet.";

        int activeCount = 0;
        activeCount += bGodMode ? 1 : 0;
        activeCount += bInfiniteStamina ? 1 : 0;
        activeCount += bNoHunger ? 1 : 0;
        activeCount += bNoPoison ? 1 : 0;
        activeCount += bNoCold ? 1 : 0;
        activeCount += bNoHot ? 1 : 0;
        activeCount += bNoDrowsy ? 1 : 0;
        activeCount += bNoCurse ? 1 : 0;
        activeCount += bNoFallDamage ? 1 : 0;
        activeCount += bSuperJump ? 1 : 0;
        activeCount += bNoAnimation ? 1 : 0;
        activeCount += bNoCooldown ? 1 : 0;
        activeCount += bFly ? 1 : 0;
        activeCount += bTeleport ? 1 : 0;
        activeCount += bPlayerESP ? 1 : 0;
        activeCount += bAntiKick ? 1 : 0;
        activeCount += bAntiInventoryStrip ? 1 : 0;
        activeCount += bInventoryEditor ? 1 : 0;

        ImGui::SetNextWindowSize(ImVec2(600, 560), ImGuiCond_FirstUseEver);
        ImGui::Begin("PEAK+ Control Center", &bShowMenu);

        ImGui::TextColored(ImVec4(0.95f, 0.35f, 0.35f, 1.0f), "ENGINE: DX11/DX12 Interception");
        ImGui::SameLine();
        ImGui::TextDisabled("| Active Features: %d", activeCount);
        ImGui::Separator();

        if (ImGui::BeginTabBar("PeakTabs")) {
            if (ImGui::BeginTabItem("Player")) {
                float halfWidth = (ImGui::GetContentRegionAvail().x - ImGui::GetStyle().ItemSpacing.x) * 0.5f;

                ImGui::BeginChild("CoreControls", ImVec2(halfWidth, 290), true);
                ImGui::Text("Core Controls");
                ImGui::Separator();
                ImGui::Checkbox("God Mode", &bGodMode);
                ImGui::Checkbox("Infinite Stamina", &bInfiniteStamina);
                ImGui::Checkbox("No Fall Damage", &bNoFallDamage);
                ImGui::Checkbox("Super Jump", &bSuperJump);
                ImGui::Checkbox("No Animation (Disabled: stability mode)", &bNoAnimation);
                ImGui::Checkbox("No Cooldown (Disabled: stability mode)", &bNoCooldown);
                ImGui::Checkbox("Fly (SPACE up / CTRL down)", &bFly);
                ImGui::Checkbox("Teleport (F7 spawn)", &bTeleport);
                ImGui::Checkbox("Player ESP", &bPlayerESP);
                ImGui::Checkbox("Inventory Editor", &bInventoryEditor);
                ImGui::EndChild();

                ImGui::SameLine();

                ImGui::BeginChild("StatusFilters", ImVec2(0, 290), true);
                ImGui::Text("Status Filters");
                ImGui::Separator();
                ImGui::Checkbox("No Hunger", &bNoHunger);
                ImGui::Checkbox("No Poison", &bNoPoison);
                ImGui::Checkbox("No Cold", &bNoCold);
                ImGui::Checkbox("No Hot", &bNoHot);
                ImGui::Checkbox("No Drowsy", &bNoDrowsy);
                ImGui::Checkbox("No Curse", &bNoCurse);
                ImGui::EndChild();

                ImGui::Spacing();
                ImGui::BeginChild("NetworkSafety", ImVec2(0, 95), true);
                ImGui::Text("Networking Safety");
                ImGui::Separator();
                ImGui::Checkbox("Anti Kick (Disabled: stability mode)", &bAntiKick);
                ImGui::Checkbox("Anti Inventory Strip (Block RPCRemoveItemFromSlot)", &bAntiInventoryStrip);
                ImGui::EndChild();

                ImGui::Spacing();
                ImGui::TextDisabled("Tip: Press INSERT anytime to hide/show this menu.");
                ImGui::EndTabItem();
            }

            if (ImGui::BeginTabItem("Inventory")) {
                ImGui::BeginChild("InventoryEditorPanel", ImVec2(0, 0), true);
                ImGui::Text("Inventory Editor");
                ImGui::Separator();

                ImGui::Checkbox("Enable Inventory Actions", &bInventoryEditor);
                ImGui::Spacing();

                bool invReady = InventoryEditor::IsReady();
                ImVec4 hookColor = invReady ? ImVec4(0.45f, 1.0f, 0.45f, 1.0f) : ImVec4(1.0f, 0.75f, 0.3f, 1.0f);
                ImGui::TextColored(hookColor, "Hook State: %s", invReady ? "Ready" : "Waiting for local character...");
                ImGui::TextDisabled("Use prefab name from 0_Items (example: RopeShooter)");
                ImGui::InputText("Item Name", itemNameBuf, IM_ARRAYSIZE(itemNameBuf));

                ImGui::BeginDisabled(!bInventoryEditor);
                if (ImGui::Button("Add Item To Hand", ImVec2(220, 0))) {
                    lastOpSuccess = InventoryEditor::AddItemByName(itemNameBuf);
                    lastOpMessage = lastOpSuccess ? "Add request sent." : "Add failed (missing local refs or invalid name).";
                }

                ImGui::Spacing();
                ImGui::SliderInt("Target Slot", &removeSlot, 0, 3);
                if (ImGui::Button("Remove Item In Slot", ImVec2(220, 0))) {
                    lastOpSuccess = InventoryEditor::RemoveSlot(removeSlot);
                    lastOpMessage = lastOpSuccess ? "Remove request sent." : "Remove failed (missing local refs).";
                }
                ImGui::EndDisabled();

                ImGui::Spacing();
                ImVec4 statusColor = lastOpSuccess ? ImVec4(0.45f, 1.0f, 0.45f, 1.0f) : ImVec4(1.0f, 0.5f, 0.5f, 1.0f);
                ImGui::TextColored(statusColor, "Last Action: %s", lastOpMessage);
                ImGui::EndChild();
                ImGui::EndTabItem();
            }

            if (ImGui::BeginTabItem("Info")) {
                ImGui::BeginChild("InfoPanel", ImVec2(0, 0), true);
                ImGui::Text("Hotkeys");
                ImGui::Separator();
                ImGui::BulletText("INSERT: Open/Close menu");
                ImGui::BulletText("END: Eject DLL");
                ImGui::Spacing();
                ImGui::Text("Session");
                ImGui::Separator();
                ImGui::Text("Features Enabled: %d", activeCount);
                ImGui::TextDisabled("Keep inventory actions disabled when not needed.");
                ImGui::EndChild();
                ImGui::EndTabItem();
            }

            ImGui::EndTabBar();
        }

        ImGui::End();
    }
}

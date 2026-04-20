#include "Menu.h"
#include <imgui.h>
#include "../Features/InventoryEditor.h"

namespace Menu {
    bool bShowMenu = true;
    
    // Player State
    bool bGodMode = false;
    bool bInfiniteStamina = false;
    bool bNoHunger = false;
    bool bNoFallDamage = false;
    bool bSuperJump = false;
    bool bPlayerESP = false;
    bool bInventoryEditor = false;

    void Render() {
        if (!bShowMenu) return;

        static char itemNameBuf[64] = "RopeShooter";
        static int removeSlot = 0;
        static bool lastOpSuccess = false;
        static const char* lastOpMessage = "No action yet.";

        ImGui::SetNextWindowSize(ImVec2(430, 420), ImGuiCond_FirstUseEver);
        ImGui::Begin("PEAK+ Goddess Menu", &bShowMenu);
        
        ImGui::TextColored(ImVec4(0.8f, 0.2f, 0.2f, 1.0f), "[ Raw D3D11 Interception Active ]");
        ImGui::Separator();
        
        ImGui::Spacing();
        ImGui::Text("Player Controls");
        ImGui::Separator();
        ImGui::Checkbox("God Mode (Indestructible)", &bGodMode);
        ImGui::Checkbox("Infinite Stamina", &bInfiniteStamina);
        ImGui::Checkbox("No Hunger", &bNoHunger);
        ImGui::Checkbox("No Fall Damage", &bNoFallDamage);
        ImGui::Checkbox("Super Jump (Pal Jump Force)", &bSuperJump);
        ImGui::Checkbox("Player ESP", &bPlayerESP);
        ImGui::Checkbox("Inventory Editor", &bInventoryEditor);

        if (bInventoryEditor) {
            ImGui::Spacing();
            ImGui::Text("Inventory Editor");
            ImGui::Separator();
            ImGui::InputText("Item Name", itemNameBuf, IM_ARRAYSIZE(itemNameBuf));
            ImGui::TextDisabled("Use prefab name from 0_Items (example: RopeShooter).");

            if (ImGui::Button("Add Item To Hand", ImVec2(190, 0))) {
                lastOpSuccess = InventoryEditor::AddItemByName(itemNameBuf);
                lastOpMessage = lastOpSuccess ? "Add request sent." : "Add failed (missing local refs or invalid name).";
            }

            ImGui::Spacing();
            ImGui::SliderInt("Remove Slot", &removeSlot, 0, 3);
            if (ImGui::Button("Remove Item In Slot", ImVec2(190, 0))) {
                lastOpSuccess = InventoryEditor::RemoveSlot(removeSlot);
                lastOpMessage = lastOpSuccess ? "Remove request sent." : "Remove failed (missing local refs).";
            }

            ImVec4 statusColor = lastOpSuccess ? ImVec4(0.4f, 1.0f, 0.4f, 1.0f) : ImVec4(1.0f, 0.5f, 0.5f, 1.0f);
            ImGui::TextColored(statusColor, "%s", lastOpMessage);
            ImGui::TextDisabled("Inventory hook: %s", InventoryEditor::IsReady() ? "Ready" : "Waiting for local character...");
        }

        ImGui::End();
    }
}

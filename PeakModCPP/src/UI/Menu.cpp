#include "Menu.h"
#include <imgui.h>

namespace Menu {
    bool bShowMenu = true;
    
    // Player State
    bool bGodMode = false;
    bool bInfiniteStamina = false;
    void Render() {
        if (!bShowMenu) return;

        ImGui::SetNextWindowSize(ImVec2(350, 200), ImGuiCond_FirstUseEver);
        ImGui::Begin("PEAK+ Goddess Menu", &bShowMenu);
        
        ImGui::TextColored(ImVec4(0.8f, 0.2f, 0.2f, 1.0f), "[ Raw D3D11 Interception Active ]");
        ImGui::Separator();
        
        ImGui::Spacing();
        ImGui::Text("Player Controls");
        ImGui::Separator();
        ImGui::Checkbox("God Mode (Indestructible)", &bGodMode);
        ImGui::Checkbox("Infinite Stamina", &bInfiniteStamina);

        ImGui::End();
    }
}

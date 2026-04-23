#include "Theme.h"

namespace Theme {

    // Accent: deep crimson-red  #C8284A  → 0.784, 0.157, 0.290
    // BG:     near-black slate  #0D0F14
    // Panel:  dark navy         #13151C
    // Frame:  muted steel       #1E2130

    static ImVec4 Col(float r, float g, float b, float a = 1.0f) {
        return ImVec4(r, g, b, a);
    }

    void Apply() {
        ImGuiStyle& s = ImGui::GetStyle();

        // ── Rounding & borders ───────────────────────────────────────────
        s.WindowRounding    = 8.0f;
        s.FrameRounding     = 5.0f;
        s.ChildRounding     = 5.0f;
        s.PopupRounding     = 5.0f;
        s.ScrollbarRounding = 5.0f;
        s.GrabRounding      = 4.0f;
        s.TabRounding       = 5.0f;

        s.WindowBorderSize  = 1.0f;
        s.FrameBorderSize   = 0.0f;
        s.ChildBorderSize   = 1.0f;

        // ── Padding & spacing ─────────────────────────────────────────────
        s.WindowPadding     = ImVec2(12.0f, 10.0f);
        s.FramePadding      = ImVec2( 8.0f,  4.0f);
        s.ItemSpacing       = ImVec2( 8.0f,  6.0f);
        s.ItemInnerSpacing  = ImVec2( 6.0f,  4.0f);
        s.IndentSpacing     = 18.0f;
        s.ScrollbarSize     = 10.0f;
        s.GrabMinSize       = 10.0f;

        // ── Colours ───────────────────────────────────────────────────────
        ImVec4* c = s.Colors;

        // Backgrounds
        c[ImGuiCol_WindowBg]          = Col(0.051f, 0.059f, 0.078f, 0.97f);  // #0D0F14
        c[ImGuiCol_ChildBg]           = Col(0.075f, 0.082f, 0.110f, 1.00f);  // #13151C
        c[ImGuiCol_PopupBg]           = Col(0.062f, 0.070f, 0.094f, 0.98f);
        c[ImGuiCol_FrameBg]           = Col(0.118f, 0.129f, 0.188f, 1.00f);  // #1E2130
        c[ImGuiCol_FrameBgHovered]    = Col(0.160f, 0.173f, 0.239f, 1.00f);
        c[ImGuiCol_FrameBgActive]     = Col(0.196f, 0.212f, 0.286f, 1.00f);

        // Title bar
        c[ImGuiCol_TitleBg]           = Col(0.043f, 0.047f, 0.063f, 1.00f);
        c[ImGuiCol_TitleBgActive]     = Col(0.784f, 0.157f, 0.290f, 0.90f);  // crimson
        c[ImGuiCol_TitleBgCollapsed]  = Col(0.043f, 0.047f, 0.063f, 0.75f);

        // Menu bar
        c[ImGuiCol_MenuBarBg]         = Col(0.043f, 0.047f, 0.063f, 1.00f);

        // Scrollbar
        c[ImGuiCol_ScrollbarBg]       = Col(0.043f, 0.047f, 0.063f, 0.60f);
        c[ImGuiCol_ScrollbarGrab]     = Col(0.784f, 0.157f, 0.290f, 0.70f);
        c[ImGuiCol_ScrollbarGrabHovered] = Col(0.860f, 0.220f, 0.360f, 1.00f);
        c[ImGuiCol_ScrollbarGrabActive]  = Col(0.980f, 0.260f, 0.400f, 1.00f);

        // Borders
        c[ImGuiCol_Border]            = Col(0.784f, 0.157f, 0.290f, 0.30f);
        c[ImGuiCol_BorderShadow]      = Col(0.000f, 0.000f, 0.000f, 0.00f);

        // Buttons
        c[ImGuiCol_Button]            = Col(0.784f, 0.157f, 0.290f, 0.70f);
        c[ImGuiCol_ButtonHovered]     = Col(0.860f, 0.220f, 0.360f, 1.00f);
        c[ImGuiCol_ButtonActive]      = Col(0.980f, 0.260f, 0.400f, 1.00f);

        // Checkmarks & sliders
        c[ImGuiCol_CheckMark]         = Col(0.980f, 0.260f, 0.400f, 1.00f);
        c[ImGuiCol_SliderGrab]        = Col(0.784f, 0.157f, 0.290f, 1.00f);
        c[ImGuiCol_SliderGrabActive]  = Col(0.980f, 0.260f, 0.400f, 1.00f);

        // Headers (collapsing / selectable)
        c[ImGuiCol_Header]            = Col(0.784f, 0.157f, 0.290f, 0.35f);
        c[ImGuiCol_HeaderHovered]     = Col(0.784f, 0.157f, 0.290f, 0.60f);
        c[ImGuiCol_HeaderActive]      = Col(0.784f, 0.157f, 0.290f, 0.90f);

        // Separator
        c[ImGuiCol_Separator]         = Col(0.784f, 0.157f, 0.290f, 0.40f);
        c[ImGuiCol_SeparatorHovered]  = Col(0.860f, 0.220f, 0.360f, 0.80f);
        c[ImGuiCol_SeparatorActive]   = Col(0.980f, 0.260f, 0.400f, 1.00f);

        // Resize grip
        c[ImGuiCol_ResizeGrip]        = Col(0.784f, 0.157f, 0.290f, 0.25f);
        c[ImGuiCol_ResizeGripHovered] = Col(0.860f, 0.220f, 0.360f, 0.60f);
        c[ImGuiCol_ResizeGripActive]  = Col(0.980f, 0.260f, 0.400f, 1.00f);

        // Tabs
        c[ImGuiCol_Tab]               = Col(0.075f, 0.082f, 0.110f, 1.00f);
        c[ImGuiCol_TabHovered]        = Col(0.784f, 0.157f, 0.290f, 0.70f);
        c[ImGuiCol_TabActive]         = Col(0.784f, 0.157f, 0.290f, 1.00f);
        c[ImGuiCol_TabUnfocused]      = Col(0.075f, 0.082f, 0.110f, 1.00f);
        c[ImGuiCol_TabUnfocusedActive]= Col(0.500f, 0.100f, 0.190f, 1.00f);

        // Text
        c[ImGuiCol_Text]              = Col(0.92f, 0.92f, 0.96f, 1.00f);
        c[ImGuiCol_TextDisabled]      = Col(0.45f, 0.45f, 0.52f, 1.00f);
        c[ImGuiCol_TextSelectedBg]    = Col(0.784f, 0.157f, 0.290f, 0.35f);

        // Drag-drop / nav
        c[ImGuiCol_DragDropTarget]    = Col(0.980f, 0.260f, 0.400f, 1.00f);
        c[ImGuiCol_NavHighlight]      = Col(0.784f, 0.157f, 0.290f, 1.00f);
        c[ImGuiCol_NavWindowingHighlight] = Col(1.00f, 1.00f, 1.00f, 0.70f);
        c[ImGuiCol_NavWindowingDimBg] = Col(0.20f, 0.20f, 0.20f, 0.20f);
        c[ImGuiCol_ModalWindowDimBg]  = Col(0.20f, 0.20f, 0.20f, 0.60f);

        // Plot
        c[ImGuiCol_PlotLines]         = Col(0.784f, 0.157f, 0.290f, 1.00f);
        c[ImGuiCol_PlotLinesHovered]  = Col(0.980f, 0.260f, 0.400f, 1.00f);
        c[ImGuiCol_PlotHistogram]     = Col(0.784f, 0.157f, 0.290f, 1.00f);
        c[ImGuiCol_PlotHistogramHovered] = Col(0.980f, 0.260f, 0.400f, 1.00f);
    }
}

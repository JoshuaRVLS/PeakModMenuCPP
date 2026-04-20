#pragma once
#include "FeatureManager.h"

class PlayerESP : public IFeature {
public:
    void Initialize() override;
    static void RenderOverlay();
};

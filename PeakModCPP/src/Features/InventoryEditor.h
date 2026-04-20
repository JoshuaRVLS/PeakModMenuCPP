#pragma once
#include "FeatureManager.h"

class InventoryEditor : public IFeature {
public:
    void Initialize() override;

    static bool IsReady();
    static bool AddItemByName(const char* itemName);
    static bool RemoveSlot(int slotIndex);
};

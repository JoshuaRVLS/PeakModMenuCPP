#include "FeatureManager.h"

FeatureManager& FeatureManager::GetInstance() {
    static FeatureManager instance;
    return instance;
}

void FeatureManager::RegisterFeature(IFeature* feature) {
    features.push_back(feature);
}

void FeatureManager::InitializeAll() {
    for (auto feature : features) {
        feature->Initialize();
    }
}

void FeatureManager::DestroyAll() {
    for (auto feature : features) {
        delete feature;
    }
    features.clear();
}

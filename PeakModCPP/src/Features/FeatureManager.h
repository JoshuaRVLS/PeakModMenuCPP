#pragma once
#include <vector>

class IFeature {
public:
    virtual ~IFeature() = default;
    virtual void Initialize() = 0;
};

class FeatureManager {
private:
    std::vector<IFeature*> features;
public:
    static FeatureManager& GetInstance();
    void RegisterFeature(IFeature* feature);
    void InitializeAll();
    void DestroyAll();
};

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Zorro.Settings;

// Token: 0x020001A2 RID: 418
public class RenderScaleSetting : CustomLocalizedEnumSetting<RenderScaleSetting.RenderScaleQuality>, IExposedSetting
{
	// Token: 0x06000DA6 RID: 3494 RVA: 0x000458E0 File Offset: 0x00043AE0
	public override void ApplyValue()
	{
		UniversalRenderPipelineAsset universalRenderPipelineAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
		if (universalRenderPipelineAsset != null)
		{
			universalRenderPipelineAsset.renderScale = this.GetRenderScale(base.Value);
			Debug.Log(string.Format("Set Render Scale: {0}", universalRenderPipelineAsset.renderScale));
			if (base.Value == RenderScaleSetting.RenderScaleQuality.Native)
			{
				universalRenderPipelineAsset.upscalingFilter = UpscalingFilterSelection.Linear;
				return;
			}
			universalRenderPipelineAsset.upscalingFilter = UpscalingFilterSelection.STP;
		}
	}

	// Token: 0x06000DA7 RID: 3495 RVA: 0x00045940 File Offset: 0x00043B40
	public float GetRenderScale(RenderScaleSetting.RenderScaleQuality quality)
	{
		float result;
		switch (quality)
		{
		case RenderScaleSetting.RenderScaleQuality.Native:
			result = 1f;
			break;
		case RenderScaleSetting.RenderScaleQuality.High:
			result = 0.8f;
			break;
		case RenderScaleSetting.RenderScaleQuality.Medium:
			result = 0.4f;
			break;
		case RenderScaleSetting.RenderScaleQuality.Low:
			result = 0.2f;
			break;
		default:
			throw new ArgumentOutOfRangeException("quality", quality, null);
		}
		return result;
	}

	// Token: 0x06000DA8 RID: 3496 RVA: 0x00045998 File Offset: 0x00043B98
	protected override RenderScaleSetting.RenderScaleQuality GetDefaultValue()
	{
		if (SettingsHandler.IsOnSteamDeck)
		{
			return RenderScaleSetting.RenderScaleQuality.Medium;
		}
		return RenderScaleSetting.RenderScaleQuality.High;
	}

	// Token: 0x06000DA9 RID: 3497 RVA: 0x000459A4 File Offset: 0x00043BA4
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000DAA RID: 3498 RVA: 0x000459A7 File Offset: 0x00043BA7
	public string GetDisplayName()
	{
		return "Render Scale";
	}

	// Token: 0x06000DAB RID: 3499 RVA: 0x000459AE File Offset: 0x00043BAE
	public string GetCategory()
	{
		return "Graphics";
	}

	// Token: 0x020004BF RID: 1215
	public enum RenderScaleQuality
	{
		// Token: 0x04001ADA RID: 6874
		Native,
		// Token: 0x04001ADB RID: 6875
		High,
		// Token: 0x04001ADC RID: 6876
		Medium,
		// Token: 0x04001ADD RID: 6877
		Low
	}
}

using System;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Zorro.Settings;

// Token: 0x020001A4 RID: 420
public class ShadowDistanceSettings : CustomLocalizedEnumSetting<ShadowDistanceSettings.ShadowDistanceQuality>, IExposedSetting
{
	// Token: 0x06000DB1 RID: 3505 RVA: 0x000459DC File Offset: 0x00043BDC
	public override void ApplyValue()
	{
		UniversalRenderPipelineAsset universalRenderPipelineAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
		if (universalRenderPipelineAsset != null)
		{
			switch (base.Value)
			{
			case ShadowDistanceSettings.ShadowDistanceQuality.High:
				universalRenderPipelineAsset.shadowDistance = 200f;
				universalRenderPipelineAsset.shadowCascadeCount = 2;
				return;
			case ShadowDistanceSettings.ShadowDistanceQuality.Medium:
				universalRenderPipelineAsset.shadowDistance = 150f;
				universalRenderPipelineAsset.shadowCascadeCount = 2;
				return;
			case ShadowDistanceSettings.ShadowDistanceQuality.Low:
				universalRenderPipelineAsset.shadowDistance = 75f;
				universalRenderPipelineAsset.shadowCascadeCount = 1;
				return;
			case ShadowDistanceSettings.ShadowDistanceQuality.Off:
				universalRenderPipelineAsset.shadowDistance = 0f;
				universalRenderPipelineAsset.shadowCascadeCount = 1;
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x06000DB2 RID: 3506 RVA: 0x00045A60 File Offset: 0x00043C60
	protected override ShadowDistanceSettings.ShadowDistanceQuality GetDefaultValue()
	{
		if (SettingsHandler.IsOnSteamDeck)
		{
			return ShadowDistanceSettings.ShadowDistanceQuality.Low;
		}
		return ShadowDistanceSettings.ShadowDistanceQuality.Medium;
	}

	// Token: 0x06000DB3 RID: 3507 RVA: 0x00045A6C File Offset: 0x00043C6C
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000DB4 RID: 3508 RVA: 0x00045A6F File Offset: 0x00043C6F
	public string GetDisplayName()
	{
		return "Shadow Distance";
	}

	// Token: 0x06000DB5 RID: 3509 RVA: 0x00045A76 File Offset: 0x00043C76
	public string GetCategory()
	{
		return "Graphics";
	}

	// Token: 0x020004C0 RID: 1216
	public enum ShadowDistanceQuality
	{
		// Token: 0x04001ADF RID: 6879
		High,
		// Token: 0x04001AE0 RID: 6880
		Medium,
		// Token: 0x04001AE1 RID: 6881
		Low,
		// Token: 0x04001AE2 RID: 6882
		Off
	}
}

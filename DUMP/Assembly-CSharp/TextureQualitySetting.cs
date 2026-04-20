using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Rendering;
using Zorro.Settings;

// Token: 0x020001A5 RID: 421
public class TextureQualitySetting : CustomLocalizedEnumSetting<TextureQualitySetting.TextureQuality>, IExposedSetting
{
	// Token: 0x06000DB7 RID: 3511 RVA: 0x00045A88 File Offset: 0x00043C88
	public override void ApplyValue()
	{
		RenderPipelineAsset currentRenderPipeline = GraphicsSettings.currentRenderPipeline;
		switch (base.Value)
		{
		case TextureQualitySetting.TextureQuality.Native:
			QualitySettings.globalTextureMipmapLimit = 0;
			return;
		case TextureQualitySetting.TextureQuality.High:
			QualitySettings.globalTextureMipmapLimit = 1;
			return;
		case TextureQualitySetting.TextureQuality.Medium:
			QualitySettings.globalTextureMipmapLimit = 2;
			return;
		case TextureQualitySetting.TextureQuality.Low:
			QualitySettings.globalTextureMipmapLimit = 3;
			return;
		default:
			return;
		}
	}

	// Token: 0x06000DB8 RID: 3512 RVA: 0x00045AD4 File Offset: 0x00043CD4
	protected override TextureQualitySetting.TextureQuality GetDefaultValue()
	{
		if (SettingsHandler.IsOnSteamDeck)
		{
			return TextureQualitySetting.TextureQuality.High;
		}
		return TextureQualitySetting.TextureQuality.Native;
	}

	// Token: 0x06000DB9 RID: 3513 RVA: 0x00045AE0 File Offset: 0x00043CE0
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000DBA RID: 3514 RVA: 0x00045AE3 File Offset: 0x00043CE3
	public string GetDisplayName()
	{
		return "Texture Quality";
	}

	// Token: 0x06000DBB RID: 3515 RVA: 0x00045AEA File Offset: 0x00043CEA
	public string GetCategory()
	{
		return "Graphics";
	}

	// Token: 0x020004C1 RID: 1217
	public enum TextureQuality
	{
		// Token: 0x04001AE4 RID: 6884
		Native,
		// Token: 0x04001AE5 RID: 6885
		High,
		// Token: 0x04001AE6 RID: 6886
		Medium,
		// Token: 0x04001AE7 RID: 6887
		Low
	}
}

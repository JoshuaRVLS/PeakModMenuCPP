using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x0200019A RID: 410
public class LodQuality : CustomLocalizedEnumSetting<LodQuality.Quality>, IExposedSetting
{
	// Token: 0x06000D6F RID: 3439 RVA: 0x00045530 File Offset: 0x00043730
	public override void ApplyValue()
	{
		QualitySettings.lodBias = this.GetBias(base.Value);
	}

	// Token: 0x06000D70 RID: 3440 RVA: 0x00045543 File Offset: 0x00043743
	private float GetBias(LodQuality.Quality value)
	{
		if (value == LodQuality.Quality.High)
		{
			return 1f;
		}
		if (value == LodQuality.Quality.Medium)
		{
			return 0.85f;
		}
		return 0.7f;
	}

	// Token: 0x06000D71 RID: 3441 RVA: 0x0004555E File Offset: 0x0004375E
	protected override LodQuality.Quality GetDefaultValue()
	{
		if (SettingsHandler.IsOnSteamDeck)
		{
			return LodQuality.Quality.Low;
		}
		return LodQuality.Quality.Medium;
	}

	// Token: 0x06000D72 RID: 3442 RVA: 0x0004556A File Offset: 0x0004376A
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000D73 RID: 3443 RVA: 0x0004556D File Offset: 0x0004376D
	public string GetDisplayName()
	{
		return "World Quality";
	}

	// Token: 0x06000D74 RID: 3444 RVA: 0x00045574 File Offset: 0x00043774
	public string GetCategory()
	{
		return "Graphics";
	}

	// Token: 0x020004BA RID: 1210
	public enum Quality
	{
		// Token: 0x04001ACC RID: 6860
		Low,
		// Token: 0x04001ACD RID: 6861
		Medium,
		// Token: 0x04001ACE RID: 6862
		High
	}
}

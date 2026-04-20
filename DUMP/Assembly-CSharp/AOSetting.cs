using System;
using System.Collections.Generic;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x02000187 RID: 391
public class AOSetting : CustomLocalizedOffOnSetting, IExposedSetting
{
	// Token: 0x06000CEE RID: 3310 RVA: 0x00044FB3 File Offset: 0x000431B3
	public override void ApplyValue()
	{
	}

	// Token: 0x06000CEF RID: 3311 RVA: 0x00044FB5 File Offset: 0x000431B5
	protected override OffOnMode GetDefaultValue()
	{
		if (SettingsHandler.IsOnSteamDeck)
		{
			return OffOnMode.OFF;
		}
		return OffOnMode.ON;
	}

	// Token: 0x06000CF0 RID: 3312 RVA: 0x00044FC1 File Offset: 0x000431C1
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000CF1 RID: 3313 RVA: 0x00044FC4 File Offset: 0x000431C4
	public string GetDisplayName()
	{
		return "Ambient Occlusion";
	}

	// Token: 0x06000CF2 RID: 3314 RVA: 0x00044FCB File Offset: 0x000431CB
	public string GetCategory()
	{
		return "Graphics";
	}
}

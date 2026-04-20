using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x020001A7 RID: 423
public class VSyncSetting : CustomLocalizedEnumSetting<VSyncSetting.VSyncMode>, IExposedSetting
{
	// Token: 0x06000DC1 RID: 3521 RVA: 0x00045B17 File Offset: 0x00043D17
	public override void ApplyValue()
	{
		QualitySettings.vSyncCount = (int)base.Value;
	}

	// Token: 0x06000DC2 RID: 3522 RVA: 0x00045B24 File Offset: 0x00043D24
	protected override VSyncSetting.VSyncMode GetDefaultValue()
	{
		return (VSyncSetting.VSyncMode)QualitySettings.vSyncCount;
	}

	// Token: 0x06000DC3 RID: 3523 RVA: 0x00045B2B File Offset: 0x00043D2B
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000DC4 RID: 3524 RVA: 0x00045B2E File Offset: 0x00043D2E
	public string GetDisplayName()
	{
		return "Vsync";
	}

	// Token: 0x06000DC5 RID: 3525 RVA: 0x00045B35 File Offset: 0x00043D35
	public string GetCategory()
	{
		return "Graphics";
	}

	// Token: 0x06000DC6 RID: 3526 RVA: 0x00045B3C File Offset: 0x00043D3C
	public override List<string> GetUnlocalizedChoices()
	{
		return new List<string>
		{
			"OFF",
			"ON"
		};
	}

	// Token: 0x020004C2 RID: 1218
	public enum VSyncMode
	{
		// Token: 0x04001AE9 RID: 6889
		None,
		// Token: 0x04001AEA RID: 6890
		Enabled
	}
}

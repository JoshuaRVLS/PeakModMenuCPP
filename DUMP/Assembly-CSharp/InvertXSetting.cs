using System;
using System.Collections.Generic;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x02000195 RID: 405
public class InvertXSetting : CustomLocalizedOffOnSetting, IExposedSetting
{
	// Token: 0x06000D4A RID: 3402 RVA: 0x0004536A File Offset: 0x0004356A
	public override void ApplyValue()
	{
	}

	// Token: 0x06000D4B RID: 3403 RVA: 0x0004536C File Offset: 0x0004356C
	protected override OffOnMode GetDefaultValue()
	{
		return OffOnMode.OFF;
	}

	// Token: 0x06000D4C RID: 3404 RVA: 0x0004536F File Offset: 0x0004356F
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000D4D RID: 3405 RVA: 0x00045372 File Offset: 0x00043572
	public string GetDisplayName()
	{
		return "Invert X";
	}

	// Token: 0x06000D4E RID: 3406 RVA: 0x00045379 File Offset: 0x00043579
	public string GetCategory()
	{
		return "General";
	}
}

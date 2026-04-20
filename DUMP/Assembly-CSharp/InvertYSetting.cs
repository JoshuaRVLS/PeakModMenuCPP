using System;
using System.Collections.Generic;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x02000196 RID: 406
public class InvertYSetting : CustomLocalizedOffOnSetting, IExposedSetting
{
	// Token: 0x06000D50 RID: 3408 RVA: 0x00045388 File Offset: 0x00043588
	public override void ApplyValue()
	{
	}

	// Token: 0x06000D51 RID: 3409 RVA: 0x0004538A File Offset: 0x0004358A
	protected override OffOnMode GetDefaultValue()
	{
		return OffOnMode.OFF;
	}

	// Token: 0x06000D52 RID: 3410 RVA: 0x0004538D File Offset: 0x0004358D
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000D53 RID: 3411 RVA: 0x00045390 File Offset: 0x00043590
	public string GetDisplayName()
	{
		return "Invert Y";
	}

	// Token: 0x06000D54 RID: 3412 RVA: 0x00045397 File Offset: 0x00043597
	public string GetCategory()
	{
		return "General";
	}
}

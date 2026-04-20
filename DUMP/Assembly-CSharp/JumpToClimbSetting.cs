using System;
using System.Collections.Generic;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x02000197 RID: 407
public class JumpToClimbSetting : CustomLocalizedOffOnSetting, IExposedSetting, IConditionalSetting
{
	// Token: 0x06000D56 RID: 3414 RVA: 0x000453A6 File Offset: 0x000435A6
	public override void ApplyValue()
	{
	}

	// Token: 0x06000D57 RID: 3415 RVA: 0x000453A8 File Offset: 0x000435A8
	protected override OffOnMode GetDefaultValue()
	{
		return OffOnMode.ON;
	}

	// Token: 0x06000D58 RID: 3416 RVA: 0x000453AB File Offset: 0x000435AB
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000D59 RID: 3417 RVA: 0x000453AE File Offset: 0x000435AE
	public string GetDisplayName()
	{
		return "JUMPTOCLIMB";
	}

	// Token: 0x06000D5A RID: 3418 RVA: 0x000453B5 File Offset: 0x000435B5
	public string GetCategory()
	{
		return "General";
	}

	// Token: 0x06000D5B RID: 3419 RVA: 0x000453BC File Offset: 0x000435BC
	public bool ShouldShow()
	{
		return true;
	}
}

using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x0200018A RID: 394
public class CannibalismSetting : CustomLocalizedOffOnSetting, IExposedSetting, IConditionalSetting
{
	// Token: 0x06000D01 RID: 3329 RVA: 0x00045020 File Offset: 0x00043220
	public override void ApplyValue()
	{
	}

	// Token: 0x06000D02 RID: 3330 RVA: 0x00045022 File Offset: 0x00043222
	protected override OffOnMode GetDefaultValue()
	{
		return OffOnMode.ON;
	}

	// Token: 0x06000D03 RID: 3331 RVA: 0x00045025 File Offset: 0x00043225
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000D04 RID: 3332 RVA: 0x00045028 File Offset: 0x00043228
	public string GetDisplayName()
	{
		return "ENABLECANNIBALISM";
	}

	// Token: 0x06000D05 RID: 3333 RVA: 0x0004502F File Offset: 0x0004322F
	public string GetCategory()
	{
		return "Accessibility";
	}

	// Token: 0x06000D06 RID: 3334 RVA: 0x00045036 File Offset: 0x00043236
	public bool ShouldShow()
	{
		return !PhotonNetwork.InRoom;
	}
}

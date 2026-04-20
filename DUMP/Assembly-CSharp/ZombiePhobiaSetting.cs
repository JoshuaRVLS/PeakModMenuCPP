using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x020001A8 RID: 424
public class ZombiePhobiaSetting : CustomLocalizedOffOnSetting, IExposedSetting, IConditionalSetting
{
	// Token: 0x06000DC8 RID: 3528 RVA: 0x00045B61 File Offset: 0x00043D61
	public override void ApplyValue()
	{
	}

	// Token: 0x06000DC9 RID: 3529 RVA: 0x00045B63 File Offset: 0x00043D63
	protected override OffOnMode GetDefaultValue()
	{
		return OffOnMode.OFF;
	}

	// Token: 0x06000DCA RID: 3530 RVA: 0x00045B66 File Offset: 0x00043D66
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000DCB RID: 3531 RVA: 0x00045B69 File Offset: 0x00043D69
	public string GetDisplayName()
	{
		return "ZOMBIEPHOBIAMODE";
	}

	// Token: 0x06000DCC RID: 3532 RVA: 0x00045B70 File Offset: 0x00043D70
	public string GetCategory()
	{
		return "Accessibility";
	}

	// Token: 0x06000DCD RID: 3533 RVA: 0x00045B77 File Offset: 0x00043D77
	public bool ShouldShow()
	{
		return !PhotonNetwork.InRoom;
	}
}

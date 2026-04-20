using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x0200019B RID: 411
public class LookerSetting : CustomLocalizedOffOnSetting, IExposedSetting, IConditionalSetting
{
	// Token: 0x06000D76 RID: 3446 RVA: 0x00045583 File Offset: 0x00043783
	public override void ApplyValue()
	{
	}

	// Token: 0x06000D77 RID: 3447 RVA: 0x00045585 File Offset: 0x00043785
	protected override OffOnMode GetDefaultValue()
	{
		return OffOnMode.ON;
	}

	// Token: 0x06000D78 RID: 3448 RVA: 0x00045588 File Offset: 0x00043788
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000D79 RID: 3449 RVA: 0x0004558B File Offset: 0x0004378B
	public string GetDisplayName()
	{
		return "ENABLETHELOOKER";
	}

	// Token: 0x06000D7A RID: 3450 RVA: 0x00045592 File Offset: 0x00043792
	public string GetCategory()
	{
		return "Accessibility";
	}

	// Token: 0x06000D7B RID: 3451 RVA: 0x00045599 File Offset: 0x00043799
	public bool ShouldShow()
	{
		return !PhotonNetwork.InRoom;
	}
}

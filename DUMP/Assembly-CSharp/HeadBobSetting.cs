using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x02000194 RID: 404
public class HeadBobSetting : CustomLocalizedOffOnSetting, IExposedSetting, IConditionalSetting
{
	// Token: 0x06000D43 RID: 3395 RVA: 0x00045342 File Offset: 0x00043542
	public override void ApplyValue()
	{
	}

	// Token: 0x06000D44 RID: 3396 RVA: 0x00045344 File Offset: 0x00043544
	protected override OffOnMode GetDefaultValue()
	{
		return OffOnMode.OFF;
	}

	// Token: 0x06000D45 RID: 3397 RVA: 0x00045347 File Offset: 0x00043547
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000D46 RID: 3398 RVA: 0x0004534A File Offset: 0x0004354A
	public string GetDisplayName()
	{
		return "Reduce Camera Bobbing";
	}

	// Token: 0x06000D47 RID: 3399 RVA: 0x00045351 File Offset: 0x00043551
	public string GetCategory()
	{
		return "Accessibility";
	}

	// Token: 0x06000D48 RID: 3400 RVA: 0x00045358 File Offset: 0x00043558
	public bool ShouldShow()
	{
		return !PhotonNetwork.InRoom;
	}
}

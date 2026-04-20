using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x020001A0 RID: 416
public class PhotosensitiveSetting : CustomLocalizedOffOnSetting, IExposedSetting, IConditionalSetting
{
	// Token: 0x06000D98 RID: 3480 RVA: 0x00045872 File Offset: 0x00043A72
	public override void ApplyValue()
	{
	}

	// Token: 0x06000D99 RID: 3481 RVA: 0x00045874 File Offset: 0x00043A74
	protected override OffOnMode GetDefaultValue()
	{
		return OffOnMode.OFF;
	}

	// Token: 0x06000D9A RID: 3482 RVA: 0x00045877 File Offset: 0x00043A77
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000D9B RID: 3483 RVA: 0x0004587A File Offset: 0x00043A7A
	public string GetDisplayName()
	{
		return "PHOTOSENSITIVEMODE";
	}

	// Token: 0x06000D9C RID: 3484 RVA: 0x00045881 File Offset: 0x00043A81
	public string GetCategory()
	{
		return "Accessibility";
	}

	// Token: 0x06000D9D RID: 3485 RVA: 0x00045888 File Offset: 0x00043A88
	public bool ShouldShow()
	{
		return !PhotonNetwork.InRoom;
	}
}

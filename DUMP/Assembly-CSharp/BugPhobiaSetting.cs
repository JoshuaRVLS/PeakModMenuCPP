using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x02000189 RID: 393
public class BugPhobiaSetting : CustomLocalizedOffOnSetting, IExposedSetting, IConditionalSetting
{
	// Token: 0x06000CFA RID: 3322 RVA: 0x00044FF8 File Offset: 0x000431F8
	public override void ApplyValue()
	{
	}

	// Token: 0x06000CFB RID: 3323 RVA: 0x00044FFA File Offset: 0x000431FA
	protected override OffOnMode GetDefaultValue()
	{
		return OffOnMode.OFF;
	}

	// Token: 0x06000CFC RID: 3324 RVA: 0x00044FFD File Offset: 0x000431FD
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000CFD RID: 3325 RVA: 0x00045000 File Offset: 0x00043200
	public string GetDisplayName()
	{
		return "BUGPHOBIAMODE";
	}

	// Token: 0x06000CFE RID: 3326 RVA: 0x00045007 File Offset: 0x00043207
	public string GetCategory()
	{
		return "Accessibility";
	}

	// Token: 0x06000CFF RID: 3327 RVA: 0x0004500E File Offset: 0x0004320E
	public bool ShouldShow()
	{
		return !PhotonNetwork.InRoom;
	}
}

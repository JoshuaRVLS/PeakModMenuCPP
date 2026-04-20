using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x0200018B RID: 395
public class ColorblindSetting : CustomLocalizedOffOnSetting, IExposedSetting, IConditionalSetting
{
	// Token: 0x06000D08 RID: 3336 RVA: 0x00045048 File Offset: 0x00043248
	public override void ApplyValue()
	{
	}

	// Token: 0x06000D09 RID: 3337 RVA: 0x0004504A File Offset: 0x0004324A
	protected override OffOnMode GetDefaultValue()
	{
		return OffOnMode.OFF;
	}

	// Token: 0x06000D0A RID: 3338 RVA: 0x0004504D File Offset: 0x0004324D
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000D0B RID: 3339 RVA: 0x00045050 File Offset: 0x00043250
	public string GetDisplayName()
	{
		return "COLORBLINDNESSMODE";
	}

	// Token: 0x06000D0C RID: 3340 RVA: 0x00045057 File Offset: 0x00043257
	public string GetCategory()
	{
		return "Accessibility";
	}

	// Token: 0x06000D0D RID: 3341 RVA: 0x0004505E File Offset: 0x0004325E
	public bool ShouldShow()
	{
		return !PhotonNetwork.InRoom;
	}
}

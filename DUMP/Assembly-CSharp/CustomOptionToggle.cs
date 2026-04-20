using System;
using UnityEngine.UI;

// Token: 0x02000249 RID: 585
public class CustomOptionToggle : CustomOptionBase
{
	// Token: 0x060011C1 RID: 4545 RVA: 0x0005954F File Offset: 0x0005774F
	private void Refresh()
	{
		this.toggle.SetIsOnWithoutNotify(RunSettings.GetValue(this.settingType, true) == 1);
	}

	// Token: 0x060011C2 RID: 4546 RVA: 0x0005956B File Offset: 0x0005776B
	private void OnEnable()
	{
		this.Refresh();
	}

	// Token: 0x060011C3 RID: 4547 RVA: 0x00059573 File Offset: 0x00057773
	public override void OnClick()
	{
		base.OnClick();
		RunSettings.Increment(this.settingType, true);
		this.Refresh();
	}

	// Token: 0x060011C4 RID: 4548 RVA: 0x0005958D File Offset: 0x0005778D
	public override void RestoreDefault()
	{
		this.Refresh();
	}

	// Token: 0x04000FC1 RID: 4033
	public RunSettings.SETTINGTYPE settingType;

	// Token: 0x04000FC2 RID: 4034
	public Toggle toggle;
}

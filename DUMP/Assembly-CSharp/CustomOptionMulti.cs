using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000248 RID: 584
public class CustomOptionMulti : CustomOptionBase
{
	// Token: 0x060011BB RID: 4539 RVA: 0x00059430 File Offset: 0x00057630
	private void Refresh()
	{
		this.buttonText.SetIndex(this.labels[RunSettings.GetValue(this.settingType, true)].ToString());
		this.buttonImage.color = ((RunSettings.GetValue(this.settingType, true) == RunSettings.GetDefaultValue(this.settingType)) ? this.defaultColor : this.adjustedColor);
		if (this.tooltip)
		{
			this.tooltip.option = RunSettings.GetValue(this.settingType, true);
		}
	}

	// Token: 0x060011BC RID: 4540 RVA: 0x000594B6 File Offset: 0x000576B6
	private void OnEnable()
	{
		this.Refresh();
	}

	// Token: 0x060011BD RID: 4541 RVA: 0x000594BE File Offset: 0x000576BE
	public override void OnClick()
	{
		base.OnClick();
		RunSettings.Increment(this.settingType, true);
		this.Refresh();
		if (this.tooltip)
		{
			this.tooltip.ShowTooltip();
		}
	}

	// Token: 0x060011BE RID: 4542 RVA: 0x000594F0 File Offset: 0x000576F0
	public void Set()
	{
		this.label.SetIndex(this.settingType.ToString());
		base.gameObject.name = "Option_" + this.settingType.ToString();
	}

	// Token: 0x060011BF RID: 4543 RVA: 0x0005953F File Offset: 0x0005773F
	public override void RestoreDefault()
	{
		this.Refresh();
	}

	// Token: 0x04000FB7 RID: 4023
	public RunSettings.SETTINGTYPE settingType;

	// Token: 0x04000FB8 RID: 4024
	public Button button;

	// Token: 0x04000FB9 RID: 4025
	public LocalizedText buttonText;

	// Token: 0x04000FBA RID: 4026
	public LocalizedText label;

	// Token: 0x04000FBB RID: 4027
	public Image buttonImage;

	// Token: 0x04000FBC RID: 4028
	public Color offColor;

	// Token: 0x04000FBD RID: 4029
	public Color defaultColor;

	// Token: 0x04000FBE RID: 4030
	public Color adjustedColor;

	// Token: 0x04000FBF RID: 4031
	public CustomOptionTooltip tooltip;

	// Token: 0x04000FC0 RID: 4032
	public string[] labels;
}

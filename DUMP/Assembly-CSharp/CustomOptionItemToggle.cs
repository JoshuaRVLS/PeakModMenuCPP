using System;
using UnityEngine.UI;

// Token: 0x02000247 RID: 583
public class CustomOptionItemToggle : CustomOptionBase
{
	// Token: 0x060011B5 RID: 4533 RVA: 0x000593A3 File Offset: 0x000575A3
	public void Init(Item item)
	{
		this.item = item;
		this.text.SetIndex("NAME_" + item.UIData.itemName);
		this.Refresh();
	}

	// Token: 0x060011B6 RID: 4534 RVA: 0x000593D2 File Offset: 0x000575D2
	public void Refresh()
	{
		if (this.item)
		{
			this.toggle.SetIsOnWithoutNotify(RunSettings.GetItemValue(this.item, true) == 1);
		}
	}

	// Token: 0x060011B7 RID: 4535 RVA: 0x000593FB File Offset: 0x000575FB
	private void OnEnable()
	{
		this.Refresh();
	}

	// Token: 0x060011B8 RID: 4536 RVA: 0x00059403 File Offset: 0x00057603
	public override void OnClick()
	{
		base.OnClick();
		RunSettings.IncrementItem(this.item, true);
		this.Refresh();
	}

	// Token: 0x060011B9 RID: 4537 RVA: 0x0005941D File Offset: 0x0005761D
	public override void RestoreDefault()
	{
		this.Refresh();
	}

	// Token: 0x04000FB4 RID: 4020
	public Item item;

	// Token: 0x04000FB5 RID: 4021
	public Toggle toggle;

	// Token: 0x04000FB6 RID: 4022
	public LocalizedText text;
}

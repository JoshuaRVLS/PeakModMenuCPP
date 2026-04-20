using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001DB RID: 475
public class MenuWindowTabbed : MenuWindow
{
	// Token: 0x1700011F RID: 287
	// (get) Token: 0x06000F32 RID: 3890 RVA: 0x0004AF9E File Offset: 0x0004919E
	public virtual int startOnTab
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x06000F33 RID: 3891 RVA: 0x0004AFA1 File Offset: 0x000491A1
	internal override void Open()
	{
		this.InitTabs();
		base.Open();
		this.SelectTab(this.startOnTab);
	}

	// Token: 0x06000F34 RID: 3892 RVA: 0x0004AFBB File Offset: 0x000491BB
	protected virtual void InitTabs()
	{
	}

	// Token: 0x06000F35 RID: 3893 RVA: 0x0004AFC0 File Offset: 0x000491C0
	public void SelectTab(int index)
	{
		if (this.tabs.Count <= index || index < 0)
		{
			Debug.LogError(string.Format("{0} tried to select out of range tab: {1}", base.gameObject.name, index));
			return;
		}
		for (int i = 0; i < this.tabs.Count; i++)
		{
			if (i == index)
			{
				this.tabs[i].Open();
			}
			else
			{
				this.tabs[i].Close();
			}
		}
		this.currentTab = index;
	}

	// Token: 0x06000F36 RID: 3894 RVA: 0x0004B048 File Offset: 0x00049248
	public void SelectNextTab(bool forward)
	{
		this.currentTab += (forward ? 1 : -1);
		if (this.currentTab >= this.tabs.Count)
		{
			this.currentTab = 0;
		}
		else if (this.currentTab < 0)
		{
			this.currentTab = this.tabs.Count - 1;
		}
		this.SelectTab(this.currentTab);
	}

	// Token: 0x04000CC3 RID: 3267
	protected List<MenuWindow> tabs = new List<MenuWindow>();

	// Token: 0x04000CC4 RID: 3268
	private int currentTab;
}

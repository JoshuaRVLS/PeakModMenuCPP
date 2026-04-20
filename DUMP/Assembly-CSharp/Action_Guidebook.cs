using System;
using Zorro.Core;

// Token: 0x020000E9 RID: 233
public class Action_Guidebook : ItemAction
{
	// Token: 0x0600087E RID: 2174 RVA: 0x0002F4CD File Offset: 0x0002D6CD
	private void Awake()
	{
		this.guidebook = base.GetComponent<Guidebook>();
	}

	// Token: 0x0600087F RID: 2175 RVA: 0x0002F4DB File Offset: 0x0002D6DB
	public override void RunAction()
	{
		this.guidebook.ToggleGuidebook();
		if (this.isSinglePage)
		{
			Singleton<AchievementManager>.Instance.TriggerSeenGuidebookPage(this.singlePageIndex);
		}
	}

	// Token: 0x0400081F RID: 2079
	private Guidebook guidebook;

	// Token: 0x04000820 RID: 2080
	public bool isSinglePage;

	// Token: 0x04000821 RID: 2081
	public int singlePageIndex;
}

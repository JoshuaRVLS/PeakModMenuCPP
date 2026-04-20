using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x020000EB RID: 235
public class Action_IncrementSteamStat : ItemAction
{
	// Token: 0x06000888 RID: 2184 RVA: 0x0002F690 File Offset: 0x0002D890
	public override void RunAction()
	{
		int num = Singleton<AchievementManager>.Instance.IncrementSteamStat(this.statType, this.incrementAmount);
		Debug.Log(string.Format("New value of {0} is {1}", this.statType, num));
	}

	// Token: 0x04000823 RID: 2083
	public STEAMSTATTYPE statType;

	// Token: 0x04000824 RID: 2084
	public int incrementAmount;
}

using System;
using UnityEngine;

// Token: 0x02000102 RID: 258
public class Action_WarpToBiome : ItemAction
{
	// Token: 0x060008C4 RID: 2244 RVA: 0x000301C9 File Offset: 0x0002E3C9
	public override void RunAction()
	{
		Debug.Log("WARP TO " + this.segmentToWarpTo.ToString());
		MapHandler.JumpToSegment(this.segmentToWarpTo);
	}

	// Token: 0x04000859 RID: 2137
	public Segment segmentToWarpTo;
}

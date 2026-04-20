using System;
using UnityEngine;

// Token: 0x020000F6 RID: 246
public class Action_PlayItemAnimation : ItemAction
{
	// Token: 0x0600089F RID: 2207 RVA: 0x0002FA0E File Offset: 0x0002DC0E
	public override void RunAction()
	{
		this.anim.Play(this.animationName, 0, 0f);
	}

	// Token: 0x04000834 RID: 2100
	public Animator anim;

	// Token: 0x04000835 RID: 2101
	public string animationName;
}

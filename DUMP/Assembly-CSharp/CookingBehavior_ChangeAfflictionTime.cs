using System;

// Token: 0x02000053 RID: 83
public class CookingBehavior_ChangeAfflictionTime : AdditionalCookingBehavior
{
	// Token: 0x06000455 RID: 1109 RVA: 0x0001B1E6 File Offset: 0x000193E6
	protected override void TriggerBehaviour(int cookedAmount)
	{
		this.action.affliction.totalTime += this.change;
	}

	// Token: 0x040004C5 RID: 1221
	public Action_ApplyAffliction action;

	// Token: 0x040004C6 RID: 1222
	public float change;
}

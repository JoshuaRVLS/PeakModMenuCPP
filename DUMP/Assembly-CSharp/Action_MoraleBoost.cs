using System;

// Token: 0x020000F0 RID: 240
public class Action_MoraleBoost : ItemAction
{
	// Token: 0x06000893 RID: 2195 RVA: 0x0002F948 File Offset: 0x0002DB48
	public override void RunAction()
	{
		MoraleBoost.SpawnMoraleBoost(base.transform.position, this.boostRadius, this.baselineStaminaBoost, this.staminaBoostPerAdditionalScout, true, 1);
	}

	// Token: 0x0400082D RID: 2093
	public float boostRadius;

	// Token: 0x0400082E RID: 2094
	public float baselineStaminaBoost;

	// Token: 0x0400082F RID: 2095
	public float staminaBoostPerAdditionalScout;
}

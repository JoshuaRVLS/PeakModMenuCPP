using System;

// Token: 0x02000052 RID: 82
public class CookingBehavior_ModifyBugleWobble : AdditionalCookingBehavior
{
	// Token: 0x06000453 RID: 1107 RVA: 0x0001B1A4 File Offset: 0x000193A4
	protected override void TriggerBehaviour(int cookedAmount)
	{
		int num = 0;
		while (num < cookedAmount && num <= this.maxCooking)
		{
			this.source.pitchWobble += this.changePerCooking;
			num++;
		}
	}

	// Token: 0x040004C2 RID: 1218
	public float changePerCooking;

	// Token: 0x040004C3 RID: 1219
	public int maxCooking;

	// Token: 0x040004C4 RID: 1220
	public BugleSFX source;
}

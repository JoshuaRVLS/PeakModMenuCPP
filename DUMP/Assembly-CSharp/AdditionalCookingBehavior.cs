using System;

// Token: 0x02000049 RID: 73
[Serializable]
public abstract class AdditionalCookingBehavior
{
	// Token: 0x06000440 RID: 1088 RVA: 0x0001AF8A File Offset: 0x0001918A
	public void Cook(ItemCooking sourceScript, int cookedAmount)
	{
		this.itemCooking = sourceScript;
		if (this.onlyOnce && this.triggered)
		{
			return;
		}
		if (cookedAmount >= this.cookedAmountToTrigger)
		{
			this.TriggerBehaviour(cookedAmount);
			this.triggered = true;
		}
	}

	// Token: 0x06000441 RID: 1089 RVA: 0x0001AFBB File Offset: 0x000191BB
	protected virtual void TriggerBehaviour(int cookedAmount)
	{
	}

	// Token: 0x040004B3 RID: 1203
	public int cookedAmountToTrigger = 1;

	// Token: 0x040004B4 RID: 1204
	internal ItemCooking itemCooking;

	// Token: 0x040004B5 RID: 1205
	public bool onlyOnce;

	// Token: 0x040004B6 RID: 1206
	private bool triggered;
}

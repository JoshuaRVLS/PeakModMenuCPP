using System;

// Token: 0x0200004C RID: 76
public class CookingBehavior_Explode : AdditionalCookingBehavior
{
	// Token: 0x06000447 RID: 1095 RVA: 0x0001B04A File Offset: 0x0001924A
	protected override void TriggerBehaviour(int cookedAmount)
	{
		if (this.dontRunIfOutOfFuel && this.itemCooking.item.GetData<FloatItemData>(DataEntryKey.Fuel).Value <= 0f)
		{
			return;
		}
		this.itemCooking.Explode();
	}

	// Token: 0x040004B9 RID: 1209
	public bool dontRunIfOutOfFuel;
}

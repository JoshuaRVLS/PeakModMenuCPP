using System;

// Token: 0x0200004E RID: 78
public class CookingBehavior_Wreck : AdditionalCookingBehavior
{
	// Token: 0x0600044B RID: 1099 RVA: 0x0001B0C6 File Offset: 0x000192C6
	protected override void TriggerBehaviour(int cookedAmount)
	{
		this.itemCooking.Wreck();
	}
}

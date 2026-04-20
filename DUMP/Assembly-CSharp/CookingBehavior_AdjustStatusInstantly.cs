using System;

// Token: 0x0200004F RID: 79
public class CookingBehavior_AdjustStatusInstantly : AdditionalCookingBehavior
{
	// Token: 0x0600044D RID: 1101 RVA: 0x0001B0DC File Offset: 0x000192DC
	protected override void TriggerBehaviour(int cookedAmount)
	{
		if (this.itemCooking.item.holderCharacter)
		{
			this.itemCooking.item.holderCharacter.refs.afflictions.AdjustStatus(this.statusType, this.amount, false);
		}
	}

	// Token: 0x040004BD RID: 1213
	public CharacterAfflictions.STATUSTYPE statusType;

	// Token: 0x040004BE RID: 1214
	public float amount;
}

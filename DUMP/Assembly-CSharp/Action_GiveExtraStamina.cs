using System;

// Token: 0x020000E8 RID: 232
public class Action_GiveExtraStamina : ItemAction
{
	// Token: 0x0600087C RID: 2172 RVA: 0x0002F4B2 File Offset: 0x0002D6B2
	public override void RunAction()
	{
		base.character.AddExtraStamina(this.amount);
	}

	// Token: 0x0400081E RID: 2078
	public float amount;
}

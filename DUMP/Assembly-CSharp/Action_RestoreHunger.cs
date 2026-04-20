using System;

// Token: 0x020000F9 RID: 249
public class Action_RestoreHunger : ItemAction
{
	// Token: 0x060008A8 RID: 2216 RVA: 0x0002FBFB File Offset: 0x0002DDFB
	public override void RunAction()
	{
		base.character.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Hunger, this.restorationAmount, false, false);
	}

	// Token: 0x0400083F RID: 2111
	public float restorationAmount;
}

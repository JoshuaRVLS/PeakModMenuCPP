using System;

// Token: 0x020000E6 RID: 230
public class Action_Die : ItemAction
{
	// Token: 0x06000878 RID: 2168 RVA: 0x0002F471 File Offset: 0x0002D671
	public override void RunAction()
	{
		if (base.character)
		{
			base.character.Invoke("DieInstantly", 0.02f);
		}
	}
}

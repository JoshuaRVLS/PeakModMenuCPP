using System;

// Token: 0x020000E7 RID: 231
public class Action_Flare : ItemAction
{
	// Token: 0x0600087A RID: 2170 RVA: 0x0002F49D File Offset: 0x0002D69D
	public override void RunAction()
	{
		this.flare.LightFlare();
	}

	// Token: 0x0400081D RID: 2077
	public Flare flare;
}

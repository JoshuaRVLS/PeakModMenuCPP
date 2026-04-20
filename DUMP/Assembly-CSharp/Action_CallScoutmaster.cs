using System;

// Token: 0x020000E1 RID: 225
public class Action_CallScoutmaster : ItemAction
{
	// Token: 0x06000869 RID: 2153 RVA: 0x0002F158 File Offset: 0x0002D358
	public override void RunAction()
	{
		Scoutmaster scoutmaster;
		if (Scoutmaster.GetPrimaryScoutmaster(out scoutmaster))
		{
			scoutmaster.SetCurrentTarget(this.item.holderCharacter, this.forcedChaseTime);
		}
	}

	// Token: 0x04000816 RID: 2070
	public float forcedChaseTime;
}

using System;
using Peak.Afflictions;

// Token: 0x020000F1 RID: 241
public class Action_Numb : ItemAction
{
	// Token: 0x06000895 RID: 2197 RVA: 0x0002F978 File Offset: 0x0002DB78
	public override void RunAction()
	{
		Affliction_Numb affliction = new Affliction_Numb
		{
			totalTime = this.numbAmount
		};
		base.character.refs.afflictions.AddAffliction(affliction, false);
	}

	// Token: 0x04000830 RID: 2096
	public float numbAmount = 60f;
}

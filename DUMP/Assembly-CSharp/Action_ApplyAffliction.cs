using System;
using Peak.Afflictions;
using UnityEngine;

// Token: 0x020000DA RID: 218
public class Action_ApplyAffliction : ItemAction
{
	// Token: 0x06000858 RID: 2136 RVA: 0x0002EEB4 File Offset: 0x0002D0B4
	public override void RunAction()
	{
		if (this.affliction == null)
		{
			Debug.LogError("Your affliction is null bro");
			return;
		}
		base.character.refs.afflictions.AddAffliction(this.affliction, false);
		if (this.extraAfflictions != null)
		{
			foreach (Affliction affliction in this.extraAfflictions)
			{
				base.character.refs.afflictions.AddAffliction(affliction, false);
			}
		}
	}

	// Token: 0x0400080F RID: 2063
	[SerializeReference]
	public Affliction affliction;

	// Token: 0x04000810 RID: 2064
	[SerializeReference]
	public Affliction[] extraAfflictions;
}

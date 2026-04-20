using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000E2 RID: 226
public class Action_ClearAllStatus : ItemAction
{
	// Token: 0x0600086B RID: 2155 RVA: 0x0002F190 File Offset: 0x0002D390
	public override void RunAction()
	{
		int num = Enum.GetNames(typeof(CharacterAfflictions.STATUSTYPE)).Length;
		for (int i = 0; i < num; i++)
		{
			CharacterAfflictions.STATUSTYPE statustype = (CharacterAfflictions.STATUSTYPE)i;
			if ((!this.excludeCurse || statustype != CharacterAfflictions.STATUSTYPE.Curse) && !this.otherExclusions.Contains(statustype))
			{
				base.character.refs.afflictions.SubtractStatus(statustype, (float)Mathf.Abs(5), false, false);
			}
		}
	}

	// Token: 0x04000817 RID: 2071
	public bool excludeCurse = true;

	// Token: 0x04000818 RID: 2072
	public List<CharacterAfflictions.STATUSTYPE> otherExclusions = new List<CharacterAfflictions.STATUSTYPE>();
}

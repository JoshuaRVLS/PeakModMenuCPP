using System;
using Peak.Afflictions;
using UnityEngine;

// Token: 0x020000DC RID: 220
public class Action_ApplyInfiniteStamina : ItemAction
{
	// Token: 0x0600085D RID: 2141 RVA: 0x0002EF3C File Offset: 0x0002D13C
	public override void RunAction()
	{
		Debug.Log("Adding infinite stamina buff");
		base.character.refs.afflictions.AddAffliction(new Affliction_InfiniteStamina(this.buffTime), false);
	}

	// Token: 0x04000811 RID: 2065
	public float buffTime;

	// Token: 0x04000812 RID: 2066
	public float drowsyAmount = 0.25f;
}

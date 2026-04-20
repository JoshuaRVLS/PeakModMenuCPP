using System;
using UnityEngine;

// Token: 0x02000123 RID: 291
public class LuggageCursed : Luggage
{
	// Token: 0x06000992 RID: 2450 RVA: 0x00032EDC File Offset: 0x000310DC
	public override void Interact_CastFinished(Character interactor)
	{
		if (!interactor.IsLocal)
		{
			return;
		}
		float num = (float)Random.Range(this.minCurse, this.maxCurse + 1) * 0.025f;
		if (num > 0f)
		{
			interactor.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Curse, num, false, true, true);
		}
		if (interactor.data.isSkeleton)
		{
			interactor.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, this.injuryAmt * 0.125f, false, true, true);
		}
		else
		{
			interactor.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, this.injuryAmt, false, true, true);
		}
		base.Interact_CastFinished(interactor);
	}

	// Token: 0x040008D4 RID: 2260
	public int minCurse;

	// Token: 0x040008D5 RID: 2261
	public int maxCurse;

	// Token: 0x040008D6 RID: 2262
	public float injuryAmt;
}

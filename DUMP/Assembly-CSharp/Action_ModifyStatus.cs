using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x020000EF RID: 239
public class Action_ModifyStatus : ItemAction
{
	// Token: 0x06000891 RID: 2193 RVA: 0x0002F770 File Offset: 0x0002D970
	public override void RunAction()
	{
		if (this.ifSkeleton && !base.character.data.isSkeleton)
		{
			return;
		}
		bool passedOut = base.character.data.passedOut;
		if (this.changeAmount < 0f)
		{
			if (this.statusType == CharacterAfflictions.STATUSTYPE.Poison)
			{
				base.character.refs.afflictions.ClearPoisonAfflictions();
				int num = Mathf.RoundToInt(Mathf.Min(base.character.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Poison), Mathf.Abs(this.changeAmount)) * 100f);
				Character character;
				if (this.item.TryGetFeeder(out character))
				{
					GameUtils.instance.IncrementFriendPoisonHealing(num, character.photonView.Owner);
				}
				else
				{
					Singleton<AchievementManager>.Instance.IncrementSteamStat(STEAMSTATTYPE.PoisonHealed, num);
				}
			}
			Character character2;
			if (this.statusType == CharacterAfflictions.STATUSTYPE.Injury && this.item.TryGetFeeder(out character2))
			{
				int amt = Mathf.RoundToInt(Mathf.Min(base.character.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Injury), Mathf.Abs(this.changeAmount)) * 100f);
				GameUtils.instance.IncrementFriendHealing(amt, character2.photonView.Owner);
			}
			base.character.refs.afflictions.SubtractStatus(this.statusType, Mathf.Abs(this.changeAmount), false, false);
		}
		else
		{
			base.character.refs.afflictions.AddStatus(this.statusType, Mathf.Abs(this.changeAmount), false, true, true);
		}
		float statusSum = base.character.refs.afflictions.statusSum;
		if (passedOut && statusSum <= 1f)
		{
			Debug.Log("LIFE WAS SAVED");
			Character character3;
			if (this.item.TryGetFeeder(out character3))
			{
				GameUtils.instance.ThrowEmergencyPreparednessAchievement(character3.photonView.Owner);
			}
		}
	}

	// Token: 0x0400082A RID: 2090
	public CharacterAfflictions.STATUSTYPE statusType;

	// Token: 0x0400082B RID: 2091
	public float changeAmount;

	// Token: 0x0400082C RID: 2092
	public bool ifSkeleton;
}

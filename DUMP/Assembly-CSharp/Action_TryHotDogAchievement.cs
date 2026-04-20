using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000100 RID: 256
public class Action_TryHotDogAchievement : ItemAction
{
	// Token: 0x060008BC RID: 2236 RVA: 0x0002FFCC File Offset: 0x0002E1CC
	public override void RunAction()
	{
		Action_TryHotDogAchievement.hotDogEatTimes.Add(Time.time);
		while (Action_TryHotDogAchievement.hotDogEatTimes[0] + 5f < Time.time)
		{
			Action_TryHotDogAchievement.hotDogEatTimes.RemoveAt(0);
		}
		if (Action_TryHotDogAchievement.hotDogEatTimes.Count >= 3)
		{
			Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.CompetitiveEatingBadge);
		}
	}

	// Token: 0x0400084C RID: 2124
	public static List<float> hotDogEatTimes = new List<float>();
}

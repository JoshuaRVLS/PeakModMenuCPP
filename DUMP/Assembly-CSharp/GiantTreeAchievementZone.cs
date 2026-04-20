using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x020000AC RID: 172
public class GiantTreeAchievementZone : MonoBehaviour
{
	// Token: 0x060006A0 RID: 1696 RVA: 0x00025F1D File Offset: 0x0002411D
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Character") && other.GetComponentInParent<Character>().IsLocal)
		{
			Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.ArboristBadge);
		}
	}
}

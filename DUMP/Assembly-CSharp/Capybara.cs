using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x020000A9 RID: 169
public class Capybara : MonoBehaviour
{
	// Token: 0x0600068D RID: 1677 RVA: 0x00025BB6 File Offset: 0x00023DB6
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, this.serenadeDistance);
	}

	// Token: 0x0600068E RID: 1678 RVA: 0x00025BD8 File Offset: 0x00023DD8
	private void OnEnable()
	{
		GlobalEvents.OnBugleTooted = (Action<Item>)Delegate.Combine(GlobalEvents.OnBugleTooted, new Action<Item>(this.TestBugleTooted));
	}

	// Token: 0x0600068F RID: 1679 RVA: 0x00025BFA File Offset: 0x00023DFA
	private void OnDisable()
	{
		GlobalEvents.OnBugleTooted = (Action<Item>)Delegate.Remove(GlobalEvents.OnBugleTooted, new Action<Item>(this.TestBugleTooted));
	}

	// Token: 0x06000690 RID: 1680 RVA: 0x00025C1C File Offset: 0x00023E1C
	private void TestBugleTooted(Item bugle)
	{
		if (Vector3.Distance(base.transform.position, bugle.transform.position) < this.serenadeDistance && bugle.holderCharacter && bugle.holderCharacter.IsLocal)
		{
			Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.AnimalSerenadingBadge);
		}
	}

	// Token: 0x04000686 RID: 1670
	public float serenadeDistance;
}

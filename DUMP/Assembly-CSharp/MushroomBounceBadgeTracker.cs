using System;
using System.Collections;
using UnityEngine;
using Zorro.Core;

// Token: 0x020002B7 RID: 695
public class MushroomBounceBadgeTracker : MonoBehaviour, ICollisionModifierEvent
{
	// Token: 0x060013A9 RID: 5033 RVA: 0x00063D44 File Offset: 0x00061F44
	public void OnBouncedCharacter(Character character)
	{
		if (character.IsLocal && !this.checkingPlayerHeight)
		{
			this.checkingPlayerHeight = true;
			GameUtils.instance.StartCoroutine(this.TestAchievementRoutine(character, character.Center.y));
		}
	}

	// Token: 0x060013AA RID: 5034 RVA: 0x00063D7A File Offset: 0x00061F7A
	private IEnumerator TestAchievementRoutine(Character character, float startingHeight)
	{
		yield return new WaitForSeconds(0.6f);
		float timeout = 1f;
		while (character)
		{
			if (!character.data.isGrounded)
			{
				break;
			}
			timeout -= Time.deltaTime;
			if (timeout <= 0f)
			{
				this.checkingPlayerHeight = false;
				yield break;
			}
			yield return null;
		}
		while (character != null && !character.data.isGrounded)
		{
			yield return null;
			if ((character.Center.y - startingHeight) * CharacterStats.unitsToMeters >= 40f)
			{
				Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.MycoacrobaticsBadge);
				this.checkingPlayerHeight = false;
				yield break;
			}
		}
		this.checkingPlayerHeight = false;
		yield break;
	}

	// Token: 0x040011FA RID: 4602
	public const float HEIGHT_REQUIREMENT_IN_METERS = 40f;

	// Token: 0x040011FB RID: 4603
	private bool checkingPlayerHeight;
}

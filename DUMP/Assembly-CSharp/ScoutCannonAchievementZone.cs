using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000183 RID: 387
[ExecuteAlways]
public class ScoutCannonAchievementZone : MonoBehaviour
{
	// Token: 0x06000CCC RID: 3276 RVA: 0x00044929 File Offset: 0x00042B29
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(this.bounds.center, this.bounds.size);
	}

	// Token: 0x06000CCD RID: 3277 RVA: 0x00044950 File Offset: 0x00042B50
	private void Update()
	{
		this.bounds.center = base.transform.position;
		this.bounds.size = base.transform.localScale;
	}

	// Token: 0x06000CCE RID: 3278 RVA: 0x0004497E File Offset: 0x00042B7E
	private void FixedUpdate()
	{
		if (Application.isPlaying)
		{
			this.DetectPlayerWasLaunched();
			this.TestAchievement();
		}
	}

	// Token: 0x06000CCF RID: 3279 RVA: 0x00044994 File Offset: 0x00042B94
	private void DetectPlayerWasLaunched()
	{
		if (!Character.localCharacter)
		{
			return;
		}
		if (!this.bounds.Contains(Character.localCharacter.Center))
		{
			return;
		}
		if (Character.localCharacter.data.launchedByCannon)
		{
			Debug.Log("Player launched by Cannon");
			this.playerWasLaunched = true;
		}
	}

	// Token: 0x06000CD0 RID: 3280 RVA: 0x000449E8 File Offset: 0x00042BE8
	private void TestAchievement()
	{
		if (this.playerWasLaunched && Character.localCharacter.data.fallSeconds <= 0f && Character.localCharacter.data.isGrounded)
		{
			this.playerWasLaunched = false;
			if (Character.localCharacter.Center.y >= this.bounds.min.y)
			{
				Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.DaredevilBadge);
			}
		}
	}

	// Token: 0x04000B95 RID: 2965
	public Bounds bounds;

	// Token: 0x04000B96 RID: 2966
	private bool playerWasLaunched;
}

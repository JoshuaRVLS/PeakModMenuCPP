using System;
using UnityEngine;

// Token: 0x02000239 RID: 569
public class ClimbModifierSurface : MonoBehaviour
{
	// Token: 0x06001188 RID: 4488 RVA: 0x00058534 File Offset: 0x00056734
	public void OnClimb(Character character)
	{
		Action<Character> action = this.onClimbAction;
		if (action != null)
		{
			action(character);
		}
		if (!this.applyStatus)
		{
			return;
		}
		if (!character.IsLocal)
		{
			return;
		}
		if (Time.time < this.lastTriggerTime + this.statusCooldown)
		{
			return;
		}
		character.refs.afflictions.AddStatus(this.statusType, this.statusAmount, false, true, true);
		this.lastTriggerTime = Time.time;
	}

	// Token: 0x06001189 RID: 4489 RVA: 0x000585A5 File Offset: 0x000567A5
	public void OnClimbEnter()
	{
	}

	// Token: 0x0600118A RID: 4490 RVA: 0x000585A7 File Offset: 0x000567A7
	public void OnClimbExit()
	{
	}

	// Token: 0x0600118B RID: 4491 RVA: 0x000585A9 File Offset: 0x000567A9
	internal float OverrideClimbAngle(Character character, float climbAngle)
	{
		if (this.hasAlwaysClimbableRange && Vector3.Distance(base.transform.position, character.Center) < this.alwaysClimbableRange)
		{
			return 90f;
		}
		return climbAngle;
	}

	// Token: 0x04000F50 RID: 3920
	public bool onlySlideDown;

	// Token: 0x04000F51 RID: 3921
	public float speedMultiplier = 1f;

	// Token: 0x04000F52 RID: 3922
	public float staminaUsageMultiplier = 1f;

	// Token: 0x04000F53 RID: 3923
	public bool applyStatus;

	// Token: 0x04000F54 RID: 3924
	public CharacterAfflictions.STATUSTYPE statusType;

	// Token: 0x04000F55 RID: 3925
	public float statusAmount = 0.5f;

	// Token: 0x04000F56 RID: 3926
	public float statusCooldown = 0.5f;

	// Token: 0x04000F57 RID: 3927
	private float lastTriggerTime;

	// Token: 0x04000F58 RID: 3928
	public bool staticClimbCost;

	// Token: 0x04000F59 RID: 3929
	public Action<Character> onClimbAction;

	// Token: 0x04000F5A RID: 3930
	internal bool hasAlwaysClimbableRange;

	// Token: 0x04000F5B RID: 3931
	internal float alwaysClimbableRange;
}

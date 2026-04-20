using System;
using UnityEngine;

// Token: 0x02000068 RID: 104
public class BotBoar : MonoBehaviour
{
	// Token: 0x06000506 RID: 1286 RVA: 0x0001DBC3 File Offset: 0x0001BDC3
	private void Awake()
	{
		this.bot = base.GetComponentInChildren<Bot>();
		this.character = base.GetComponent<Character>();
	}

	// Token: 0x06000507 RID: 1287 RVA: 0x0001DBDD File Offset: 0x0001BDDD
	private void Start()
	{
	}

	// Token: 0x06000508 RID: 1288 RVA: 0x0001DBDF File Offset: 0x0001BDDF
	public void ClearTarget()
	{
		this.bot.ClearTarget();
	}

	// Token: 0x06000509 RID: 1289 RVA: 0x0001DBEC File Offset: 0x0001BDEC
	private void Update()
	{
		this.bot.navigator.SetAgentVelocity(this.character.GetBodypart(BodypartType.Torso).Rig.linearVelocity);
		if (this.bot.timeSprinting > 3f)
		{
			this.bot.IsSprinting = false;
		}
		if (this.flee)
		{
			Debug.Log("Fleeing");
			if (this.bot.TargetCharacter == null || this.outOfSightTime >= 4f)
			{
				this.flee = false;
				this.outOfSightTime = 0f;
				this.bot.ClearTarget();
				this.potentialTarget = null;
				return;
			}
			this.bot.FleeFromPoint(this.bot.TargetCharacter.Center);
			if (this.bot.CanSee(this.bot.TargetCharacter.Head, this.bot.Center, 20f, 360f))
			{
				Debug.DrawLine(this.bot.TargetCharacter.Head, this.bot.Center, Color.green);
				this.outOfSightTime = 0f;
				return;
			}
			Debug.DrawLine(this.bot.TargetCharacter.Head, this.bot.Center, Color.red);
			this.outOfSightTime += Time.deltaTime;
			return;
		}
		else
		{
			if (this.bot.TargetCharacter)
			{
				Debug.Log("Chasing");
				Vector3? distanceToTargetCharacter = this.bot.DistanceToTargetCharacter;
				if (distanceToTargetCharacter == null || distanceToTargetCharacter.GetValueOrDefault().magnitude <= 4f)
				{
					if (this.bot.timeWithTarget <= 15f)
					{
						goto IL_1E6;
					}
					distanceToTargetCharacter = this.bot.DistanceToTargetCharacter;
					if (distanceToTargetCharacter == null || distanceToTargetCharacter.GetValueOrDefault().magnitude <= 2f)
					{
						goto IL_1E6;
					}
				}
				this.bot.ClearTarget();
				IL_1E6:
				this.bot.Chase();
				this.bot.CanSeeTarget(20f, 120f);
				if (this.bot.timeSinceSawTarget > 5f)
				{
					this.bot.ClearTarget();
				}
				if (!this.bot.IsSprinting)
				{
					this.flee = true;
				}
				return;
			}
			if (this.potentialTarget != null)
			{
				Debug.Log("Looking at target");
				if (!this.bot.CanSee(this.bot.HeadPosition, this.potentialTarget.Center, 70f, 110f))
				{
					this.potentialTarget = null;
					this.timeLookingAtTarget = 0f;
					return;
				}
				this.bot.StandStill();
				this.bot.LookAtPoint(this.potentialTarget.Center, 3f);
				this.timeLookingAtTarget += Time.deltaTime;
				if (this.timeLookingAtTarget > 4f)
				{
					this.bot.TargetCharacter = this.potentialTarget;
					this.bot.IsSprinting = true;
					this.potentialTarget = null;
					this.timeLookingAtTarget = 0f;
				}
			}
			if (this.potentialTarget == null)
			{
				this.bot.Patrol();
				Rigidbody rigidbody = this.bot.LookForPlayerHead(this.bot.HeadPosition, 20f, 110f);
				this.potentialTarget = ((rigidbody != null) ? rigidbody.GetComponentInParent<Character>() : null);
			}
			return;
		}
	}

	// Token: 0x0400055A RID: 1370
	private Bot bot;

	// Token: 0x0400055B RID: 1371
	private Rigidbody rig_g;

	// Token: 0x0400055C RID: 1372
	private Character character;

	// Token: 0x0400055D RID: 1373
	private Vector3 startPosition;

	// Token: 0x0400055E RID: 1374
	public float timeSinceSawTarget;

	// Token: 0x0400055F RID: 1375
	public Character potentialTarget;

	// Token: 0x04000560 RID: 1376
	public float timeLookingAtTarget;

	// Token: 0x04000561 RID: 1377
	public float timeSprinting;

	// Token: 0x04000562 RID: 1378
	private bool flee;

	// Token: 0x04000563 RID: 1379
	private float outOfSightTime;
}

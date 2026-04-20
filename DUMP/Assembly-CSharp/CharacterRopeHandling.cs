using System;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000230 RID: 560
public class CharacterRopeHandling : MonoBehaviour
{
	// Token: 0x06001145 RID: 4421 RVA: 0x00056C30 File Offset: 0x00054E30
	private void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06001146 RID: 4422 RVA: 0x00056C3E File Offset: 0x00054E3E
	private void Start()
	{
		this.character = base.GetComponent<Character>();
	}

	// Token: 0x06001147 RID: 4423 RVA: 0x00056C4C File Offset: 0x00054E4C
	private void Update()
	{
		if (!this.view.IsMine)
		{
			return;
		}
		if (this.character.data.isRopeClimbing)
		{
			if (!this.character.data.heldRope.UnityObjectExists<Rope>())
			{
				this.view.RPC("StopRopeClimbingRpc", RpcTarget.All, Array.Empty<object>());
				return;
			}
			if (this.character.data.heldRope != null)
			{
				float angleAtPercent = this.character.data.heldRope.climbingAPI.GetAngleAtPercent(this.character.data.ropePercent);
				if (!this.character.data.heldRope.IsActive() || (angleAtPercent > this.maxRopeAngle && 180f - angleAtPercent > this.maxRopeAngle))
				{
					Debug.Log(string.Format("Rope climbing failed. Angle up: {0} Angle down: {1}", angleAtPercent, 180f - angleAtPercent));
					this.view.RPC("StopRopeClimbingRpc", RpcTarget.All, Array.Empty<object>());
					return;
				}
			}
			float num = (this.character.input.movementInput.y < 0f) ? 3f : 1f;
			this.character.data.ropePercent += this.character.data.heldRope.climbingAPI.GetMove() * this.character.input.movementInput.y * num * this.climbSpeed * this.climbSpeedMod * Time.deltaTime * this.character.data.heldRope.climbingAPI.UpMult(this.character.data.ropePercent);
			this.character.data.ropePercent = Mathf.Clamp01(this.character.data.ropePercent);
			float num2 = this.staminaUsage;
			if (this.character.input.movementInput.y > 0.01f)
			{
				num2 = this.staminaUsageUp;
			}
			if (this.character.IsLocal && (this.character.input.jumpWasPressed || !this.character.UseStamina(num2 * Time.deltaTime, true) || this.character.data.currentRagdollControll < 0.3f))
			{
				this.view.RPC("StopRopeClimbingRpc", RpcTarget.All, Array.Empty<object>());
			}
		}
	}

	// Token: 0x06001148 RID: 4424 RVA: 0x00056EB8 File Offset: 0x000550B8
	public void Stop()
	{
		this.view.RPC("StopRopeClimbingRpc", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06001149 RID: 4425 RVA: 0x00056ED0 File Offset: 0x000550D0
	[PunRPC]
	private void StopRopeClimbingRpc()
	{
		if (this.character.data.heldRope != null)
		{
			this.character.data.heldRope.RemoveCharacterClimbing(this.character);
		}
		this.character.data.isRopeClimbing = false;
		this.character.data.isJumping = false;
		this.character.data.sinceGrounded = 0f;
		this.character.data.heldRope = null;
	}

	// Token: 0x0600114A RID: 4426 RVA: 0x00056F58 File Offset: 0x00055158
	private void FixedUpdate()
	{
		if (this.character.data.isRopeClimbing)
		{
			this.Climbing();
			return;
		}
		this.TryToStartWallClimb();
	}

	// Token: 0x0600114B RID: 4427 RVA: 0x00056F7C File Offset: 0x0005517C
	private void Climbing()
	{
		if (this.character.data.heldRope == null || this.character.data.heldRope.photonView == null)
		{
			if (this.character.photonView.IsMine)
			{
				this.character.refs.climbing.StopAnyClimbing();
				return;
			}
		}
		else
		{
			this.character.data.ropeClimbWorldNormal = this.character.data.ropeClimbNormal;
			this.character.data.ropeClimbWorldUp = this.character.data.heldRope.climbingAPI.GetUp(this.character.data.ropePercent);
			this.character.AddForce(this.ClimbForce(), 1f, 1f);
		}
	}

	// Token: 0x0600114C RID: 4428 RVA: 0x0005705E File Offset: 0x0005525E
	private Vector3 ClimbForce()
	{
		return (this.GetPosition() - this.character.TorsoPos()) * this.climbForce;
	}

	// Token: 0x0600114D RID: 4429 RVA: 0x00057084 File Offset: 0x00055284
	private Vector3 GetPosition()
	{
		return this.character.data.heldRope.climbingAPI.GetPosition(this.character.data.ropePercent) + this.character.data.ropeClimbWorldNormal * 0.5f;
	}

	// Token: 0x0600114E RID: 4430 RVA: 0x000570DA File Offset: 0x000552DA
	private void TryToStartWallClimb()
	{
	}

	// Token: 0x0600114F RID: 4431 RVA: 0x000570DC File Offset: 0x000552DC
	[PunRPC]
	public void GrabRopeRpc(PhotonView ropeView, int segmentIndex)
	{
		Rope componentInChildren = ropeView.GetComponentInChildren<Rope>();
		if (componentInChildren == null)
		{
			Debug.LogError("Failed to get rope from network object");
			return;
		}
		Debug.Log("Start Rope Climbing!");
		componentInChildren.AddCharacterClimbing(this.character);
		this.character.data.isRopeClimbing = true;
		this.character.data.heldRope = componentInChildren;
		this.character.data.ropePercent = componentInChildren.climbingAPI.GetPercentFromSegmentIndex(segmentIndex);
		this.character.data.ropeClimbNormal = -this.character.data.lookDirection_Flat;
		this.character.data.isClimbing = false;
		this.character.data.isVineClimbing = false;
	}

	// Token: 0x04000F13 RID: 3859
	private Character character;

	// Token: 0x04000F14 RID: 3860
	public float climbForce;

	// Token: 0x04000F15 RID: 3861
	public float climbSpeed;

	// Token: 0x04000F16 RID: 3862
	public float climbSpeedMod = 1f;

	// Token: 0x04000F17 RID: 3863
	public float climbDrag = 0.85f;

	// Token: 0x04000F18 RID: 3864
	public float staminaUsage;

	// Token: 0x04000F19 RID: 3865
	public float staminaUsageUp;

	// Token: 0x04000F1A RID: 3866
	private PhotonView view;

	// Token: 0x04000F1B RID: 3867
	public float maxRopeAngle = 90f;
}

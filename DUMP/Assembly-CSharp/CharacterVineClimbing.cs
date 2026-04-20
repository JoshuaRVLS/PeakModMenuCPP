using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000231 RID: 561
public class CharacterVineClimbing : MonoBehaviour
{
	// Token: 0x06001151 RID: 4433 RVA: 0x000571C8 File Offset: 0x000553C8
	private void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06001152 RID: 4434 RVA: 0x000571D6 File Offset: 0x000553D6
	private void Start()
	{
		this.character = base.GetComponent<Character>();
	}

	// Token: 0x06001153 RID: 4435 RVA: 0x000571E4 File Offset: 0x000553E4
	private void Update()
	{
		if (!this.character.IsLocal || !this.character.data.isVineClimbing || Time.timeScale == 0f)
		{
			return;
		}
		if (this.character.data.heldVine == null)
		{
			this.Stop();
			return;
		}
		float sign = this.character.data.heldVine.GetSign(this.character.data.lookDirection_Flat, this.character.data.vinePercent);
		this._currentClimbInput = sign * this.character.input.movementInput.y;
		float num = (this.Sliding() || Mathf.Abs(this.character.input.movementInput.y) < 0.01f) ? 0.005f : this.staminaUsage;
		if (this.character.input.jumpWasPressed || !this.character.UseStamina(num * Time.deltaTime, true) || this.character.data.currentRagdollControll < 0.5f)
		{
			this.view.RPC("StopVineClimbingRpc", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.syncC += Time.deltaTime;
		if (this.syncC > 0.25f)
		{
			this.syncC = 0f;
			this.view.RPC("RPCA_SyncVineClimb", RpcTarget.Others, new object[]
			{
				this.character.data.vinePercent,
				this.attachVel
			});
		}
	}

	// Token: 0x06001154 RID: 4436 RVA: 0x00057387 File Offset: 0x00055587
	[PunRPC]
	private void RPCA_SyncVineClimb(float p, float vel)
	{
		this.character.data.vinePercent = p;
		this.attachVel = vel;
	}

	// Token: 0x06001155 RID: 4437 RVA: 0x000573A4 File Offset: 0x000555A4
	private void FixedUpdate()
	{
		if (!this.character.data.isVineClimbing)
		{
			return;
		}
		if (this.character.data.heldVine == null)
		{
			this.Stop();
			return;
		}
		float num = this.character.data.heldVine.LengthFactor();
		if (this.Sliding())
		{
			float vinePercent = this.character.data.vinePercent;
			if (vinePercent > 0.99f || vinePercent < 0.01f)
			{
				this.attachVel = 0f;
			}
			else
			{
				this.attachVel *= this.slideDeceleration;
			}
			this.character.data.vinePercent += this.slideFactor * num * Time.deltaTime * this.attachVel;
		}
		else
		{
			this.attachVel = 0f;
			float num2 = num * this.climbSpeed * this.climbSpeedMod * Time.deltaTime * this._currentClimbInput;
			this.character.data.vinePercent = Mathf.Clamp(this.character.data.vinePercent + num2, 0.01f, 0.99f);
		}
		this.Climbing();
	}

	// Token: 0x06001156 RID: 4438 RVA: 0x000574CF File Offset: 0x000556CF
	public bool Sliding()
	{
		return Mathf.Abs(this.attachVel) > 3f;
	}

	// Token: 0x06001157 RID: 4439 RVA: 0x000574E3 File Offset: 0x000556E3
	public void Stop()
	{
		this.view.RPC("StopVineClimbingRpc", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06001158 RID: 4440 RVA: 0x000574FC File Offset: 0x000556FC
	[PunRPC]
	private void StopVineClimbingRpc()
	{
		this.character.data.isVineClimbing = false;
		this.character.data.isJumping = false;
		this.character.data.sinceGrounded = 0f;
		this.syncC = 0f;
	}

	// Token: 0x06001159 RID: 4441 RVA: 0x0005754C File Offset: 0x0005574C
	private void Climbing()
	{
		Vector3 move = (this.character.data.heldVine.GetPosition(this.character.data.vinePercent, 1, 1) + Vector3.down - this.character.TorsoPos()) * this.climbForce;
		this.character.AddForce(move, 1f, 1f);
	}

	// Token: 0x0600115A RID: 4442 RVA: 0x000575BC File Offset: 0x000557BC
	[PunRPC]
	public void GrabVineRpc(PhotonView ropeView, int segmentIndex)
	{
		JungleVine component = ropeView.GetComponent<JungleVine>();
		if (component == null)
		{
			Debug.LogError("Failed to get rope from network object");
			return;
		}
		Debug.Log("Start Rope Climbing!");
		this.character.data.isRopeClimbing = false;
		this.character.data.isClimbing = false;
		this.character.data.isVineClimbing = true;
		this.character.data.heldVine = component;
		this.character.data.vinePercent = Mathf.Clamp(component.GetPercentFromSegmentIndex(segmentIndex), 0.01f, 0.99f);
		this.attachVel = component.GetVineVel(this.character.data.avarageVelocity, this.character.data.vinePercent);
	}

	// Token: 0x04000F1C RID: 3868
	private Character character;

	// Token: 0x04000F1D RID: 3869
	private float _currentClimbInput;

	// Token: 0x04000F1E RID: 3870
	public float climbForce;

	// Token: 0x04000F1F RID: 3871
	public float climbSpeed;

	// Token: 0x04000F20 RID: 3872
	public float climbSpeedMod = 1f;

	// Token: 0x04000F21 RID: 3873
	[Range(1f, 2f)]
	public float slideFactor = 1.3f;

	// Token: 0x04000F22 RID: 3874
	[Range(0.9f, 0.9999f)]
	public float slideDeceleration = 0.985f;

	// Token: 0x04000F23 RID: 3875
	public float staminaUsage;

	// Token: 0x04000F24 RID: 3876
	private const float minVinePercent = 0.01f;

	// Token: 0x04000F25 RID: 3877
	private const float maxVinePercent = 0.99f;

	// Token: 0x04000F26 RID: 3878
	private PhotonView view;

	// Token: 0x04000F27 RID: 3879
	private float attachVel;

	// Token: 0x04000F28 RID: 3880
	private const float syncPeriod = 0.25f;

	// Token: 0x04000F29 RID: 3881
	private const float idleStaminaCost = 0.005f;

	// Token: 0x04000F2A RID: 3882
	private float syncC;
}

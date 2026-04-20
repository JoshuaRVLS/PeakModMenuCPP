using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200017E RID: 382
public class TempleEntranceRope : RopeAnchorWithRope
{
	// Token: 0x06000C6C RID: 3180 RVA: 0x00042E99 File Offset: 0x00041099
	public override void Awake()
	{
		base.Awake();
		this.doorStartingPosition = this.doorRb.transform.position;
	}

	// Token: 0x06000C6D RID: 3181 RVA: 0x00042EB7 File Offset: 0x000410B7
	public void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		this.UpdateWeight();
	}

	// Token: 0x06000C6E RID: 3182 RVA: 0x00042EC7 File Offset: 0x000410C7
	public void FixedUpdate()
	{
		this.UpdateDoorOpen();
	}

	// Token: 0x06000C6F RID: 3183 RVA: 0x00042ECF File Offset: 0x000410CF
	[PunRPC]
	private void SetWeightRPC(float weight)
	{
		Debug.Log(string.Format("Received weight RPC. {0}", weight));
		this.currentWeight = weight;
		if (this.currentWeight > this.lockWeight)
		{
			this.lockedOpen = true;
		}
	}

	// Token: 0x06000C70 RID: 3184 RVA: 0x00042F02 File Offset: 0x00041102
	private void UpdateDescent()
	{
		float num = this.currentWeight / this.weightPerSegment;
	}

	// Token: 0x06000C71 RID: 3185 RVA: 0x00042F14 File Offset: 0x00041114
	private void UpdateDoorOpen()
	{
		float d = Mathf.Min(this.doorHeightPerWeight * this.currentWeight, this.maxDoorHeight);
		this.currentDoorTarget = this.doorStartingPosition + Vector3.up * d;
		Vector3 vector = this.currentDoorTarget - this.doorRb.transform.position;
		if (vector.y > 0f)
		{
			Vector3 b = Vector3.ClampMagnitude(Vector3.Lerp(this.doorRb.position, this.currentDoorTarget, this.doorLerpSpeedUp * Time.fixedDeltaTime) - this.doorRb.position, this.maxDoorMoveSpeedUp * Time.fixedDeltaTime);
			this.doorRb.MovePosition(this.doorRb.position + b);
			return;
		}
		if (vector.y < 0f)
		{
			this.doorRb.MovePosition(Vector3.MoveTowards(this.doorRb.position, this.currentDoorTarget, this.doorMoveSpeedDown * Time.fixedDeltaTime));
		}
	}

	// Token: 0x06000C72 RID: 3186 RVA: 0x0004301A File Offset: 0x0004121A
	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		if (base.photonView.IsMine)
		{
			base.photonView.RPC("SetWeightRPC", RpcTarget.All, new object[]
			{
				this.currentWeight
			});
		}
	}

	// Token: 0x06000C73 RID: 3187 RVA: 0x00043054 File Offset: 0x00041254
	private void UpdateWeight()
	{
		if (!base.photonView.IsMine || !this.rope)
		{
			return;
		}
		float num = 0f;
		foreach (Character character in this.rope.charactersClimbing)
		{
			num += this.baseScoutWeight;
			num += character.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Weight);
		}
		if ((!this.lockedOpen || num > this.currentWeight) && this.currentWeight != num)
		{
			base.photonView.RPC("SetWeightRPC", RpcTarget.All, new object[]
			{
				num
			});
		}
	}

	// Token: 0x04000B60 RID: 2912
	public float baseScoutWeight;

	// Token: 0x04000B61 RID: 2913
	public float weightPerSegment;

	// Token: 0x04000B62 RID: 2914
	public float currentWeight;

	// Token: 0x04000B63 RID: 2915
	[Header("Weight at which the door will lock open.")]
	public float lockWeight;

	// Token: 0x04000B64 RID: 2916
	public Rigidbody doorRb;

	// Token: 0x04000B65 RID: 2917
	public float doorHeightPerWeight;

	// Token: 0x04000B66 RID: 2918
	public float maxDoorHeight;

	// Token: 0x04000B67 RID: 2919
	public float doorLerpSpeedUp;

	// Token: 0x04000B68 RID: 2920
	public float maxDoorMoveSpeedUp;

	// Token: 0x04000B69 RID: 2921
	public float doorMoveSpeedDown;

	// Token: 0x04000B6A RID: 2922
	private Vector3 doorStartingPosition;

	// Token: 0x04000B6B RID: 2923
	private Vector3 currentDoorTarget;

	// Token: 0x04000B6C RID: 2924
	private bool lockedOpen;
}

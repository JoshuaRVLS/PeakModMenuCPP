using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000218 RID: 536
[DefaultExecutionOrder(1000002)]
public class BingBongForceAbilities : MonoBehaviour
{
	// Token: 0x06001067 RID: 4199 RVA: 0x00051929 File Offset: 0x0004FB29
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
		if (this.physicsType == BingBongPhysics.PhysicsType.ForcePush_Gentle || this.physicsType == BingBongPhysics.PhysicsType.ForcePush)
		{
			this.DoEffect();
		}
	}

	// Token: 0x06001068 RID: 4200 RVA: 0x0005194F File Offset: 0x0004FB4F
	[PunRPC]
	public void RPCA_BingBongInitObj(int bingbongID)
	{
		this.bingbong = PhotonView.Find(bingbongID).transform;
	}

	// Token: 0x06001069 RID: 4201 RVA: 0x00051962 File Offset: 0x0004FB62
	private void LateUpdate()
	{
		base.transform.position = this.bingbong.position;
		base.transform.rotation = this.bingbong.rotation;
	}

	// Token: 0x0600106A RID: 4202 RVA: 0x00051990 File Offset: 0x0004FB90
	private void Update()
	{
		this.removeAfterSeconds -= Time.deltaTime;
		this.effectTime -= Time.deltaTime;
		if (this.view.IsMine && this.removeAfterSeconds <= 0f)
		{
			PhotonNetwork.Destroy(base.gameObject);
			return;
		}
	}

	// Token: 0x0600106B RID: 4203 RVA: 0x000519E7 File Offset: 0x0004FBE7
	private void FixedUpdate()
	{
		if (this.effectTime > 0f && this.physicsType != BingBongPhysics.PhysicsType.ForcePush_Gentle && this.physicsType != BingBongPhysics.PhysicsType.ForcePush)
		{
			this.DoEffect();
		}
	}

	// Token: 0x0600106C RID: 4204 RVA: 0x00051A10 File Offset: 0x0004FC10
	private void DoEffect()
	{
		foreach (Character character in this.GetTargets())
		{
			character.refs.movement.ApplyExtraDrag(this.drag, true);
			character.AddForce(this.GetForceDirection(character.Center) * this.force, 1f, 1f);
			character.data.sinceGrounded = Mathf.Clamp(character.data.sinceGrounded, 0f, 0.25f);
			if (this.fallAmount > 0.01f && character.IsLocal)
			{
				character.Fall(this.fallAmount, 0f);
			}
		}
	}

	// Token: 0x0600106D RID: 4205 RVA: 0x00051AEC File Offset: 0x0004FCEC
	private Vector3 GetForceDirection(Vector3 playerPos)
	{
		if (this.physicsType == BingBongPhysics.PhysicsType.Blow)
		{
			return this.bingbong.forward;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.Suck)
		{
			return -this.bingbong.forward;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.ForcePush)
		{
			return this.bingbong.forward;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.ForcePush_Gentle)
		{
			return this.bingbong.forward;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.ForceGrab)
		{
			return this.TargetPos() - playerPos;
		}
		return Vector3.zero;
	}

	// Token: 0x0600106E RID: 4206 RVA: 0x00051B6C File Offset: 0x0004FD6C
	private List<Character> GetTargets()
	{
		Vector3 a = this.TargetPos();
		float num = 5f;
		List<Character> list = new List<Character>();
		foreach (Character character in Character.AllCharacters)
		{
			if (Vector3.Distance(a, character.Center) < num)
			{
				list.Add(character);
			}
		}
		return list;
	}

	// Token: 0x0600106F RID: 4207 RVA: 0x00051BE4 File Offset: 0x0004FDE4
	private Vector3 TargetPos()
	{
		return base.transform.TransformPoint(Vector3.forward * 5f);
	}

	// Token: 0x04000E5A RID: 3674
	private PhotonView view;

	// Token: 0x04000E5B RID: 3675
	private Transform bingbong;

	// Token: 0x04000E5C RID: 3676
	public BingBongPhysics.PhysicsType physicsType;

	// Token: 0x04000E5D RID: 3677
	public float force;

	// Token: 0x04000E5E RID: 3678
	public float drag;

	// Token: 0x04000E5F RID: 3679
	public float fallAmount;

	// Token: 0x04000E60 RID: 3680
	public float removeAfterSeconds = 2f;

	// Token: 0x04000E61 RID: 3681
	public float effectTime = 2f;
}

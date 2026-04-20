using System;
using Peak.Afflictions;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001FF RID: 511
public class Action_RaycastDart : ItemAction
{
	// Token: 0x06000FFE RID: 4094 RVA: 0x0004E45F File Offset: 0x0004C65F
	public override void RunAction()
	{
		this.FireDart();
	}

	// Token: 0x06000FFF RID: 4095 RVA: 0x0004E467 File Offset: 0x0004C667
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(this.spawnTransform.position, this.dartCollisionSize);
	}

	// Token: 0x06001000 RID: 4096 RVA: 0x0004E48C File Offset: 0x0004C68C
	private void FireDart()
	{
		if (this.shotSFX)
		{
			this.shotSFX.Play(base.transform.position);
		}
		Physics.Raycast(this.spawnTransform.position, MainCamera.instance.transform.forward, out this.lineHit, this.maxDistance, HelperFunctions.terrainMapMask, QueryTriggerInteraction.Ignore);
		if (!this.lineHit.collider)
		{
			this.lineHit.distance = this.maxDistance;
			this.lineHit.point = this.spawnTransform.position + MainCamera.instance.transform.forward * this.maxDistance;
		}
		this.sphereHits = Physics.SphereCastAll(this.spawnTransform.position, this.dartCollisionSize, MainCamera.instance.transform.forward, this.lineHit.distance, LayerMask.GetMask(new string[]
		{
			"Character"
		}), QueryTriggerInteraction.Ignore);
		foreach (RaycastHit raycastHit in this.sphereHits)
		{
			if (raycastHit.collider)
			{
				Character componentInParent = raycastHit.collider.GetComponentInParent<Character>();
				if (componentInParent)
				{
					Debug.Log("HIT");
					if (componentInParent != base.character)
					{
						this.DartImpact(componentInParent, this.spawnTransform.position, raycastHit.point);
						return;
					}
				}
			}
		}
		this.DartImpact(null, this.spawnTransform.position, this.lineHit.point);
	}

	// Token: 0x06001001 RID: 4097 RVA: 0x0004E624 File Offset: 0x0004C824
	private void DartImpact(Character hitCharacter, Vector3 origin, Vector3 endpoint)
	{
		if (hitCharacter)
		{
			base.photonView.RPC("RPC_DartImpact", RpcTarget.All, new object[]
			{
				hitCharacter.photonView.ViewID,
				origin,
				endpoint
			});
			return;
		}
		base.photonView.RPC("RPC_DartImpact", RpcTarget.All, new object[]
		{
			-1,
			origin,
			endpoint
		});
	}

	// Token: 0x06001002 RID: 4098 RVA: 0x0004E6A8 File Offset: 0x0004C8A8
	[PunRPC]
	private void RPC_DartImpact(int characterID, Vector3 origin, Vector3 endpoint)
	{
		if (characterID != -1)
		{
			Character component = PhotonNetwork.GetPhotonView(characterID).gameObject.GetComponent<Character>();
			if (component != null && component.photonView.IsMine)
			{
				Debug.Log("I'M HIT");
				foreach (Affliction affliction in this.afflictionsOnHit)
				{
					component.refs.afflictions.AddAffliction(affliction, false);
				}
			}
		}
		Object.Instantiate<GameObject>(this.dartVFX, endpoint, Quaternion.identity);
		GamefeelHandler.instance.AddPerlinShakeProximity(endpoint, 5f, 0.2f, 15f, 10f);
	}

	// Token: 0x04000D9A RID: 3482
	public float maxDistance;

	// Token: 0x04000D9B RID: 3483
	public float dartCollisionSize;

	// Token: 0x04000D9C RID: 3484
	[SerializeReference]
	public Affliction[] afflictionsOnHit;

	// Token: 0x04000D9D RID: 3485
	public Transform spawnTransform;

	// Token: 0x04000D9E RID: 3486
	public GameObject dartVFX;

	// Token: 0x04000D9F RID: 3487
	private HelperFunctions.LayerType layerMaskType;

	// Token: 0x04000DA0 RID: 3488
	private RaycastHit lineHit;

	// Token: 0x04000DA1 RID: 3489
	private RaycastHit[] sphereHits;

	// Token: 0x04000DA2 RID: 3490
	public SFX_Instance shotSFX;
}

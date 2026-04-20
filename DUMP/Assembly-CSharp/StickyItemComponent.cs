using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200003D RID: 61
public class StickyItemComponent : ItemComponent
{
	// Token: 0x1700004D RID: 77
	// (get) Token: 0x060003CA RID: 970 RVA: 0x00019204 File Offset: 0x00017404
	// (set) Token: 0x060003CB RID: 971 RVA: 0x0001920C File Offset: 0x0001740C
	public Character stuckToCharacter { get; protected set; }

	// Token: 0x060003CC RID: 972 RVA: 0x00019215 File Offset: 0x00017415
	public override void Awake()
	{
		base.Awake();
		this.physicsSyncer = base.GetComponent<ItemPhysicsSyncer>();
	}

	// Token: 0x060003CD RID: 973 RVA: 0x0001922C File Offset: 0x0001742C
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		if (this.photonView.IsMine && this.stuckToCharacter)
		{
			this.photonView.RPC("RPC_StickToCharacterRemote", newPlayer, new object[]
			{
				this.stuckToCharacter.photonView.ViewID,
				(int)this.stuckToBodypart,
				this.stuckLocalOffset
			});
		}
	}

	// Token: 0x060003CE RID: 974 RVA: 0x000192A0 File Offset: 0x000174A0
	[PunRPC]
	public void RPC_StickToCharacterRemote(int characterViewID, int bodyPartType, Vector3 offset)
	{
		PhotonView photonView = PhotonNetwork.GetPhotonView(characterViewID);
		if (!photonView)
		{
			return;
		}
		Character component = photonView.GetComponent<Character>();
		if (component.IsLocal)
		{
			return;
		}
		Bodypart bodypart = component.GetBodypart((BodypartType)bodyPartType);
		if (bodypart == null)
		{
			return;
		}
		this.StickToCharacterLocal(component, bodypart, offset);
	}

	// Token: 0x060003CF RID: 975 RVA: 0x000192E8 File Offset: 0x000174E8
	internal virtual void StickToCharacterLocal(Character character, Bodypart bodypart, Vector3 worldOffset)
	{
		if (this.item.itemState != ItemState.Ground)
		{
			return;
		}
		if (character == null)
		{
			return;
		}
		this.stuck = true;
		this.stuckToCharacter = character;
		this.stuckToTransform = bodypart.transform;
		this.item.SetKinematic(true);
		this.physicalCollider.isTrigger = true;
		this.item.rig.angularVelocity = Vector3.zero;
		this.item.rig.linearVelocity = Vector3.zero;
		Debug.Log(string.Format("Stuck to {0} with offset distance {1}", character.gameObject.name, worldOffset.magnitude));
		if (bodypart.partType == BodypartType.Foot_R || bodypart.partType == BodypartType.Foot_L)
		{
			worldOffset.y = Mathf.Max(worldOffset.y, 0f);
		}
		this.stuckToBodypart = bodypart.partType;
		this.stuckLocalOffset = this.stuckToTransform.InverseTransformVector(worldOffset);
		this.stuckLocalRotationOffset = this.stuckToTransform.rotation * Quaternion.Inverse(base.transform.rotation);
		this.physicsSyncer.shouldSync = false;
		if (!StickyItemComponent.ALL_STUCK_ITEMS.Contains(this))
		{
			StickyItemComponent.ALL_STUCK_ITEMS.Add(this);
		}
		character.refs.afflictions.UpdateWeight();
		if (character.IsLocal)
		{
			this.photonView.RPC("RPC_StickToCharacterRemote", RpcTarget.Others, new object[]
			{
				character.photonView.ViewID,
				(int)bodypart.partType,
				worldOffset
			});
		}
	}

	// Token: 0x060003D0 RID: 976 RVA: 0x0001947C File Offset: 0x0001767C
	private void Unstick()
	{
		Character stuckToCharacter = this.stuckToCharacter;
		this.stuck = false;
		this.item.SetKinematic(false);
		this.physicalCollider.isTrigger = false;
		this.stuckToCharacter = null;
		this.stuckToTransform = null;
		StickyItemComponent.ALL_STUCK_ITEMS.Remove(this);
		this.physicsSyncer.shouldSync = true;
		if (stuckToCharacter)
		{
			stuckToCharacter.refs.afflictions.UpdateWeight();
		}
	}

	// Token: 0x060003D1 RID: 977 RVA: 0x000194ED File Offset: 0x000176ED
	private void OnDestroy()
	{
		StickyItemComponent.ALL_STUCK_ITEMS.Remove(this);
		if (this.stuckToCharacter)
		{
			this.stuckToCharacter.refs.afflictions.UpdateWeight();
		}
	}

	// Token: 0x060003D2 RID: 978 RVA: 0x0001951D File Offset: 0x0001771D
	private void Update()
	{
		if (this.stuck && this.item.itemState != ItemState.Ground)
		{
			this.Unstick();
		}
	}

	// Token: 0x060003D3 RID: 979 RVA: 0x0001953C File Offset: 0x0001773C
	protected virtual void FixedUpdate()
	{
		if (this.stuck)
		{
			this.item.rig.MovePosition(this.stuckToTransform.TransformPoint(this.stuckLocalOffset));
			this.item.rig.MoveRotation(this.stuckToTransform.rotation * this.stuckLocalRotationOffset);
		}
	}

	// Token: 0x060003D4 RID: 980 RVA: 0x00019598 File Offset: 0x00017798
	public override void OnInstanceDataSet()
	{
	}

	// Token: 0x0400040D RID: 1037
	public static List<StickyItemComponent> ALL_STUCK_ITEMS = new List<StickyItemComponent>();

	// Token: 0x0400040F RID: 1039
	public Vector3 stuckLocalOffset;

	// Token: 0x04000410 RID: 1040
	public BodypartType stuckToBodypart;

	// Token: 0x04000411 RID: 1041
	public Quaternion stuckLocalRotationOffset;

	// Token: 0x04000412 RID: 1042
	protected Transform stuckToTransform;

	// Token: 0x04000413 RID: 1043
	protected bool stuck;

	// Token: 0x04000414 RID: 1044
	public float throwChargeRequirement;

	// Token: 0x04000415 RID: 1045
	public int addWeightToStuckPlayer;

	// Token: 0x04000416 RID: 1046
	public int addThornsToStuckPlayer;

	// Token: 0x04000417 RID: 1047
	public Collider physicalCollider;

	// Token: 0x04000418 RID: 1048
	public float spherecastRadius;

	// Token: 0x04000419 RID: 1049
	protected ItemPhysicsSyncer physicsSyncer;

	// Token: 0x0400041A RID: 1050
	private RaycastHit sphereCastHit;
}

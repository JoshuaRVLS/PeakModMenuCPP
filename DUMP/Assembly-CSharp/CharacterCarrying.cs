using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200022C RID: 556
public class CharacterCarrying : MonoBehaviour
{
	// Token: 0x060010F0 RID: 4336 RVA: 0x000541C0 File Offset: 0x000523C0
	private void Start()
	{
		this.character = base.GetComponent<Character>();
	}

	// Token: 0x060010F1 RID: 4337 RVA: 0x000541D0 File Offset: 0x000523D0
	private void FixedUpdate()
	{
		if (this.character.data.isCarried && this.character.data.carrier == null)
		{
			this.CarrierGone();
		}
		if (this.character.data.carrier)
		{
			this.GetCarried();
		}
	}

	// Token: 0x060010F2 RID: 4338 RVA: 0x0005422C File Offset: 0x0005242C
	private void Update()
	{
		if (this.character.data.carriedPlayer && (this.character.data.carriedPlayer.data.dead || !this.character.data.carriedPlayer.data.fullyPassedOut || this.character.data.fullyPassedOut || this.character.data.dead) && this.character.refs.view.IsMine)
		{
			this.Drop(this.character.data.carriedPlayer);
		}
	}

	// Token: 0x060010F3 RID: 4339 RVA: 0x000542DC File Offset: 0x000524DC
	private void ToggleCarryPhysics(bool setCarried)
	{
		this.character.refs.ragdoll.ToggleCollision(!setCarried);
		this.character.refs.animations.SetBool("IsCarried", setCarried);
		Debug.Log("SetIsCarried: " + setCarried.ToString());
	}

	// Token: 0x060010F4 RID: 4340 RVA: 0x00054334 File Offset: 0x00052534
	private void GetCarried()
	{
		Vector3 a = Vector3.ClampMagnitude(this.character.data.carrier.refs.carryPosRef.position + this.character.data.carrier.data.avarageVelocity * 0.06f - this.character.Center, 1f);
		this.character.AddForce(a * 500f, 1f, 1f);
		this.character.refs.movement.ApplyExtraDrag(0.5f, true);
		this.character.data.sinceGrounded = 0f;
	}

	// Token: 0x060010F5 RID: 4341 RVA: 0x000543F4 File Offset: 0x000525F4
	internal void StartCarry(Character target)
	{
		this.character.refs.items.EquipSlot(Optionable<byte>.None);
		this.character.photonView.RPC("RPCA_StartCarry", RpcTarget.All, new object[]
		{
			target.photonView
		});
	}

	// Token: 0x060010F6 RID: 4342 RVA: 0x00054440 File Offset: 0x00052640
	[PunRPC]
	public void RPCA_StartCarry(PhotonView targetView)
	{
		Character component = targetView.GetComponent<Character>();
		BackpackSlot backpackSlot = this.character.player.backpackSlot;
		if (!backpackSlot.IsEmpty())
		{
			if (PhotonNetwork.IsMasterClient)
			{
				Debug.Log(string.Format("{0} is starting to carry {1} but has backpack, dropping backpack", this.character, component));
				PhotonNetwork.InstantiateItemRoom(backpackSlot.GetPrefabName(), component.GetBodypart(BodypartType.Torso).transform.position, Quaternion.identity).GetComponent<PhotonView>().RPC("SetItemInstanceDataRPC", RpcTarget.All, new object[]
				{
					backpackSlot.data
				});
			}
			backpackSlot.EmptyOut();
		}
		else if (this.character.data.carriedPlayer != null)
		{
			this.character.refs.carriying.Drop(this.character.data.carriedPlayer);
			return;
		}
		component.refs.carriying.ToggleCarryPhysics(true);
		component.data.isCarried = true;
		this.character.data.carriedPlayer = component;
		component.data.carrier = this.character;
		List<Character> allPlayerCharacters = PlayerHandler.GetAllPlayerCharacters();
		for (int i = 0; i < allPlayerCharacters.Count; i++)
		{
			Debug.Log("Updating weight for " + allPlayerCharacters[i].gameObject.name + "...");
			allPlayerCharacters[i].refs.afflictions.UpdateWeight();
		}
	}

	// Token: 0x060010F7 RID: 4343 RVA: 0x000545A1 File Offset: 0x000527A1
	internal void Drop(Character target)
	{
		this.character.photonView.RPC("RPCA_Drop", RpcTarget.All, new object[]
		{
			target.photonView
		});
	}

	// Token: 0x060010F8 RID: 4344 RVA: 0x000545C8 File Offset: 0x000527C8
	[PunRPC]
	public void RPCA_Drop(PhotonView targetView)
	{
		Character component = targetView.GetComponent<Character>();
		component.refs.carriying.ToggleCarryPhysics(false);
		component.data.isCarried = false;
		component.data.carrier = null;
		this.character.data.carriedPlayer = null;
		List<Character> allPlayerCharacters = PlayerHandler.GetAllPlayerCharacters();
		for (int i = 0; i < allPlayerCharacters.Count; i++)
		{
			Debug.Log("Updating weight for " + allPlayerCharacters[i].gameObject.name + "...");
			allPlayerCharacters[i].refs.afflictions.UpdateWeight();
		}
	}

	// Token: 0x060010F9 RID: 4345 RVA: 0x00054666 File Offset: 0x00052866
	private void CarrierGone()
	{
		this.character.refs.carriying.ToggleCarryPhysics(false);
	}

	// Token: 0x04000EF4 RID: 3828
	private Character character;
}

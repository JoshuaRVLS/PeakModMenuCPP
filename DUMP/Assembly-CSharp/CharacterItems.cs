using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000013 RID: 19
public class CharacterItems : MonoBehaviourPunCallbacks
{
	// Token: 0x0600019B RID: 411 RVA: 0x0000BABC File Offset: 0x00009CBC
	private IEnumerator SubscribeRoutine(bool subscribe)
	{
		while (!this.character.player)
		{
			yield return null;
		}
		if (subscribe)
		{
			global::Player player = this.character.player;
			player.itemsChangedAction = (Action<ItemSlot[]>)Delegate.Combine(player.itemsChangedAction, new Action<ItemSlot[]>(this.UpdateClimbingSpikeCount));
		}
		else
		{
			global::Player player2 = this.character.player;
			player2.itemsChangedAction = (Action<ItemSlot[]>)Delegate.Remove(player2.itemsChangedAction, new Action<ItemSlot[]>(this.UpdateClimbingSpikeCount));
		}
		if (subscribe)
		{
			global::Player player3 = this.character.player;
			player3.itemsChangedAction = (Action<ItemSlot[]>)Delegate.Combine(player3.itemsChangedAction, new Action<ItemSlot[]>(this.UpdateBalloonCount));
		}
		else
		{
			global::Player player4 = this.character.player;
			player4.itemsChangedAction = (Action<ItemSlot[]>)Delegate.Remove(player4.itemsChangedAction, new Action<ItemSlot[]>(this.UpdateBalloonCount));
		}
		yield break;
	}

	// Token: 0x0600019C RID: 412 RVA: 0x0000BAD2 File Offset: 0x00009CD2
	private void Awake()
	{
		this.character = base.GetComponent<Character>();
		this.photonView = base.GetComponent<PhotonView>();
		this.currentSelectedSlot = Optionable<byte>.None;
		this.lastSelectedSlot = Optionable<byte>.Some(0);
		base.StartCoroutine(this.SubscribeRoutine(true));
	}

	// Token: 0x0600019D RID: 413 RVA: 0x0000BB14 File Offset: 0x00009D14
	private void OnDestroy()
	{
		if (this.character.player)
		{
			global::Player player = this.character.player;
			player.itemsChangedAction = (Action<ItemSlot[]>)Delegate.Remove(player.itemsChangedAction, new Action<ItemSlot[]>(this.UpdateClimbingSpikeCount));
			global::Player player2 = this.character.player;
			player2.itemsChangedAction = (Action<ItemSlot[]>)Delegate.Remove(player2.itemsChangedAction, new Action<ItemSlot[]>(this.UpdateBalloonCount));
		}
	}

	// Token: 0x0600019E RID: 414 RVA: 0x0000BB8B File Offset: 0x00009D8B
	private void FixedUpdate()
	{
		if (this.character.data.currentItem)
		{
			this.HoldItem(this.character.data.currentItem);
		}
	}

	// Token: 0x0600019F RID: 415 RVA: 0x0000BBBC File Offset: 0x00009DBC
	private void HoldItem(Item item)
	{
		Vector3 a = this.GetItemHoldPos(item) - item.transform.position;
		item.rig.AddForce(a * this.holdForce, ForceMode.Acceleration);
		Vector3 itemHoldForward = this.GetItemHoldForward(item);
		Vector3 itemHoldUp = this.GetItemHoldUp(item);
		Vector3 a2 = Vector3.Cross(item.transform.forward, itemHoldForward).normalized * Vector3.Angle(item.transform.forward, itemHoldForward);
		a2 += Vector3.Cross(item.transform.up, itemHoldUp).normalized * Vector3.Angle(item.transform.up, itemHoldUp);
		item.rig.AddTorque(a2 * this.holdTorque, ForceMode.Acceleration);
	}

	// Token: 0x060001A0 RID: 416 RVA: 0x0000BC8C File Offset: 0x00009E8C
	private void Update()
	{
		if (this.character.refs.afflictions.isWebbed)
		{
			return;
		}
		this.DoSwitching();
		this.DoDropping();
		this.DoUsing();
		this.UpdateClimbingSpikeUse();
		if (this.character.IsLocal)
		{
			this.isChargingThrow = (this.throwChargeLevel > 0f);
		}
	}

	// Token: 0x060001A1 RID: 417 RVA: 0x0000BCEC File Offset: 0x00009EEC
	private void DoUsing()
	{
		if (!this.character.data.currentItem)
		{
			return;
		}
		if (this.character.data.passedOut || this.character.data.fullyPassedOut)
		{
			return;
		}
		if (this.character.input.usePrimaryWasPressed && this.character.data.currentItem.CanUsePrimary())
		{
			this.character.data.currentItem.StartUsePrimary();
		}
		if (this.character.input.usePrimaryIsPressed && this.character.data.currentItem.CanUsePrimary())
		{
			this.character.data.currentItem.ContinueUsePrimary();
		}
		if (this.character.input.usePrimaryWasReleased || (this.character.data.currentItem.isUsingPrimary && !this.character.data.currentItem.CanUsePrimary()))
		{
			this.character.data.currentItem.CancelUsePrimary();
		}
		if (!this.character.CanDoInput())
		{
			this.character.data.currentItem.CancelUsePrimary();
		}
		if (this.character.input.useSecondaryIsPressed && this.character.data.currentItem.CanUseSecondary())
		{
			this.character.data.currentItem.StartUseSecondary();
		}
		if (this.character.input.useSecondaryIsPressed && this.character.data.currentItem.CanUseSecondary())
		{
			this.character.data.currentItem.ContinueUseSecondary();
		}
		if (this.character.input.useSecondaryWasReleased || (this.character.data.currentItem.isUsingSecondary && !this.character.data.currentItem.CanUseSecondary()))
		{
			this.character.data.currentItem.CancelUseSecondary();
		}
		if (this.character.input.scrollBackwardWasPressed)
		{
			this.character.data.currentItem.ScrollButtonBackwardPressed();
		}
		if (this.character.input.scrollForwardWasPressed)
		{
			this.character.data.currentItem.ScrollButtonForwardPressed();
		}
		if (this.character.input.scrollBackwardIsPressed)
		{
			this.character.data.currentItem.ScrollButtonBackwardHeld();
		}
		if (this.character.input.scrollForwardIsPressed)
		{
			this.character.data.currentItem.ScrollButtonForwardHeld();
		}
		if (this.character.input.scrollInput != 0f)
		{
			this.character.data.currentItem.Scroll(this.character.input.scrollInput);
		}
	}

	// Token: 0x060001A2 RID: 418 RVA: 0x0000BFD4 File Offset: 0x0000A1D4
	private void DoDropping()
	{
		if (this.character.data.currentItem != null && this.character.data.currentItem.progress > 0f)
		{
			this.throwChargeLevel = 0f;
			return;
		}
		if (this.character.input.dropWasPressed && this.character.data.currentItem && this.character.data.currentItem.UIData.canDrop)
		{
			this.lastPressedDrop = Time.time;
			this.pressedDrop = true;
		}
		if (this.pressedDrop && this.character.input.dropWasReleased && this.character.data.currentItem && this.currentSelectedSlot.IsSome)
		{
			Vector3 position = this.character.data.currentItem.transform.position;
			Vector3 vector = this.character.data.currentItem.rig.linearVelocity;
			if (this.character.data.currentItem is Backpack)
			{
				vector = Vector3.zero;
			}
			bool flag = false;
			if (this.character.data.currentItem != null)
			{
				StickyItemComponent currentStickyItem = this.character.data.currentStickyItem;
				if (currentStickyItem && this.throwChargeLevel < currentStickyItem.throwChargeRequirement)
				{
					flag = true;
				}
			}
			if ((this.throwChargeLevel > 0.1f || flag) && this.character.refs.animations)
			{
				this.character.refs.animations.throwTime = 0.125f;
			}
			if (flag)
			{
				return;
			}
			ItemSlot itemSlot = this.character.player.GetItemSlot(this.currentSelectedSlot.Value);
			if (itemSlot == null)
			{
				Debug.LogError(string.Format("Attempted to get an item in invalid slot {0}", this.currentSelectedSlot.Value));
				return;
			}
			this.photonView.RPC("DropItemRpc", RpcTarget.All, new object[]
			{
				this.throwChargeLevel,
				this.currentSelectedSlot.Value,
				position,
				vector,
				this.character.data.currentItem.transform.rotation,
				itemSlot.data,
				false
			});
			this.throwChargeLevel = 0f;
			this.EquipSlot(Optionable<byte>.None);
		}
		if (this.pressedDrop && this.character.input.dropIsPressed && Time.time - this.lastPressedDrop > this.delayBeforeThrowCharge)
		{
			this.throwChargeLevel = Mathf.Min(this.throwChargeLevel + 1f / this.throwChargeTime * Time.deltaTime, 1f);
			return;
		}
		this.throwChargeLevel = 0f;
	}

	// Token: 0x060001A3 RID: 419 RVA: 0x0000C2D8 File Offset: 0x0000A4D8
	internal void DropAllItems(bool includeBackpack)
	{
		if (!this.character.IsLocal)
		{
			return;
		}
		Transform transform = this.character.GetBodypart(BodypartType.Hip).transform;
		Vector3 vector = transform.forward;
		if (Vector3.Dot(vector, Vector3.up) < 0f)
		{
			vector = -vector;
		}
		Vector3 vector2 = transform.position + vector * 0.6f;
		if (this.currentSelectedSlot.IsSome && this.character.data.currentItem)
		{
			ItemSlot itemSlot = this.character.player.GetItemSlot(this.currentSelectedSlot.Value);
			if (itemSlot == null)
			{
				Debug.LogError(string.Format("Couldn't get item slot for index {0}", this.currentSelectedSlot.Value));
			}
			else if (!(itemSlot.prefab != null) || itemSlot.prefab.UIData.canDrop)
			{
				this.photonView.RPC("DropItemRpc", RpcTarget.All, new object[]
				{
					this.throwChargeLevel,
					this.currentSelectedSlot.Value,
					this.character.data.currentItem.transform.position,
					Vector3.zero,
					this.character.data.currentItem.transform.rotation,
					itemSlot.data,
					true
				});
				vector2 += Vector3.up * 0.5f;
			}
		}
		for (int i = includeBackpack ? 3 : 2; i >= 0; i--)
		{
			ItemSlot itemSlot2 = this.character.player.GetItemSlot((byte)i);
			if (!(itemSlot2.prefab != null) || itemSlot2.prefab.UIData.canDrop)
			{
				this.photonView.RPC("DropItemFromSlotRPC", RpcTarget.All, new object[]
				{
					(byte)i,
					vector2
				});
				vector2 += Vector3.up * 0.5f;
			}
		}
	}

	// Token: 0x060001A4 RID: 420 RVA: 0x0000C504 File Offset: 0x0000A704
	[PunRPC]
	internal void DropItemFromSlotRPC(byte slotID, Vector3 spawnPosition)
	{
		ItemSlot itemSlot = this.character.player.GetItemSlot(slotID);
		if (itemSlot == null)
		{
			Debug.LogError(string.Format("Can't drop item from non-existent slot {0}", slotID));
			return;
		}
		if (!itemSlot.IsEmpty())
		{
			if (PhotonNetwork.IsMasterClient)
			{
				PhotonView component = PhotonNetwork.Instantiate("0_Items/" + itemSlot.GetPrefabName(), spawnPosition, Quaternion.identity, 0, null).GetComponent<PhotonView>();
				component.RPC("SetItemInstanceDataRPC", RpcTarget.All, new object[]
				{
					itemSlot.data
				});
				component.RPC("SetKinematicRPC", RpcTarget.All, new object[]
				{
					false,
					component.transform.position,
					component.transform.rotation
				});
				this.droppedItems.Add(component);
			}
			this.character.player.EmptySlot(Optionable<byte>.Some(slotID));
		}
		this.character.refs.afflictions.UpdateWeight();
	}

	// Token: 0x060001A5 RID: 421 RVA: 0x0000C608 File Offset: 0x0000A808
	[PunRPC]
	public void DestroyHeldItemRpc()
	{
		Item currentItem = this.character.data.currentItem;
		if (currentItem == null)
		{
			return;
		}
		this.UnAttachEquippedItem();
		if (currentItem.photonView.IsMine || (currentItem.photonView.Controller.IsMasterClient && PhotonNetwork.IsMasterClient))
		{
			PhotonNetwork.Destroy(currentItem.gameObject);
		}
	}

	// Token: 0x060001A6 RID: 422 RVA: 0x0000C668 File Offset: 0x0000A868
	[PunRPC]
	public void DropItemRpc(float throwCharge, byte slotID, Vector3 spawnPos, Vector3 velocity, Quaternion rotation, ItemInstanceData itemInstanceData, bool cacheToDroppedItems)
	{
		if (!this.character.data.currentItem)
		{
			return;
		}
		float d = 0f;
		if (throwCharge > 0f)
		{
			d = this.minThrowForce + (this.maxThrowForce - this.minThrowForce) * throwCharge;
		}
		Item currentItem = this.character.data.currentItem;
		this.UnAttachEquippedItem();
		if (currentItem.photonView.IsMine || (currentItem.photonView.Controller.IsMasterClient && PhotonNetwork.IsMasterClient))
		{
			PhotonNetwork.Destroy(currentItem.gameObject);
		}
		ItemSlot itemSlot = this.character.player.GetItemSlot(slotID);
		if (itemSlot == null)
		{
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			string prefabName = itemSlot.GetPrefabName();
			if (string.IsNullOrEmpty(prefabName))
			{
				return;
			}
			Vector3 normalized = HelperFunctions.LookToDirection(this.character.data.lookValues, Vector3.forward).normalized;
			PhotonView component = PhotonNetwork.InstantiateItemRoom(prefabName, spawnPos, rotation).GetComponent<PhotonView>();
			GameUtils.instance.IgnoreCollisions(this.character.gameObject, component.gameObject, 0.5f);
			Rigidbody component2 = component.GetComponent<Rigidbody>();
			component.RPC("SetKinematicRPC", RpcTarget.AllBuffered, new object[]
			{
				false,
				component.transform.position,
				component.transform.rotation
			});
			if (cacheToDroppedItems)
			{
				this.droppedItems.Add(component);
			}
			Item component3 = component.GetComponent<Item>();
			if (component3)
			{
				component3.photonView.RPC("RPC_SetThrownData", RpcTarget.All, new object[]
				{
					this.character.photonView.ViewID,
					throwCharge
				});
			}
			component2.linearVelocity = velocity + normalized * d * 0.5f * component3.throwForceMultiplier;
			if (!component3.GetComponent<Frisbee>())
			{
				component2.angularVelocity = Vector3.Cross(normalized, Vector3.up) * d * 0.5f;
			}
			component.RPC("SetItemInstanceDataRPC", RpcTarget.All, new object[]
			{
				itemInstanceData
			});
		}
		this.character.player.EmptySlot(Optionable<byte>.Some(slotID));
		this.pressedDrop = false;
		this.character.refs.afflictions.UpdateWeight();
	}

	// Token: 0x060001A7 RID: 423 RVA: 0x0000C8D8 File Offset: 0x0000AAD8
	[PunRPC]
	public void OnPickupAccepted(byte slotID)
	{
		if (slotID != 3)
		{
			if (this.character.data.isClimbingAnything)
			{
				if (slotID == 250)
				{
					Debug.LogWarning("Accidental wall stow in temp slot detected! Dropping item.");
					Transform transform = this.character.GetBodypart(BodypartType.Hip).transform;
					Vector3 forward = transform.forward;
					Vector3 vector = transform.position - forward * 0.6f;
					this.photonView.RPC("DropItemFromSlotRPC", RpcTarget.All, new object[]
					{
						slotID,
						vector
					});
				}
			}
			else
			{
				this.character.refs.items.EquipSlot(Optionable<byte>.Some(slotID));
			}
		}
		else if (this.character.data.carriedPlayer != null)
		{
			this.character.refs.carriying.Drop(this.character.data.carriedPlayer);
		}
		this.RefreshAllCharacterCarryWeight();
	}

	// Token: 0x060001A8 RID: 424 RVA: 0x0000C9CC File Offset: 0x0000ABCC
	public void RefreshAllCharacterCarryWeight()
	{
		this.photonView.RPC("RefreshAllCharacterCarryWeightRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x060001A9 RID: 425 RVA: 0x0000C9E4 File Offset: 0x0000ABE4
	[PunRPC]
	public void RefreshAllCharacterCarryWeightRPC()
	{
		List<Character> allPlayerCharacters = PlayerHandler.GetAllPlayerCharacters();
		for (int i = 0; i < allPlayerCharacters.Count; i++)
		{
			allPlayerCharacters[i].refs.afflictions.UpdateWeight();
		}
	}

	// Token: 0x060001AA RID: 426 RVA: 0x0000CA20 File Offset: 0x0000AC20
	public void EquipSlot(Optionable<byte> slotID)
	{
		CharacterItems.<>c__DisplayClass32_0 CS$<>8__locals1 = new CharacterItems.<>c__DisplayClass32_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.slotID = slotID;
		this.lastEquippedSlotTime = Time.time;
		CS$<>8__locals1.waitForFrames = false;
		if (CS$<>8__locals1.slotID.IsSome)
		{
			this.lastSelectedSlot = CS$<>8__locals1.slotID;
		}
		if (this.photonView.IsMine && this.character.data.currentItem != null)
		{
			this.character.data.currentItem.CancelUsePrimary();
			this.character.data.currentItem.CancelUseSecondary();
			if (!this.character.data.currentItem.UIData.canPocket || (this.currentSelectedSlot.IsSome && this.currentSelectedSlot.Value == 250 && !this.character.player.GetItemSlot(this.currentSelectedSlot.Value).IsEmpty()))
			{
				Vector3 vector = this.character.data.currentItem.transform.position + Vector3.down * 0.2f;
				Vector3 linearVelocity = this.character.data.currentItem.rig.linearVelocity;
				CS$<>8__locals1.waitForFrames = true;
				ItemSlot itemSlot = this.character.player.GetItemSlot(this.currentSelectedSlot.Value);
				this.photonView.RPC("DropItemRpc", RpcTarget.All, new object[]
				{
					this.throwChargeLevel,
					this.currentSelectedSlot.Value,
					vector,
					linearVelocity,
					this.character.data.currentItem.transform.rotation,
					itemSlot.data,
					false
				});
			}
		}
		base.StartCoroutine(CS$<>8__locals1.<EquipSlot>g__TheRest|0());
	}

	// Token: 0x060001AB RID: 427 RVA: 0x0000CC20 File Offset: 0x0000AE20
	[PunRPC]
	public void EquipSlotRpc(int slotID, int objectViewID)
	{
		if (!this.photonView.IsMine)
		{
			if (slotID == -1)
			{
				if (this.currentSelectedSlot.IsSome)
				{
					this.lastSelectedSlot = this.currentSelectedSlot;
				}
				this.currentSelectedSlot = Optionable<byte>.None;
			}
			else
			{
				this.currentSelectedSlot = Optionable<byte>.Some((byte)slotID);
			}
		}
		PhotonView photonView = null;
		if (objectViewID != -1)
		{
			photonView = PhotonNetwork.GetPhotonView(objectViewID);
		}
		Item item;
		if (photonView != null)
		{
			item = this.Equip(photonView.GetComponent<Item>());
		}
		else
		{
			item = this.Equip(null);
		}
		if (this.photonView.IsMine && item != null)
		{
			item.OnStash();
			Debug.Log(this.character.gameObject.name + " destroying " + item.gameObject.name);
			PhotonNetwork.Destroy(item.GetComponent<PhotonView>());
		}
		if (this.character.player.itemsChangedAction != null)
		{
			this.character.player.itemsChangedAction(this.character.player.itemSlots);
		}
		Action action = this.onSlotEquipped;
		if (action == null)
		{
			return;
		}
		action();
	}

	// Token: 0x060001AC RID: 428 RVA: 0x0000CD38 File Offset: 0x0000AF38
	public Item Equip(Item item)
	{
		CharacterItems.<>c__DisplayClass35_0 CS$<>8__locals1 = new CharacterItems.<>c__DisplayClass35_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.item = item;
		Item currentItem = this.character.data.currentItem;
		this.pressedDrop = false;
		if (this.character.data.currentItem)
		{
			this.UnAttachEquippedItem();
		}
		if (CS$<>8__locals1.item == null)
		{
			return currentItem;
		}
		this.character.data.currentItem = CS$<>8__locals1.item;
		CS$<>8__locals1.item.holderCharacter = this.character;
		CS$<>8__locals1.item.SetState(ItemState.Held, this.character);
		base.StartCoroutine(CS$<>8__locals1.<Equip>g__IWait|0());
		return currentItem;
	}

	// Token: 0x060001AD RID: 429 RVA: 0x0000CDE8 File Offset: 0x0000AFE8
	private void AttachItem(Item item)
	{
		this.character.GetBodypartRig(BodypartType.Hand_R).transform.position = this.GetItemPosRightWorld(item);
		this.character.GetBodypartRig(BodypartType.Hand_L).transform.position = this.GetItemPosLeftWorld(item);
		this.character.GetBodypartRig(BodypartType.Hand_R).transform.rotation = this.GetItemRotRightWorld(item);
		this.character.GetBodypartRig(BodypartType.Hand_L).transform.rotation = this.GetItemRotLeftWorld(item);
		this.character.GetBodypartRig(BodypartType.Hand_R).gameObject.AddComponent<FixedJoint>().connectedBody = item.rig;
		this.character.GetBodypartRig(BodypartType.Hand_L).gameObject.AddComponent<FixedJoint>().connectedBody = item.rig;
	}

	// Token: 0x060001AE RID: 430 RVA: 0x0000CEB0 File Offset: 0x0000B0B0
	public void UpdateAttachedItem()
	{
		if (!this.character.data.currentItem)
		{
			return;
		}
		this.character.GetBodypartRig(BodypartType.Hand_R).transform.position = this.GetItemPosRightWorld(this.character.data.currentItem);
		this.character.GetBodypartRig(BodypartType.Hand_L).transform.position = this.GetItemPosLeftWorld(this.character.data.currentItem);
		this.character.GetBodypartRig(BodypartType.Hand_R).transform.rotation = this.GetItemRotRightWorld(this.character.data.currentItem);
		this.character.GetBodypartRig(BodypartType.Hand_L).transform.rotation = this.GetItemRotLeftWorld(this.character.data.currentItem);
	}

	// Token: 0x060001AF RID: 431 RVA: 0x0000CF87 File Offset: 0x0000B187
	private void UnAttachItem()
	{
		if (this.<UnAttachItem>g__TryDestroyJoint|38_0(BodypartType.Hand_L))
		{
			this.<UnAttachItem>g__TryDestroyJoint|38_0(BodypartType.Hand_R);
			return;
		}
	}

	// Token: 0x060001B0 RID: 432 RVA: 0x0000CF9C File Offset: 0x0000B19C
	private Quaternion GetItemHoldRotation(Item item)
	{
		return Quaternion.LookRotation(this.GetItemHoldForward(item), this.GetItemHoldUp(item));
	}

	// Token: 0x060001B1 RID: 433 RVA: 0x0000CFB1 File Offset: 0x0000B1B1
	private Vector3 GetItemHoldUp(Item item)
	{
		return this.character.data.lookDirection_Up;
	}

	// Token: 0x060001B2 RID: 434 RVA: 0x0000CFC3 File Offset: 0x0000B1C3
	private Vector3 GetItemHoldForward(Item item)
	{
		return this.character.data.lookDirection;
	}

	// Token: 0x060001B3 RID: 435 RVA: 0x0000CFD8 File Offset: 0x0000B1D8
	public Vector3 GetItemHoldPos(Item item)
	{
		Vector3 b = this.character.refs.animationItemTransform.position - this.character.refs.animationHipTransform.position;
		return this.character.refs.hip.transform.position + b;
	}

	// Token: 0x060001B4 RID: 436 RVA: 0x0000D035 File Offset: 0x0000B235
	public void UnAttachEquippedItem()
	{
		this.UnAttachItem();
		this.character.data.currentItem = null;
	}

	// Token: 0x17000029 RID: 41
	// (get) Token: 0x060001B5 RID: 437 RVA: 0x0000D04E File Offset: 0x0000B24E
	private float equippedSlotCooldown
	{
		get
		{
			if (this.timesSwitchedRecently >= 3)
			{
				return 0.25f;
			}
			return 0.1f;
		}
	}

	// Token: 0x1700002A RID: 42
	// (get) Token: 0x060001B6 RID: 438 RVA: 0x0000D064 File Offset: 0x0000B264
	private bool lockedFromSwitching
	{
		get
		{
			return this.lastEquippedSlotTime + this.equippedSlotCooldown > Time.time;
		}
	}

	// Token: 0x060001B7 RID: 439 RVA: 0x0000D07C File Offset: 0x0000B27C
	private void DoSwitching()
	{
		if (this.timesSwitchedRecently > 0 && this.lastSwitched + 0.4f < Time.time)
		{
			this.timesSwitchedRecently = 0;
		}
		if (this.character.data.currentItem != null)
		{
			if (this.character.data.currentItem.progress > 0f)
			{
				return;
			}
			if (this.character.data.currentItem.lastFinishedCast + 0.1f > Time.time)
			{
				return;
			}
		}
		if (!this.character.data.fullyConscious)
		{
			return;
		}
		if (this.character.IsLocal && this.character.CanDoInput() && !this.lockedFromSwitching)
		{
			if (this.character.data.isClimbing || this.character.data.isRopeClimbing)
			{
				return;
			}
			if (this.character.input.selectSlotForwardWasPressed)
			{
				if (this.character.data.currentStickyItem)
				{
					this.character.refs.animations.throwTime = 0.125f;
					return;
				}
				this.character.player.GetItemSlot(3).IsEmpty();
				byte b = decimal.ToByte((int)(this.lastSelectedSlot.Value + 1));
				if ((int)b > this.character.player.itemSlots.Length)
				{
					Debug.Log("Looping to start");
					b = 0;
				}
				this.lastSwitched = Time.time;
				this.timesSwitchedRecently++;
				this.EquipSlot(Optionable<byte>.Some(b));
			}
			else if (this.character.input.selectSlotBackwardWasPressed)
			{
				if (this.character.data.currentStickyItem)
				{
					this.character.refs.animations.throwTime = 0.125f;
					return;
				}
				int num = (this.lastSelectedSlot.Value == 250) ? -1 : ((int)(this.lastSelectedSlot.Value - 1));
				if (num < 0)
				{
					num = (int)decimal.ToByte(3m);
					Debug.Log("Looping to end");
				}
				this.lastSwitched = Time.time;
				this.timesSwitchedRecently++;
				this.EquipSlot(Optionable<byte>.Some(decimal.ToByte(num)));
			}
			else if (this.character.input.unselectSlotWasPressed && !this.lockedFromSwitching)
			{
				if (this.character.data.currentStickyItem)
				{
					this.character.refs.animations.throwTime = 0.125f;
					return;
				}
				if (this.currentSelectedSlot.IsSome)
				{
					this.lastSwitched = Time.time;
					this.timesSwitchedRecently++;
					this.EquipSlot(Optionable<byte>.None);
				}
				else
				{
					this.lastSwitched = Time.time;
					this.timesSwitchedRecently++;
					this.EquipSlot(this.lastSelectedSlot);
				}
			}
			for (byte b2 = 0; b2 <= 3; b2 += 1)
			{
				if (this.character.input.SelectSlotWasPressed((int)b2))
				{
					if (this.character.data.currentStickyItem)
					{
						this.character.refs.animations.throwTime = 0.125f;
						return;
					}
					if (!this.character.player.itemSlots.WithinRange((int)b2) && b2 != 3)
					{
						this.lastSwitched = Time.time;
						this.timesSwitchedRecently++;
						this.EquipSlot(Optionable<byte>.None);
					}
					else
					{
						if (this.currentSelectedSlot.IsSome && this.currentSelectedSlot.Value == b2)
						{
							this.lastSwitched = Time.time;
							this.timesSwitchedRecently++;
							this.EquipSlot(Optionable<byte>.None);
						}
						else
						{
							this.lastSwitched = Time.time;
							this.timesSwitchedRecently++;
							this.EquipSlot(Optionable<byte>.Some(b2));
						}
						if (b2 == 3 && this.character.data.carriedPlayer != null)
						{
							this.character.refs.carriying.Drop(this.character.data.carriedPlayer);
						}
					}
				}
			}
		}
	}

	// Token: 0x060001B8 RID: 440 RVA: 0x0000D4D3 File Offset: 0x0000B6D3
	internal void AddGravity(Vector3 gravity)
	{
		this.character.data.currentItem.rig.AddForce(gravity, ForceMode.Acceleration);
	}

	// Token: 0x060001B9 RID: 441 RVA: 0x0000D4F1 File Offset: 0x0000B6F1
	internal void AddMovementForce(float movementForce)
	{
		this.character.data.currentItem.rig.AddForce(movementForce * this.character.data.worldMovementInput_Grounded, ForceMode.Acceleration);
	}

	// Token: 0x060001BA RID: 442 RVA: 0x0000D524 File Offset: 0x0000B724
	internal void AddDrag(float drag, float factor = 1f)
	{
		drag = Mathf.Lerp(1f, drag, factor);
		this.character.data.currentItem.rig.linearVelocity *= drag;
		this.character.data.currentItem.rig.angularVelocity *= drag;
	}

	// Token: 0x060001BB RID: 443 RVA: 0x0000D58C File Offset: 0x0000B78C
	internal void AddParasolDrag(float drag, float xzDrag, float factor = 1f)
	{
		drag = Mathf.Lerp(1f, drag, factor);
		if (this.character.data.currentItem.rig.linearVelocity.y < 0f)
		{
			this.character.data.currentItem.rig.linearVelocity = new Vector3(this.character.data.currentItem.rig.linearVelocity.x * xzDrag, this.character.data.currentItem.rig.linearVelocity.y * drag, this.character.data.currentItem.rig.linearVelocity.z * xzDrag);
		}
	}

	// Token: 0x060001BC RID: 444 RVA: 0x0000D652 File Offset: 0x0000B852
	internal Vector3 GetItemPosRightWorld(Item item)
	{
		return item.transform.Find("Hand_R").position;
	}

	// Token: 0x060001BD RID: 445 RVA: 0x0000D669 File Offset: 0x0000B869
	internal Vector3 GetItemPosLeftWorld(Item item)
	{
		return item.transform.Find("Hand_L").position;
	}

	// Token: 0x060001BE RID: 446 RVA: 0x0000D680 File Offset: 0x0000B880
	internal Quaternion GetItemRotRightWorld(Item item)
	{
		return item.transform.Find("Hand_R").rotation;
	}

	// Token: 0x060001BF RID: 447 RVA: 0x0000D697 File Offset: 0x0000B897
	internal Quaternion GetItemRotLeftWorld(Item item)
	{
		return item.transform.Find("Hand_L").rotation;
	}

	// Token: 0x060001C0 RID: 448 RVA: 0x0000D6B0 File Offset: 0x0000B8B0
	internal Vector3 GetItemPosRight(Item item)
	{
		Vector3 localPosition = item.transform.Find("Hand_R").localPosition;
		return this.character.refs.animationItemTransform.TransformPoint(localPosition);
	}

	// Token: 0x060001C1 RID: 449 RVA: 0x0000D6EC File Offset: 0x0000B8EC
	internal Quaternion GetItemRotRight(Item item)
	{
		Transform transform = item.transform.Find("Hand_R");
		Vector3 direction = item.transform.InverseTransformDirection(transform.forward);
		Vector3 direction2 = item.transform.InverseTransformDirection(transform.up);
		Vector3 forward = this.character.refs.animationItemTransform.TransformDirection(direction);
		Vector3 upwards = this.character.refs.animationItemTransform.TransformDirection(direction2);
		return Quaternion.LookRotation(forward, upwards);
	}

	// Token: 0x060001C2 RID: 450 RVA: 0x0000D764 File Offset: 0x0000B964
	internal Quaternion GetItemRotLeft(Item item)
	{
		Transform transform = item.transform.Find("Hand_L");
		Vector3 direction = item.transform.InverseTransformDirection(transform.forward);
		Vector3 direction2 = item.transform.InverseTransformDirection(transform.up);
		Vector3 forward = this.character.refs.animationItemTransform.TransformDirection(direction);
		Vector3 upwards = this.character.refs.animationItemTransform.TransformDirection(direction2);
		return Quaternion.LookRotation(forward, upwards);
	}

	// Token: 0x060001C3 RID: 451 RVA: 0x0000D7DC File Offset: 0x0000B9DC
	internal Vector3 GetItemPosLeft(Item item)
	{
		Vector3 position = HelperFunctions.MultiplyVectors(item.transform.Find("Hand_L").localPosition, item.transform.lossyScale);
		return this.character.refs.animationItemTransform.TransformPoint(position);
	}

	// Token: 0x060001C4 RID: 452 RVA: 0x0000D828 File Offset: 0x0000BA28
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (PhotonNetwork.IsMasterClient && this.character.data.currentItem != null)
		{
			Debug.Log("Setting " + base.gameObject.name + " to hold " + this.character.data.currentItem.name);
			this.photonView.RPC("RPC_InitHoldingItem", newPlayer, new object[]
			{
				this.character.data.currentItem.GetComponent<PhotonView>()
			});
		}
	}

	// Token: 0x060001C5 RID: 453 RVA: 0x0000D8BE File Offset: 0x0000BABE
	[PunRPC]
	public void RPC_InitHoldingItem(PhotonView item)
	{
		Debug.Log("Init holding item: " + item.name);
		this.Equip(item.GetComponent<Item>());
	}

	// Token: 0x060001C6 RID: 454 RVA: 0x0000D8E4 File Offset: 0x0000BAE4
	public void UpdateClimbingSpikeCount(ItemSlot[] slots)
	{
		int num = 0;
		this.currentClimbingSpikeComponent = null;
		this.currentClimbingSpikeItemSlot = null;
		foreach (ItemSlot itemSlot in slots)
		{
			if (itemSlot != null && itemSlot.prefab != null)
			{
				ClimbingSpikeComponent component = itemSlot.prefab.GetComponent<ClimbingSpikeComponent>();
				IntItemData intItemData;
				if (component != null && (!itemSlot.data.TryGetDataEntry<IntItemData>(DataEntryKey.CookedAmount, out intItemData) || intItemData.Value <= 0))
				{
					num++;
					if (this.currentClimbingSpikeComponent == null)
					{
						this.currentClimbingSpikeComponent = component;
						this.currentClimbingSpikeItemSlot = itemSlot;
					}
				}
			}
		}
		this.character.data.climbingSpikeCount = num;
	}

	// Token: 0x060001C7 RID: 455 RVA: 0x0000D984 File Offset: 0x0000BB84
	public void UpdateBalloonCount(ItemSlot[] slots)
	{
		if (!this.character.refs.items.currentSelectedSlot.IsSome)
		{
			this.character.refs.balloons.heldBalloonCount = 0;
			return;
		}
		ItemSlot itemSlot = this.character.player.GetItemSlot(this.character.refs.items.currentSelectedSlot.Value);
		if (itemSlot == null)
		{
			Debug.LogError(string.Format("Can't get invalid item slot {0}", this.character.refs.items.currentSelectedSlot.Value));
		}
		if (!itemSlot.IsEmpty() && this.character.refs.items.currentSelectedSlot.Value != 3 && this.character.refs.items.currentSelectedSlot.IsSome && itemSlot.prefab.GetComponent<Balloon>())
		{
			this.character.refs.balloons.heldBalloonCount = 1;
			return;
		}
		this.character.refs.balloons.heldBalloonCount = 0;
	}

	// Token: 0x060001C8 RID: 456 RVA: 0x0000DAA4 File Offset: 0x0000BCA4
	private bool WithinClimbingSpikePreviewRange()
	{
		if (this.currentClimbingSpikePreview)
		{
			float num = this.character.data.isClimbingAnything ? this.currentClimbingSpikeComponent.climbingSpikePreviewDisableDistance : this.currentClimbingSpikeComponent.climbingSpikePreviewDisableDistanceGrounded;
			return Vector3.Distance(MainCamera.instance.transform.position, this.currentClimbingSpikePreview.transform.position) <= num;
		}
		return false;
	}

	// Token: 0x1700002B RID: 43
	// (get) Token: 0x060001C9 RID: 457 RVA: 0x0000DB15 File Offset: 0x0000BD15
	public float climbingSpikeCastProgress
	{
		get
		{
			if (this.currentClimbingSpikeItemSlot == null)
			{
				return 0f;
			}
			if (this.currentClimbingSpikeItemSlot.prefab == null)
			{
				return 0f;
			}
			return this.climbingSpikeTick / this.currentClimbingSpikeItemSlot.prefab.usingTimePrimary;
		}
	}

	// Token: 0x060001CA RID: 458 RVA: 0x0000DB58 File Offset: 0x0000BD58
	private void UpdateClimbingSpikeUse()
	{
		if (this.character.data.climbingSpikeCount <= 0 || this.currentClimbingSpikeItemSlot == null)
		{
			this.CancelClimbingSpike();
			return;
		}
		if (this.climbingSpikeTick > 0f)
		{
			if (!this.WithinClimbingSpikePreviewRange())
			{
				this.CancelClimbingSpike();
				return;
			}
			if ((this.spikingWithPrimary && !this.character.input.usePrimaryIsPressed) || (this.spikingWithSecondary && !this.character.input.useSecondaryIsPressed))
			{
				this.CancelClimbingSpike();
				return;
			}
			this.climbingSpikeTick += Time.deltaTime;
			if (this.climbingSpikeTick >= this.currentClimbingSpikeItemSlot.prefab.usingTimePrimary)
			{
				this.HammerClimbingSpike(this.climbingSpikeHit);
				this.CancelClimbingSpike();
			}
			return;
		}
		else
		{
			if (!this.RaycastClimbingSpikeStart())
			{
				this.climbingSpikeTick = 0f;
				return;
			}
			if (this.climbingSpikeTick == 0f)
			{
				if (this.character.input.usePrimaryIsPressed && !this.character.data.isClimbingAnything && this.climbingSpikeSelected)
				{
					this.spikingWithPrimary = true;
					this.spikingWithSecondary = false;
					this.climbingSpikeTick += Time.deltaTime;
					this.InstantiateClimbingSpikePreview(this.climbingSpikeHit);
					return;
				}
				if (this.character.input.useSecondaryIsPressed && (this.climbingSpikeSelected || this.character.data.isClimbingAnything))
				{
					this.spikingWithPrimary = false;
					this.spikingWithSecondary = true;
					this.climbingSpikeTick += Time.deltaTime;
					this.InstantiateClimbingSpikePreview(this.climbingSpikeHit);
				}
			}
			return;
		}
	}

	// Token: 0x1700002C RID: 44
	// (get) Token: 0x060001CB RID: 459 RVA: 0x0000DCF3 File Offset: 0x0000BEF3
	private bool climbingSpikeSelected
	{
		get
		{
			return this.currentClimbingSpikeItemSlot != null && this.currentSelectedSlot.IsSome && this.currentSelectedSlot.Value == this.currentClimbingSpikeItemSlot.itemSlotID;
		}
	}

	// Token: 0x060001CC RID: 460 RVA: 0x0000DD24 File Offset: 0x0000BF24
	private void CancelClimbingSpike()
	{
		if (this.currentClimbingSpikePreview)
		{
			Debug.Log("Cancelling climbing spike");
			Object.Destroy(this.currentClimbingSpikePreview);
		}
		this.climbingSpikeTick = 0f;
	}

	// Token: 0x060001CD RID: 461 RVA: 0x0000DD54 File Offset: 0x0000BF54
	private void InstantiateClimbingSpikePreview(RaycastHit hit)
	{
		if (!this.currentClimbingSpikePreview && this.currentClimbingSpikeComponent != null)
		{
			this.currentClimbingSpikePreview = Object.Instantiate<GameObject>(this.currentClimbingSpikeComponent.climbingSpikePreviewPrefab);
		}
		if (this.currentClimbingSpikePreview)
		{
			this.currentClimbingSpikePreview.transform.position = this.climbingSpikeHit.point;
			this.currentClimbingSpikePreview.transform.rotation = Quaternion.LookRotation(-this.climbingSpikeHit.normal, Vector3.up);
		}
	}

	// Token: 0x060001CE RID: 462 RVA: 0x0000DDE4 File Offset: 0x0000BFE4
	public bool RaycastClimbingSpikeStart()
	{
		float maxDistance = this.character.data.isClimbingAnything ? this.currentClimbingSpikeComponent.climbingSpikeStartDistance : this.currentClimbingSpikeComponent.climbingSpikeStartDistanceGrounded;
		return Physics.Raycast(MainCamera.instance.transform.position, MainCamera.instance.transform.forward, out this.climbingSpikeHit, maxDistance, HelperFunctions.GetMask(HelperFunctions.LayerType.TerrainMap));
	}

	// Token: 0x060001CF RID: 463 RVA: 0x0000DE54 File Offset: 0x0000C054
	private void HammerClimbingSpike(RaycastHit hit)
	{
		if (this.currentClimbingSpikeComponent != null && PhotonNetwork.Instantiate("0_Items/" + this.currentClimbingSpikeComponent.hammeredVersionPrefab.gameObject.name, hit.point, Quaternion.LookRotation(-hit.normal, Vector3.up), 0, null) != null)
		{
			if (this.currentClimbingSpikeItemSlot != null)
			{
				ItemSlot itemSlot = this.currentClimbingSpikeItemSlot;
				this.currentClimbingSpikeItemSlot = null;
				this.currentClimbingSpikeComponent = null;
				this.character.player.EmptySlot(Optionable<byte>.Some(itemSlot.itemSlotID));
				if (this.character.data.currentItem != null)
				{
					this.EquipSlot(Optionable<byte>.None);
				}
				this.UpdateClimbingSpikeCount(this.character.player.itemSlots);
				this.character.data.lastConsumedItem = Time.time;
			}
			this.character.refs.afflictions.UpdateWeight();
			Singleton<AchievementManager>.Instance.IncrementSteamStat(STEAMSTATTYPE.PitonsPlaced, 1);
			GameUtils.instance.IncrementPermanentItemsPlaced();
		}
	}

	// Token: 0x060001D0 RID: 464 RVA: 0x0000DF72 File Offset: 0x0000C172
	internal void SpawnItemInHand(string objName)
	{
		this.photonView.RPC("RPC_SpawnItemInHandMaster", RpcTarget.MasterClient, new object[]
		{
			objName
		});
	}

	// Token: 0x060001D1 RID: 465 RVA: 0x0000DF90 File Offset: 0x0000C190
	[PunRPC]
	private void RPC_SpawnItemInHandMaster(string objName)
	{
		Vector3 b = this.character.GetBodypart(BodypartType.Torso).transform.TransformVector(2f * Vector3.forward);
		PhotonNetwork.Instantiate("0_Items/" + objName, this.character.Center + b, Quaternion.identity, 0, null).GetComponent<Item>().Interact(this.character);
	}

	// Token: 0x060001D3 RID: 467 RVA: 0x0000E018 File Offset: 0x0000C218
	[CompilerGenerated]
	private bool <UnAttachItem>g__TryDestroyJoint|38_0(BodypartType bodyPart)
	{
		if (this.character == null)
		{
			return false;
		}
		Rigidbody bodypartRig = this.character.GetBodypartRig(bodyPart);
		if (bodypartRig == null)
		{
			return false;
		}
		FixedJoint obj;
		if (!bodypartRig.gameObject.TryGetComponent<FixedJoint>(out obj))
		{
			return false;
		}
		Object.Destroy(obj);
		return true;
	}

	// Token: 0x04000199 RID: 409
	public SFX_Instance cookSfx;

	// Token: 0x0400019A RID: 410
	public float holdForce;

	// Token: 0x0400019B RID: 411
	public float holdTorque;

	// Token: 0x0400019C RID: 412
	public float throwChargeTime;

	// Token: 0x0400019D RID: 413
	public float minThrowForce;

	// Token: 0x0400019E RID: 414
	public float maxThrowForce;

	// Token: 0x0400019F RID: 415
	public float delayBeforeThrowCharge;

	// Token: 0x040001A0 RID: 416
	[NonSerialized]
	public Optionable<byte> currentSelectedSlot;

	// Token: 0x040001A1 RID: 417
	[NonSerialized]
	public Optionable<byte> lastSelectedSlot;

	// Token: 0x040001A2 RID: 418
	private Character character;

	// Token: 0x040001A3 RID: 419
	private new PhotonView photonView;

	// Token: 0x040001A4 RID: 420
	public float lastEquippedSlotTime;

	// Token: 0x040001A5 RID: 421
	public List<PhotonView> droppedItems = new List<PhotonView>();

	// Token: 0x040001A6 RID: 422
	[HideInInspector]
	public float throwChargeLevel;

	// Token: 0x040001A7 RID: 423
	public bool isChargingThrow;

	// Token: 0x040001A8 RID: 424
	private float lastPressedDrop;

	// Token: 0x040001A9 RID: 425
	private bool pressedDrop;

	// Token: 0x040001AA RID: 426
	public Action onSlotEquipped;

	// Token: 0x040001AB RID: 427
	public const int MAX_SLOT = 3;

	// Token: 0x040001AC RID: 428
	private float lastSwitched;

	// Token: 0x040001AD RID: 429
	private int timesSwitchedRecently;

	// Token: 0x040001AE RID: 430
	private float climbingSpikeTick;

	// Token: 0x040001AF RID: 431
	private bool readyToSpike = true;

	// Token: 0x040001B0 RID: 432
	private bool spikingWithPrimary;

	// Token: 0x040001B1 RID: 433
	private bool spikingWithSecondary;

	// Token: 0x040001B2 RID: 434
	private ItemSlot currentClimbingSpikeItemSlot;

	// Token: 0x040001B3 RID: 435
	private ClimbingSpikeComponent currentClimbingSpikeComponent;

	// Token: 0x040001B4 RID: 436
	private GameObject currentClimbingSpikePreview;

	// Token: 0x040001B5 RID: 437
	private RaycastHit climbingSpikeHit;
}

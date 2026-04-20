using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using pworld.Scripts.Extensions;
using Sirenix.Utilities;
using Unity.Collections;
using UnityEngine;
using WebSocketSharp;
using Zorro.ControllerSupport;
using Zorro.Core;
using Zorro.Core.Serizalization;
using Zorro.Settings;

// Token: 0x02000024 RID: 36
public class Item : MonoBehaviourPunCallbacks, IInteractible
{
	// Token: 0x17000035 RID: 53
	// (get) Token: 0x0600027B RID: 635 RVA: 0x000128C0 File Offset: 0x00010AC0
	// (set) Token: 0x0600027C RID: 636 RVA: 0x000128C8 File Offset: 0x00010AC8
	public ItemState itemState { get; set; }

	// Token: 0x17000036 RID: 54
	// (get) Token: 0x0600027D RID: 637 RVA: 0x000128D4 File Offset: 0x00010AD4
	public int CarryWeight
	{
		get
		{
			int itemWeightModifier = Ascents.itemWeightModifier;
			if (itemWeightModifier == -1)
			{
				return 0;
			}
			return this.carryWeight + itemWeightModifier;
		}
	}

	// Token: 0x17000037 RID: 55
	// (get) Token: 0x0600027E RID: 638 RVA: 0x000128F5 File Offset: 0x00010AF5
	// (set) Token: 0x0600027F RID: 639 RVA: 0x000128FD File Offset: 0x00010AFD
	public bool isUsingPrimary { get; private set; }

	// Token: 0x17000038 RID: 56
	// (get) Token: 0x06000280 RID: 640 RVA: 0x00012906 File Offset: 0x00010B06
	// (set) Token: 0x06000281 RID: 641 RVA: 0x0001290E File Offset: 0x00010B0E
	public ItemCooking cooking { get; private set; }

	// Token: 0x06000282 RID: 642 RVA: 0x00012917 File Offset: 0x00010B17
	public override void OnEnable()
	{
		base.OnEnable();
		if (!this.rig.isKinematic)
		{
			this.WasActive();
		}
	}

	// Token: 0x06000283 RID: 643 RVA: 0x00012932 File Offset: 0x00010B32
	public override void OnDisable()
	{
		base.OnDisable();
		this.RemoveFromActiveList();
	}

	// Token: 0x06000284 RID: 644 RVA: 0x00012940 File Offset: 0x00010B40
	protected virtual void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
		this.cooking = base.gameObject.GetOrAddComponent<ItemCooking>();
		this._hasUseFeedback = base.TryGetComponent<ItemUseFeedback>(out this._useFeedback);
		this.AddPhysics();
		this.GetItemActions();
		this.AddPropertyBlock();
		this.particles = base.GetComponent<ItemParticles>();
		if (!this.particles)
		{
			this.particles = base.gameObject.AddComponent<ItemParticles>();
		}
		this.itemComponents = base.GetComponents<ItemComponent>();
		this.physicsSyncer = base.GetComponent<ItemPhysicsSyncer>();
		this.RegisterItem();
	}

	// Token: 0x06000285 RID: 645 RVA: 0x000129D8 File Offset: 0x00010BD8
	private void RegisterItem()
	{
		foreach (Collider key in this.colliders)
		{
			Item.COLLIDER_TO_ITEM.Add(key, this);
		}
	}

	// Token: 0x06000286 RID: 646 RVA: 0x00012A0C File Offset: 0x00010C0C
	private void DeregisterItem()
	{
		foreach (Collider key in this.colliders)
		{
			Item.COLLIDER_TO_ITEM.Remove(key);
		}
	}

	// Token: 0x06000287 RID: 647 RVA: 0x00012A40 File Offset: 0x00010C40
	protected virtual void Start()
	{
		if (!this.HasData(DataEntryKey.ItemUses))
		{
			OptionableIntItemData optionableIntItemData = this.GetData<OptionableIntItemData>(DataEntryKey.ItemUses);
			optionableIntItemData.HasData = (this.totalUses != -1);
			optionableIntItemData.Value = this.totalUses;
			if (this.totalUses > 0)
			{
				this.SetUseRemainingPercentage(1f);
			}
		}
		if (!this.rig.isKinematic)
		{
			this.WasActive();
		}
		this.packLayer = 1 << LayerMask.NameToLayer("Exclude Collisions");
	}

	// Token: 0x06000288 RID: 648 RVA: 0x00012AB6 File Offset: 0x00010CB6
	public static bool TryGetItemFromCollider(Collider collider, out Item result)
	{
		return Item.COLLIDER_TO_ITEM.TryGetValue(collider, out result);
	}

	// Token: 0x06000289 RID: 649 RVA: 0x00012AC4 File Offset: 0x00010CC4
	public string GetItemName(ItemInstanceData data = null)
	{
		int num = 0;
		IntItemData intItemData;
		if (data == null)
		{
			num = this.GetData<IntItemData>(DataEntryKey.CookedAmount).Value;
		}
		else if (data.TryGetDataEntry<IntItemData>(DataEntryKey.CookedAmount, out intItemData))
		{
			num = intItemData.Value;
		}
		string result;
		if (num < 4)
		{
			switch (num)
			{
			case 1:
				result = LocalizedText.GetText("COOKED_COOKED", true).Replace("#", this.GetName());
				break;
			case 2:
				result = LocalizedText.GetText("COOKED_WELLDONE", true).Replace("#", this.GetName());
				break;
			case 3:
				result = LocalizedText.GetText("COOKED_BURNT", true).Replace("#", this.GetName());
				break;
			default:
				result = this.GetName();
				break;
			}
		}
		else
		{
			result = LocalizedText.GetText("COOKED_INCINERATED", true).Replace("#", this.GetName());
		}
		return result;
	}

	// Token: 0x0600028A RID: 650 RVA: 0x00012B94 File Offset: 0x00010D94
	private void AddPropertyBlock()
	{
		this.mpb = new MaterialPropertyBlock();
		this.mainRenderer = base.GetComponentInChildren<MeshRenderer>();
		if (!this.mainRenderer)
		{
			this.mainRenderer = base.GetComponentInChildren<SkinnedMeshRenderer>();
		}
		this.mainRenderer.GetPropertyBlock(this.mpb);
	}

	// Token: 0x0600028B RID: 651 RVA: 0x00012BE2 File Offset: 0x00010DE2
	private void GetItemActions()
	{
		this.itemActions = base.GetComponentsInChildren<ItemActionBase>();
	}

	// Token: 0x0600028C RID: 652 RVA: 0x00012BF0 File Offset: 0x00010DF0
	protected virtual void AddPhysics()
	{
		this.rig = base.gameObject.GetOrAddComponent<Rigidbody>();
		this.rig.mass = this.mass;
		this.centerOfMass = this.rig.centerOfMass;
		this.rig.interpolation = RigidbodyInterpolation.Interpolate;
		this.rig.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		this.colliders = base.GetComponentsInChildren<Collider>();
	}

	// Token: 0x0600028D RID: 653 RVA: 0x00012C54 File Offset: 0x00010E54
	protected virtual void Update()
	{
		if (this.itemState == ItemState.InBackpack)
		{
			if (this.backpackSlotTransform == null || !this.backpackSlotTransform.UnityObjectExists<Transform>())
			{
				base.transform.position = new Vector3(0f, -500f, 0f);
			}
			else
			{
				base.transform.position = this.backpackSlotTransform.position - this.backpackSlotTransform.rotation * this.centerOfMass * 0.5f;
				base.transform.rotation = this.backpackSlotTransform.rotation;
			}
		}
		else if (this.itemState == ItemState.Ground && base.photonView.IsMine)
		{
			if (base.transform.position.y < -2000f || base.transform.position.y > 4000f)
			{
				this.destroyTick += Time.deltaTime;
				if (this.destroyTick > 2f)
				{
					PhotonNetwork.Destroy(base.gameObject);
				}
			}
			else
			{
				this.destroyTick = 0f;
			}
		}
		this.UpdateCollisionDetectionMode();
	}

	// Token: 0x0600028E RID: 654 RVA: 0x00012D7F File Offset: 0x00010F7F
	private void UpdateCollisionDetectionMode()
	{
		if (this.itemState == ItemState.Ground)
		{
			this.rig.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			return;
		}
		this.rig.collisionDetectionMode = CollisionDetectionMode.Discrete;
	}

	// Token: 0x0600028F RID: 655 RVA: 0x00012DA4 File Offset: 0x00010FA4
	public virtual void Interact(Character interactor)
	{
		if (!interactor.player.HasEmptySlot(this.itemID))
		{
			return;
		}
		if (interactor.refs.items.lastEquippedSlotTime + 0.25f > Time.time)
		{
			return;
		}
		if (interactor.data.isClimbing && !this.UIData.canPocket)
		{
			this.SetKinematicNetworked(false);
			return;
		}
		GlobalEvents.TriggerItemRequested(this, interactor);
		base.gameObject.SetActive(false);
		this.view.RPC("RequestPickup", RpcTarget.MasterClient, new object[]
		{
			interactor.GetComponent<PhotonView>()
		});
		Debug.Log("Picking up " + base.gameObject.name);
		ItemBackpackVisuals itemBackpackVisuals;
		if (base.TryGetComponent<ItemBackpackVisuals>(out itemBackpackVisuals))
		{
			itemBackpackVisuals.RemoveVisuals();
		}
	}

	// Token: 0x06000290 RID: 656 RVA: 0x00012E62 File Offset: 0x00011062
	[PunRPC]
	public void DenyPickupRPC()
	{
		base.gameObject.SetActive(true);
		this.SetKinematicNetworked(false, base.transform.position, base.transform.rotation);
	}

	// Token: 0x06000291 RID: 657 RVA: 0x00012E90 File Offset: 0x00011090
	[PunRPC]
	public void RequestPickup(PhotonView characterView)
	{
		Character component = characterView.GetComponent<Character>();
		if (this.isSecretlyOtherItemPrefab && !this.isSecretlyOtherItemPrefab.UIData.canPocket && component.data.isClimbing)
		{
			PhotonNetwork.Instantiate("0_Items/" + this.isSecretlyOtherItemPrefab.gameObject.name, base.transform.position, base.transform.rotation, 0, null);
			PhotonNetwork.Destroy(base.gameObject);
			return;
		}
		ushort num = this.isSecretlyOtherItemPrefab ? this.isSecretlyOtherItemPrefab.itemID : this.itemID;
		ItemSlot itemSlot;
		bool flag = component.player.AddItem(num, this.data, out itemSlot);
		if (this.itemState == ItemState.InBackpack)
		{
			if (this.backpackReference.IsSome)
			{
				if (flag)
				{
					this.ClearDataFromBackpack();
					component.refs.view.RPC("OnPickupAccepted", component.player.photonView.Owner, new object[]
					{
						itemSlot.itemSlotID
					});
					return;
				}
				this.view.RPC("DenyPickupRPC", component.player.photonView.Owner, Array.Empty<object>());
			}
			return;
		}
		if (flag)
		{
			component.refs.view.RPC("OnPickupAccepted", component.player.photonView.Owner, new object[]
			{
				itemSlot.itemSlotID
			});
			PhotonNetwork.Destroy(this.view);
			return;
		}
		this.view.RPC("DenyPickupRPC", component.player.photonView.Owner, Array.Empty<object>());
	}

	// Token: 0x06000292 RID: 658 RVA: 0x00013038 File Offset: 0x00011238
	public void ClearDataFromBackpack()
	{
		if (this.backpackReference.IsNone)
		{
			return;
		}
		ValueTuple<byte, BackpackReference> value = this.backpackReference.Value;
		byte item = value.Item1;
		BackpackReference item2 = value.Item2;
		item2.GetData().itemSlots[(int)item].EmptyOut();
		if (item2.type == BackpackReference.BackpackType.Item)
		{
			item2.view.RPC("SetItemInstanceDataRPC", RpcTarget.Others, new object[]
			{
				item2.GetItemInstanceData()
			});
		}
		else
		{
			Character component = item2.view.GetComponent<Character>();
			ItemSlot[] itemSlots = component.player.itemSlots;
			BackpackSlot backpackSlot = component.player.backpackSlot;
			byte[] array = IBinarySerializable.ToManagedArray<InventorySyncData>(new InventorySyncData(itemSlots, backpackSlot, component.player.tempFullSlot));
			component.player.photonView.RPC("SyncInventoryRPC", RpcTarget.Others, new object[]
			{
				array,
				false
			});
		}
		item2.GetVisuals().RefreshVisuals();
	}

	// Token: 0x06000293 RID: 659 RVA: 0x00013120 File Offset: 0x00011320
	public Vector3 Center()
	{
		if (!this.mainRenderer.UnityObjectExists<Renderer>())
		{
			return base.transform.position;
		}
		return this.mainRenderer.bounds.center;
	}

	// Token: 0x06000294 RID: 660 RVA: 0x00013159 File Offset: 0x00011359
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x06000295 RID: 661 RVA: 0x00013161 File Offset: 0x00011361
	public virtual string GetInteractionText()
	{
		return LocalizedText.GetText("PICKUP", true);
	}

	// Token: 0x06000296 RID: 662 RVA: 0x0001316E File Offset: 0x0001136E
	public string GetName()
	{
		return LocalizedText.GetText(LocalizedText.GetNameIndex(this.UIData.itemName), true);
	}

	// Token: 0x06000297 RID: 663 RVA: 0x00013186 File Offset: 0x00011386
	public virtual bool IsInteractible(Character interactor)
	{
		return !this.blockInteraction && this.itemState != ItemState.Held && this.itemState != ItemState.InBackpack;
	}

	// Token: 0x06000298 RID: 664 RVA: 0x000131AC File Offset: 0x000113AC
	internal void Move(Vector3 position, Quaternion rotation)
	{
		base.transform.position = position;
		base.transform.rotation = rotation;
		this.rig.position = position;
		this.rig.rotation = rotation;
		this.rig.linearVelocity *= 0f;
		this.rig.angularVelocity *= 0f;
	}

	// Token: 0x17000039 RID: 57
	// (get) Token: 0x06000299 RID: 665 RVA: 0x0001321F File Offset: 0x0001141F
	// (set) Token: 0x0600029A RID: 666 RVA: 0x0001323B File Offset: 0x0001143B
	public Character holderCharacter
	{
		get
		{
			if (this.overrideHolderCharacter)
			{
				return this.overrideHolderCharacter;
			}
			return this._holderCharacter;
		}
		set
		{
			if (value != null)
			{
				this.lastHolderCharacter = value;
			}
			this._holderCharacter = value;
		}
	}

	// Token: 0x1700003A RID: 58
	// (get) Token: 0x0600029B RID: 667 RVA: 0x00013254 File Offset: 0x00011454
	public Character trueHolderCharacter
	{
		get
		{
			return this._holderCharacter;
		}
	}

	// Token: 0x0600029C RID: 668 RVA: 0x0001325C File Offset: 0x0001145C
	private void SetColliders(bool enabled, bool isTrigger, bool excludeLayer = false)
	{
		for (int i = 0; i < this.colliders.Length; i++)
		{
			this.colliders[i].enabled = enabled;
			this.colliders[i].isTrigger = isTrigger;
		}
		if (excludeLayer)
		{
			this.rig.excludeLayers = 1 << LayerMask.NameToLayer("Default");
			return;
		}
		this.rig.excludeLayers = 0;
	}

	// Token: 0x0600029D RID: 669 RVA: 0x000132CC File Offset: 0x000114CC
	internal void SetState(ItemState setState, Character character = null)
	{
		Debug.Log(string.Format("Setting Item State for {0}: {1}", base.name, setState));
		this.itemState = setState;
		Action<ItemState> onStateChange = this.OnStateChange;
		if (onStateChange != null)
		{
			onStateChange(setState);
		}
		if (setState == ItemState.InBackpack)
		{
			this.holderCharacter = null;
			this.rig.useGravity = false;
			this.SetKinematic(true);
			this.rig.interpolation = RigidbodyInterpolation.None;
			this.SetColliders(false, true, false);
			if (this.forceScale)
			{
				base.transform.localScale = Vector3.one * 0.5f;
				return;
			}
		}
		else if (setState == ItemState.Ground)
		{
			this.holderCharacter = null;
			this.rig.useGravity = true;
			this.SetKinematic(false);
			this.rig.interpolation = RigidbodyInterpolation.Interpolate;
			this.centerOfMass = this.rig.centerOfMass;
			if (this is Backpack)
			{
				this.wearerCharacter = null;
			}
			this.SetColliders(true, false, false);
			if (this.forceScale)
			{
				base.transform.localScale = Vector3.one;
				return;
			}
		}
		else if (setState == ItemState.Held)
		{
			this.holderCharacter = character;
			this.rig.useGravity = false;
			this.SetKinematic(false);
			this.rig.interpolation = RigidbodyInterpolation.Interpolate;
			if (this is Backpack)
			{
				this.wearerCharacter = null;
			}
			if (PhotonNetwork.IsMasterClient)
			{
				if (character == null)
				{
					Debug.LogError("{name} set to Held but no HolderCharacter assigned! Something broken??");
				}
				else
				{
					base.photonView.TransferOwnership(character.GetComponent<PhotonView>().Owner);
				}
			}
			this.SetColliders(true, false, true);
			if (this.forceScale)
			{
				base.transform.localScale = Vector3.one;
			}
		}
	}

	// Token: 0x0600029E RID: 670 RVA: 0x00013463 File Offset: 0x00011663
	private void HideRenderers()
	{
		base.GetComponentsInChildren<Renderer>().ForEach(delegate(Renderer meshRenderer)
		{
			meshRenderer.enabled = false;
		});
	}

	// Token: 0x1700003B RID: 59
	// (get) Token: 0x0600029F RID: 671 RVA: 0x00013490 File Offset: 0x00011690
	// (set) Token: 0x060002A0 RID: 672 RVA: 0x00013498 File Offset: 0x00011698
	public bool isUsingSecondary { get; private set; }

	// Token: 0x1700003C RID: 60
	// (get) Token: 0x060002A1 RID: 673 RVA: 0x000134A1 File Offset: 0x000116A1
	// (set) Token: 0x060002A2 RID: 674 RVA: 0x000134A9 File Offset: 0x000116A9
	public float castProgress { get; private set; }

	// Token: 0x1700003D RID: 61
	// (get) Token: 0x060002A3 RID: 675 RVA: 0x000134B2 File Offset: 0x000116B2
	public float progress
	{
		get
		{
			return Mathf.Max(this.overrideProgress, this.castProgress);
		}
	}

	// Token: 0x1700003E RID: 62
	// (get) Token: 0x060002A4 RID: 676 RVA: 0x000134C5 File Offset: 0x000116C5
	public bool shouldShowCastProgress
	{
		get
		{
			return (this.showUseProgress && this.castProgress > 0f && !this.finishedCast) || this.overrideForceProgress;
		}
	}

	// Token: 0x060002A5 RID: 677 RVA: 0x000134EC File Offset: 0x000116EC
	public virtual bool CanUsePrimary()
	{
		if (!this.overrideUsability.IsNone)
		{
			return this.overrideUsability.Value;
		}
		OptionableIntItemData optionableIntItemData = this.GetData<OptionableIntItemData>(DataEntryKey.ItemUses);
		return !optionableIntItemData.HasData || optionableIntItemData.Value == -1 || optionableIntItemData.Value > 0;
	}

	// Token: 0x060002A6 RID: 678 RVA: 0x00013538 File Offset: 0x00011738
	public virtual bool CanUseSecondary()
	{
		bool flag = true;
		OptionableIntItemData optionableIntItemData = this.GetData<OptionableIntItemData>(DataEntryKey.ItemUses);
		if (optionableIntItemData.HasData)
		{
			flag = (optionableIntItemData.Value == -1 || optionableIntItemData.Value > 0);
		}
		if (!flag)
		{
			return false;
		}
		if (this.canUseOnFriend)
		{
			if (Interaction.instance.hasValidTargetCharacter)
			{
				return true;
			}
		}
		else if (this.UIData.hasSecondInteract)
		{
			return true;
		}
		return false;
	}

	// Token: 0x060002A7 RID: 679 RVA: 0x00013598 File Offset: 0x00011798
	public void StartUsePrimary()
	{
		if (this.isUsingSecondary)
		{
			this.CancelUseSecondary();
		}
		this.isUsingPrimary = true;
		this.castProgress = 0f;
		this.finishedCast = false;
		if (this.OnPrimaryStarted != null)
		{
			this.OnPrimaryStarted();
		}
	}

	// Token: 0x060002A8 RID: 680 RVA: 0x000135D4 File Offset: 0x000117D4
	public void ContinueUsePrimary()
	{
		if (this.isUsingSecondary)
		{
			this.CancelUseSecondary();
		}
		if (this.isUsingPrimary)
		{
			if (this.usingTimePrimary > 0f)
			{
				this.castProgress += 1f / this.usingTimePrimary * Time.deltaTime;
				if (this.castProgress >= 1f)
				{
					if (this.OnPrimaryHeld != null)
					{
						this.OnPrimaryHeld();
					}
					if (!this.finishedCast)
					{
						this.FinishCastPrimary();
						return;
					}
				}
			}
			else
			{
				if (!this.finishedCast)
				{
					this.FinishCastPrimary();
				}
				if (this.OnPrimaryHeld != null)
				{
					this.OnPrimaryHeld();
				}
			}
		}
	}

	// Token: 0x060002A9 RID: 681 RVA: 0x00013674 File Offset: 0x00011874
	protected virtual void FinishCastPrimary()
	{
		if (this._hasUseFeedback)
		{
			this.holderCharacter.refs.animator.SetBool(this._useFeedback.useAnimation, false);
			SFX_Instance sfxUsed = this._useFeedback.sfxUsed;
			if (sfxUsed != null)
			{
				sfxUsed.Play(base.transform.position, this._holderCharacter.refs.carryPosRef);
			}
		}
		this.finishedCast = true;
		this.lastFinishedCast = Time.time;
		this.castProgress = 0f;
		if (this.OnPrimaryFinishedCast != null)
		{
			this.OnPrimaryFinishedCast();
		}
	}

	// Token: 0x060002AA RID: 682 RVA: 0x0001370C File Offset: 0x0001190C
	public void CancelUsePrimary()
	{
		this.isUsingPrimary = false;
		this.castProgress = 0f;
		this.finishedCast = false;
		if (this.OnPrimaryCancelled != null)
		{
			this.OnPrimaryCancelled();
		}
		if (global::Player.localPlayer == null)
		{
			Debug.LogError("Player.localPlayer is null, cannot play movement animation");
			return;
		}
		if (global::Player.localPlayer.character == null)
		{
			Debug.LogError("Player.localPlayer.character is null, cannot play movement animation");
			return;
		}
		if (global::Player.localPlayer.character.refs == null)
		{
			Debug.LogError("Player.localPlayer.character.refs is null, cannot play movement animation");
			return;
		}
		if (global::Player.localPlayer.character.refs.animations == null)
		{
			Debug.LogError("Player.localPlayer.character.refs.animations is null, cannot play movement animation");
			return;
		}
		global::Player.localPlayer.character.refs.animations.PlaySpecificAnimation("Movement");
	}

	// Token: 0x060002AB RID: 683 RVA: 0x000137DB File Offset: 0x000119DB
	public void ScrollButtonBackwardPressed()
	{
		if (this.OnScrollBackwardPressed != null)
		{
			this.OnScrollBackwardPressed();
		}
	}

	// Token: 0x060002AC RID: 684 RVA: 0x000137F0 File Offset: 0x000119F0
	public void ScrollButtonForwardPressed()
	{
		if (this.OnScrollForwardPressed != null)
		{
			this.OnScrollForwardPressed();
		}
	}

	// Token: 0x060002AD RID: 685 RVA: 0x00013805 File Offset: 0x00011A05
	public void ScrollButtonBackwardHeld()
	{
		if (this.OnScrollBackwardHeld != null)
		{
			this.OnScrollBackwardHeld();
		}
	}

	// Token: 0x060002AE RID: 686 RVA: 0x0001381A File Offset: 0x00011A1A
	public void ScrollButtonForwardHeld()
	{
		if (this.OnScrollForwardHeld != null)
		{
			this.OnScrollForwardHeld();
		}
	}

	// Token: 0x060002AF RID: 687 RVA: 0x0001382F File Offset: 0x00011A2F
	public void Scroll(float value)
	{
		if (this.OnScrolled != null)
		{
			this.OnScrolled(value);
		}
		if (InputHandler.GetCurrentUsedInputScheme() == InputScheme.KeyboardMouse && this.OnScrolledMouseOnly != null)
		{
			this.OnScrolledMouseOnly(value);
		}
	}

	// Token: 0x060002B0 RID: 688 RVA: 0x00013860 File Offset: 0x00011A60
	public void StartUseSecondary()
	{
		if (this.isUsingPrimary)
		{
			return;
		}
		if (this.isUsingSecondary)
		{
			return;
		}
		this.isUsingSecondary = true;
		this.castProgress = 0f;
		this.finishedCast = false;
		if (this.holderCharacter && this.canUseOnFriend && Interaction.instance.hasValidTargetCharacter)
		{
			base.photonView.RPC("SendFeedDataRPC", RpcTarget.All, new object[]
			{
				this.holderCharacter.photonView.ViewID,
				Interaction.instance.bestCharacter.character.photonView.ViewID,
				(int)this.itemID,
				this.totalSecondaryUsingTime
			});
		}
		if (this.OnSecondaryStarted != null)
		{
			this.OnSecondaryStarted();
		}
	}

	// Token: 0x060002B1 RID: 689 RVA: 0x0001393B File Offset: 0x00011B3B
	[PunRPC]
	internal void SendFeedDataRPC(int giverID, int recieverID, int itemID, float totalUsingTime)
	{
		GameUtils.instance.StartFeed(giverID, recieverID, (ushort)itemID, totalUsingTime);
	}

	// Token: 0x060002B2 RID: 690 RVA: 0x0001394D File Offset: 0x00011B4D
	[PunRPC]
	internal void RemoveFeedDataRPC(int giverID)
	{
		GameUtils.instance.EndFeed(giverID);
	}

	// Token: 0x1700003F RID: 63
	// (get) Token: 0x060002B3 RID: 691 RVA: 0x0001395A File Offset: 0x00011B5A
	public float totalSecondaryUsingTime
	{
		get
		{
			if (!this.canUseOnFriend)
			{
				return this.usingTimePrimary;
			}
			return this.usingTimePrimary * 0.7f;
		}
	}

	// Token: 0x060002B4 RID: 692 RVA: 0x00013978 File Offset: 0x00011B78
	public void ContinueUseSecondary()
	{
		if (this.isUsingPrimary)
		{
			return;
		}
		if (this.isUsingSecondary)
		{
			if (this.usingTimePrimary > 0f)
			{
				this.castProgress += 1f / this.totalSecondaryUsingTime * Time.deltaTime;
				if (this.castProgress >= 1f)
				{
					if (this.OnSecondaryHeld != null)
					{
						this.OnSecondaryHeld();
					}
					if (!this.finishedCast)
					{
						this.FinishCastSecondary();
						return;
					}
				}
			}
			else if (this.OnSecondaryHeld != null)
			{
				this.OnSecondaryHeld();
			}
		}
	}

	// Token: 0x060002B5 RID: 693 RVA: 0x00013A04 File Offset: 0x00011C04
	public void FinishCastSecondary()
	{
		this.finishedCast = true;
		this.lastFinishedCast = Time.time;
		this.castProgress = 0f;
		if (this.canUseOnFriend && Interaction.instance.hasValidTargetCharacter)
		{
			if (this.holderCharacter)
			{
				this.holderCharacter.data.lastConsumedItem = Time.time;
				base.photonView.RPC("RemoveFeedDataRPC", RpcTarget.All, new object[]
				{
					this.holderCharacter.photonView.ViewID
				});
			}
			Interaction.instance.bestCharacter.character.FeedItem(this);
			base.photonView.RPC("RemoveFeedDataRPC", RpcTarget.All, new object[]
			{
				(int)this.itemID
			});
			return;
		}
		if (this.OnSecondaryFinishedCast != null)
		{
			this.OnSecondaryFinishedCast();
		}
	}

	// Token: 0x060002B6 RID: 694 RVA: 0x00013AE8 File Offset: 0x00011CE8
	public void CancelUseSecondary()
	{
		this.isUsingSecondary = false;
		this.castProgress = 0f;
		this.finishedCast = false;
		if (this.OnSecondaryCancelled != null)
		{
			this.OnSecondaryCancelled();
		}
		global::Player.localPlayer.character.refs.animations.PlaySpecificAnimation("Movement");
		if (this.lastHolderCharacter)
		{
			base.photonView.RPC("RemoveFeedDataRPC", RpcTarget.All, new object[]
			{
				this.lastHolderCharacter.photonView.ViewID
			});
		}
	}

	// Token: 0x17000040 RID: 64
	// (get) Token: 0x060002B7 RID: 695 RVA: 0x00013B7B File Offset: 0x00011D7B
	// (set) Token: 0x060002B8 RID: 696 RVA: 0x00013B83 File Offset: 0x00011D83
	public bool consuming { get; private set; }

	// Token: 0x060002B9 RID: 697 RVA: 0x00013B8C File Offset: 0x00011D8C
	public IEnumerator ConsumeDelayed(bool ignoreActions = false)
	{
		int consumerID = -1;
		if (this.holderCharacter)
		{
			consumerID = this.holderCharacter.photonView.ViewID;
		}
		this.consuming = true;
		if (!ignoreActions && this.OnConsumed != null)
		{
			this.OnConsumed();
		}
		yield return null;
		base.photonView.RPC("Consume", RpcTarget.All, new object[]
		{
			consumerID
		});
		yield break;
	}

	// Token: 0x060002BA RID: 698 RVA: 0x00013BA4 File Offset: 0x00011DA4
	[PunRPC]
	public void Consume(int consumerID)
	{
		if (this.holderCharacter)
		{
			PhotonView photonView = PhotonNetwork.GetPhotonView(consumerID);
			if (photonView)
			{
				Character component = photonView.GetComponent<Character>();
				if (component)
				{
					GlobalEvents.TriggerItemConsumed(this, component);
				}
			}
			this.holderCharacter.data.lastConsumedItem = Time.time;
			if (this.holderCharacter.data.currentItem == this)
			{
				Optionable<byte> currentSelectedSlot = this.holderCharacter.refs.items.currentSelectedSlot;
				this.holderCharacter.refs.animator.SetBool("Consumed Item", true);
				if (this.holderCharacter.IsLocal)
				{
					if (currentSelectedSlot.IsSome)
					{
						this.holderCharacter.player.EmptySlot(currentSelectedSlot);
						this.holderCharacter.refs.items.EquipSlot(currentSelectedSlot);
					}
					else
					{
						Debug.LogError("No Item Selected locally but still consuming?? THIS IS BAD. CALL ZORRO");
					}
				}
			}
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x060002BB RID: 699 RVA: 0x00013C99 File Offset: 0x00011E99
	public virtual void OnStash()
	{
		Action action = this.onStashAction;
		if (action != null)
		{
			action();
		}
		this.CancelUsePrimary();
		this.CancelUseSecondary();
	}

	// Token: 0x060002BC RID: 700 RVA: 0x00013CB8 File Offset: 0x00011EB8
	[ContextMenu("Add Default Food Scripts")]
	public void AddDefaultFoodScripts()
	{
		this.usingTimePrimary = 1.2f;
		Action_PlayAnimation action_PlayAnimation = base.gameObject.AddComponent<Action_PlayAnimation>();
		action_PlayAnimation.OnPressed = true;
		action_PlayAnimation.animationName = "PlayerEat";
		Action_ModifyStatus action_ModifyStatus = base.gameObject.AddComponent<Action_ModifyStatus>();
		action_ModifyStatus.OnCastFinished = true;
		action_ModifyStatus.statusType = CharacterAfflictions.STATUSTYPE.Hunger;
		action_ModifyStatus.changeAmount = -0.1f;
		base.gameObject.AddComponent<Action_Consume>().OnCastFinished = true;
	}

	// Token: 0x060002BD RID: 701 RVA: 0x00013D20 File Offset: 0x00011F20
	public void HoverEnter()
	{
		this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 1f);
		this.mainRenderer.SetPropertyBlock(this.mpb);
		for (int i = 0; i < this.addtlRenderers.Length; i++)
		{
			this.addtlRenderers[i].SetPropertyBlock(this.mpb);
		}
	}

	// Token: 0x060002BE RID: 702 RVA: 0x00013D7C File Offset: 0x00011F7C
	public void HoverExit()
	{
		this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 0f);
		this.mainRenderer.SetPropertyBlock(this.mpb);
		for (int i = 0; i < this.addtlRenderers.Length; i++)
		{
			this.addtlRenderers[i].SetPropertyBlock(this.mpb);
		}
	}

	// Token: 0x060002BF RID: 703 RVA: 0x00013DD5 File Offset: 0x00011FD5
	public void SetKinematic(bool value)
	{
		this.rig.isKinematic = value;
		GlobalEvents.TriggerItemSetKinematic(this, value);
	}

	// Token: 0x060002C0 RID: 704 RVA: 0x00013DEC File Offset: 0x00011FEC
	public void SetKinematicNetworked(bool value)
	{
		base.photonView.RPC("SetKinematicRPC", RpcTarget.AllBuffered, new object[]
		{
			value,
			base.transform.position,
			base.transform.rotation
		});
	}

	// Token: 0x060002C1 RID: 705 RVA: 0x00013E3F File Offset: 0x0001203F
	public void SetKinematicNetworked(bool value, Vector3 position, Quaternion rotation)
	{
		base.photonView.RPC("SetKinematicRPC", RpcTarget.AllBuffered, new object[]
		{
			value,
			position,
			rotation
		});
	}

	// Token: 0x060002C2 RID: 706 RVA: 0x00013E74 File Offset: 0x00012074
	[PunRPC]
	public void SetKinematicAndResetSyncData(bool value, Vector3 position, Quaternion rotation)
	{
		this.SetKinematic(value);
		this.rig.position = position;
		this.rig.rotation = rotation;
		if (value)
		{
			this.rig.linearVelocity = Vector3.zero;
			this.rig.angularVelocity = Vector3.zero;
			this.physicsSyncer.ResetRecievedData();
		}
	}

	// Token: 0x060002C3 RID: 707 RVA: 0x00013ECE File Offset: 0x000120CE
	[PunRPC]
	public void SetKinematicRPC(bool value, Vector3 position, Quaternion rotation)
	{
		this.SetKinematic(value);
		this.rig.position = position;
		this.rig.rotation = rotation;
	}

	// Token: 0x060002C4 RID: 708 RVA: 0x00013EEF File Offset: 0x000120EF
	public bool HasData(DataEntryKey key)
	{
		return this.data != null && this.data.HasData(key);
	}

	// Token: 0x060002C5 RID: 709 RVA: 0x00013F08 File Offset: 0x00012108
	public T GetData<T>(DataEntryKey key, Func<T> createDefault) where T : DataEntryValue, new()
	{
		if (this.data == null)
		{
			this.data = new ItemInstanceData(Guid.NewGuid());
			ItemInstanceDataHandler.AddInstanceData(this.data);
		}
		T result;
		if (this.data.TryGetDataEntry<T>(key, out result))
		{
			return result;
		}
		if (createDefault != null)
		{
			return this.data.RegisterEntry<T>(key, createDefault());
		}
		return this.data.RegisterNewEntry<T>(key);
	}

	// Token: 0x060002C6 RID: 710 RVA: 0x00013F6C File Offset: 0x0001216C
	public T GetData<T>(DataEntryKey key) where T : DataEntryValue, new()
	{
		return this.GetData<T>(key, null);
	}

	// Token: 0x060002C7 RID: 711 RVA: 0x00013F76 File Offset: 0x00012176
	internal void ForceSyncForFrames(int frames = 10)
	{
		if (this.physicsSyncer != null)
		{
			this.physicsSyncer.ForceSyncForFrames(frames);
		}
	}

	// Token: 0x060002C8 RID: 712 RVA: 0x00013F94 File Offset: 0x00012194
	[PunRPC]
	public void SetItemInstanceDataRPC(ItemInstanceData instanceData)
	{
		this.data = instanceData;
		if (this.data != null)
		{
			this.OnInstanceDataRecieved();
			ItemComponent[] array = this.itemComponents;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnInstanceDataSet();
			}
		}
	}

	// Token: 0x060002C9 RID: 713 RVA: 0x00013FD3 File Offset: 0x000121D3
	public virtual void OnInstanceDataRecieved()
	{
	}

	// Token: 0x060002CA RID: 714 RVA: 0x00013FD8 File Offset: 0x000121D8
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		this.ForceSyncForFrames(10);
		ItemState itemState = this.itemState;
		if ((itemState == ItemState.Ground || itemState == ItemState.Held || itemState == ItemState.InBackpack) && this.data != null)
		{
			this.view.RPC("SetItemInstanceDataRPC", newPlayer, new object[]
			{
				this.data
			});
		}
		if (this.itemState == ItemState.InBackpack)
		{
			ValueTuple<byte, BackpackReference> value = this.backpackReference.Value;
			byte item = value.Item1;
			BackpackReference item2 = value.Item2;
			this.view.RPC("PutInBackpackRPC", newPlayer, new object[]
			{
				item,
				item2
			});
		}
		if (this.rig.isKinematic)
		{
			this.view.RPC("SetKinematicRPC", newPlayer, new object[]
			{
				this.rig.isKinematic,
				this.rig.position,
				this.rig.rotation
			});
		}
	}

	// Token: 0x060002CB RID: 715 RVA: 0x000140E0 File Offset: 0x000122E0
	[PunRPC]
	public void PutInBackpackRPC(byte slotID, BackpackReference backpackReference)
	{
		Transform[] backpackSlots = backpackReference.GetVisuals().backpackSlots;
		this.backpackReference = Optionable<ValueTuple<byte, BackpackReference>>.Some(new ValueTuple<byte, BackpackReference>(slotID, backpackReference));
		this.backpackSlotTransform = backpackSlots[(int)slotID];
		this.SetState(ItemState.InBackpack, null);
		backpackReference.GetVisuals().SetSpawnedBackpackItem(slotID, this);
		if (backpackReference.IsOnMyBack())
		{
			this.HideRenderers();
		}
	}

	// Token: 0x060002CC RID: 716 RVA: 0x0001413A File Offset: 0x0001233A
	[PunRPC]
	public void SetCookedAmountRPC(int amount)
	{
		this.GetData<IntItemData>(DataEntryKey.CookedAmount).Value = amount;
		this.cooking.UpdateCookedBehavior();
	}

	// Token: 0x060002CD RID: 717 RVA: 0x00014154 File Offset: 0x00012354
	public void SetUseRemainingPercentage(float percentage)
	{
		this.GetData<FloatItemData>(DataEntryKey.UseRemainingPercentage).Value = Mathf.Clamp01(percentage);
	}

	// Token: 0x17000041 RID: 65
	// (get) Token: 0x060002CE RID: 718 RVA: 0x00014169 File Offset: 0x00012369
	// (set) Token: 0x060002CF RID: 719 RVA: 0x00014171 File Offset: 0x00012371
	public bool inActiveList { get; private set; }

	// Token: 0x060002D0 RID: 720 RVA: 0x0001417A File Offset: 0x0001237A
	public void WasActive()
	{
		if (!this.inActiveList)
		{
			Item.ALL_ACTIVE_ITEMS.Add(this);
		}
		this.inActiveList = true;
		this.timeSinceWasActive = 0f;
	}

	// Token: 0x060002D1 RID: 721 RVA: 0x000141A1 File Offset: 0x000123A1
	public void UpdateEntryInActiveList()
	{
		if (this.inActiveList)
		{
			this.timeSinceWasActive += Time.deltaTime;
			if (this.timeSinceWasActive > 30f)
			{
				this.RemoveFromActiveList();
			}
		}
	}

	// Token: 0x060002D2 RID: 722 RVA: 0x000141D0 File Offset: 0x000123D0
	private void RemoveFromActiveList()
	{
		if (this.inActiveList)
		{
			Item.ALL_ACTIVE_ITEMS.Remove(this);
			this.inActiveList = false;
		}
	}

	// Token: 0x060002D3 RID: 723 RVA: 0x000141ED File Offset: 0x000123ED
	private void OnDestroy()
	{
		this.RemoveFromActiveList();
		this.DeregisterItem();
	}

	// Token: 0x060002D4 RID: 724 RVA: 0x000141FB File Offset: 0x000123FB
	public bool TryGetFeeder(out Character feeder)
	{
		if (this.trueHolderCharacter != null && this.trueHolderCharacter != this.holderCharacter)
		{
			feeder = this.trueHolderCharacter;
			return true;
		}
		feeder = null;
		return false;
	}

	// Token: 0x060002D5 RID: 725 RVA: 0x0001422C File Offset: 0x0001242C
	public bool IsValidToSpawn()
	{
		LootData component = base.GetComponent<LootData>();
		return RunSettings.IsItemEnabled((component != null && component.useOtherItemForSpawningValidity != null) ? component.useOtherItemForSpawningValidity.gameObject.name : base.gameObject.name) && (!component || component.IsValidToSpawn());
	}

	// Token: 0x060002D6 RID: 726 RVA: 0x00014290 File Offset: 0x00012490
	public void AddNameToCSV()
	{
		LocalizedText.AppendCSVLine(string.Concat(new string[]
		{
			"NAME_",
			this.UIData.itemName.ToUpperInvariant(),
			",",
			this.UIData.itemName.ToUpperInvariant(),
			",,,,,,,,,,,,,ENDLINE"
		}), "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
	}

	// Token: 0x060002D7 RID: 727 RVA: 0x000142F8 File Offset: 0x000124F8
	public List<string> AddPromptToCSV(List<string> totalStrings)
	{
		List<string> list = new List<string>();
		if (!this.UIData.mainInteractPrompt.IsNullOrEmpty() && !totalStrings.Contains(this.UIData.mainInteractPrompt.ToUpperInvariant()))
		{
			LocalizedText.AppendCSVLine(this.UIData.mainInteractPrompt.ToUpperInvariant() + "," + this.UIData.mainInteractPrompt.ToLowerInvariant() + ",,,,,,,,,,,,,ENDLINE", "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
			list.Add(this.UIData.mainInteractPrompt.ToUpperInvariant());
		}
		if (!this.UIData.scrollInteractPrompt.IsNullOrEmpty() && !totalStrings.Contains(this.UIData.scrollInteractPrompt.ToUpperInvariant()))
		{
			LocalizedText.AppendCSVLine(this.UIData.scrollInteractPrompt.ToUpperInvariant() + "," + this.UIData.scrollInteractPrompt.ToLowerInvariant() + ",,,,,,,,,,,,,ENDLINE", "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
			list.Add(this.UIData.scrollInteractPrompt.ToUpperInvariant());
		}
		if (!this.UIData.secondaryInteractPrompt.IsNullOrEmpty() && !totalStrings.Contains(this.UIData.secondaryInteractPrompt.ToUpperInvariant()))
		{
			LocalizedText.AppendCSVLine(this.UIData.secondaryInteractPrompt.ToUpperInvariant() + "," + this.UIData.secondaryInteractPrompt.ToLowerInvariant() + ",,,,,,,,,,,,,ENDLINE", "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
			list.Add(this.UIData.secondaryInteractPrompt.ToUpperInvariant());
		}
		return list;
	}

	// Token: 0x060002D8 RID: 728 RVA: 0x00014488 File Offset: 0x00012688
	[PunRPC]
	public void RPC_SetThrownData(int characterID, float thrownAmount)
	{
		PhotonView photonView = PhotonNetwork.GetPhotonView(characterID);
		if (photonView)
		{
			photonView.TryGetComponent<Character>(out this.lastThrownCharacter);
		}
		this.lastThrownAmount = thrownAmount;
		this.lastThrownTime = Time.time;
		GlobalEvents.TriggerItemThrown(this);
	}

	// Token: 0x04000254 RID: 596
	private bool _hasUseFeedback;

	// Token: 0x04000255 RID: 597
	private ItemUseFeedback _useFeedback;

	// Token: 0x04000256 RID: 598
	public static readonly int PROPERTY_INTERACTABLE = Shader.PropertyToID("_Interactable");

	// Token: 0x04000257 RID: 599
	public static List<Item> ALL_ACTIVE_ITEMS = new List<Item>();

	// Token: 0x04000258 RID: 600
	private static Dictionary<Collider, Item> COLLIDER_TO_ITEM = new Dictionary<Collider, Item>();

	// Token: 0x04000259 RID: 601
	public Vector3 defaultPos;

	// Token: 0x0400025A RID: 602
	public Vector3 defaultForward = new Vector3(0f, 0f, 1f);

	// Token: 0x0400025B RID: 603
	public float mass = 5f;

	// Token: 0x0400025C RID: 604
	public float throwForceMultiplier = 1f;

	// Token: 0x0400025E RID: 606
	[SerializeField]
	private int carryWeight = 1;

	// Token: 0x0400025F RID: 607
	internal float lastThrownAmount;

	// Token: 0x04000260 RID: 608
	internal Character lastThrownCharacter;

	// Token: 0x04000261 RID: 609
	internal float lastThrownTime;

	// Token: 0x04000263 RID: 611
	public float usingTimePrimary;

	// Token: 0x04000264 RID: 612
	public bool showUseProgress = true;

	// Token: 0x04000265 RID: 613
	public Item isSecretlyOtherItemPrefab;

	// Token: 0x04000266 RID: 614
	public Action OnPrimaryStarted;

	// Token: 0x04000267 RID: 615
	public Action OnPrimaryHeld;

	// Token: 0x04000268 RID: 616
	public Action OnPrimaryFinishedCast;

	// Token: 0x04000269 RID: 617
	public Action OnPrimaryReleased;

	// Token: 0x0400026A RID: 618
	public Action OnPrimaryCancelled;

	// Token: 0x0400026B RID: 619
	public Action OnConsumed;

	// Token: 0x0400026C RID: 620
	public Action OnSecondaryStarted;

	// Token: 0x0400026D RID: 621
	public Action OnSecondaryHeld;

	// Token: 0x0400026E RID: 622
	public Action OnSecondaryFinishedCast;

	// Token: 0x0400026F RID: 623
	public Action OnSecondaryCancelled;

	// Token: 0x04000270 RID: 624
	public Action<ItemState> OnStateChange;

	// Token: 0x04000271 RID: 625
	public Action<float> OnScrolled;

	// Token: 0x04000272 RID: 626
	public Action<float> OnScrolledMouseOnly;

	// Token: 0x04000273 RID: 627
	public Action OnScrollBackwardPressed;

	// Token: 0x04000274 RID: 628
	public Action OnScrollForwardPressed;

	// Token: 0x04000275 RID: 629
	public Action OnScrollBackwardHeld;

	// Token: 0x04000276 RID: 630
	public Action OnScrollForwardHeld;

	// Token: 0x04000277 RID: 631
	public Item.ItemUIData UIData;

	// Token: 0x04000278 RID: 632
	[NonSerialized]
	public Transform backpackSlotTransform;

	// Token: 0x04000279 RID: 633
	public Optionable<ValueTuple<byte, BackpackReference>> backpackReference;

	// Token: 0x0400027A RID: 634
	private Optionable<RigidbodySyncData> m_lastState = Optionable<RigidbodySyncData>.None;

	// Token: 0x0400027B RID: 635
	protected PhotonView view;

	// Token: 0x0400027C RID: 636
	public int totalUses = -1;

	// Token: 0x0400027D RID: 637
	public ItemInstanceData data;

	// Token: 0x0400027E RID: 638
	public Item.ItemTags itemTags;

	// Token: 0x0400027F RID: 639
	public Rigidbody rig;

	// Token: 0x04000280 RID: 640
	internal ItemActionBase[] itemActions;

	// Token: 0x04000281 RID: 641
	[HideInInspector]
	public Collider[] colliders;

	// Token: 0x04000282 RID: 642
	public ushort itemID;

	// Token: 0x04000283 RID: 643
	private MaterialPropertyBlock mpb;

	// Token: 0x04000284 RID: 644
	public Renderer mainRenderer;

	// Token: 0x04000285 RID: 645
	public Renderer[] addtlRenderers;

	// Token: 0x04000286 RID: 646
	private double timeSinceTick;

	// Token: 0x04000288 RID: 648
	private ItemComponent[] itemComponents;

	// Token: 0x04000289 RID: 649
	protected Color originalTint;

	// Token: 0x0400028A RID: 650
	internal ItemPhysicsSyncer physicsSyncer;

	// Token: 0x0400028B RID: 651
	[HideInInspector]
	public ItemParticles particles;

	// Token: 0x0400028C RID: 652
	private int packLayer;

	// Token: 0x0400028D RID: 653
	public bool offsetLuggageSpawn;

	// Token: 0x0400028E RID: 654
	public Vector3 offsetLuggagePosition;

	// Token: 0x0400028F RID: 655
	public Vector3 offsetLuggageRotation;

	// Token: 0x04000290 RID: 656
	private float destroyTick;

	// Token: 0x04000291 RID: 657
	[HideInInspector]
	public bool forceScale = true;

	// Token: 0x04000292 RID: 658
	[HideInInspector]
	public bool blockInteraction;

	// Token: 0x04000293 RID: 659
	public Vector3 centerOfMass;

	// Token: 0x04000294 RID: 660
	private Character lastHolderCharacter;

	// Token: 0x04000295 RID: 661
	[ReadOnly]
	public Character wearerCharacter;

	// Token: 0x04000296 RID: 662
	[SerializeField]
	[ReadOnly]
	private Character _holderCharacter;

	// Token: 0x04000297 RID: 663
	[ReadOnly]
	public Character overrideHolderCharacter;

	// Token: 0x04000299 RID: 665
	public bool canUseOnFriend;

	// Token: 0x0400029B RID: 667
	[HideInInspector]
	public bool finishedCast;

	// Token: 0x0400029C RID: 668
	[HideInInspector]
	public float lastFinishedCast;

	// Token: 0x0400029D RID: 669
	internal float overrideProgress;

	// Token: 0x0400029E RID: 670
	internal Optionable<bool> overrideUsability;

	// Token: 0x040002A0 RID: 672
	public Action onStashAction;

	// Token: 0x040002A1 RID: 673
	internal bool overrideForceProgress;

	// Token: 0x040002A3 RID: 675
	[HideInInspector]
	public float timeSinceWasActive;

	// Token: 0x0200041E RID: 1054
	[Flags]
	public enum ItemTags
	{
		// Token: 0x04001816 RID: 6166
		None = 0,
		// Token: 0x04001817 RID: 6167
		Mystical = 1,
		// Token: 0x04001818 RID: 6168
		PackagedFood = 2,
		// Token: 0x04001819 RID: 6169
		Berry = 4,
		// Token: 0x0400181A RID: 6170
		Mushroom = 8,
		// Token: 0x0400181B RID: 6171
		BingBong = 16,
		// Token: 0x0400181C RID: 6172
		GourmandRequirement = 32,
		// Token: 0x0400181D RID: 6173
		GoldenIdol = 64,
		// Token: 0x0400181E RID: 6174
		Bird = 128,
		// Token: 0x0400181F RID: 6175
		BookOfBones = 256
	}

	// Token: 0x0200041F RID: 1055
	[Serializable]
	public class ItemUIData
	{
		// Token: 0x06001B8C RID: 7052 RVA: 0x0008554C File Offset: 0x0008374C
		public Texture2D GetIcon()
		{
			if (this.hasAltIcon && GameHandler.Instance.SettingsHandler.GetSetting<BugPhobiaSetting>().Value == OffOnMode.ON)
			{
				return this.altIcon;
			}
			if (this.hasColorBlindIcon && GUIManager.instance.colorblindness)
			{
				return this.altIcon;
			}
			return this.icon;
		}

		// Token: 0x04001820 RID: 6176
		public string itemName;

		// Token: 0x04001821 RID: 6177
		public Texture2D icon;

		// Token: 0x04001822 RID: 6178
		public bool hasAltIcon;

		// Token: 0x04001823 RID: 6179
		public bool hasColorBlindIcon;

		// Token: 0x04001824 RID: 6180
		public Texture2D altIcon;

		// Token: 0x04001825 RID: 6181
		public bool hasMainInteract = true;

		// Token: 0x04001826 RID: 6182
		public string mainInteractPrompt;

		// Token: 0x04001827 RID: 6183
		public bool hasSecondInteract;

		// Token: 0x04001828 RID: 6184
		public string secondaryInteractPrompt;

		// Token: 0x04001829 RID: 6185
		public bool hasScrollingInteract;

		// Token: 0x0400182A RID: 6186
		public string scrollInteractPrompt;

		// Token: 0x0400182B RID: 6187
		public bool canDrop = true;

		// Token: 0x0400182C RID: 6188
		public bool canPocket = true;

		// Token: 0x0400182D RID: 6189
		public bool canBackpack = true;

		// Token: 0x0400182E RID: 6190
		public bool canThrow = true;

		// Token: 0x0400182F RID: 6191
		public bool isShootable;

		// Token: 0x04001830 RID: 6192
		public bool hideFuel;

		// Token: 0x04001831 RID: 6193
		public Vector3 iconPositionOffset;

		// Token: 0x04001832 RID: 6194
		public Vector3 iconRotationOffset;

		// Token: 0x04001833 RID: 6195
		public float iconScaleOffset = 1f;
	}
}

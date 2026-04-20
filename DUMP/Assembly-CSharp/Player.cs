using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Peak.Network;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.CLI;
using Zorro.Core.Serizalization;

// Token: 0x0200002D RID: 45
public class Player : MonoBehaviourPunCallbacks
{
	// Token: 0x1700004A RID: 74
	// (get) Token: 0x06000350 RID: 848 RVA: 0x00016FC3 File Offset: 0x000151C3
	public Character character
	{
		get
		{
			return PlayerHandler.GetPlayerCharacter(this.view.Owner);
		}
	}

	// Token: 0x06000351 RID: 849 RVA: 0x00016FD8 File Offset: 0x000151D8
	private void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
		byte b = 0;
		while ((int)b < this.itemSlots.Length)
		{
			this.itemSlots[(int)b] = new ItemSlot(b);
			b += 1;
		}
		this.tempFullSlot = new TemporaryItemSlot(250);
		this.backpackSlot = new BackpackSlot(3);
		if (this.view != null)
		{
			PlayerHandler.RegisterPlayer(this);
			if (this.view.IsMine)
			{
				global::Player.localPlayer = this;
			}
		}
		base.gameObject.name = "Player: " + this.view.Owner.NickName;
		if (base.photonView.IsMine)
		{
			GlobalEvents.OnAchievementThrown = (Action<ACHIEVEMENTTYPE>)Delegate.Combine(GlobalEvents.OnAchievementThrown, new Action<ACHIEVEMENTTYPE>(this.OnAchievementProgressChanged));
		}
	}

	// Token: 0x06000352 RID: 850 RVA: 0x000170A8 File Offset: 0x000152A8
	private void OnDestroy()
	{
		if (global::Player.localPlayer == this)
		{
			GlobalEvents.OnAchievementThrown = (Action<ACHIEVEMENTTYPE>)Delegate.Remove(GlobalEvents.OnAchievementThrown, new Action<ACHIEVEMENTTYPE>(this.OnAchievementProgressChanged));
		}
	}

	// Token: 0x06000353 RID: 851 RVA: 0x000170D8 File Offset: 0x000152D8
	private void Update()
	{
		if (this._shouldSendCheevUpdate && AchievementManager.Initialized)
		{
			base.photonView.RPC("UpdateAchievementProgress", RpcTarget.Others, new object[]
			{
				Singleton<AchievementManager>.Instance.runBasedValueData
			});
			this._shouldSendCheevUpdate = false;
		}
	}

	// Token: 0x06000354 RID: 852 RVA: 0x00017124 File Offset: 0x00015324
	public bool AddItem(ushort itemID, ItemInstanceData instanceData, out ItemSlot slot)
	{
		global::Player.<>c__DisplayClass16_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.instanceData = instanceData;
		if (CS$<>8__locals1.instanceData == null)
		{
			CS$<>8__locals1.instanceData = new ItemInstanceData(Guid.NewGuid());
			ItemInstanceDataHandler.AddInstanceData(CS$<>8__locals1.instanceData);
		}
		if (!PhotonNetwork.IsMasterClient)
		{
			Debug.LogError("Only Master Client can add items!");
			slot = null;
			return false;
		}
		if (!ItemDatabase.TryGetItem(itemID, out CS$<>8__locals1.ItemPrefab))
		{
			Debug.LogError(string.Format("Failed to get item from item ID: {0}", itemID));
			slot = null;
			return false;
		}
		slot = this.<AddItem>g__AddToSlot|16_0(ref CS$<>8__locals1);
		if (slot == null)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Failed adding ",
				CS$<>8__locals1.ItemPrefab.name,
				" to ",
				base.name,
				"'s inventory, no slots available!"
			}));
			return false;
		}
		Debug.Log(string.Format("Granting {0}: {1} and added to slot: {2}", base.name, CS$<>8__locals1.ItemPrefab.name, slot.itemSlotID));
		byte[] array = IBinarySerializable.ToManagedArray<InventorySyncData>(new InventorySyncData(this.itemSlots, this.backpackSlot, this.tempFullSlot));
		this.view.RPC("SyncInventoryRPC", RpcTarget.Others, new object[]
		{
			array,
			false
		});
		return true;
	}

	// Token: 0x06000355 RID: 853 RVA: 0x00017261 File Offset: 0x00015461
	public static void LeaveCurrentGame()
	{
		if (NetCode.Matchmaking.InLobby)
		{
			NetCode.Matchmaking.LeaveLobby();
		}
		PhotonNetwork.Disconnect();
		Debug.Log("Leaving game and returning to main menu.");
	}

	// Token: 0x06000356 RID: 854 RVA: 0x00017288 File Offset: 0x00015488
	private void OnAchievementProgressChanged(ACHIEVEMENTTYPE _)
	{
		this.OnAchievementProgressChanged();
	}

	// Token: 0x06000357 RID: 855 RVA: 0x00017290 File Offset: 0x00015490
	public void OnAchievementProgressChanged()
	{
		if (!base.photonView.IsMine)
		{
			Debug.LogError("Can't broadcast " + base.name + "'s achievement data because that's not the local player!");
			return;
		}
		this._shouldSendCheevUpdate = true;
	}

	// Token: 0x06000358 RID: 856 RVA: 0x000172C1 File Offset: 0x000154C1
	[PunRPC]
	private void UpdateAchievementProgress(SerializableRunBasedValues progress)
	{
		Singleton<ReconnectHandler>.Instance.UpdateAchievementProgress(this, progress);
	}

	// Token: 0x06000359 RID: 857 RVA: 0x000172D0 File Offset: 0x000154D0
	[PunRPC]
	public void SyncInventoryRPC(byte[] data, bool forceSync)
	{
		if (!forceSync && PhotonNetwork.IsMasterClient)
		{
			Debug.LogError("SyncInventoryRPC should not sync to Master client. They are the boss");
			return;
		}
		InventorySyncData fromManagedArray = IBinarySerializable.GetFromManagedArray<InventorySyncData>(data);
		byte b = 0;
		while ((int)b < this.itemSlots.Length)
		{
			Item item;
			this.itemSlots[(int)b].prefab = (ItemDatabase.TryGetItem(fromManagedArray.slots[(int)b].ItemID, out item) ? item : null);
			this.itemSlots[(int)b].data = fromManagedArray.slots[(int)b].Data;
			b += 1;
		}
		this.backpackSlot.hasBackpack = fromManagedArray.hasBackpack;
		this.backpackSlot.data = fromManagedArray.backpackInstanceData;
		Item item2;
		this.tempFullSlot.prefab = (ItemDatabase.TryGetItem(fromManagedArray.tempSlot.ItemID, out item2) ? item2 : null);
		this.tempFullSlot.data = fromManagedArray.tempSlot.Data;
		if (this.view.IsMine)
		{
			this.character.refs.items.RefreshAllCharacterCarryWeightRPC();
		}
	}

	// Token: 0x0600035A RID: 858 RVA: 0x000173D3 File Offset: 0x000155D3
	[PunRPC]
	public void RPC_GetKicked(PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			Debug.LogWarning("Some naughty player attempted to kick me without authority to do so!");
			return;
		}
		NetworkConnector.ChangeConnectionState<KickedState>();
	}

	// Token: 0x0600035B RID: 859 RVA: 0x000173F4 File Offset: 0x000155F4
	[PunRPC]
	public void RPCRemoveItemFromSlot(byte slotID)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			Debug.LogError("Only Master Client can remove items!");
			return;
		}
		ItemSlot itemSlot = this.GetItemSlot(slotID);
		if (itemSlot != null)
		{
			itemSlot.EmptyOut();
		}
		InventorySyncData syncData = new InventorySyncData(this.itemSlots, this.backpackSlot, this.tempFullSlot);
		this.view.RPC("SyncInventoryRPC", RpcTarget.Others, new object[]
		{
			IBinarySerializable.ToManagedArray<InventorySyncData>(syncData),
			false
		});
	}

	// Token: 0x0600035C RID: 860 RVA: 0x00017468 File Offset: 0x00015668
	public void EmptySlot(Optionable<byte> slot)
	{
		if (slot.IsNone)
		{
			Debug.LogError("Can't empty none slot");
			return;
		}
		byte value = slot.Value;
		ItemSlot itemSlot = this.GetItemSlot(value);
		if (itemSlot != null)
		{
			itemSlot.EmptyOut();
		}
		if (PhotonNetwork.IsMasterClient)
		{
			InventorySyncData syncData = new InventorySyncData(this.itemSlots, this.backpackSlot, this.tempFullSlot);
			this.view.RPC("SyncInventoryRPC", RpcTarget.Others, new object[]
			{
				IBinarySerializable.ToManagedArray<InventorySyncData>(syncData),
				false
			});
			return;
		}
		this.view.RPC("RPCRemoveItemFromSlot", RpcTarget.MasterClient, new object[]
		{
			value
		});
	}

	// Token: 0x0600035D RID: 861 RVA: 0x00017510 File Offset: 0x00015710
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		InventorySyncData syncData = new InventorySyncData(this.itemSlots, this.backpackSlot, this.tempFullSlot);
		this.view.RPC("SyncInventoryRPC", newPlayer, new object[]
		{
			IBinarySerializable.ToManagedArray<InventorySyncData>(syncData),
			false
		});
	}

	// Token: 0x0600035E RID: 862 RVA: 0x00017566 File Offset: 0x00015766
	[PunRPC]
	public void RPC_SetInventory(byte[] newInventory)
	{
	}

	// Token: 0x0600035F RID: 863 RVA: 0x00017568 File Offset: 0x00015768
	public ItemSlot GetItemSlot(byte slotID)
	{
		if (slotID == 3)
		{
			return this.backpackSlot;
		}
		if (slotID == 250)
		{
			return this.tempFullSlot;
		}
		if (!this.itemSlots.WithinRange((int)slotID))
		{
			Debug.LogError(string.Format("{0} is attempting to get a non-existent ItemSlot index: {1}", base.name, slotID), this);
			return null;
		}
		return this.itemSlots[(int)slotID];
	}

	// Token: 0x06000360 RID: 864 RVA: 0x000175C4 File Offset: 0x000157C4
	public bool HasEmptySlot(ushort itemID)
	{
		Item item;
		if (!ItemDatabase.TryGetItem(itemID, out item))
		{
			Debug.LogError(string.Format("Failed to get item from item ID: {0}", itemID));
			return false;
		}
		if (item is Backpack)
		{
			return this.backpackSlot.IsEmpty();
		}
		ItemSlot[] array = this.itemSlots;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].IsEmpty())
			{
				return true;
			}
		}
		return this.tempFullSlot.IsEmpty();
	}

	// Token: 0x06000361 RID: 865 RVA: 0x00017632 File Offset: 0x00015832
	[ContextMenu("Debug Print Player ID")]
	private void DebugPrintPlayerID()
	{
		Debug.Log(base.photonView.Owner.ActorNumber);
	}

	// Token: 0x06000362 RID: 866 RVA: 0x00017650 File Offset: 0x00015850
	public bool HasInAnySlot(ushort itemID)
	{
		foreach (ItemSlot itemSlot in this.itemSlots)
		{
			if (!itemSlot.IsEmpty() && itemSlot.prefab.itemID == itemID)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000363 RID: 867 RVA: 0x00017690 File Offset: 0x00015890
	[ConsoleCommand]
	public static void PrintInventory(global::Player player)
	{
		byte b = 0;
		foreach (ItemSlot itemSlot in player.itemSlots)
		{
			Debug.Log(string.Format("Slot{0}: {1}", b, itemSlot.ToString()));
			if (!itemSlot.IsEmpty())
			{
				Debug.Log(string.Format("Data [{0}, keys: {1}]", itemSlot.data.guid, itemSlot.data.data.Count));
				foreach (KeyValuePair<DataEntryKey, DataEntryValue> keyValuePair in itemSlot.data.data)
				{
					Debug.Log(string.Format("{0} : {1}", keyValuePair.Key, keyValuePair.Value.GetType().Name));
					Debug.Log(keyValuePair.Value.ToString());
				}
			}
			b += 1;
		}
	}

	// Token: 0x06000365 RID: 869 RVA: 0x000177B4 File Offset: 0x000159B4
	[CompilerGenerated]
	private ItemSlot <AddItem>g__AddToSlot|16_0(ref global::Player.<>c__DisplayClass16_0 A_1)
	{
		if (A_1.ItemPrefab is Backpack)
		{
			if (this.backpackSlot.IsEmpty())
			{
				this.backpackSlot.hasBackpack = true;
				this.backpackSlot.data = A_1.instanceData;
				return this.backpackSlot;
			}
			return null;
		}
		else
		{
			for (int i = 0; i < this.itemSlots.Length; i++)
			{
				if (this.itemSlots[i].IsEmpty())
				{
					this.itemSlots[i].SetItem(A_1.ItemPrefab, A_1.instanceData);
					return this.itemSlots[i];
				}
			}
			if (this.tempFullSlot.IsEmpty() && !this.character.data.isClimbingAnything)
			{
				this.tempFullSlot.SetItem(A_1.ItemPrefab, A_1.instanceData);
				return this.tempFullSlot;
			}
			return null;
		}
	}

	// Token: 0x0400030A RID: 778
	public const int BACKPACKSLOTINDEX = 3;

	// Token: 0x0400030B RID: 779
	public ItemSlot[] itemSlots = new ItemSlot[3];

	// Token: 0x0400030C RID: 780
	public ItemSlot tempFullSlot;

	// Token: 0x0400030D RID: 781
	public BackpackSlot backpackSlot;

	// Token: 0x0400030E RID: 782
	public Action<int> hotbarSelectionChanged;

	// Token: 0x0400030F RID: 783
	public Action<ItemSlot[]> itemsChangedAction;

	// Token: 0x04000310 RID: 784
	public static global::Player localPlayer;

	// Token: 0x04000311 RID: 785
	public bool hasClosedEndScreen;

	// Token: 0x04000312 RID: 786
	public bool doneWithCutscene;

	// Token: 0x04000313 RID: 787
	private bool _shouldSendCheevUpdate;

	// Token: 0x04000314 RID: 788
	private PhotonView view;
}

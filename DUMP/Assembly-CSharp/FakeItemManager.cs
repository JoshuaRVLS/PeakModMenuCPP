using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI.Extensions;
using Zorro.Core.Serizalization;

// Token: 0x020000BA RID: 186
public class FakeItemManager : MonoBehaviourPunCallbacks
{
	// Token: 0x1700008D RID: 141
	// (get) Token: 0x060006FA RID: 1786 RVA: 0x00028263 File Offset: 0x00026463
	// (set) Token: 0x060006FB RID: 1787 RVA: 0x000282A2 File Offset: 0x000264A2
	public static FakeItemManager Instance
	{
		get
		{
			if (FakeItemManager._instance == null)
			{
				FakeItemManager._instance = Object.FindFirstObjectByType<FakeItemManager>();
				if (FakeItemManager._instance == null)
				{
					FakeItemManager._instance = GameUtils.instance.gameObject.AddComponent<FakeItemManager>();
				}
			}
			return FakeItemManager._instance;
		}
		private set
		{
			FakeItemManager._instance = value;
		}
	}

	// Token: 0x060006FC RID: 1788 RVA: 0x000282AA File Offset: 0x000264AA
	private void Awake()
	{
		FakeItemManager.Instance = this;
		if (this.fakeItemData.hiddenItems == null)
		{
			this.fakeItemData.hiddenItems = new List<int>();
		}
	}

	// Token: 0x060006FD RID: 1789 RVA: 0x000282D0 File Offset: 0x000264D0
	public void CullNullItems()
	{
		for (int i = this.allFakeItems.Count - 1; i >= 0; i--)
		{
			if (this.allFakeItems[i] == null)
			{
				this.allFakeItems.RemoveAt(i);
			}
		}
	}

	// Token: 0x060006FE RID: 1790 RVA: 0x00028318 File Offset: 0x00026518
	public int GetAvailableIndex()
	{
		for (int i = 0; i < 99999; i++)
		{
			bool flag = false;
			for (int j = 0; j < this.allFakeItems.Count; j++)
			{
				if (this.allFakeItems[j] != null && this.allFakeItems[j].index == i)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060006FF RID: 1791 RVA: 0x00028380 File Offset: 0x00026580
	public bool TryGetFakeItem(int index, out FakeItem item)
	{
		for (int i = 0; i < this.allFakeItems.Count; i++)
		{
			if (this.allFakeItems[i] != null && this.allFakeItems[i].index == index)
			{
				item = this.allFakeItems[i];
				return true;
			}
		}
		item = null;
		return false;
	}

	// Token: 0x06000700 RID: 1792 RVA: 0x000283E0 File Offset: 0x000265E0
	public void RefreshList()
	{
		this.allFakeItems = Object.FindObjectsByType<FakeItem>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList<FakeItem>();
		for (int i = 0; i < this.allFakeItems.Count; i++)
		{
			this.allFakeItems[i].index = i;
		}
	}

	// Token: 0x06000701 RID: 1793 RVA: 0x00028427 File Offset: 0x00026627
	public void AddToList(FakeItem item)
	{
		if (!Application.isPlaying)
		{
			this.allFakeItems.Add(item);
		}
	}

	// Token: 0x06000702 RID: 1794 RVA: 0x0002843C File Offset: 0x0002663C
	public void RemoveFromList(FakeItem item)
	{
		if (!Application.isPlaying)
		{
			this.allFakeItems.Remove(item);
		}
	}

	// Token: 0x06000703 RID: 1795 RVA: 0x00028452 File Offset: 0x00026652
	public bool Contains(FakeItem item)
	{
		return this.allFakeItems.Contains(item);
	}

	// Token: 0x1700008E RID: 142
	// (get) Token: 0x06000704 RID: 1796 RVA: 0x00028460 File Offset: 0x00026660
	public int ItemCount
	{
		get
		{
			return this.allFakeItems.Count;
		}
	}

	// Token: 0x06000705 RID: 1797 RVA: 0x00028470 File Offset: 0x00026670
	[PunRPC]
	public void RPC_RequestFakeItemPickup(PhotonView characterView, int fakeItemIndex)
	{
		Character component = characterView.GetComponent<Character>();
		FakeItem fakeItem;
		if (!this.TryGetFakeItem(fakeItemIndex, out fakeItem))
		{
			return;
		}
		ItemSlot itemSlot;
		if (component.player.AddItem(fakeItem.realItemPrefab.itemID, null, out itemSlot) && !fakeItem.pickedUp)
		{
			component.refs.view.RPC("OnPickupAccepted", component.player.photonView.Owner, new object[]
			{
				itemSlot.itemSlotID
			});
			base.photonView.RPC("RPC_FakeItemPickupSuccess", RpcTarget.All, new object[]
			{
				fakeItemIndex
			});
			return;
		}
		base.photonView.RPC("RPC_DenyFakeItemPickup", component.player.photonView.Owner, new object[]
		{
			fakeItem.index
		});
	}

	// Token: 0x06000706 RID: 1798 RVA: 0x00028548 File Offset: 0x00026748
	[PunRPC]
	internal void RPC_FakeItemPickupSuccess(int fakeItemIndex)
	{
		FakeItem fakeItem;
		if (this.TryGetFakeItem(fakeItemIndex, out fakeItem))
		{
			fakeItem.PickUpVisibly();
			return;
		}
		Debug.LogWarning(string.Format("Uh oh! We should have successfully picked up a fake item at index {0} ", fakeItemIndex) + string.Format("but that doesn't exist in our list which has {0} items.", this.allFakeItems.Count));
	}

	// Token: 0x06000707 RID: 1799 RVA: 0x0002859C File Offset: 0x0002679C
	[PunRPC]
	public void RPC_DenyFakeItemPickup(int fakeItemIndex)
	{
		FakeItem fakeItem;
		if (this.TryGetFakeItem(fakeItemIndex, out fakeItem))
		{
			fakeItem.UnPickUpVisibly();
			return;
		}
		Debug.LogWarning(string.Format("Uh oh! We should have failed to pick up a fake item at index {0} ", fakeItemIndex) + string.Format("but that doesn't exist in our list which has {0} items. Won't be able to unhide it.", this.allFakeItems.Count));
	}

	// Token: 0x06000708 RID: 1800 RVA: 0x000285F0 File Offset: 0x000267F0
	[PunRPC]
	public void RPC_RequestStickFakeItemToPlayer(int characterViewID, int fakeItemIndex, int bodyPartType, Vector3 offset)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		PhotonView photonView = PhotonNetwork.GetPhotonView(characterViewID);
		photonView.GetComponent<Character>().GetBodypart((BodypartType)bodyPartType);
		FakeItem fakeItem;
		if (this.TryGetFakeItem(fakeItemIndex, out fakeItem))
		{
			if (fakeItem.gameObject.activeInHierarchy)
			{
				StickyItemComponent stickyItemComponent;
				if (PhotonNetwork.InstantiateItemRoom(fakeItem.realItemPrefab.name, fakeItem.transform.position, fakeItem.transform.rotation).TryGetComponent<StickyItemComponent>(out stickyItemComponent))
				{
					stickyItemComponent.photonView.RPC("RPC_StickToCharacterRemote", photonView.Owner, new object[]
					{
						characterViewID,
						bodyPartType,
						offset
					});
				}
				base.photonView.RPC("RPC_FakeItemPickupSuccess", RpcTarget.All, new object[]
				{
					fakeItemIndex
				});
				return;
			}
			base.photonView.RPC("RPC_DenyFakeItemPickup", RpcTarget.All, new object[]
			{
				fakeItemIndex
			});
		}
	}

	// Token: 0x06000709 RID: 1801 RVA: 0x000286E0 File Offset: 0x000268E0
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			byte[] array = IBinarySerializable.ToManagedArray<FakeItemManager.FakeItemData>(this.fakeItemData);
			base.photonView.RPC("RPC_SyncFakeItems", newPlayer, new object[]
			{
				array
			});
		}
	}

	// Token: 0x0600070A RID: 1802 RVA: 0x0002871C File Offset: 0x0002691C
	[PunRPC]
	public void RPC_SyncFakeItems(byte[] data)
	{
		this.fakeItemData = IBinarySerializable.GetFromManagedArray<FakeItemManager.FakeItemData>(data);
		for (int i = 0; i < this.fakeItemData.hiddenItems.Count; i++)
		{
			int num = this.fakeItemData.hiddenItems[i];
			FakeItem fakeItem;
			if (this.TryGetFakeItem(num, out fakeItem))
			{
				fakeItem.gameObject.SetActive(false);
				fakeItem.pickedUp = true;
			}
			else
			{
				Debug.LogWarning(string.Format("Uh oh! Hidden item at index {0} doesn't exist with index {1} in the ", i, num) + "fake item list! Won't be able to properly sync this item.");
			}
		}
	}

	// Token: 0x0400070B RID: 1803
	private static FakeItemManager _instance;

	// Token: 0x0400070C RID: 1804
	internal FakeItemManager.FakeItemData fakeItemData;

	// Token: 0x0400070D RID: 1805
	[SerializeField]
	[ReadOnly]
	private List<FakeItem> allFakeItems = new List<FakeItem>();

	// Token: 0x02000455 RID: 1109
	public struct FakeItemData : IBinarySerializable
	{
		// Token: 0x06001C13 RID: 7187 RVA: 0x00087064 File Offset: 0x00085264
		public void Serialize(BinarySerializer serializer)
		{
			serializer.WriteInt(this.hiddenItems.Count);
			for (int i = 0; i < this.hiddenItems.Count; i++)
			{
				serializer.WriteInt(this.hiddenItems[i]);
			}
		}

		// Token: 0x06001C14 RID: 7188 RVA: 0x000870AC File Offset: 0x000852AC
		public void Deserialize(BinaryDeserializer deserializer)
		{
			int num = deserializer.ReadInt();
			this.hiddenItems = new List<int>();
			for (int i = 0; i < num; i++)
			{
				this.hiddenItems.Add(deserializer.ReadInt());
			}
		}

		// Token: 0x04001905 RID: 6405
		public List<int> hiddenItems;
	}
}

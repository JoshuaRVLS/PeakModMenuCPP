using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000060 RID: 96
public abstract class BackpackVisuals : MonoBehaviour
{
	// Token: 0x060004D2 RID: 1234
	public abstract BackpackData GetBackpackData();

	// Token: 0x060004D3 RID: 1235 RVA: 0x0001CD6C File Offset: 0x0001AF6C
	private void OnDestroy()
	{
		foreach (ValueTuple<GameObject, ushort> valueTuple in this.visualItems.Values)
		{
			PhotonNetwork.Destroy(valueTuple.Item1);
		}
	}

	// Token: 0x060004D4 RID: 1236 RVA: 0x0001CDC8 File Offset: 0x0001AFC8
	public void RefreshVisuals()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		BackpackData backpackData = this.GetBackpackData();
		if (backpackData == null)
		{
			return;
		}
		for (byte b = 0; b < 4; b += 1)
		{
			ItemSlot itemSlot = backpackData.itemSlots[(int)b];
			Optionable<ushort> left = itemSlot.IsEmpty() ? Optionable<ushort>.None : Optionable<ushort>.Some(itemSlot.prefab.itemID);
			ValueTuple<GameObject, ushort> valueTuple;
			Optionable<ushort> right = this.visualItems.TryGetValue(b, out valueTuple) ? Optionable<ushort>.Some(valueTuple.Item2) : Optionable<ushort>.None;
			if (left != right)
			{
				if (left.IsSome && right.IsSome)
				{
					Debug.LogError("Item Visuals Missmatch!");
				}
				else if (left.IsSome && right.IsNone)
				{
					Debug.Log(string.Format("Spawning Backpack Visual for {0}", left.Value));
					GameObject gameObject = PhotonNetwork.Instantiate("0_Items/" + itemSlot.GetPrefabName(), new Vector3(0f, -500f, 0f), Quaternion.identity, 0, null);
					this.PutItemInBackpack(gameObject, b);
					gameObject.GetComponent<PhotonView>().RPC("SetItemInstanceDataRPC", RpcTarget.All, new object[]
					{
						itemSlot.data
					});
					this.visualItems.Add(b, new ValueTuple<GameObject, ushort>(gameObject, left.Value));
				}
				else if (left.IsNone || right.IsSome)
				{
					Debug.Log(string.Format("Removing backpack visual for {0}", right.Value));
					ValueTuple<GameObject, ushort> valueTuple2;
					if (!this.visualItems.TryGetValue(b, out valueTuple2))
					{
						Debug.LogError(string.Format("Failed to get spawned object from slotID {0}", b));
					}
					PhotonView component = valueTuple2.Item1.GetComponent<PhotonView>();
					Debug.Log(string.Format("Destroying photon view: {0}", component));
					PhotonNetwork.Destroy(component);
					this.visualItems.Remove(b);
				}
				else
				{
					Debug.LogError("Should be unreachable");
				}
			}
			else if (left.IsNone)
			{
				Debug.Log(string.Format("Not Spawning backpack visual for slot id: {0} because it's empty...", b));
			}
		}
	}

	// Token: 0x060004D5 RID: 1237
	protected abstract void PutItemInBackpack(GameObject visual, byte slotID);

	// Token: 0x060004D6 RID: 1238 RVA: 0x0001CFD6 File Offset: 0x0001B1D6
	private void OnApplicationQuit()
	{
		this.m_shuttingDown = true;
	}

	// Token: 0x060004D7 RID: 1239 RVA: 0x0001CFE0 File Offset: 0x0001B1E0
	public void RemoveVisuals()
	{
		if (this.m_shuttingDown)
		{
			return;
		}
		foreach (ValueTuple<GameObject, ushort> valueTuple in this.visualItems.Values)
		{
			GameObject item = valueTuple.Item1;
			if (PhotonNetwork.IsMasterClient)
			{
				PhotonNetwork.Destroy(item);
			}
			else
			{
				item.gameObject.SetActive(false);
			}
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.visualItems.Clear();
		}
	}

	// Token: 0x060004D8 RID: 1240 RVA: 0x0001D06C File Offset: 0x0001B26C
	public bool TryGetSpawnedItem(byte slotID, out Item item)
	{
		return this.spawnedVisualItems.TryGetValue(slotID, out item) && item != null;
	}

	// Token: 0x060004D9 RID: 1241 RVA: 0x0001D087 File Offset: 0x0001B287
	public void SetSpawnedBackpackItem(byte slotID, Item item)
	{
		this.spawnedVisualItems[slotID] = item;
	}

	// Token: 0x04000531 RID: 1329
	public Transform[] backpackSlots;

	// Token: 0x04000532 RID: 1330
	private Dictionary<byte, ValueTuple<GameObject, ushort>> visualItems = new Dictionary<byte, ValueTuple<GameObject, ushort>>();

	// Token: 0x04000533 RID: 1331
	private Dictionary<byte, Item> spawnedVisualItems = new Dictionary<byte, Item>();

	// Token: 0x04000534 RID: 1332
	protected bool m_shuttingDown;
}

using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.CLI;

// Token: 0x0200011F RID: 287
[ConsoleClassCustomizer("Item")]
[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Scouts/ItemDatabase")]
public class ItemDatabase : ObjectDatabaseAsset<ItemDatabase, Item>
{
	// Token: 0x0600095E RID: 2398 RVA: 0x00032625 File Offset: 0x00030825
	public override void OnLoaded()
	{
		base.OnLoaded();
	}

	// Token: 0x0600095F RID: 2399 RVA: 0x0003262D File Offset: 0x0003082D
	public void LoadItems()
	{
	}

	// Token: 0x06000960 RID: 2400 RVA: 0x0003262F File Offset: 0x0003082F
	[ContextMenu("Reload entire database")]
	public void ReloadAllItems()
	{
	}

	// Token: 0x06000961 RID: 2401 RVA: 0x00032634 File Offset: 0x00030834
	private ushort GetAvailableID()
	{
		for (ushort num = 0; num < 65535; num += 1)
		{
			if (!this.itemLookup.ContainsKey(num))
			{
				return num;
			}
		}
		return 0;
	}

	// Token: 0x06000962 RID: 2402 RVA: 0x00032664 File Offset: 0x00030864
	private bool ItemExistsInDatabase(Item item, out ushort itemID)
	{
		foreach (ushort num in this.itemLookup.Keys)
		{
			if (this.itemLookup[num].Equals(item))
			{
				itemID = num;
				return true;
			}
		}
		itemID = 0;
		return false;
	}

	// Token: 0x06000963 RID: 2403 RVA: 0x000326D8 File Offset: 0x000308D8
	[ConsoleCommand]
	public static void Add(Item item)
	{
		if (MainCamera.instance == null)
		{
			return;
		}
		if (!PhotonNetwork.IsConnected)
		{
			return;
		}
		Transform transform = MainCamera.instance.transform;
		RaycastHit raycastHit;
		if (!Physics.Raycast(transform.position, transform.forward, out raycastHit))
		{
			raycastHit = default(RaycastHit);
		}
		ItemDatabase.Add(item, raycastHit.point + raycastHit.normal);
	}

	// Token: 0x06000964 RID: 2404 RVA: 0x0003273C File Offset: 0x0003093C
	public static void Add(Item item, Vector3 point)
	{
		if (!PhotonNetwork.IsConnected)
		{
			return;
		}
		Debug.Log(string.Format("Spawn item: {0} at {1}", item, point));
		PhotonNetwork.Instantiate("0_Items/" + item.name, point, Quaternion.identity, 0, null).GetComponent<Item>().RequestPickup(Character.localCharacter.GetComponent<PhotonView>());
	}

	// Token: 0x06000965 RID: 2405 RVA: 0x00032798 File Offset: 0x00030998
	public static bool TryGetItem(ushort itemID, out Item item)
	{
		return SingletonAsset<ItemDatabase>.Instance.itemLookup.TryGetValue(itemID, out item);
	}

	// Token: 0x06000966 RID: 2406 RVA: 0x000327AC File Offset: 0x000309AC
	public static bool TryGetItem(string itemNameOnFile, out Item item)
	{
		item = null;
		foreach (KeyValuePair<ushort, Item> keyValuePair in SingletonAsset<ItemDatabase>.Instance.itemLookup)
		{
			if (keyValuePair.Value.gameObject.name == itemNameOnFile)
			{
				item = keyValuePair.Value;
			}
		}
		return item != null;
	}

	// Token: 0x06000967 RID: 2407 RVA: 0x0003282C File Offset: 0x00030A2C
	public void AddAllNamesToCSV()
	{
		for (int i = 0; i < this.Objects.Count; i++)
		{
			this.Objects[i].AddNameToCSV();
		}
	}

	// Token: 0x06000968 RID: 2408 RVA: 0x00032860 File Offset: 0x00030A60
	public void PrintComparedLists()
	{
		string text = "";
		List<Item> list = (from i in new List<Item>(this.itemLookup.Values)
		orderby i.name
		select i).ToList<Item>();
		List<Item> list2 = (from i in new List<Item>(this.Objects)
		orderby i.name
		select i).ToList<Item>();
		int num = 0;
		while (num < list.Count || num < list2.Count)
		{
			string text2 = "NULL";
			string text3 = "NULL";
			int num2 = -1;
			if (num < list.Count && list[num] != null)
			{
				text2 = list[num].name;
				num2 = (int)list[num].itemID;
			}
			if (num < list2.Count && list2[num] != null)
			{
				text3 = list2[num].name;
				if (num2 == -1)
				{
					num2 = (int)list2[num].itemID;
				}
			}
			string text4 = "OK";
			if (text2 != text3)
			{
				text4 = "MISMATCH";
			}
			text += string.Format("index: {0} ID {1}: {2} vs {3} : {4}\n", new object[]
			{
				num,
				num2,
				text2,
				text3,
				text4
			});
			num++;
		}
		Debug.Log(text);
	}

	// Token: 0x06000969 RID: 2409 RVA: 0x000329DC File Offset: 0x00030BDC
	public void AddAllPromptsToCSV()
	{
		List<string> list = new List<string>();
		for (int i = 0; i < this.Objects.Count; i++)
		{
			List<string> collection = this.Objects[i].AddPromptToCSV(list);
			list.AddRange(collection);
		}
	}

	// Token: 0x040008C4 RID: 2244
	public Dictionary<ushort, Item> itemLookup = new Dictionary<ushort, Item>();
}

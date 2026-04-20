using System;
using UnityEngine;
using Zorro.Core.Serizalization;

// Token: 0x02000110 RID: 272
public class BackpackData : DataEntryValue
{
	// Token: 0x06000910 RID: 2320 RVA: 0x0003176C File Offset: 0x0002F96C
	public override void Init()
	{
		base.Init();
		byte b = 0;
		while ((int)b < this.itemSlots.Length)
		{
			this.itemSlots[(int)b] = new ItemSlot(b);
			b += 1;
		}
	}

	// Token: 0x06000911 RID: 2321 RVA: 0x000317A4 File Offset: 0x0002F9A4
	public override void SerializeValue(BinarySerializer serializer)
	{
		InventorySyncData inventorySyncData = new InventorySyncData(this.itemSlots, new BackpackSlot(4)
		{
			data = new ItemInstanceData(Guid.Empty)
		}, new TemporaryItemSlot(250));
		inventorySyncData.Serialize(serializer);
	}

	// Token: 0x06000912 RID: 2322 RVA: 0x000317E8 File Offset: 0x0002F9E8
	public override void DeserializeValue(BinaryDeserializer deserializer)
	{
		InventorySyncData inventorySyncData = default(InventorySyncData);
		inventorySyncData.Deserialize(deserializer);
		for (byte b = 0; b < 4; b += 1)
		{
			if (this.itemSlots[(int)b] == null)
			{
				this.itemSlots[(int)b] = new ItemSlot(b);
			}
			Item item;
			this.itemSlots[(int)b].prefab = (ItemDatabase.TryGetItem(inventorySyncData.slots[(int)b].ItemID, out item) ? item : null);
			this.itemSlots[(int)b].data = inventorySyncData.slots[(int)b].Data;
			Debug.Log(string.Format("Sync Back Inventory is setting slot: {0} to {1}", b, this.itemSlots[(int)b].ToString()));
		}
	}

	// Token: 0x06000913 RID: 2323 RVA: 0x0003189C File Offset: 0x0002FA9C
	public void AddItem(Item prefab, ItemInstanceData data, byte backpackSlotID)
	{
		if (data == null)
		{
			Debug.Log("DATA IS NULL??");
			data = new ItemInstanceData(Guid.NewGuid());
			ItemInstanceDataHandler.AddInstanceData(data);
		}
		if ((int)backpackSlotID < this.itemSlots.Length && this.itemSlots[(int)backpackSlotID].IsEmpty())
		{
			Debug.Log(string.Format("Added item: {0} to slot {1}", prefab.gameObject.name, backpackSlotID));
			this.itemSlots[(int)backpackSlotID].prefab = prefab;
			this.itemSlots[(int)backpackSlotID].data = data;
			return;
		}
	}

	// Token: 0x06000914 RID: 2324 RVA: 0x00031920 File Offset: 0x0002FB20
	public bool HasFreeSlot()
	{
		for (int i = 0; i < this.itemSlots.Length; i++)
		{
			if (this.itemSlots[i].IsEmpty())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000915 RID: 2325 RVA: 0x00031954 File Offset: 0x0002FB54
	public int FilledSlotCount()
	{
		int num = this.itemSlots.Length;
		for (int i = 0; i < this.itemSlots.Length; i++)
		{
			if (this.itemSlots[i].IsEmpty())
			{
				num--;
			}
		}
		return num;
	}

	// Token: 0x06000916 RID: 2326 RVA: 0x00031994 File Offset: 0x0002FB94
	public override string ToString()
	{
		string text = "";
		foreach (ItemSlot itemSlot in this.itemSlots)
		{
			text = text + itemSlot.ToString() + ", " + Environment.NewLine;
		}
		return text;
	}

	// Token: 0x0400089A RID: 2202
	public const int slots = 4;

	// Token: 0x0400089B RID: 2203
	public ItemSlot[] itemSlots = new ItemSlot[4];
}

using System;
using Zorro.Core.Serizalization;

// Token: 0x0200014B RID: 331
public struct InventorySyncData : IBinarySerializable
{
	// Token: 0x06000B04 RID: 2820 RVA: 0x0003B3A8 File Offset: 0x000395A8
	public InventorySyncData(ItemSlot[] itemSlots, BackpackSlot backpackSlot, ItemSlot tempSlot)
	{
		this.slotCount = (byte)itemSlots.Length;
		this.slots = new InventorySyncData.SlotData[itemSlots.Length];
		InventorySyncData.SlotData slotData;
		for (int i = 0; i < (int)this.slotCount; i++)
		{
			ushort itemID = (itemSlots[i].prefab == null) ? ushort.MaxValue : itemSlots[i].prefab.itemID;
			InventorySyncData.SlotData[] array = this.slots;
			int num = i;
			slotData = new InventorySyncData.SlotData
			{
				ItemID = itemID,
				Data = itemSlots[i].data
			};
			array[num] = slotData;
		}
		slotData = new InventorySyncData.SlotData
		{
			ItemID = ((tempSlot.prefab == null) ? ushort.MaxValue : tempSlot.prefab.itemID),
			Data = tempSlot.data
		};
		this.tempSlot = slotData;
		this.hasBackpack = !backpackSlot.IsEmpty();
		this.backpackInstanceData = backpackSlot.data;
	}

	// Token: 0x06000B05 RID: 2821 RVA: 0x0003B490 File Offset: 0x00039690
	public void Serialize(BinarySerializer serializer)
	{
		serializer.WriteByte(this.slotCount);
		for (int i = 0; i < (int)this.slotCount; i++)
		{
			this.slots[i].Serialize(serializer);
		}
		this.tempSlot.Serialize(serializer);
		serializer.WriteBool(this.hasBackpack);
		if (this.hasBackpack)
		{
			if (this.backpackInstanceData == null)
			{
				this.backpackInstanceData = new ItemInstanceData(Guid.NewGuid());
				ItemInstanceDataHandler.AddInstanceData(this.backpackInstanceData);
			}
			serializer.WriteGuid(this.backpackInstanceData.guid);
			this.backpackInstanceData.Serialize(serializer);
		}
	}

	// Token: 0x06000B06 RID: 2822 RVA: 0x0003B52C File Offset: 0x0003972C
	public void Deserialize(BinaryDeserializer deserializer)
	{
		this.slotCount = deserializer.ReadByte();
		this.slots = new InventorySyncData.SlotData[(int)this.slotCount];
		this.tempSlot = default(InventorySyncData.SlotData);
		for (int i = 0; i < (int)this.slotCount; i++)
		{
			InventorySyncData.SlotData slotData = default(InventorySyncData.SlotData);
			slotData.Deserialize(deserializer);
			this.slots[i] = slotData;
		}
		this.tempSlot.Deserialize(deserializer);
		this.hasBackpack = deserializer.ReadBool();
		if (this.hasBackpack)
		{
			Guid guid = deserializer.ReadGuid();
			if (!ItemInstanceDataHandler.TryGetInstanceData(guid, out this.backpackInstanceData))
			{
				this.backpackInstanceData = new ItemInstanceData(guid);
				ItemInstanceDataHandler.AddInstanceData(this.backpackInstanceData);
			}
			this.backpackInstanceData.Deserialize(deserializer);
		}
	}

	// Token: 0x04000A31 RID: 2609
	public byte slotCount;

	// Token: 0x04000A32 RID: 2610
	public InventorySyncData.SlotData[] slots;

	// Token: 0x04000A33 RID: 2611
	public InventorySyncData.SlotData tempSlot;

	// Token: 0x04000A34 RID: 2612
	public bool hasBackpack;

	// Token: 0x04000A35 RID: 2613
	public ItemInstanceData backpackInstanceData;

	// Token: 0x02000494 RID: 1172
	public struct SlotData : IBinarySerializable
	{
		// Token: 0x06001CE9 RID: 7401 RVA: 0x00088FC2 File Offset: 0x000871C2
		public void Serialize(BinarySerializer serializer)
		{
			serializer.WriteUshort(this.ItemID);
			if (this.ItemID != 65535)
			{
				serializer.WriteGuid(this.Data.guid);
				this.Data.Serialize(serializer);
			}
		}

		// Token: 0x06001CEA RID: 7402 RVA: 0x00088FFC File Offset: 0x000871FC
		public void Deserialize(BinaryDeserializer deserializer)
		{
			this.ItemID = deserializer.ReadUShort();
			if (this.ItemID != 65535)
			{
				Guid guid = deserializer.ReadGuid();
				if (!ItemInstanceDataHandler.TryGetInstanceData(guid, out this.Data))
				{
					this.Data = new ItemInstanceData(guid);
					ItemInstanceDataHandler.AddInstanceData(this.Data);
				}
				this.Data.Deserialize(deserializer);
			}
		}

		// Token: 0x04001A21 RID: 6689
		public ushort ItemID;

		// Token: 0x04001A22 RID: 6690
		public ItemInstanceData Data;
	}
}

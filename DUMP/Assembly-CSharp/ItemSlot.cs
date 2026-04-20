using System;

// Token: 0x02000026 RID: 38
[Serializable]
public class ItemSlot
{
	// Token: 0x060002DD RID: 733 RVA: 0x0001458C File Offset: 0x0001278C
	public ItemSlot(byte slotID)
	{
		this.itemSlotID = slotID;
	}

	// Token: 0x060002DE RID: 734 RVA: 0x0001459B File Offset: 0x0001279B
	public virtual bool IsEmpty()
	{
		return this.prefab == null;
	}

	// Token: 0x060002DF RID: 735 RVA: 0x000145A9 File Offset: 0x000127A9
	public void SetItem(Item itemPrefab, ItemInstanceData itemData)
	{
		this.data = itemData;
		this.prefab = itemPrefab;
	}

	// Token: 0x060002E0 RID: 736 RVA: 0x000145B9 File Offset: 0x000127B9
	public virtual void EmptyOut()
	{
		this.prefab = null;
	}

	// Token: 0x060002E1 RID: 737 RVA: 0x000145C4 File Offset: 0x000127C4
	public override string ToString()
	{
		string arg = (this.prefab == null) ? "null" : this.prefab.name;
		return string.Format("Slot ({0}): {1}", this.itemSlotID, arg);
	}

	// Token: 0x060002E2 RID: 738 RVA: 0x00014608 File Offset: 0x00012808
	public virtual string GetPrefabName()
	{
		if (this.prefab == null)
		{
			return "";
		}
		return this.prefab.gameObject.name;
	}

	// Token: 0x040002A5 RID: 677
	public Item prefab;

	// Token: 0x040002A6 RID: 678
	public ItemInstanceData data;

	// Token: 0x040002A7 RID: 679
	public byte itemSlotID;
}

using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using Zorro.Core.CLI;

// Token: 0x020000D4 RID: 212
public class ItemInstanceDataDebugPage : DebugPage
{
	// Token: 0x06000840 RID: 2112 RVA: 0x0002E765 File Offset: 0x0002C965
	public ItemInstanceDataDebugPage()
	{
		this.ScrollView = new ScrollView();
		base.Add(this.ScrollView);
	}

	// Token: 0x06000841 RID: 2113 RVA: 0x0002E790 File Offset: 0x0002C990
	public override void Update()
	{
		base.Update();
		List<ItemInstanceData> list = new List<ItemInstanceData>();
		foreach (Player player in PlayerHandler.GetAllPlayers())
		{
			foreach (ItemSlot itemSlot in player.itemSlots)
			{
				if (!itemSlot.IsEmpty())
				{
					list.Add(itemSlot.data);
				}
			}
		}
		foreach (ItemInstanceData itemInstanceData in list)
		{
			if (!this.m_spawnedCells.ContainsKey(itemInstanceData.guid))
			{
				DataEntryValue dataEntryValue;
				if (itemInstanceData.data.Count == 1 && itemInstanceData.data.TryGetValue(DataEntryKey.ItemUses, out dataEntryValue))
				{
					OptionableIntItemData optionableIntItemData = dataEntryValue as OptionableIntItemData;
					if (optionableIntItemData != null && !optionableIntItemData.HasData)
					{
						continue;
					}
				}
				ItemInstanceDataUICell itemInstanceDataUICell = new ItemInstanceDataUICell(itemInstanceData);
				this.ScrollView.Add(itemInstanceDataUICell);
				this.m_spawnedCells.Add(itemInstanceData.guid, itemInstanceDataUICell);
			}
		}
		foreach (KeyValuePair<Guid, ItemInstanceDataUICell> keyValuePair in this.m_spawnedCells)
		{
			keyValuePair.Value.Update();
		}
	}

	// Token: 0x04000800 RID: 2048
	private Dictionary<Guid, ItemInstanceDataUICell> m_spawnedCells = new Dictionary<Guid, ItemInstanceDataUICell>();

	// Token: 0x04000801 RID: 2049
	private ScrollView ScrollView;
}

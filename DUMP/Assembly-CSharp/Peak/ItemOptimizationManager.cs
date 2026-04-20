using System;
using System.Collections.Generic;
using UnityEngine;

namespace Peak
{
	// Token: 0x020003C7 RID: 967
	public class ItemOptimizationManager : MonoBehaviour
	{
		// Token: 0x06001995 RID: 6549 RVA: 0x00081840 File Offset: 0x0007FA40
		public static void RegisterItem(Item item)
		{
			ItemOptimizationManager.DATA_LIST.Add(item, new ItemOptimizationManager.ItemOptimizationData(item));
		}

		// Token: 0x06001996 RID: 6550 RVA: 0x00081853 File Offset: 0x0007FA53
		public static void DeregisterItem(Item item)
		{
			ItemOptimizationManager.DATA_LIST.Remove(item);
		}

		// Token: 0x06001997 RID: 6551 RVA: 0x00081864 File Offset: 0x0007FA64
		public void Update()
		{
			foreach (KeyValuePair<Item, ItemOptimizationManager.ItemOptimizationData> keyValuePair in ItemOptimizationManager.DATA_LIST)
			{
				this.UpdateItemActivity(keyValuePair.Value);
				this.UpdateItemScriptEnabled(keyValuePair.Value);
			}
		}

		// Token: 0x06001998 RID: 6552 RVA: 0x000818CC File Offset: 0x0007FACC
		private void UpdateItemActivity(ItemOptimizationManager.ItemOptimizationData itemData)
		{
			if (itemData.item.itemState == ItemState.Held)
			{
				itemData.item.WasActive();
			}
			itemData.item.UpdateEntryInActiveList();
		}

		// Token: 0x06001999 RID: 6553 RVA: 0x000818F4 File Offset: 0x0007FAF4
		private void UpdateItemScriptEnabled(ItemOptimizationManager.ItemOptimizationData itemData)
		{
			if (itemData.item.rig.isKinematic && itemData.item.itemState == ItemState.Ground)
			{
				itemData.scriptsActive = false;
			}
			else
			{
				itemData.scriptsActive = true;
			}
			itemData.item.enabled = itemData.scriptsActive;
			itemData.item.physicsSyncer.enabled = itemData.scriptsActive;
			if (itemData.hasBreakable)
			{
				itemData.breakable.enabled = itemData.scriptsActive;
			}
			if (itemData.hasImpactSFX)
			{
				itemData.impactSFX.enabled = itemData.scriptsActive;
			}
		}

		// Token: 0x04001741 RID: 5953
		private static Dictionary<Item, ItemOptimizationManager.ItemOptimizationData> DATA_LIST = new Dictionary<Item, ItemOptimizationManager.ItemOptimizationData>();

		// Token: 0x0200056C RID: 1388
		private class ItemOptimizationData
		{
			// Token: 0x06001FDB RID: 8155 RVA: 0x00090558 File Offset: 0x0008E758
			public ItemOptimizationData(Item item)
			{
				this.item = item;
				this.hasBreakable = item.TryGetComponent<Breakable>(out this.breakable);
				this.hasImpactSFX = item.TryGetComponent<ItemImpactSFX>(out this.impactSFX);
				this.scriptsActive = true;
			}

			// Token: 0x04001D59 RID: 7513
			public Item item;

			// Token: 0x04001D5A RID: 7514
			public Breakable breakable;

			// Token: 0x04001D5B RID: 7515
			public bool hasBreakable;

			// Token: 0x04001D5C RID: 7516
			public ItemImpactSFX impactSFX;

			// Token: 0x04001D5D RID: 7517
			public bool hasImpactSFX;

			// Token: 0x04001D5E RID: 7518
			internal bool scriptsActive = true;
		}
	}
}

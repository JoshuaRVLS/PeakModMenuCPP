using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000E5 RID: 229
public class Action_ConsumeAndSpawn : ItemAction
{
	// Token: 0x06000875 RID: 2165 RVA: 0x0002F3F0 File Offset: 0x0002D5F0
	public override void RunAction()
	{
		if (base.character)
		{
			int cookedAmount = 0;
			IntItemData intItemData;
			if (this.item.data.TryGetDataEntry<IntItemData>(DataEntryKey.CookedAmount, out intItemData))
			{
				cookedAmount = intItemData.Value;
			}
			this.item.StartCoroutine(this.item.ConsumeDelayed(false));
			base.character.StartCoroutine(this.SpawnItemDelayed(cookedAmount));
		}
	}

	// Token: 0x06000876 RID: 2166 RVA: 0x0002F453 File Offset: 0x0002D653
	public IEnumerator SpawnItemDelayed(int cookedAmount)
	{
		Character c = base.character;
		Item item = this.itemToSpawn;
		float timeout = 2f;
		while (this != null)
		{
			timeout -= Time.deltaTime;
			if (timeout <= 0f)
			{
				yield break;
			}
			yield return null;
		}
		GameUtils.instance.InstantiateAndGrab(item, c, cookedAmount);
		yield break;
	}

	// Token: 0x0400081C RID: 2076
	public Item itemToSpawn;
}

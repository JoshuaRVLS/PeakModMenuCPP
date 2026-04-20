using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200012F RID: 303
public class SpawnList : MonoBehaviour
{
	// Token: 0x060009DA RID: 2522 RVA: 0x000347FC File Offset: 0x000329FC
	private void RefreshPercentageOdds()
	{
		int num = 0;
		foreach (SpawnEntry spawnEntry in this.items)
		{
			num += spawnEntry.weight;
		}
		foreach (SpawnEntry spawnEntry2 in this.items)
		{
			spawnEntry2.percentageOdds = (float)spawnEntry2.weight / (float)num;
			spawnEntry2.percentageOdds = (float)Mathf.FloorToInt(spawnEntry2.percentageOdds * 1000f) / 10f;
		}
	}

	// Token: 0x060009DB RID: 2523 RVA: 0x000348BC File Offset: 0x00032ABC
	private void SortByWeight()
	{
		this.items = (from item in this.items
		orderby item.weight descending
		select item).ToList<SpawnEntry>();
	}

	// Token: 0x060009DC RID: 2524 RVA: 0x000348F3 File Offset: 0x00032AF3
	public GameObject GetSingleSpawn()
	{
		return this.items.RandomSelection((SpawnEntry i) => i.weight).prefab;
	}

	// Token: 0x060009DD RID: 2525 RVA: 0x00034924 File Offset: 0x00032B24
	public List<GameObject> GetSpawns(int count, bool canRepeat = true)
	{
		List<SpawnEntry> list = new List<SpawnEntry>(this.items);
		List<GameObject> list2 = new List<GameObject>();
		for (int j = 0; j < count; j++)
		{
			SpawnEntry spawnEntry = list.RandomSelection((SpawnEntry i) => i.weight);
			if (spawnEntry != null)
			{
				list2.Add(spawnEntry.prefab);
				if (!canRepeat)
				{
					if (list.Count <= 1)
					{
						list = new List<SpawnEntry>(this.items);
					}
					list.Remove(spawnEntry);
				}
			}
			else
			{
				list2.Add(null);
			}
		}
		return list2;
	}

	// Token: 0x060009DE RID: 2526 RVA: 0x000349B0 File Offset: 0x00032BB0
	private void FindReferencesInScene()
	{
		Spawner[] array = Object.FindObjectsOfType<Spawner>();
		bool flag = false;
		foreach (Spawner spawner in array)
		{
			if (spawner.spawns.gameObject.name == base.gameObject.name)
			{
				Debug.Log("Found " + base.gameObject.name + " on " + spawner.gameObject.name);
				flag = true;
			}
		}
		if (!flag)
		{
			Debug.Log(base.gameObject.name + " not present in scene.");
		}
	}

	// Token: 0x04000931 RID: 2353
	public List<SpawnEntry> items;
}

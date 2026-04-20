using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x020002A5 RID: 677
public class LootData : MonoBehaviour
{
	// Token: 0x06001341 RID: 4929 RVA: 0x0006123C File Offset: 0x0005F43C
	public static List<Item> GetAllItemsInPool(SpawnPool pool)
	{
		List<Item> list = new List<Item>();
		LootData.PopulateLootData();
		Dictionary<ushort, int> dictionary;
		if (LootData.AllSpawnWeightData.TryGetValue(pool, out dictionary))
		{
			foreach (KeyValuePair<ushort, int> keyValuePair in dictionary)
			{
				Item item;
				if (ItemDatabase.TryGetItem(keyValuePair.Key, out item))
				{
					list.Add(item);
				}
			}
		}
		return list;
	}

	// Token: 0x06001342 RID: 4930 RVA: 0x000612B8 File Offset: 0x0005F4B8
	public bool IsValidToSpawn()
	{
		if (this.banInSolo)
		{
			if (PhotonNetwork.OfflineMode)
			{
				return false;
			}
			if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount <= 1)
			{
				return false;
			}
		}
		return !this.excludeBasedOnCustomRunSetting || !RunSettings.IsCustomRun || RunSettings.GetValue(this.excludeIfSettingDisabled, false) != 0;
	}

	// Token: 0x06001343 RID: 4931 RVA: 0x0006130C File Offset: 0x0005F50C
	private void PrintOdds()
	{
		LootData.PopulateLootData();
		Item component = base.GetComponent<Item>();
		if (!component)
		{
			Debug.LogError("Loot data only works on Items right now.");
		}
		string text = base.gameObject.name + " appears in pools:\n";
		foreach (KeyValuePair<SpawnPool, Dictionary<ushort, int>> keyValuePair in LootData.AllSpawnWeightData)
		{
			if (keyValuePair.Value.ContainsKey(component.itemID))
			{
				text += string.Format("{0} ({1}%)\n", keyValuePair.Key.ToString(), LootData.GetPercentageOdds(component.itemID, keyValuePair.Key));
			}
		}
		Debug.Log(text);
	}

	// Token: 0x06001344 RID: 4932 RVA: 0x000613E4 File Offset: 0x0005F5E4
	public void TestSpawning()
	{
		string text = "Test 1:\n";
		foreach (GameObject gameObject in LootData.GetRandomItems(SpawnPool.All, 100, true, null))
		{
			text = text + gameObject.GetComponent<Item>().GetName() + "\n";
		}
		Debug.Log(text);
		text = "Test 2:\n";
		for (int i = 0; i < 100; i++)
		{
			GameObject randomItem = LootData.GetRandomItem(SpawnPool.All, false);
			text = text + randomItem.GetComponent<Item>().GetName() + "\n";
		}
		Debug.Log(text);
	}

	// Token: 0x06001345 RID: 4933 RVA: 0x0006149C File Offset: 0x0005F69C
	public static void PrintOdds(SpawnPool pool)
	{
		LootData.PopulateLootData();
		string text = pool.ToString() + " contains items:\n";
		Dictionary<ushort, int> dictionary;
		if (LootData.AllSpawnWeightData.TryGetValue(pool, out dictionary))
		{
			foreach (KeyValuePair<ushort, int> keyValuePair in dictionary)
			{
				Item item;
				if (ItemDatabase.TryGetItem(keyValuePair.Key, out item))
				{
					LootData component = item.GetComponent<LootData>();
					if (component)
					{
						text += string.Format("{0} ({1}% ({2}))\n", item.gameObject.name, LootData.GetPercentageOdds(keyValuePair.Key, pool), component.Rarity.ToString());
					}
					else
					{
						text += string.Format("{0} ({1}%)\n", item.gameObject.name, LootData.GetPercentageOdds(keyValuePair.Key, pool));
					}
				}
			}
		}
		Debug.Log(text);
	}

	// Token: 0x06001346 RID: 4934 RVA: 0x000615B8 File Offset: 0x0005F7B8
	public static GameObject GetRandomItem(SpawnPool spawnPool, bool debug = false)
	{
		if (debug)
		{
			Debug.Log(string.Format("chaos: Getting random item from spawn pool: {0}", spawnPool));
		}
		if (LootData.AllSpawnWeightData == null)
		{
			LootData.PopulateLootData();
		}
		Dictionary<ushort, int> enumerable;
		if (LootData.AllSpawnWeightData.TryGetValue(spawnPool, out enumerable))
		{
			Item item;
			ItemDatabase.TryGetItem(enumerable.RandomSelection((KeyValuePair<ushort, int> i) => i.Value).Key, out item);
			return item.gameObject;
		}
		return null;
	}

	// Token: 0x06001347 RID: 4935 RVA: 0x00061638 File Offset: 0x0005F838
	public static List<GameObject> GetRandomItems(SpawnPool spawnPool, int count, bool canRepeat = false, GameObject fallback = null)
	{
		if (LootData.AllSpawnWeightData == null)
		{
			LootData.PopulateLootData();
		}
		if (RunSettings.GetValue(RunSettings.SETTINGTYPE.ChaosItems, false) == 1)
		{
			spawnPool = SpawnPool.All;
		}
		Dictionary<ushort, int> dictionary;
		if (!LootData.AllSpawnWeightData.TryGetValue(spawnPool, out dictionary))
		{
			return null;
		}
		LootData.cachedSpawnPoolCopy = new Dictionary<ushort, int>(dictionary);
		LootData.ValidateSpawnPoolDictionary(LootData.cachedSpawnPoolCopy);
		List<GameObject> list = new List<GameObject>();
		if (LootData.cachedSpawnPoolCopy.Count == 0)
		{
			for (int i = 0; i < count; i++)
			{
				list.Add(fallback);
			}
			return list;
		}
		LootData.cachedSpawnPoolWorking = new Dictionary<ushort, int>(LootData.cachedSpawnPoolCopy);
		for (int j = 0; j < count; j++)
		{
			if (LootData.cachedSpawnPoolWorking.Count == 0)
			{
				if (!fallback)
				{
					break;
				}
				Debug.LogWarning("Added fallback item in the post-validation phase! This shouldn't be happening!!");
				list.Add(fallback);
			}
			else
			{
				ushort key = LootData.cachedSpawnPoolWorking.RandomSelection((KeyValuePair<ushort, int> kvp) => kvp.Value).Key;
				Item item;
				if (ItemDatabase.TryGetItem(key, out item))
				{
					list.Add(item.gameObject);
					if (!canRepeat)
					{
						LootData.cachedSpawnPoolWorking.Remove(key);
					}
				}
				if (LootData.cachedSpawnPoolWorking.Count == 0)
				{
					LootData.cachedSpawnPoolWorking = new Dictionary<ushort, int>(LootData.cachedSpawnPoolCopy);
				}
			}
		}
		return list;
	}

	// Token: 0x06001348 RID: 4936 RVA: 0x0006177C File Offset: 0x0005F97C
	private static void ValidateSpawnPoolDictionary(Dictionary<ushort, int> spawnPoolDictionary)
	{
		LootData.cachedInvalidSpawns.Clear();
		foreach (ushort num in spawnPoolDictionary.Keys)
		{
			Item item;
			if (ItemDatabase.TryGetItem(num, out item) && !item.IsValidToSpawn())
			{
				LootData.cachedInvalidSpawns.Add(num);
			}
		}
		foreach (ushort key in LootData.cachedInvalidSpawns)
		{
			spawnPoolDictionary.Remove(key);
		}
	}

	// Token: 0x06001349 RID: 4937 RVA: 0x00061834 File Offset: 0x0005FA34
	public static float GetPercentageOdds(ushort itemID, SpawnPool pool)
	{
		if (LootData.AllSpawnWeightData.ContainsKey(pool))
		{
			int num = 0;
			int num2 = 0;
			foreach (KeyValuePair<ushort, int> keyValuePair in LootData.AllSpawnWeightData[pool])
			{
				num += keyValuePair.Value;
				if (keyValuePair.Key == itemID)
				{
					num2 = keyValuePair.Value;
				}
			}
			return (float)Mathf.FloorToInt((float)num2 / (float)num * 1000f) / 10f;
		}
		return 0f;
	}

	// Token: 0x0600134A RID: 4938 RVA: 0x000618D0 File Offset: 0x0005FAD0
	public static void PopulateLootData()
	{
		LootData.AllSpawnWeightData = new Dictionary<SpawnPool, Dictionary<ushort, int>>();
		Array values = Enum.GetValues(typeof(SpawnPool));
		foreach (KeyValuePair<ushort, Item> keyValuePair in SingletonAsset<ItemDatabase>.Instance.itemLookup)
		{
			LootData component = keyValuePair.Value.GetComponent<LootData>();
			if (component)
			{
				foreach (object obj in values)
				{
					SpawnPool spawnPool = (SpawnPool)obj;
					if (spawnPool != SpawnPool.None && component.spawnLocations.HasFlag(spawnPool))
					{
						int value = LootData.RarityWeights[component.Rarity];
						if (!LootData.AllSpawnWeightData.ContainsKey(spawnPool))
						{
							LootData.AllSpawnWeightData.Add(spawnPool, new Dictionary<ushort, int>
							{
								{
									keyValuePair.Key,
									value
								}
							});
						}
						else
						{
							LootData.AllSpawnWeightData[spawnPool].Add(keyValuePair.Key, value);
						}
					}
				}
			}
		}
	}

	// Token: 0x04001156 RID: 4438
	public Rarity Rarity;

	// Token: 0x04001157 RID: 4439
	public SpawnPool spawnLocations;

	// Token: 0x04001158 RID: 4440
	public List<ItemRarityOverride> rarityOverrides = new List<ItemRarityOverride>();

	// Token: 0x04001159 RID: 4441
	public bool banInSolo;

	// Token: 0x0400115A RID: 4442
	public bool excludeFromCustomRunSelection;

	// Token: 0x0400115B RID: 4443
	public bool excludeBasedOnCustomRunSetting;

	// Token: 0x0400115C RID: 4444
	public RunSettings.SETTINGTYPE excludeIfSettingDisabled;

	// Token: 0x0400115D RID: 4445
	public GameObject useOtherItemForSpawningValidity;

	// Token: 0x0400115E RID: 4446
	private static Dictionary<ushort, int> cachedSpawnPoolCopy = new Dictionary<ushort, int>();

	// Token: 0x0400115F RID: 4447
	private static Dictionary<ushort, int> cachedSpawnPoolWorking = new Dictionary<ushort, int>();

	// Token: 0x04001160 RID: 4448
	private static List<ushort> cachedInvalidSpawns = new List<ushort>();

	// Token: 0x04001161 RID: 4449
	public static Dictionary<SpawnPool, Dictionary<ushort, int>> AllSpawnWeightData = null;

	// Token: 0x04001162 RID: 4450
	public static Dictionary<Rarity, int> RarityWeights = new Dictionary<Rarity, int>
	{
		{
			Rarity.Common,
			100
		},
		{
			Rarity.Uncommon,
			50
		},
		{
			Rarity.Rare,
			30
		},
		{
			Rarity.Epic,
			20
		},
		{
			Rarity.Legendary,
			15
		},
		{
			Rarity.Mythic,
			5
		},
		{
			Rarity.RidiculouslyRare,
			1
		}
	};
}

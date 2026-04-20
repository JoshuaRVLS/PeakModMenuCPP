using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.CLI;
using Zorro.Core.Serizalization;

// Token: 0x02000180 RID: 384
public static class RunSettings
{
	// Token: 0x170000F2 RID: 242
	// (get) Token: 0x06000C8B RID: 3211 RVA: 0x000434FE File Offset: 0x000416FE
	// (set) Token: 0x06000C8C RID: 3212 RVA: 0x00043505 File Offset: 0x00041705
	public static bool initialized { get; private set; }

	// Token: 0x170000F3 RID: 243
	// (get) Token: 0x06000C8D RID: 3213 RVA: 0x0004350D File Offset: 0x0004170D
	public static string GRAPPLE_MODE_ITEM
	{
		get
		{
			return "RescueHook_Infinite";
		}
	}

	// Token: 0x170000F4 RID: 244
	// (get) Token: 0x06000C8E RID: 3214 RVA: 0x00043514 File Offset: 0x00041714
	public static bool blockingAchievements
	{
		get
		{
			return Character.localCharacter != null && !Character.localCharacter.inAirport && RunSettings.IsCustomRun;
		}
	}

	// Token: 0x06000C8F RID: 3215 RVA: 0x00043538 File Offset: 0x00041738
	private static void InitializeDefaultValues()
	{
		RunSettings.Log("Initializing default values");
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.Fog, 1, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.HungerRate, 2, 3);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.FallDamage, 2, 3);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.EtcDamage, 2, 3);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.ColdNight, 0, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.ItemWeight, 1, 2);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.FlaresAtPeak, 1, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.ClimbingStaminaUsage, 2, 3);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.RevivalAllowed, 1, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.Hazard_Jellyfish, 1, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.Hazard_Urchins, 1, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.Hazard_ExplodingMushrooms, 1, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.Hazard_Rain, 1, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.Hazard_Bees, 1, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.Hazard_Zombies, 1, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.Hazard_Beetles, 1, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.Hazard_Spiders, 1, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.Hazard_SporeClouds, 1, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.Hazard_Wind, 1, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.Hazard_Storm, 1, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.Hazard_FlashPlant, 1, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.Hazard_Geysers, 1, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.Hazard_Tornado, 1, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.Hazard_Antlion, 1, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.Hazard_Scorpions, 1, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.Hazard_Tumbleweeds, 1, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.Hazard_TheLavaRises, 1, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.Hazard_Thorns, 1, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.Hazard_Dynamite, 1, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.Hazard_Heat, 1, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.Hazard_NapberryHypno, 1, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.Hazard_Mandrake, 1, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.Hazard_Scoutmaster, 1, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.Hazard_TheLavaRises, 1, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.MiniRun, 0, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.MiniRunBiome, 0, 3);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.GameMode, 0, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.GrappleMode, 0, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.ChaosItems, 0, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.HelpingHand, 0, 1);
		RunSettings.AddDefaultSetting(RunSettings.SETTINGTYPE.TimeScale, 0, 1);
	}

	// Token: 0x06000C90 RID: 3216 RVA: 0x00043738 File Offset: 0x00041938
	public static void Init()
	{
		if (!RunSettings.initialized)
		{
			RunSettings.InitializeDefaultValues();
			RunSettings.InitializeDefaultItemValues();
			RunSettings.InitAlphabetizedIndicesLINQ();
			RunSettings.SetValuesToDefault();
			RunSettings.LoadFromDisk();
			RunSettings.initialized = true;
		}
	}

	// Token: 0x06000C91 RID: 3217 RVA: 0x00043760 File Offset: 0x00041960
	private static void InitAlphabetizedIndicesLINQ()
	{
		RunSettings.alphabetizedIndices = (from s in RunSettings.defaultRunSettings
		select s.Key into s1
		orderby s1
		select s1).ToArray<string>();
	}

	// Token: 0x06000C92 RID: 3218 RVA: 0x000437C4 File Offset: 0x000419C4
	private static void InitializeDefaultItemValues()
	{
		RunSettings.InitModifiableItemNames();
		foreach (KeyValuePair<ushort, Item> keyValuePair in SingletonAsset<ItemDatabase>.Instance.itemLookup)
		{
			if (keyValuePair.Value)
			{
				RunSettings.AddDefaultSetting("Item_" + keyValuePair.Value.gameObject.name, 1, 1);
			}
		}
	}

	// Token: 0x06000C93 RID: 3219 RVA: 0x0004384C File Offset: 0x00041A4C
	private static string GetPlayerPrefsKey(string id, bool item = false)
	{
		return "RunSetting_" + id;
	}

	// Token: 0x06000C94 RID: 3220 RVA: 0x0004385C File Offset: 0x00041A5C
	public static void SaveToDisk()
	{
		foreach (KeyValuePair<string, RunSettings.RunSetting> keyValuePair in RunSettings.runSettingsDict)
		{
			PlayerPrefs.SetInt(RunSettings.GetPlayerPrefsKey(keyValuePair.Key, false), keyValuePair.Value.currentValue);
		}
	}

	// Token: 0x06000C95 RID: 3221 RVA: 0x000438C8 File Offset: 0x00041AC8
	[ConsoleCommand]
	public static void Test()
	{
		int value = RunSettings.GetValue("FallDamage", false);
		RunSettings.Log(string.Format("Current value of FallDamage is {0}", value));
		RunSettings.SetValue("FallDamage", (value == 0) ? 1 : 0);
		RunSettings.SaveToDisk();
	}

	// Token: 0x06000C96 RID: 3222 RVA: 0x0004390D File Offset: 0x00041B0D
	[ConsoleCommand]
	public static void ResetAll()
	{
		RunSettings.SetValuesToDefault();
		RunSettings.SaveToDisk();
	}

	// Token: 0x06000C97 RID: 3223 RVA: 0x0004391C File Offset: 0x00041B1C
	[ConsoleCommand]
	public static void Increment(RunSettings.SETTINGTYPE type, bool saveToDisk = true)
	{
		int num = RunSettings.GetValue(type, true);
		num++;
		if (num > RunSettings.GetMaxValue(type))
		{
			num = 0;
		}
		RunSettings.SetValue(type, num);
		if (saveToDisk)
		{
			RunSettings.SaveToDisk();
		}
	}

	// Token: 0x06000C98 RID: 3224 RVA: 0x00043950 File Offset: 0x00041B50
	public static void IncrementItem(Item item, bool saveToDisk = true)
	{
		int num = RunSettings.GetItemValue(item, true);
		num++;
		if (num > 1)
		{
			num = 0;
		}
		RunSettings.SetItemValue(item, num);
		if (saveToDisk)
		{
			RunSettings.SaveToDisk();
		}
	}

	// Token: 0x06000C99 RID: 3225 RVA: 0x00043980 File Offset: 0x00041B80
	[ConsoleCommand]
	public static void PrintValue(RunSettings.SETTINGTYPE type)
	{
		int value = RunSettings.GetValue(type, true);
		Debug.Log(string.Format("Value of {0} is {1}", type, value));
	}

	// Token: 0x06000C9A RID: 3226 RVA: 0x000439B0 File Offset: 0x00041BB0
	public static void LoadFromDisk()
	{
		int value = 0;
		foreach (KeyValuePair<string, RunSettings.RunSetting> keyValuePair in RunSettings.defaultRunSettings)
		{
			if (RunSettings.TryGetSettingValueFromDisk(keyValuePair.Key, out value))
			{
				RunSettings.SetValue(keyValuePair.Key, value);
			}
		}
		Debug.Log("Loaded run settings values from disk.");
	}

	// Token: 0x170000F5 RID: 245
	// (get) Token: 0x06000C9B RID: 3227 RVA: 0x00043A28 File Offset: 0x00041C28
	// (set) Token: 0x06000C9C RID: 3228 RVA: 0x00043A2F File Offset: 0x00041C2F
	public static bool IsCustomRun
	{
		get
		{
			return RunSettings._isCustomRun;
		}
		set
		{
			RunSettings._isCustomRun = value;
			RunSettings.Log(string.Format("Set custom run to {0}", value));
		}
	}

	// Token: 0x06000C9D RID: 3229 RVA: 0x00043A4C File Offset: 0x00041C4C
	private static int GetValue(string key, bool forceGetCustomValue = false)
	{
		RunSettings.RunSetting runSetting;
		if ((!RunSettings.IsCustomRun || GameUtils.instance.m_inAirport) && !forceGetCustomValue && RunSettings.defaultRunSettings.TryGetValue(key, out runSetting))
		{
			return runSetting.currentValue;
		}
		RunSettings.RunSetting runSetting2;
		if (RunSettings.runSettingsDict.TryGetValue(key, out runSetting2))
		{
			return runSetting2.currentValue;
		}
		return -1;
	}

	// Token: 0x06000C9E RID: 3230 RVA: 0x00043A9C File Offset: 0x00041C9C
	private static int GetMaxValue(string key)
	{
		RunSettings.RunSetting runSetting;
		if (RunSettings.runSettingsDict.TryGetValue(key, out runSetting))
		{
			return runSetting.maxValue;
		}
		return 0;
	}

	// Token: 0x06000C9F RID: 3231 RVA: 0x00043AC0 File Offset: 0x00041CC0
	private static int GetDefaultValue(string key)
	{
		RunSettings.RunSetting runSetting;
		if (RunSettings.runSettingsDict.TryGetValue(key, out runSetting))
		{
			return runSetting.defaultValue;
		}
		return 0;
	}

	// Token: 0x06000CA0 RID: 3232 RVA: 0x00043AE4 File Offset: 0x00041CE4
	public static int GetValue(RunSettings.SETTINGTYPE setting, bool forceGetCustomValue = false)
	{
		return RunSettings.GetValue(setting.ToString(), forceGetCustomValue);
	}

	// Token: 0x06000CA1 RID: 3233 RVA: 0x00043AF9 File Offset: 0x00041CF9
	public static int GetMaxValue(RunSettings.SETTINGTYPE setting)
	{
		return RunSettings.GetMaxValue(setting.ToString());
	}

	// Token: 0x06000CA2 RID: 3234 RVA: 0x00043B0D File Offset: 0x00041D0D
	public static int GetDefaultValue(RunSettings.SETTINGTYPE setting)
	{
		return RunSettings.GetDefaultValue(setting.ToString());
	}

	// Token: 0x06000CA3 RID: 3235 RVA: 0x00043B21 File Offset: 0x00041D21
	public static int GetItemValue(Item item, bool forceGetCustomValue = false)
	{
		return RunSettings.GetItemValue(item.gameObject.name, forceGetCustomValue);
	}

	// Token: 0x06000CA4 RID: 3236 RVA: 0x00043B34 File Offset: 0x00041D34
	public static int GetItemValue(string itemName, bool forceGetCustomValue = false)
	{
		return RunSettings.GetValue("Item_" + itemName, forceGetCustomValue);
	}

	// Token: 0x06000CA5 RID: 3237 RVA: 0x00043B48 File Offset: 0x00041D48
	private static bool TryGetSettingValueFromDisk(string id, out int currentValue)
	{
		string playerPrefsKey = RunSettings.GetPlayerPrefsKey(id, false);
		if (PlayerPrefs.HasKey(playerPrefsKey))
		{
			currentValue = PlayerPrefs.GetInt(playerPrefsKey);
			return true;
		}
		currentValue = -1;
		return false;
	}

	// Token: 0x06000CA6 RID: 3238 RVA: 0x00043B73 File Offset: 0x00041D73
	public static bool SetItemValue(Item item, int value)
	{
		return RunSettings.SetItemValue(item.gameObject.name, value);
	}

	// Token: 0x06000CA7 RID: 3239 RVA: 0x00043B86 File Offset: 0x00041D86
	public static bool SetItemValue(string itemName, int value)
	{
		bool result = RunSettings.SetValue("Item_" + itemName, value);
		RunSettings.UpdateBannedItems();
		return result;
	}

	// Token: 0x06000CA8 RID: 3240 RVA: 0x00043B9E File Offset: 0x00041D9E
	public static bool SetValue(RunSettings.SETTINGTYPE settingType, int value)
	{
		return RunSettings.SetValue(settingType.ToString(), value);
	}

	// Token: 0x06000CA9 RID: 3241 RVA: 0x00043BB4 File Offset: 0x00041DB4
	private static bool SetValue(string settingID, int value)
	{
		RunSettings.RunSetting runSetting;
		if (RunSettings.runSettingsDict.TryGetValue(settingID, out runSetting))
		{
			value = Mathf.Clamp(value, 0, runSetting.maxValue);
			runSetting.currentValue = value;
			if (settingID == "HelpingHand")
			{
				RunSettings.Log(string.Format("Successfully set value {0} to {1}", settingID, value));
			}
			return true;
		}
		return false;
	}

	// Token: 0x06000CAA RID: 3242 RVA: 0x00043C0C File Offset: 0x00041E0C
	private static void SetValuesToDefault()
	{
		RunSettings.runSettingsDict.Clear();
		foreach (KeyValuePair<string, RunSettings.RunSetting> keyValuePair in RunSettings.defaultRunSettings)
		{
			RunSettings.RunSetting value = new RunSettings.RunSetting(keyValuePair.Key, keyValuePair.Value.defaultValue, keyValuePair.Value.maxValue);
			RunSettings.runSettingsDict.TryAdd(keyValuePair.Key, value);
		}
	}

	// Token: 0x06000CAB RID: 3243 RVA: 0x00043C9C File Offset: 0x00041E9C
	private static void AddDefaultSetting(RunSettings.SETTINGTYPE id, int defaultValue, int maxValue)
	{
		RunSettings.AddDefaultSetting(id.ToString(), defaultValue, maxValue);
	}

	// Token: 0x06000CAC RID: 3244 RVA: 0x00043CB4 File Offset: 0x00041EB4
	private static void AddDefaultSetting(string id, int defaultValue, int maxValue)
	{
		RunSettings.RunSetting value = new RunSettings.RunSetting(id, defaultValue, maxValue);
		RunSettings.Log(string.Format("Adding default setting {0} with value {1}", id, defaultValue));
		RunSettings.defaultRunSettings.TryAdd(id, value);
	}

	// Token: 0x06000CAD RID: 3245 RVA: 0x00043CF0 File Offset: 0x00041EF0
	private static void InitModifiableItemNames()
	{
		RunSettings.ModifiableItemPrefabs = new List<Item>();
		foreach (Item item in SingletonAsset<ItemDatabase>.Instance.itemLookup.Values)
		{
			LootData lootData;
			if (!RunSettings.ModifiableItemPrefabs.Contains(item) && item.TryGetComponent<LootData>(out lootData))
			{
				foreach (SpawnPool spawnPool in RunSettings.includedSpawnPools)
				{
					if (lootData.spawnLocations.HasFlag(spawnPool) && !lootData.excludeFromCustomRunSelection)
					{
						RunSettings.ModifiableItemPrefabs.Add(item);
						break;
					}
				}
			}
		}
	}

	// Token: 0x06000CAE RID: 3246 RVA: 0x00043DB4 File Offset: 0x00041FB4
	public static void Log(string message)
	{
	}

	// Token: 0x06000CAF RID: 3247 RVA: 0x00043DB8 File Offset: 0x00041FB8
	public static List<ParameterAutocomplete> GetAutocompleteOptions(string parameterText)
	{
		List<ParameterAutocomplete> list = new List<ParameterAutocomplete>();
		foreach (object obj in Enum.GetValues(typeof(RunSettings.SETTINGTYPE)))
		{
			RunSettings.SETTINGTYPE settingtype = (RunSettings.SETTINGTYPE)obj;
			if (settingtype.ToString().ToLower().StartsWith(parameterText.ToLower()))
			{
				list.Add(new ParameterAutocomplete(settingtype.ToString().Trim()));
			}
		}
		return list;
	}

	// Token: 0x170000F6 RID: 246
	// (get) Token: 0x06000CB0 RID: 3248 RVA: 0x00043E58 File Offset: 0x00042058
	public static RunSettingSyncData SyncData
	{
		get
		{
			RunSettingSyncData result = default(RunSettingSyncData);
			result.SetData(RunSettings.runSettingsDict, RunSettings.IsCustomRun);
			return result;
		}
	}

	// Token: 0x06000CB1 RID: 3249 RVA: 0x00043E7F File Offset: 0x0004207F
	public static byte[] GetSerializedRunSettings()
	{
		if (!GameUtils.instance)
		{
			return Array.Empty<byte>();
		}
		return IBinarySerializable.ToManagedArray<RunSettingSyncData>(RunSettings.SyncData);
	}

	// Token: 0x06000CB2 RID: 3250 RVA: 0x00043EA0 File Offset: 0x000420A0
	public static void PushRunSettings(Photon.Realtime.Player player = null)
	{
		if (!GameUtils.instance)
		{
			return;
		}
		byte[] array = IBinarySerializable.ToManagedArray<RunSettingSyncData>(RunSettings.SyncData);
		if (player != null)
		{
			GameUtils.instance.photonView.RPC("RPC_SyncRunSettings", player, new object[]
			{
				array
			});
			return;
		}
		GameUtils.instance.photonView.RPC("RPC_SyncRunSettings", RpcTarget.Others, new object[]
		{
			array
		});
	}

	// Token: 0x06000CB3 RID: 3251 RVA: 0x00043F08 File Offset: 0x00042108
	public static void UpdateBannedItems()
	{
		RunSettings.bannedItemIDs.Clear();
		foreach (KeyValuePair<ushort, Item> keyValuePair in SingletonAsset<ItemDatabase>.Instance.itemLookup)
		{
			if (!RunSettings.IsItemEnabled(keyValuePair.Value.gameObject.name))
			{
				RunSettings.bannedItemIDs.Add(keyValuePair.Key);
			}
		}
	}

	// Token: 0x06000CB4 RID: 3252 RVA: 0x00043F8C File Offset: 0x0004218C
	public static bool IsItemEnabled(string itemObjectName)
	{
		return !RunSettings.runSettingsDict.ContainsKey("Item_" + itemObjectName) || RunSettings.GetItemValue(itemObjectName, false) > 0;
	}

	// Token: 0x06000CB5 RID: 3253 RVA: 0x00043FB1 File Offset: 0x000421B1
	public static bool IsItemEnabled(ushort id)
	{
		return !RunSettings.IsCustomRun || !RunSettings.bannedItemIDs.Contains(id);
	}

	// Token: 0x06000CB6 RID: 3254 RVA: 0x00043FCC File Offset: 0x000421CC
	public static void SetDataFromSyncData(RunSettingSyncData syncData)
	{
		RunSettings.IsCustomRun = syncData.isCustomRun;
		foreach (KeyValuePair<string, int> keyValuePair in syncData.settingValues)
		{
			RunSettings.SetValue(keyValuePair.Key, keyValuePair.Value);
		}
		GlobalEvents.TriggerRunSettingsUpdated();
	}

	// Token: 0x170000F7 RID: 247
	// (get) Token: 0x06000CB7 RID: 3255 RVA: 0x00044040 File Offset: 0x00042240
	public static bool isMiniRun
	{
		get
		{
			return RunSettings.GetValue(RunSettings.SETTINGTYPE.MiniRun, false) > 0;
		}
	}

	// Token: 0x04000B78 RID: 2936
	private static Dictionary<string, RunSettings.RunSetting> defaultRunSettings = new Dictionary<string, RunSettings.RunSetting>();

	// Token: 0x04000B79 RID: 2937
	private static Dictionary<string, RunSettings.RunSetting> runSettingsDict = new Dictionary<string, RunSettings.RunSetting>();

	// Token: 0x04000B7A RID: 2938
	public static string[] alphabetizedIndices;

	// Token: 0x04000B7B RID: 2939
	private static List<ushort> bannedItemIDs = new List<ushort>();

	// Token: 0x04000B7C RID: 2940
	public const bool PRINT_DEBUG_LOGS = false;

	// Token: 0x04000B7D RID: 2941
	public const string ITEM_PREFIX = "Item_";

	// Token: 0x04000B7E RID: 2942
	public static List<Item> ModifiableItemPrefabs = new List<Item>();

	// Token: 0x04000B7F RID: 2943
	private static bool _isCustomRun;

	// Token: 0x04000B80 RID: 2944
	private static SpawnPool[] includedSpawnPools = new SpawnPool[]
	{
		SpawnPool.LuggageBeach,
		SpawnPool.LuggageJungle,
		SpawnPool.LuggageTundra,
		SpawnPool.LuggageCaldera,
		SpawnPool.LuggageClimber,
		SpawnPool.LuggageAncient,
		SpawnPool.LuggageCursed,
		SpawnPool.LuggageMesa,
		SpawnPool.LuggageRoots
	};

	// Token: 0x020004AD RID: 1197
	public enum SETTINGTYPE
	{
		// Token: 0x04001A68 RID: 6760
		Fog = 100,
		// Token: 0x04001A69 RID: 6761
		HungerRate = 200,
		// Token: 0x04001A6A RID: 6762
		FallDamage = 300,
		// Token: 0x04001A6B RID: 6763
		EtcDamage = 400,
		// Token: 0x04001A6C RID: 6764
		ColdNight = 500,
		// Token: 0x04001A6D RID: 6765
		ItemWeight = 600,
		// Token: 0x04001A6E RID: 6766
		FlaresAtPeak = 700,
		// Token: 0x04001A6F RID: 6767
		ClimbingStaminaUsage = 800,
		// Token: 0x04001A70 RID: 6768
		RevivalAllowed = 900,
		// Token: 0x04001A71 RID: 6769
		Hazard_Jellyfish = 1000,
		// Token: 0x04001A72 RID: 6770
		Hazard_Urchins = 1100,
		// Token: 0x04001A73 RID: 6771
		Hazard_ExplodingMushrooms = 1200,
		// Token: 0x04001A74 RID: 6772
		Hazard_Rain = 1250,
		// Token: 0x04001A75 RID: 6773
		Hazard_Bees = 1300,
		// Token: 0x04001A76 RID: 6774
		Hazard_Zombies = 1400,
		// Token: 0x04001A77 RID: 6775
		Hazard_Beetles = 1500,
		// Token: 0x04001A78 RID: 6776
		Hazard_Spiders = 1600,
		// Token: 0x04001A79 RID: 6777
		Hazard_SporeClouds = 1700,
		// Token: 0x04001A7A RID: 6778
		Hazard_Wind = 1800,
		// Token: 0x04001A7B RID: 6779
		Hazard_Storm = 1900,
		// Token: 0x04001A7C RID: 6780
		Hazard_FlashPlant = 1950,
		// Token: 0x04001A7D RID: 6781
		Hazard_Geysers = 2000,
		// Token: 0x04001A7E RID: 6782
		Hazard_Tornado = 2100,
		// Token: 0x04001A7F RID: 6783
		Hazard_Scorpions = 2125,
		// Token: 0x04001A80 RID: 6784
		Hazard_Antlion = 2150,
		// Token: 0x04001A81 RID: 6785
		Hazard_Tumbleweeds = 2175,
		// Token: 0x04001A82 RID: 6786
		Hazard_Thorns = 2200,
		// Token: 0x04001A83 RID: 6787
		Hazard_Dynamite = 2300,
		// Token: 0x04001A84 RID: 6788
		Hazard_Heat = 2400,
		// Token: 0x04001A85 RID: 6789
		Hazard_NapberryHypno = 2500,
		// Token: 0x04001A86 RID: 6790
		Hazard_Mandrake = 2600,
		// Token: 0x04001A87 RID: 6791
		Hazard_Scoutmaster = 2700,
		// Token: 0x04001A88 RID: 6792
		Hazard_TheLavaRises = 2800,
		// Token: 0x04001A89 RID: 6793
		MiniRun = 10000,
		// Token: 0x04001A8A RID: 6794
		MiniRunBiome = 10050,
		// Token: 0x04001A8B RID: 6795
		GameMode = 10100,
		// Token: 0x04001A8C RID: 6796
		HelpingHand = 10200,
		// Token: 0x04001A8D RID: 6797
		GrappleMode = 20000,
		// Token: 0x04001A8E RID: 6798
		ChaosItems = 20010,
		// Token: 0x04001A8F RID: 6799
		TimeScale = 30000,
		// Token: 0x04001A90 RID: 6800
		Hazard_Ghost = 60000,
		// Token: 0x04001A91 RID: 6801
		Hazard_Ghost_Baby = 60010,
		// Token: 0x04001A92 RID: 6802
		Hazard_Ghost_Baby_Baby = 60020,
		// Token: 0x04001A93 RID: 6803
		Hazard_Monkey = 60030,
		// Token: 0x04001A94 RID: 6804
		Hazard_Octopus = 60040,
		// Token: 0x04001A95 RID: 6805
		Hazard_Deer = 60050,
		// Token: 0x04001A96 RID: 6806
		Hazard_Demon = 60060,
		// Token: 0x04001A97 RID: 6807
		Hazard_OreVein = 60070,
		// Token: 0x04001A98 RID: 6808
		Romance_BingBong = 80000,
		// Token: 0x04001A99 RID: 6809
		Romance_Scoutmaster = 80010,
		// Token: 0x04001A9A RID: 6810
		Firearms = 80020,
		// Token: 0x04001A9B RID: 6811
		MonkeyMode = 80030
	}

	// Token: 0x020004AE RID: 1198
	public class RunSetting
	{
		// Token: 0x06001D3E RID: 7486 RVA: 0x00089A2E File Offset: 0x00087C2E
		public RunSetting(string id, int defaultValue, int maxValue)
		{
			this.id = id;
			this.defaultValue = defaultValue;
			this.maxValue = maxValue;
			this.currentValue = defaultValue;
		}

		// Token: 0x04001A9C RID: 6812
		public string id;

		// Token: 0x04001A9D RID: 6813
		public int defaultValue;

		// Token: 0x04001A9E RID: 6814
		public int maxValue;

		// Token: 0x04001A9F RID: 6815
		public int currentValue;
	}
}

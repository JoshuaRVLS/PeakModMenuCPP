using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Peak.Afflictions;
using Steamworks;
using Unity.Multiplayer.Playmode;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.CLI;

// Token: 0x02000045 RID: 69
[ConsoleClassCustomizer("Achievements")]
public class AchievementManager : Singleton<AchievementManager>
{
	// Token: 0x1700004F RID: 79
	// (get) Token: 0x060003F2 RID: 1010 RVA: 0x00019895 File Offset: 0x00017A95
	// (set) Token: 0x060003F3 RID: 1011 RVA: 0x0001989C File Offset: 0x00017A9C
	public static bool Initialized { get; private set; }

	// Token: 0x060003F4 RID: 1012 RVA: 0x000198A4 File Offset: 0x00017AA4
	public void DebugGetAchievement()
	{
		this.ThrowAchievement(this.debugAchievement);
	}

	// Token: 0x060003F5 RID: 1013 RVA: 0x000198B4 File Offset: 0x00017AB4
	public void DebugGetAllAchievements()
	{
		foreach (object obj in Enum.GetValues(typeof(ACHIEVEMENTTYPE)))
		{
			ACHIEVEMENTTYPE type = (ACHIEVEMENTTYPE)obj;
			this.ThrowAchievement(type);
		}
	}

	// Token: 0x17000050 RID: 80
	// (get) Token: 0x060003F6 RID: 1014 RVA: 0x00019918 File Offset: 0x00017B18
	// (set) Token: 0x060003F7 RID: 1015 RVA: 0x00019928 File Offset: 0x00017B28
	public static bool GotStats
	{
		get
		{
			return AchievementManager.s_gotStats && SteamManager.Initialized;
		}
		private set
		{
			AchievementManager.s_gotStats = value;
		}
	}

	// Token: 0x17000051 RID: 81
	// (get) Token: 0x060003F8 RID: 1016 RVA: 0x00019930 File Offset: 0x00017B30
	// (set) Token: 0x060003F9 RID: 1017 RVA: 0x00019938 File Offset: 0x00017B38
	public SerializableRunBasedValues runBasedValueData { get; private set; }

	// Token: 0x060003FA RID: 1018 RVA: 0x00019941 File Offset: 0x00017B41
	public void InitRunBasedValues()
	{
		this.InitRunBasedValues(SerializableRunBasedValues.ConstructNew());
	}

	// Token: 0x060003FB RID: 1019 RVA: 0x00019950 File Offset: 0x00017B50
	public void InitRunBasedValues(SerializableRunBasedValues initialValues)
	{
		AchievementManager.<>c__DisplayClass25_0 CS$<>8__locals1 = new AchievementManager.<>c__DisplayClass25_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.initialValues = initialValues;
		base.StartCoroutine(CS$<>8__locals1.<InitRunBasedValues>g__InitWhenReady|0());
	}

	// Token: 0x060003FC RID: 1020 RVA: 0x0001997E File Offset: 0x00017B7E
	[ConsoleCommand]
	public static void ClearAchievements()
	{
		Singleton<AchievementManager>.Instance.ResetAllUserStats();
	}

	// Token: 0x060003FD RID: 1021 RVA: 0x0001998A File Offset: 0x00017B8A
	[ContextMenu("RESET ALL DATA")]
	private void ResetAllUserStats()
	{
		SteamUserStats.ResetAllStats(true);
		this.StoreUserStats();
		this.InitRunBasedValues();
	}

	// Token: 0x060003FE RID: 1022 RVA: 0x0001999F File Offset: 0x00017B9F
	private void Start()
	{
		if (CurrentPlayer.ReadOnlyTags().Contains("NoSteam"))
		{
			return;
		}
		Callback<UserStatsReceived_t>.Create(new Callback<UserStatsReceived_t>.DispatchDelegate(this.OnUserStatsRecieved));
		base.StartCoroutine(this.GetUserStatsRoutine());
	}

	// Token: 0x060003FF RID: 1023 RVA: 0x000199D2 File Offset: 0x00017BD2
	public override void OnDestroy()
	{
		base.OnDestroy();
		this.UnsubscribeFromEvents();
	}

	// Token: 0x06000400 RID: 1024 RVA: 0x000199E0 File Offset: 0x00017BE0
	private IEnumerator GetUserStatsRoutine()
	{
		while (SteamManager.Instance == null)
		{
			Debug.Log("Waiting for steam manager");
			yield return null;
		}
		while (!SteamManager.Initialized)
		{
			yield return null;
		}
		while (!AchievementManager.GotStats)
		{
			SteamUserStats.RequestUserStats(SteamUser.GetSteamID());
			yield return new WaitForSeconds(2f);
		}
		yield break;
	}

	// Token: 0x06000401 RID: 1025 RVA: 0x000199E8 File Offset: 0x00017BE8
	private void StoreUserStats()
	{
		base.StartCoroutine(this.StoreUserStatsRoutine());
	}

	// Token: 0x06000402 RID: 1026 RVA: 0x000199F7 File Offset: 0x00017BF7
	private IEnumerator StoreUserStatsRoutine()
	{
		while (!SteamManager.Initialized)
		{
			yield return null;
		}
		SteamUserStats.StoreStats();
		yield break;
	}

	// Token: 0x06000403 RID: 1027 RVA: 0x00019A00 File Offset: 0x00017C00
	public int GetMaxAscent()
	{
		if (this.useDebugAscent)
		{
			return this.debugAscent;
		}
		int result;
		if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.MaxAscent, out result))
		{
			return result;
		}
		return 0;
	}

	// Token: 0x06000404 RID: 1028 RVA: 0x00019A30 File Offset: 0x00017C30
	public bool AllBaseAchievementsUnlocked()
	{
		for (int i = 1; i <= 32; i++)
		{
			if (!Singleton<AchievementManager>.Instance.IsAchievementUnlocked((ACHIEVEMENTTYPE)i))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000405 RID: 1029 RVA: 0x00019A5C File Offset: 0x00017C5C
	public bool IsAchievementUnlocked(ACHIEVEMENTTYPE achievementType)
	{
		if (!SteamManager.Initialized)
		{
			return false;
		}
		bool result;
		if (this.CheckSteamStatLinkedAchievementUnlocked(achievementType, out result))
		{
			return result;
		}
		bool result2;
		SteamUserStats.GetAchievement(achievementType.ToString(), out result2);
		return result2;
	}

	// Token: 0x06000406 RID: 1030 RVA: 0x00019A98 File Offset: 0x00017C98
	private bool TryGetSteamStatLinkedAchievement(ACHIEVEMENTTYPE achievementType, out AchievementManager.SteamStatBasedAchievementData result)
	{
		result = null;
		if (!SteamManager.Initialized)
		{
			return false;
		}
		foreach (AchievementManager.SteamStatBasedAchievementData steamStatBasedAchievementData in this.steamStatBasedAchievements)
		{
			if (steamStatBasedAchievementData.achievementType == achievementType)
			{
				result = steamStatBasedAchievementData;
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000407 RID: 1031 RVA: 0x00019B04 File Offset: 0x00017D04
	public bool CheckSteamStatLinkedAchievementUnlocked(ACHIEVEMENTTYPE achievementType, out bool isUnlocked)
	{
		isUnlocked = false;
		AchievementManager.SteamStatBasedAchievementData steamStatBasedAchievementData;
		if (this.TryGetSteamStatLinkedAchievement(achievementType, out steamStatBasedAchievementData))
		{
			isUnlocked = steamStatBasedAchievementData.IsAchieved();
			return true;
		}
		return false;
	}

	// Token: 0x06000408 RID: 1032 RVA: 0x00019B2C File Offset: 0x00017D2C
	public bool TryThrowStatLinkedAchievement(ACHIEVEMENTTYPE achievementType)
	{
		AchievementManager.SteamStatBasedAchievementData steamStatBasedAchievementData;
		if (this.TryGetSteamStatLinkedAchievement(achievementType, out steamStatBasedAchievementData))
		{
			this.SetSteamStat(steamStatBasedAchievementData.statType, steamStatBasedAchievementData.requiredValue);
			return true;
		}
		return false;
	}

	// Token: 0x06000409 RID: 1033 RVA: 0x00019B5C File Offset: 0x00017D5C
	private void CheckRunBasedAchievement(RUNBASEDVALUETYPE type)
	{
		foreach (AchievementManager.RunBasedAchievementData runBasedAchievementData in this.runBasedAchievements)
		{
			if (runBasedAchievementData.valueType == type && runBasedAchievementData.IsAchieved())
			{
				this.ThrowAchievement(runBasedAchievementData.achievementType);
			}
		}
	}

	// Token: 0x0600040A RID: 1034 RVA: 0x00019BC8 File Offset: 0x00017DC8
	private void CheckNewAchievements()
	{
		if (this.runBasedValueData.steamAchievementsPreviouslyUnlocked == null)
		{
			return;
		}
		foreach (object obj in Enum.GetValues(typeof(ACHIEVEMENTTYPE)))
		{
			ACHIEVEMENTTYPE achievementtype = (ACHIEVEMENTTYPE)obj;
			if (!this.runBasedValueData.steamAchievementsPreviouslyUnlocked.Contains(achievementtype) && this.IsAchievementUnlocked(achievementtype))
			{
				Debug.Log("EARNED ACHIEVEMENT: " + achievementtype.ToString());
				if (!this.runBasedValueData.achievementsEarnedThisRun.Contains(achievementtype))
				{
					this.runBasedValueData.achievementsEarnedThisRun.Add(achievementtype);
				}
				this.runBasedValueData.steamAchievementsPreviouslyUnlocked.Add(achievementtype);
				GlobalEvents.OnAchievementThrown(achievementtype);
			}
		}
	}

	// Token: 0x0600040B RID: 1035 RVA: 0x00019CB0 File Offset: 0x00017EB0
	public void SetSteamStat(STEAMSTATTYPE steamStatType, int value)
	{
		if (!SteamManager.Initialized || !AchievementManager.GotStats)
		{
			return;
		}
		bool flag = Character.localCharacter && !Character.localCharacter.inAirport;
		if (flag && RunSettings.IsCustomRun)
		{
			return;
		}
		SteamUserStats.SetStat(steamStatType.ToString(), value);
		this.StoreUserStats();
		if (flag)
		{
			this.CheckNewAchievements();
		}
	}

	// Token: 0x0600040C RID: 1036 RVA: 0x00019D17 File Offset: 0x00017F17
	public bool GetSteamStatInt(STEAMSTATTYPE steamStatType, out int value)
	{
		if (!SteamManager.Initialized)
		{
			value = -1;
			return false;
		}
		return SteamUserStats.GetStat(steamStatType.ToString(), out value);
	}

	// Token: 0x0600040D RID: 1037 RVA: 0x00019D38 File Offset: 0x00017F38
	public int GetNextPage()
	{
		if (!SteamManager.Initialized)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < 8; i++)
		{
			int num2;
			SteamUserStats.GetStat("ReadGuidebookPage_" + i.ToString(), out num2);
			if (num2 != 1)
			{
				return num;
			}
			num++;
		}
		return num;
	}

	// Token: 0x0600040E RID: 1038 RVA: 0x00019D84 File Offset: 0x00017F84
	public int GetTotalPagesSeen()
	{
		if (!SteamManager.Initialized)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < 8; i++)
		{
			int num2;
			SteamUserStats.GetStat("ReadGuidebookPage_" + i.ToString(), out num2);
			if (num2 == 1)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x0600040F RID: 1039 RVA: 0x00019DCC File Offset: 0x00017FCC
	public bool SeenGuidebookPage(int index)
	{
		int num;
		SteamUserStats.GetStat("ReadGuidebookPage_" + index.ToString(), out num);
		return num == 1;
	}

	// Token: 0x06000410 RID: 1040 RVA: 0x00019DF8 File Offset: 0x00017FF8
	public void TriggerSeenGuidebookPage(int index)
	{
		SteamUserStats.SetStat("ReadGuidebookPage_" + index.ToString(), 1);
		this.StoreUserStats();
		int totalPagesSeen = this.GetTotalPagesSeen();
		Debug.Log("Total pages seen: " + totalPagesSeen.ToString());
		this.SetSteamStat(STEAMSTATTYPE.TotalPagesRead, totalPagesSeen);
		if (totalPagesSeen >= 8)
		{
			this.ThrowAchievement(ACHIEVEMENTTYPE.BookwormBadge);
		}
		this.StoreUserStats();
		Debug.Log("Saw page " + index.ToString());
	}

	// Token: 0x06000411 RID: 1041 RVA: 0x00019E74 File Offset: 0x00018074
	public int IncrementSteamStat(STEAMSTATTYPE steamStatType, int value)
	{
		int result;
		try
		{
			if (!SteamManager.Initialized)
			{
				result = 0;
			}
			else if (RunSettings.blockingAchievements)
			{
				result = 0;
			}
			else
			{
				int num;
				SteamUserStats.GetStat(steamStatType.ToString(), out num);
				num += value;
				SteamUserStats.SetStat(steamStatType.ToString(), num);
				this.StoreUserStats();
				this.CheckNewAchievements();
				result = num;
			}
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			result = 0;
		}
		return result;
	}

	// Token: 0x06000412 RID: 1042 RVA: 0x00019EF0 File Offset: 0x000180F0
	[ConsoleCommand]
	internal static void UnlockAll()
	{
		foreach (object obj in Enum.GetValues(typeof(ACHIEVEMENTTYPE)))
		{
			AchievementManager.Grant((ACHIEVEMENTTYPE)obj);
		}
	}

	// Token: 0x06000413 RID: 1043 RVA: 0x00019F50 File Offset: 0x00018150
	[ConsoleCommand]
	internal static void Grant(ACHIEVEMENTTYPE type)
	{
		Singleton<AchievementManager>.Instance.ThrowAchievement(type);
	}

	// Token: 0x06000414 RID: 1044 RVA: 0x00019F5D File Offset: 0x0001815D
	[ConsoleCommand]
	internal static void GiveAscentLevel(int level)
	{
		Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.MaxAscent, level);
	}

	// Token: 0x06000415 RID: 1045 RVA: 0x00019F6C File Offset: 0x0001816C
	internal void ThrowAchievement(ACHIEVEMENTTYPE type)
	{
		try
		{
			if (!SteamManager.Initialized)
			{
				if (!CurrentPlayer.ReadOnlyTags().Contains("NoSteam"))
				{
					Debug.LogError(string.Format("Tried to throw achievement of type {0} before Steam Manager was initialized. ", type) + "This probably resulted in the achievement not being granted.");
				}
			}
			else if (!RunSettings.blockingAchievements)
			{
				if (!this.IsAchievementUnlocked(type))
				{
					if (GameUtils.instance.m_inAirport)
					{
						SteamUserStats.SetAchievement(type.ToString());
						GlobalEvents.OnAchievementThrown(type);
					}
					else if (this.runBasedValueData.steamAchievementsPreviouslyUnlocked != null && !this.runBasedValueData.steamAchievementsPreviouslyUnlocked.Contains(type))
					{
						Debug.Log("Throwing achievement: " + type.ToString());
						if (!this.TryThrowStatLinkedAchievement(type))
						{
							if (!this.runBasedValueData.achievementsEarnedThisRun.Contains(type))
							{
								this.runBasedValueData.achievementsEarnedThisRun.Add(type);
							}
							SteamUserStats.SetAchievement(type.ToString());
							GlobalEvents.OnAchievementThrown(type);
							this.runBasedValueData.steamAchievementsPreviouslyUnlocked.Add(type);
						}
					}
				}
				this.StoreUserStats();
			}
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
	}

	// Token: 0x06000416 RID: 1046 RVA: 0x0001A0C4 File Offset: 0x000182C4
	public void SetRunBasedInt(RUNBASEDVALUETYPE type, int value)
	{
		this.runBasedValueData.runBasedInts[type] = value;
		this.CheckRunBasedAchievement(type);
		Player.localPlayer.OnAchievementProgressChanged();
	}

	// Token: 0x06000417 RID: 1047 RVA: 0x0001A0EC File Offset: 0x000182EC
	public int GetRunBasedInt(RUNBASEDVALUETYPE type)
	{
		if (!this.runBasedValueData.runBasedInts.ContainsKey(type))
		{
			this.SetRunBasedInt(type, 0);
		}
		try
		{
			return this.runBasedValueData.runBasedInts[type];
		}
		catch
		{
			Debug.LogError(string.Format("Tried to retrieve run based int {0} that is not an int.", type));
		}
		return 0;
	}

	// Token: 0x06000418 RID: 1048 RVA: 0x0001A154 File Offset: 0x00018354
	public void AddToRunBasedInt(RUNBASEDVALUETYPE type, int valueToAdd)
	{
		int runBasedInt = this.GetRunBasedInt(type);
		this.SetRunBasedInt(type, runBasedInt + valueToAdd);
	}

	// Token: 0x06000419 RID: 1049 RVA: 0x0001A173 File Offset: 0x00018373
	public void SetRunBasedFloat(RUNBASEDVALUETYPE type, float value)
	{
		this.runBasedValueData.runBasedFloats[type] = value;
		this.CheckRunBasedAchievement(type);
		Player.localPlayer.OnAchievementProgressChanged();
	}

	// Token: 0x0600041A RID: 1050 RVA: 0x0001A198 File Offset: 0x00018398
	public float GetRunBasedFloat(RUNBASEDVALUETYPE type)
	{
		if (!this.runBasedValueData.runBasedFloats.ContainsKey(type))
		{
			this.SetRunBasedFloat(type, 0f);
		}
		try
		{
			return Convert.ToSingle(this.runBasedValueData.runBasedFloats[type]);
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Format("Tried to retrieve run based float {0} that is not a float.\n{1}", type, ex.ToString()));
		}
		return 0f;
	}

	// Token: 0x0600041B RID: 1051 RVA: 0x0001A214 File Offset: 0x00018414
	public void AddToRunBasedFloat(RUNBASEDVALUETYPE type, float valueToAdd)
	{
		float runBasedFloat = this.GetRunBasedFloat(type);
		this.SetRunBasedFloat(type, runBasedFloat + valueToAdd);
	}

	// Token: 0x0600041C RID: 1052 RVA: 0x0001A234 File Offset: 0x00018434
	private void SubscribeToEvents()
	{
		GlobalEvents.OnItemRequested = (Action<Item, Character>)Delegate.Combine(GlobalEvents.OnItemRequested, new Action<Item, Character>(this.TestRequestedItem));
		GlobalEvents.OnItemConsumed = (Action<Item, Character>)Delegate.Combine(GlobalEvents.OnItemConsumed, new Action<Item, Character>(this.TestItemConsumed));
		GlobalEvents.OnLocalStatusIncremented = (Action<Character, CharacterAfflictions.STATUSTYPE, bool>)Delegate.Combine(GlobalEvents.OnLocalStatusIncremented, new Action<Character, CharacterAfflictions.STATUSTYPE, bool>(this.TestStatusIncremented));
		GlobalEvents.OnRespawnChestOpened = (Action<RespawnChest, Character>)Delegate.Combine(GlobalEvents.OnRespawnChestOpened, new Action<RespawnChest, Character>(this.TestRespawnChestOpened));
		GlobalEvents.OnLuggageOpened = (Action<Luggage, Character>)Delegate.Combine(GlobalEvents.OnLuggageOpened, new Action<Luggage, Character>(this.TestLuggageOpened));
		GlobalEvents.OnLocalCharacterWonRun = (Action)Delegate.Combine(GlobalEvents.OnLocalCharacterWonRun, new Action(this.TestWonRun));
		GlobalEvents.OnCharacterPassedOut = (Action<Character>)Delegate.Combine(GlobalEvents.OnCharacterPassedOut, new Action<Character>(this.TestCharacterPassedOut));
		GlobalEvents.OnSomeoneWonRun = (Action)Delegate.Combine(GlobalEvents.OnSomeoneWonRun, new Action(this.TestSomeoneWonRun));
		GlobalEvents.OnRunEnded = (Action)Delegate.Combine(GlobalEvents.OnRunEnded, new Action(this.TestRunEnded));
		GlobalEvents.OnCharacterDied = (Action<Character>)Delegate.Combine(GlobalEvents.OnCharacterDied, new Action<Character>(this.TestPlayerDied));
	}

	// Token: 0x0600041D RID: 1053 RVA: 0x0001A384 File Offset: 0x00018584
	private void UnsubscribeFromEvents()
	{
		GlobalEvents.OnItemRequested = (Action<Item, Character>)Delegate.Remove(GlobalEvents.OnItemRequested, new Action<Item, Character>(this.TestRequestedItem));
		GlobalEvents.OnItemConsumed = (Action<Item, Character>)Delegate.Remove(GlobalEvents.OnItemConsumed, new Action<Item, Character>(this.TestItemConsumed));
		GlobalEvents.OnLocalStatusIncremented = (Action<Character, CharacterAfflictions.STATUSTYPE, bool>)Delegate.Remove(GlobalEvents.OnLocalStatusIncremented, new Action<Character, CharacterAfflictions.STATUSTYPE, bool>(this.TestStatusIncremented));
		GlobalEvents.OnRespawnChestOpened = (Action<RespawnChest, Character>)Delegate.Remove(GlobalEvents.OnRespawnChestOpened, new Action<RespawnChest, Character>(this.TestRespawnChestOpened));
		GlobalEvents.OnLuggageOpened = (Action<Luggage, Character>)Delegate.Remove(GlobalEvents.OnLuggageOpened, new Action<Luggage, Character>(this.TestLuggageOpened));
		GlobalEvents.OnLocalCharacterWonRun = (Action)Delegate.Remove(GlobalEvents.OnLocalCharacterWonRun, new Action(this.TestWonRun));
		GlobalEvents.OnCharacterPassedOut = (Action<Character>)Delegate.Remove(GlobalEvents.OnCharacterPassedOut, new Action<Character>(this.TestCharacterPassedOut));
		GlobalEvents.OnSomeoneWonRun = (Action)Delegate.Remove(GlobalEvents.OnSomeoneWonRun, new Action(this.TestSomeoneWonRun));
		GlobalEvents.OnRunEnded = (Action)Delegate.Remove(GlobalEvents.OnRunEnded, new Action(this.TestRunEnded));
		GlobalEvents.OnCharacterDied = (Action<Character>)Delegate.Remove(GlobalEvents.OnCharacterDied, new Action<Character>(this.TestPlayerDied));
	}

	// Token: 0x0600041E RID: 1054 RVA: 0x0001A4D1 File Offset: 0x000186D1
	private void OnUserStatsRecieved(UserStatsReceived_t result)
	{
		if (result.m_eResult != EResult.k_EResultFail)
		{
			AchievementManager.GotStats = true;
		}
	}

	// Token: 0x0600041F RID: 1055 RVA: 0x0001A4E2 File Offset: 0x000186E2
	private void TestRequestedItem(Item item, Character character)
	{
		if (character.IsLocal && item.itemTags.HasFlag(Item.ItemTags.Mystical))
		{
			this.ThrowAchievement(ACHIEVEMENTTYPE.EsotericaBadge);
		}
	}

	// Token: 0x06000420 RID: 1056 RVA: 0x0001A50C File Offset: 0x0001870C
	private void TestItemConsumed(Item item, Character character)
	{
		if (character.IsLocal)
		{
			if (item.itemTags.HasFlag(Item.ItemTags.Berry))
			{
				this.AddToRunBasedFruitsEaten(item.itemID);
				if (item.itemTags.HasFlag(Item.ItemTags.Mushroom))
				{
					this.AddToShroomBerriesEaten(item.itemID);
				}
			}
			if (item.itemTags.HasFlag(Item.ItemTags.Bird) && !Character.localCharacter.data.cannibalismPermitted)
			{
				this.ThrowAchievement(ACHIEVEMENTTYPE.ResourcefulnessBadge);
			}
			if (item.itemTags.HasFlag(Item.ItemTags.PackagedFood))
			{
				this.AddToRunBasedInt(RUNBASEDVALUETYPE.PackagedFoodEaten, 1);
			}
			if (item.itemTags.HasFlag(Item.ItemTags.Mushroom) && !item.itemTags.HasFlag(Item.ItemTags.Berry) && item.GetComponent<Action_InflictPoison>() == null)
			{
				this.AddToNonToxicMushroomsEaten(item.itemID);
			}
			if (item.itemTags.HasFlag(Item.ItemTags.GourmandRequirement) && item.GetData<IntItemData>(DataEntryKey.CookedAmount).Value >= 1)
			{
				this.AddToGourmandRequirementsEaten(item.itemID);
			}
		}
	}

	// Token: 0x06000421 RID: 1057 RVA: 0x0001A640 File Offset: 0x00018840
	private void TestStatusIncremented(Character character, CharacterAfflictions.STATUSTYPE statusType, bool changeWasPositive)
	{
		if (!changeWasPositive && this.GetRunBasedInt(RUNBASEDVALUETYPE.BitByZombie) > 0)
		{
			Debug.Log("Was previously bit");
			bool flag = character.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Spores) >= 0.025f;
			Affliction affliction;
			bool flag2 = character.refs.afflictions.HasAfflictionType(Affliction.AfflictionType.ZombieBite, out affliction);
			Debug.Log(string.Format("Has spores: {0}, hasAffliction: {1}", flag, flag2));
			if (!flag && !flag2)
			{
				this.ThrowAchievement(ACHIEVEMENTTYPE.UndeadEncounterBadge);
			}
		}
	}

	// Token: 0x06000422 RID: 1058 RVA: 0x0001A6C0 File Offset: 0x000188C0
	private void TestRespawnChestOpened(RespawnChest chest, Character opener)
	{
		if (opener.IsLocal)
		{
			foreach (Character character in Character.AllCharacters)
			{
				if (character.data.dead || character.data.fullyPassedOut)
				{
					this.AddToRunBasedInt(RUNBASEDVALUETYPE.ScoutsResurrected, 1);
				}
			}
		}
	}

	// Token: 0x06000423 RID: 1059 RVA: 0x0001A738 File Offset: 0x00018938
	private void TestLuggageOpened(Luggage luggage, Character opener)
	{
		if (opener.IsLocal)
		{
			this.AddToRunBasedInt(RUNBASEDVALUETYPE.LuggageOpened, 1);
		}
	}

	// Token: 0x06000424 RID: 1060 RVA: 0x0001A74C File Offset: 0x0001894C
	public void TestTimeAchievements()
	{
		int num = Mathf.FloorToInt(RunManager.Instance.timeSinceRunStarted);
		if ((float)num <= 3600f)
		{
			Debug.Log("Sub one hour!!");
			this.ThrowAchievement(ACHIEVEMENTTYPE.SpeedClimberBadge);
		}
		int num2;
		this.GetSteamStatInt(STEAMSTATTYPE.BestTime, out num2);
		if (num < num2)
		{
			this.SetSteamStat(STEAMSTATTYPE.BestTime, num);
		}
	}

	// Token: 0x06000425 RID: 1061 RVA: 0x0001A79C File Offset: 0x0001899C
	private void TestRunEnded()
	{
		this.TestAscentAchievements(-1);
	}

	// Token: 0x06000426 RID: 1062 RVA: 0x0001A7A5 File Offset: 0x000189A5
	public void TestCoolCucumberAchievement()
	{
		if (this.GetRunBasedFloat(RUNBASEDVALUETYPE.MaxHeatTakenInMesa) <= 0.1f)
		{
			this.ThrowAchievement(ACHIEVEMENTTYPE.CoolCucumberBadge);
		}
	}

	// Token: 0x06000427 RID: 1063 RVA: 0x0001A7BE File Offset: 0x000189BE
	public void TestTreadLightlyAchievement()
	{
		if (this.GetRunBasedFloat(RUNBASEDVALUETYPE.MaxSporesTakenInRoots) <= 0.25f)
		{
			this.ThrowAchievement(ACHIEVEMENTTYPE.TreadLightlyBadge);
		}
	}

	// Token: 0x06000428 RID: 1064 RVA: 0x0001A7D7 File Offset: 0x000189D7
	public void TestBundledUpAchievement()
	{
		if (this.GetRunBasedFloat(RUNBASEDVALUETYPE.MaxColdTakenInAlpine) <= 0.2f)
		{
			this.ThrowAchievement(ACHIEVEMENTTYPE.BundledUpBadge);
		}
	}

	// Token: 0x06000429 RID: 1065 RVA: 0x0001A7F0 File Offset: 0x000189F0
	private void TestWonRun()
	{
		this.ThrowAchievement(ACHIEVEMENTTYPE.PeakBadge);
		this.IncrementSteamStat(STEAMSTATTYPE.TimesPeaked, 1);
		if (Character.AllCharacters.Count == 1)
		{
			this.ThrowAchievement(ACHIEVEMENTTYPE.LoneWolfBadge);
		}
		if (this.GetRunBasedFloat(RUNBASEDVALUETYPE.FallDamageTaken) == 0f)
		{
			this.ThrowAchievement(ACHIEVEMENTTYPE.BalloonBadge);
		}
		if (this.GetRunBasedInt(RUNBASEDVALUETYPE.PackagedFoodEaten) == 0)
		{
			this.ThrowAchievement(ACHIEVEMENTTYPE.NaturalistBadge);
		}
		if (this.GetRunBasedInt(RUNBASEDVALUETYPE.TimesPassedOut) == 0)
		{
			this.ThrowAchievement(ACHIEVEMENTTYPE.SurvivalistBadge);
		}
		if (this.GetRunBasedInt(RUNBASEDVALUETYPE.PermanentItemsPlaced) == 0)
		{
			this.ThrowAchievement(ACHIEVEMENTTYPE.LeaveNoTraceBadge);
		}
		if (this.HasBingBong(Character.localCharacter))
		{
			GameUtils.instance.ThrowBingBongAchievement();
		}
		if (this.runBasedValueData.gourmandRequirementsEaten.Count >= 4)
		{
			this.ThrowAchievement(ACHIEVEMENTTYPE.GourmandBadge);
		}
	}

	// Token: 0x0600042A RID: 1066 RVA: 0x0001A8A0 File Offset: 0x00018AA0
	private bool HasBingBong(Character character)
	{
		if (character.data.currentItem && character.data.currentItem.itemTags.HasFlag(Item.ItemTags.BingBong))
		{
			return true;
		}
		foreach (ItemSlot itemSlot in character.player.itemSlots)
		{
			if (itemSlot != null && itemSlot.prefab != null && itemSlot.prefab.itemTags.HasFlag(Item.ItemTags.BingBong))
			{
				return true;
			}
		}
		BackpackData backpackData;
		if (!character.player.backpackSlot.IsEmpty() && character.player.backpackSlot.data.TryGetDataEntry<BackpackData>(DataEntryKey.BackpackData, out backpackData))
		{
			foreach (ItemSlot itemSlot2 in backpackData.itemSlots)
			{
				if (itemSlot2 != null && itemSlot2.prefab != null && itemSlot2.prefab.itemTags.HasFlag(Item.ItemTags.BingBong))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x0600042B RID: 1067 RVA: 0x0001A9AE File Offset: 0x00018BAE
	private void TestCharacterPassedOut(Character character)
	{
		if (character.IsLocal)
		{
			this.AddToRunBasedInt(RUNBASEDVALUETYPE.TimesPassedOut, 1);
		}
	}

	// Token: 0x0600042C RID: 1068 RVA: 0x0001A9C0 File Offset: 0x00018BC0
	private void TestSomeoneWonRun()
	{
		if (Character.localCharacter.refs.stats.lost)
		{
			Debug.Log("YOU TRIED");
			this.ThrowAchievement(ACHIEVEMENTTYPE.TriedYourBestBadge);
		}
		this.TryCompleteAscent();
	}

	// Token: 0x0600042D RID: 1069 RVA: 0x0001A9F0 File Offset: 0x00018BF0
	private void TryCompleteAscent()
	{
		if (RunSettings.blockingAchievements)
		{
			return;
		}
		int i;
		if (this.GetSteamStatInt(STEAMSTATTYPE.MaxAscent, out i))
		{
			if (Ascents.currentAscent >= i)
			{
				while (i <= Ascents.currentAscent)
				{
					Debug.Log("Completed Ascent: " + i.ToString());
					if (!this.runBasedValueData.completedAscentsThisRun.Contains(i))
					{
						this.runBasedValueData.completedAscentsThisRun.Add(i);
						Player.localPlayer.OnAchievementProgressChanged();
					}
					i++;
				}
				this.SetSteamStat(STEAMSTATTYPE.MaxAscent, Ascents.currentAscent + 1);
			}
			this.TestAscentAchievements(i);
		}
	}

	// Token: 0x0600042E RID: 1070 RVA: 0x0001AA84 File Offset: 0x00018C84
	private void AddToRunBasedFruitsEaten(ushort value)
	{
		if (this.runBasedValueData.runBasedFruitsEaten.Contains(value))
		{
			return;
		}
		this.runBasedValueData.runBasedFruitsEaten.Add(value);
		Player.localPlayer.OnAchievementProgressChanged();
		if (this.runBasedValueData.runBasedFruitsEaten.Count >= 5)
		{
			this.ThrowAchievement(ACHIEVEMENTTYPE.ForagingBadge);
		}
	}

	// Token: 0x0600042F RID: 1071 RVA: 0x0001AADC File Offset: 0x00018CDC
	private void AddToShroomBerriesEaten(ushort value)
	{
		if (!this.runBasedValueData.shroomBerriesEaten.Contains(value))
		{
			this.runBasedValueData.shroomBerriesEaten.Add(value);
			Player.localPlayer.OnAchievementProgressChanged();
			if (this.runBasedValueData.shroomBerriesEaten.Count >= 5)
			{
				this.ThrowAchievement(ACHIEVEMENTTYPE.AdvancedMycologyBadge);
			}
		}
	}

	// Token: 0x06000430 RID: 1072 RVA: 0x0001AB34 File Offset: 0x00018D34
	private void AddToNonToxicMushroomsEaten(ushort value)
	{
		Debug.Log("Local player ate non toxic mushroom: " + value.ToString());
		if (!this.runBasedValueData.nonToxicMushroomsEaten.Contains(value))
		{
			this.runBasedValueData.nonToxicMushroomsEaten.Add(value);
			Player.localPlayer.OnAchievementProgressChanged();
			if (this.runBasedValueData.nonToxicMushroomsEaten.Count >= 4)
			{
				this.ThrowAchievement(ACHIEVEMENTTYPE.MycologyBadge);
			}
		}
	}

	// Token: 0x06000431 RID: 1073 RVA: 0x0001ABA0 File Offset: 0x00018DA0
	private void AddToGourmandRequirementsEaten(ushort value)
	{
		if (!this.runBasedValueData.gourmandRequirementsEaten.Contains(value))
		{
			Debug.Log("ATE GOURMAND REQUIREMENT: " + value.ToString());
			this.runBasedValueData.gourmandRequirementsEaten.Add(value);
		}
	}

	// Token: 0x06000432 RID: 1074 RVA: 0x0001ABDC File Offset: 0x00018DDC
	internal void RecordMaxHeight(int meters)
	{
		if (meters < 25)
		{
			return;
		}
		int runBasedInt = this.GetRunBasedInt(RUNBASEDVALUETYPE.MaxHeightReached);
		if (meters >= runBasedInt + 5)
		{
			this.IncrementSteamStat(STEAMSTATTYPE.HeightClimbed, meters - runBasedInt);
			this.SetRunBasedInt(RUNBASEDVALUETYPE.MaxHeightReached, meters);
		}
	}

	// Token: 0x06000433 RID: 1075 RVA: 0x0001AC14 File Offset: 0x00018E14
	internal void TestAscentAchievements(int maxAscent = -1)
	{
		if (maxAscent == -1)
		{
			this.GetSteamStatInt(STEAMSTATTYPE.MaxAscent, out maxAscent);
		}
		int num = 0;
		while (num <= maxAscent - 2 && num < this.ascentAchievements.Length)
		{
			this.ThrowAchievement(this.ascentAchievements[num]);
			num++;
		}
	}

	// Token: 0x06000434 RID: 1076 RVA: 0x0001AC57 File Offset: 0x00018E57
	public void TestPlayerDied(Character c)
	{
		if (c.IsLocal)
		{
			this.SetRunBasedInt(RUNBASEDVALUETYPE.BitByZombie, 0);
		}
	}

	// Token: 0x06000435 RID: 1077 RVA: 0x0001AC6C File Offset: 0x00018E6C
	public void AddStatusBlockedByMilk(float amount)
	{
		int num = 0;
		this.statusBlockedByMilk += amount;
		while (this.statusBlockedByMilk >= 0.01f)
		{
			num++;
			this.statusBlockedByMilk -= 0.01f;
		}
		if (num > 0)
		{
			int num2 = this.IncrementSteamStat(STEAMSTATTYPE.DamageBlockedByMilk, num);
			Debug.Log(string.Format("Total damage blocked by milk: {0}", num2));
		}
	}

	// Token: 0x04000491 RID: 1169
	public List<AchievementData> allAchievements;

	// Token: 0x04000492 RID: 1170
	[SerializeField]
	private ACHIEVEMENTTYPE debugAchievement;

	// Token: 0x04000493 RID: 1171
	public const int MAX_GUIDEBOOK_PAGES = 8;

	// Token: 0x04000494 RID: 1172
	public const int DISC_THROW_DISTANCE_REQUIREMENT = 100;

	// Token: 0x04000495 RID: 1173
	public const float MAX_MESA_HEAT_PERCENTAGE = 0.1f;

	// Token: 0x04000496 RID: 1174
	public const float MAX_ALPINE_COLD_PERCENTAGE = 0.2f;

	// Token: 0x04000497 RID: 1175
	public const float MAX_ROOTS_SPORES_PERCENTAGE = 0.25f;

	// Token: 0x04000498 RID: 1176
	public const float DISTANCE_FOR_RESCUE_HOOK_ACHIEVEMENT = 30f;

	// Token: 0x04000499 RID: 1177
	private static bool s_gotStats;

	// Token: 0x0400049A RID: 1178
	private List<AchievementManager.RunBasedAchievementData> runBasedAchievements = new List<AchievementManager.RunBasedAchievementData>();

	// Token: 0x0400049B RID: 1179
	private List<AchievementManager.SteamStatBasedAchievementData> steamStatBasedAchievements = new List<AchievementManager.SteamStatBasedAchievementData>();

	// Token: 0x0400049D RID: 1181
	public bool useDebugAscent;

	// Token: 0x0400049E RID: 1182
	public int debugAscent;

	// Token: 0x0400049F RID: 1183
	public const int TOTAL_GUIDEBOOK_PAGES = 8;

	// Token: 0x040004A0 RID: 1184
	public const string STEAMSTAT_GUIDEBOOK_PREFIX = "ReadGuidebookPage_";

	// Token: 0x040004A1 RID: 1185
	private const float ONE_HOUR_IN_SECONDS = 3600f;

	// Token: 0x040004A2 RID: 1186
	private const int FRUITSNEEDEDFORACHIEVEMENT = 5;

	// Token: 0x040004A3 RID: 1187
	private const int MUSHROOMSNEEDEDFORACHIEVEMENT = 4;

	// Token: 0x040004A4 RID: 1188
	private const int SHROOMBERRIESNEEDEDFORACHIEVEMENT = 5;

	// Token: 0x040004A5 RID: 1189
	private ACHIEVEMENTTYPE[] ascentAchievements = new ACHIEVEMENTTYPE[]
	{
		ACHIEVEMENTTYPE.Ascent1,
		ACHIEVEMENTTYPE.Ascent2,
		ACHIEVEMENTTYPE.Ascent3,
		ACHIEVEMENTTYPE.Ascent4,
		ACHIEVEMENTTYPE.Ascent5,
		ACHIEVEMENTTYPE.Ascent6,
		ACHIEVEMENTTYPE.Ascent7
	};

	// Token: 0x040004A6 RID: 1190
	private float statusBlockedByMilk;

	// Token: 0x02000429 RID: 1065
	private class SteamStatBasedAchievementData
	{
		// Token: 0x06001BAB RID: 7083 RVA: 0x000859A3 File Offset: 0x00083BA3
		public SteamStatBasedAchievementData(ACHIEVEMENTTYPE achievementType, STEAMSTATTYPE statType, int requiredValue)
		{
			this.achievementType = achievementType;
			this.statType = statType;
			this.requiredValue = requiredValue;
		}

		// Token: 0x06001BAC RID: 7084 RVA: 0x000859C0 File Offset: 0x00083BC0
		public bool IsAchieved()
		{
			int num;
			return SteamUserStats.GetStat(this.statType.ToString(), out num) && num >= this.requiredValue;
		}

		// Token: 0x04001854 RID: 6228
		public ACHIEVEMENTTYPE achievementType;

		// Token: 0x04001855 RID: 6229
		public STEAMSTATTYPE statType;

		// Token: 0x04001856 RID: 6230
		public int requiredValue;
	}

	// Token: 0x0200042A RID: 1066
	private class RunBasedAchievementData
	{
		// Token: 0x06001BAD RID: 7085 RVA: 0x000859F5 File Offset: 0x00083BF5
		public RunBasedAchievementData(ACHIEVEMENTTYPE achievementType, RUNBASEDVALUETYPE valueType, int requiredValue)
		{
			this.achievementType = achievementType;
			this.valueType = valueType;
			this.requiredValue = requiredValue;
		}

		// Token: 0x06001BAE RID: 7086 RVA: 0x00085A14 File Offset: 0x00083C14
		public bool IsAchieved()
		{
			try
			{
				bool result = false;
				if (Singleton<AchievementManager>.Instance.GetRunBasedFloat(this.valueType) >= (float)this.requiredValue)
				{
					result = true;
				}
				if ((float)Singleton<AchievementManager>.Instance.GetRunBasedInt(this.valueType) >= (float)this.requiredValue)
				{
					result = true;
				}
				return result;
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
			return false;
		}

		// Token: 0x04001857 RID: 6231
		public ACHIEVEMENTTYPE achievementType;

		// Token: 0x04001858 RID: 6232
		public RUNBASEDVALUETYPE valueType;

		// Token: 0x04001859 RID: 6233
		public int requiredValue;
	}
}

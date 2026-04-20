using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Peak;
using Peak.Network;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.CLI;
using Zorro.PhotonUtility;

// Token: 0x0200013C RID: 316
public class MapHandler : Singleton<MapHandler>
{
	// Token: 0x170000AC RID: 172
	// (get) Token: 0x06000A49 RID: 2633 RVA: 0x00036B98 File Offset: 0x00034D98
	// (set) Token: 0x06000A4A RID: 2634 RVA: 0x00036BA0 File Offset: 0x00034DA0
	public int LastRevivedSegment
	{
		get
		{
			return this._lastRevivedSegment;
		}
		private set
		{
			Debug.Log(string.Format("Revive segment updated to {0}", value));
			this._lastRevivedSegment = value;
		}
	}

	// Token: 0x170000AD RID: 173
	// (get) Token: 0x06000A4B RID: 2635 RVA: 0x00036BBE File Offset: 0x00034DBE
	public static bool Exists
	{
		get
		{
			return Singleton<MapHandler>.Instance != null;
		}
	}

	// Token: 0x170000AE RID: 174
	// (get) Token: 0x06000A4C RID: 2636 RVA: 0x00036BCB File Offset: 0x00034DCB
	public static bool ExistsAndInitialized
	{
		get
		{
			return MapHandler.Exists && Singleton<MapHandler>.Instance.hasFinishedStartRoutine;
		}
	}

	// Token: 0x170000AF RID: 175
	// (get) Token: 0x06000A4D RID: 2637 RVA: 0x00036BE0 File Offset: 0x00034DE0
	public static MapHandler.MapSegment CurrentMapSegment
	{
		get
		{
			return Singleton<MapHandler>.Instance.segments[Singleton<MapHandler>.Instance.currentSegment];
		}
	}

	// Token: 0x170000B0 RID: 176
	// (get) Token: 0x06000A4E RID: 2638 RVA: 0x00036BF7 File Offset: 0x00034DF7
	public static bool LastSeenCampfireIsSafe
	{
		get
		{
			if (Singleton<MapHandler>.Instance.currentSegment < 4)
			{
				return !OrbFogHandler.IsFoggingCurrentSegment || MapHandler.CurrentCampfire.EveryoneInRange();
			}
			return !Singleton<LavaRising>.Instance.started;
		}
	}

	// Token: 0x170000B1 RID: 177
	// (get) Token: 0x06000A4F RID: 2639 RVA: 0x00036C27 File Offset: 0x00034E27
	private static bool PreviousSegmentIsStillBaseCamp
	{
		get
		{
			return Singleton<MapHandler>.Instance.currentSegment != 0 && (Singleton<MapHandler>.Instance.currentSegment == 4 || !MapHandler.CurrentCampfire.EveryoneInRange());
		}
	}

	// Token: 0x170000B2 RID: 178
	// (get) Token: 0x06000A50 RID: 2640 RVA: 0x00036C53 File Offset: 0x00034E53
	public static RespawnChest BaseCampScoutStatue
	{
		get
		{
			if (!MapHandler.PreviousSegmentIsStillBaseCamp)
			{
				return MapHandler.CurrentScoutStatue;
			}
			return MapHandler.PreviousScoutStatue;
		}
	}

	// Token: 0x170000B3 RID: 179
	// (get) Token: 0x06000A51 RID: 2641 RVA: 0x00036C67 File Offset: 0x00034E67
	public static bool BaseCampHasRevived
	{
		get
		{
			return Singleton<MapHandler>.Instance.currentSegment == Singleton<MapHandler>.Instance.LastRevivedSegment || (MapHandler.PreviousSegmentIsStillBaseCamp && Singleton<MapHandler>.Instance.LastRevivedSegment == Singleton<MapHandler>.Instance.currentSegment - 1);
		}
	}

	// Token: 0x170000B4 RID: 180
	// (get) Token: 0x06000A52 RID: 2642 RVA: 0x00036CA2 File Offset: 0x00034EA2
	public static Transform CurrentBaseCampSpawnPoint
	{
		get
		{
			if (MapHandler.PreviousSegmentIsStillBaseCamp)
			{
				return MapHandler.PreviousCampfire.transform;
			}
			if (Singleton<MapHandler>.Instance.GetCurrentSegment() != Segment.Beach)
			{
				return MapHandler.CurrentCampfire.transform;
			}
			return SpawnPoint.LocalSpawnPoint.transform;
		}
	}

	// Token: 0x06000A53 RID: 2643 RVA: 0x00036CD7 File Offset: 0x00034ED7
	public static GameObject GetCampfireRoot(int segmentIndex)
	{
		if (segmentIndex >= 0)
		{
			return Singleton<MapHandler>.Instance.segments[segmentIndex].segmentCampfire;
		}
		return null;
	}

	// Token: 0x170000B5 RID: 181
	// (get) Token: 0x06000A54 RID: 2644 RVA: 0x00036CF0 File Offset: 0x00034EF0
	public static Campfire PreviousCampfire
	{
		get
		{
			if (Singleton<MapHandler>.Instance.currentSegment != 0)
			{
				return MapHandler.GetCampfireRoot(Singleton<MapHandler>.Instance.currentSegment - 1).GetComponentInChildren<Campfire>(true);
			}
			return null;
		}
	}

	// Token: 0x170000B6 RID: 182
	// (get) Token: 0x06000A55 RID: 2645 RVA: 0x00036D18 File Offset: 0x00034F18
	public static RespawnChest PreviousScoutStatue
	{
		get
		{
			if (Singleton<MapHandler>.Instance.currentSegment == 0)
			{
				return null;
			}
			if (Singleton<MapHandler>.Instance.GetCurrentSegment() != Segment.TheKiln)
			{
				return MapHandler.GetCampfireRoot(Singleton<MapHandler>.Instance.currentSegment - 1).GetComponentInChildren<RespawnChest>();
			}
			return Singleton<MapHandler>.Instance.segments[Singleton<MapHandler>.Instance.currentSegment - 1].segmentParent.GetComponentInChildren<RespawnChest>(true);
		}
	}

	// Token: 0x170000B7 RID: 183
	// (get) Token: 0x06000A56 RID: 2646 RVA: 0x00036D79 File Offset: 0x00034F79
	public static Campfire CurrentCampfire
	{
		get
		{
			return MapHandler.GetCampfireRoot(Singleton<MapHandler>.Instance.currentSegment).GetComponentInChildren<Campfire>();
		}
	}

	// Token: 0x170000B8 RID: 184
	// (get) Token: 0x06000A57 RID: 2647 RVA: 0x00036D90 File Offset: 0x00034F90
	public static RespawnChest CurrentScoutStatue
	{
		get
		{
			if (Singleton<MapHandler>.Instance.GetCurrentSegment() > Segment.Caldera)
			{
				return null;
			}
			if (Singleton<MapHandler>.Instance.GetCurrentSegment() != Segment.Caldera)
			{
				return MapHandler.GetCampfireRoot(Singleton<MapHandler>.Instance.currentSegment).GetComponentInChildren<RespawnChest>();
			}
			return Singleton<MapHandler>.Instance.segments[Singleton<MapHandler>.Instance.currentSegment].segmentParent.GetComponentInChildren<RespawnChest>();
		}
	}

	// Token: 0x06000A58 RID: 2648 RVA: 0x00036DED File Offset: 0x00034FED
	protected override void Awake()
	{
		this.hasFinishedStartRoutine = false;
		base.Awake();
		this.debugCommandHandle = CustomCommands<CustomCommandType>.RegisterListener<SyncMapHandlerDebugCommandPackage>(new Action<SyncMapHandlerDebugCommandPackage>(this.OnPackageHandle));
		if (Application.isEditor)
		{
			this.DetectBiomes();
		}
	}

	// Token: 0x06000A59 RID: 2649 RVA: 0x00036E20 File Offset: 0x00035020
	public override void OnDestroy()
	{
		base.OnDestroy();
		CustomCommands<CustomCommandType>.UnregisterListener(this.debugCommandHandle);
	}

	// Token: 0x06000A5A RID: 2650 RVA: 0x00036E34 File Offset: 0x00035034
	private static void EnsureSpawnTrackersAttached()
	{
		if (Singleton<MapHandler>.Instance._spawnTrackers != null)
		{
			return;
		}
		Singleton<MapHandler>.Instance._spawnTrackers = new List<SpawnedItemTracker>();
		MapHandler.MapSegment[] array = Singleton<MapHandler>.Instance.segments;
		for (int i = 0; i < array.Length; i++)
		{
			if (!(array[i].segmentCampfire == null))
			{
				IEnumerable componentsInChildren = array[i].segmentCampfire.GetComponentsInChildren<ISpawner>(true);
				int num = 0;
				foreach (Component component in componentsInChildren.Cast<Component>())
				{
					num++;
					SpawnedItemTracker spawnedItemTracker = component.gameObject.AddComponent<SpawnedItemTracker>();
					spawnedItemTracker.Init();
					Singleton<MapHandler>.Instance._spawnTrackers.Add(spawnedItemTracker);
				}
				if (num > 0)
				{
					Debug.Log(string.Format("Attached {0} spawn trackers under {1}", num, array[i].segmentCampfire.name));
				}
			}
		}
		foreach (SingleItemSpawner singleItemSpawner in Object.FindObjectsByType<SingleItemSpawner>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID))
		{
			if (singleItemSpawner.name.Contains("Backpack"))
			{
				Debug.Log("Tracking " + singleItemSpawner.name + ".");
				SpawnedItemTracker spawnedItemTracker2 = singleItemSpawner.gameObject.AddComponent<SpawnedItemTracker>();
				spawnedItemTracker2.Init();
				Singleton<MapHandler>.Instance._spawnTrackers.Add(spawnedItemTracker2);
			}
		}
	}

	// Token: 0x170000B9 RID: 185
	// (get) Token: 0x06000A5B RID: 2651 RVA: 0x00036F9C File Offset: 0x0003519C
	private static List<SpawnedItemTracker> SpawnTrackers
	{
		get
		{
			MapHandler.EnsureSpawnTrackersAttached();
			return Singleton<MapHandler>.Instance._spawnTrackers;
		}
	}

	// Token: 0x06000A5C RID: 2652 RVA: 0x00036FB0 File Offset: 0x000351B0
	public static void PopulateSpawnHistories(Dictionary<Hash128, List<SpawnedItemTracker.SpawnRecord>> spawnRecords)
	{
		Debug.Log(string.Format("Found {0} spawn trackers with {1} in history.", MapHandler.SpawnTrackers.Count, spawnRecords.Count));
		foreach (SpawnedItemTracker spawnedItemTracker in MapHandler.SpawnTrackers)
		{
			if (spawnRecords.ContainsKey(spawnedItemTracker.SpawnerId))
			{
				spawnedItemTracker.PopulateHistory(spawnRecords[spawnedItemTracker.SpawnerId]);
			}
		}
	}

	// Token: 0x06000A5D RID: 2653 RVA: 0x00037044 File Offset: 0x00035244
	public static Luggage[] GetBaseCampLuggage(Segment segment)
	{
		if (segment >= Segment.Tropics && segment <= Segment.TheKiln)
		{
			return MapHandler.GetCampfireRoot((int)(segment - Segment.Tropics)).GetComponentsInChildren<Luggage>();
		}
		return new Luggage[0];
	}

	// Token: 0x06000A5E RID: 2654 RVA: 0x00037064 File Offset: 0x00035264
	public static void OpenBaseCampLuggage(Segment segment, List<int> openLuggageIds)
	{
		Luggage[] baseCampLuggage = MapHandler.GetBaseCampLuggage(segment);
		if (openLuggageIds.Count > 0)
		{
			Debug.Log(string.Format("Iterating through {0} luggage objects and opening {1} luggages.", baseCampLuggage.Length, openLuggageIds.Count));
		}
		foreach (Luggage luggage in baseCampLuggage)
		{
			PhotonView photonView;
			if (luggage.TryGetComponent<PhotonView>(out photonView) && openLuggageIds.Contains(photonView.ViewID))
			{
				Debug.Log("Opening " + luggage.name + " because it was in save.");
				luggage.OpenImmediatelyNoNotify();
			}
		}
	}

	// Token: 0x06000A5F RID: 2655 RVA: 0x000370F0 File Offset: 0x000352F0
	public static void InitializeMap()
	{
		MapHandler.EnsureSpawnTrackersAttached();
		MapHandler.CurrentScoutStatue.SegmentNumber = Segment.Beach;
		MapHandler.CurrentScoutStatue.ReviveUsed += delegate(RespawnChest statue)
		{
			Singleton<MapHandler>.Instance.LastRevivedSegment = (int)statue.SegmentNumber;
		};
		MapHandler.MapSegment[] array = Singleton<MapHandler>.Instance.segments;
		for (int i = 1; i < array.Length; i++)
		{
			array[i].segmentParent.SetActive(false);
			if (array[i].segmentCampfire != null)
			{
				array[i].segmentCampfire.SetActive(false);
			}
			Debug.Log(string.Format("Disabling segment: {0} with parent: {1}", i, array[i].segmentParent.name));
		}
		array[0].wallNext.SetActive(true);
		Singleton<MapHandler>.Instance.hasFinishedStartRoutine = true;
	}

	// Token: 0x06000A60 RID: 2656 RVA: 0x000371B8 File Offset: 0x000353B8
	internal void DetectBiomes()
	{
		this.biomes.Clear();
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			for (int j = 0; j < child.childCount; j++)
			{
				Biome biome;
				if (child.GetChild(j).gameObject.activeInHierarchy && child.GetChild(j).TryGetComponent<Biome>(out biome))
				{
					this.biomes.Add(biome.biomeType);
				}
			}
		}
	}

	// Token: 0x06000A61 RID: 2657 RVA: 0x00037238 File Offset: 0x00035438
	private void Update()
	{
		if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient && this.hasFinishedStartRoutine && !this.hasSpawnedInitialSpawners)
		{
			ISpawner[] componentsInChildren = this.segments[0].segmentParent.GetComponentsInChildren<ISpawner>();
			ISpawner[] componentsInChildren2 = this.globalParent.GetComponentsInChildren<ISpawner>();
			this.hasSpawnedInitialSpawners = true;
			foreach (ISpawner spawner in componentsInChildren)
			{
				this.viewsToDestoryIfNotAlreadyWhenSwitchingSegments.AddRange(spawner.TrySpawnItems());
			}
			MapHandler.SpawnCampfireItems(this.segments[0].segmentCampfire, false);
			ISpawner[] array = componentsInChildren2;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].TrySpawnItems();
			}
		}
		else if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
		{
			this.hasSpawnedInitialSpawners = true;
		}
		bool flag = false;
		global::Player[] array2 = PlayerHandler.GetAllPlayers().ToArray<global::Player>();
		int num = array2.Length;
		if (array2.Length > 4)
		{
			num = array2.Length / 2;
		}
		else if (array2.Length == 4)
		{
			num = 3;
		}
		else if (array2.Length == 3)
		{
			num = 2;
		}
		if (array2.Count((global::Player player) => player.hasClosedEndScreen) >= num)
		{
			flag = true;
		}
		EndScreenStatus endScreenStatus;
		if (flag && array2.Length != 0 && !GameHandler.TryGetStatus<EndScreenStatus>(out endScreenStatus) && !this.hasEnded)
		{
			bool flag2 = Character.localCharacter.refs.stats.won || Character.localCharacter.refs.stats.somebodyElseWon;
			this.hasEnded = true;
			if (flag2 && !RunSettings.isMiniRun)
			{
				GameHandler.AddStatus<EndScreenStatus>(new EndScreenStatus());
				Debug.LogError("Load credits");
				Singleton<PeakHandler>.Instance.EndScreenComplete();
			}
			else
			{
				Debug.LogError("Everyone has closed end screen.. Loading airport");
				Singleton<GameOverHandler>.Instance.LoadAirport();
			}
		}
		bool flag3 = false;
		global::Player[] array3 = array2;
		for (int i = 0; i < array3.Length; i++)
		{
			if (array3[i].doneWithCutscene)
			{
				flag3 = true;
				break;
			}
		}
		if (flag3 && array2.Length != 0 && !this.hasCutsceneEnded)
		{
			this.hasCutsceneEnded = true;
			Debug.Log("Everyone is done with cutscene, loading airport");
			GameHandler.AddStatus<SceneSwitchingStatus>(new SceneSwitchingStatus());
			RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Basic, null, new IEnumerator[]
			{
				RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess("Airport", true, true, 1f)
			});
		}
	}

	// Token: 0x06000A62 RID: 2658 RVA: 0x00037479 File Offset: 0x00035679
	private static void ActivateWithoutMessageQueue(GameObject heavyGameObject)
	{
		PhotonNetwork.IsMessageQueueRunning = false;
		heavyGameObject.SetActive(true);
		PhotonNetwork.IsMessageQueueRunning = true;
	}

	// Token: 0x06000A63 RID: 2659 RVA: 0x0003748E File Offset: 0x0003568E
	private void ActivateCurrentSegment()
	{
		MapHandler.ActivateWithoutMessageQueue(this.segments[this.currentSegment].segmentParent);
	}

	// Token: 0x06000A64 RID: 2660 RVA: 0x000374A8 File Offset: 0x000356A8
	public void GoToSegment(Segment s)
	{
		MapHandler.<>c__DisplayClass58_0 CS$<>8__locals1 = new MapHandler.<>c__DisplayClass58_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.s = s;
		if ((int)CS$<>8__locals1.s <= this.currentSegment)
		{
			Debug.LogError(string.Format("Trying to transition to segment already passed: {0}", CS$<>8__locals1.s));
			return;
		}
		Debug.Log(string.Format("Going to segment: {0}", CS$<>8__locals1.s));
		base.StartCoroutine(CS$<>8__locals1.<GoToSegment>g__ShowNextSegmentCoroutine|0());
	}

	// Token: 0x06000A65 RID: 2661 RVA: 0x0003751C File Offset: 0x0003571C
	[ConsoleCommand]
	public static void JumpToSegment(Segment segment)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			GameUtils.instance.photonView.RPC("RPC_WarpToSegment", RpcTarget.MasterClient, new object[]
			{
				(int)segment
			});
			return;
		}
		MapHandler.JumpToSegmentLogic(segment, (from player in PlayerHandler.GetAllPlayers()
		select player.photonView.Owner.ActorNumber).ToHashSet<int>(), !NetCode.Session.IsOffline, true);
		Campfire previousCampfire = MapHandler.PreviousCampfire;
		if (previousCampfire == null)
		{
			return;
		}
		previousCampfire.LightWithoutReveal();
	}

	// Token: 0x06000A66 RID: 2662 RVA: 0x000375A8 File Offset: 0x000357A8
	public static void SetSegmentOnSpawn(Segment segment, int lastRevivedSegment)
	{
		Singleton<MapHandler>.Instance.LastRevivedSegment = lastRevivedSegment;
		if (segment < Segment.TheKiln && (Singleton<OrbFogHandler>.Instance == null || Singleton<OrbFogHandler>.Instance.currentID != (int)segment))
		{
			Debug.LogError("Uh oh! The fog handler needs to be initialized before we spawn or the spawn will be broken!!!");
		}
		if (segment == Segment.Beach)
		{
			return;
		}
		MapHandler.JumpToSegmentLogic(segment, new HashSet<int>
		{
			NetCode.Session.SeatNumber
		}, false, false);
	}

	// Token: 0x06000A67 RID: 2663 RVA: 0x0003760C File Offset: 0x0003580C
	private static bool IsMallowSpawner(ISpawner spawner)
	{
		BerryBush berryBush = spawner as BerryBush;
		return berryBush != null && berryBush.spawnPool == SpawnPool.Campfire;
	}

	// Token: 0x06000A68 RID: 2664 RVA: 0x00037634 File Offset: 0x00035834
	private static void SpawnCampfireItems(GameObject campfireRoot, bool skipMallows = false)
	{
		Debug.Log("Spawning campfire items for " + campfireRoot.gameObject.name);
		foreach (ISpawner spawner in campfireRoot.GetComponentsInChildren<ISpawner>())
		{
			if ((!skipMallows || !MapHandler.IsMallowSpawner(spawner)) && !(spawner is Luggage))
			{
				spawner.TrySpawnItems();
			}
		}
	}

	// Token: 0x06000A69 RID: 2665 RVA: 0x00037690 File Offset: 0x00035890
	private static void JumpToSegmentLogic(Segment segment, HashSet<int> playersToTeleport, bool sendToEveryone, bool updateFog = false)
	{
		Debug.Log(string.Format("Jumping to beginning of segment: {0}", segment));
		foreach (MapHandler.MapSegment mapSegment in Singleton<MapHandler>.Instance.segments)
		{
			mapSegment.segmentParent.SetActive(false);
			if (mapSegment.segmentCampfire)
			{
				mapSegment.segmentCampfire.SetActive(false);
			}
			if (mapSegment.wallNext)
			{
				mapSegment.wallNext.gameObject.SetActive(false);
			}
			if (mapSegment.wallPrevious)
			{
				mapSegment.wallPrevious.gameObject.SetActive(false);
			}
		}
		int num = Singleton<MapHandler>.Instance.currentSegment;
		Singleton<MapHandler>.Instance.currentSegment = (int)segment;
		int num2 = Singleton<MapHandler>.Instance.currentSegment;
		if (segment == Segment.Peak)
		{
			num2--;
		}
		MapHandler.MapSegment mapSegment2 = Singleton<MapHandler>.Instance.segments[num2];
		Debug.Log(string.Format("Setting up segment walls for {0}", mapSegment2));
		MapHandler.ActivateWithoutMessageQueue(mapSegment2.segmentParent);
		if (mapSegment2.segmentCampfire)
		{
			mapSegment2.segmentCampfire.SetActive(true);
		}
		if (mapSegment2.wallNext)
		{
			mapSegment2.wallNext.gameObject.SetActive(true);
		}
		if (mapSegment2.wallPrevious)
		{
			mapSegment2.wallPrevious.gameObject.SetActive(true);
		}
		Vector3 vector = MapHandler.CurrentBaseCampSpawnPoint.position + CharacterSpawner.RandomBaseCampOffset;
		if (segment == Segment.Peak)
		{
			vector = Singleton<MapHandler>.Instance.respawnThePeak.position;
		}
		if (num2 > 0)
		{
			MapHandler.MapSegment mapSegment3 = Singleton<MapHandler>.Instance.segments[num2 - 1];
			if (mapSegment3.segmentCampfire != null)
			{
				mapSegment3.segmentCampfire.SetActive(true);
			}
		}
		if (PhotonNetwork.IsMasterClient)
		{
			List<ISpawner> list = mapSegment2.segmentParent.GetComponentsInChildren<ISpawner>().ToList<ISpawner>();
			Debug.Log(string.Format("Spawning items in {0} from {1} spawners. ", segment, list.Count) + "Parent: " + mapSegment2.segmentParent.gameObject.name);
			int num3 = 0;
			if (segment == Segment.TheKiln)
			{
				list.AddRange(Singleton<PeakHandler>.Instance.gameObject.GetComponentsInChildren<ISpawner>());
			}
			foreach (ISpawner spawner in list)
			{
				spawner.TrySpawnItems();
				num3++;
			}
			if (mapSegment2.segmentCampfire)
			{
				MapHandler.SpawnCampfireItems(mapSegment2.segmentCampfire, false);
				if (num2 > 1)
				{
					if (num != num2 - 1)
					{
						MapHandler.SpawnCampfireItems(Singleton<MapHandler>.Instance.segments[num2 - 1].segmentCampfire, RunSettings.isMiniRun);
					}
					if (RunSettings.isMiniRun)
					{
						MapHandler.PreviousScoutStatue.Break();
					}
				}
			}
			else
			{
				Debug.Log("NO CAMPFIRE SEGMENT");
			}
		}
		if (updateFog)
		{
			Debug.Log(string.Format("Moving fog to index {0}", num2));
			Singleton<OrbFogHandler>.Instance.SetFogOrigin(num2);
		}
		MountainProgressHandler.JumpToSegment(num2, 5f);
		if (mapSegment2.dayNightProfile != null)
		{
			DayNightManager.instance.BlendProfiles(mapSegment2.dayNightProfile);
		}
		if (PhotonNetwork.IsMasterClient)
		{
			List<Character> allPlayerCharacters = PlayerHandler.GetAllPlayerCharacters();
			Debug.Log(string.Format("Teleporting {0} out of {1} players to {2} campfire..", playersToTeleport.Count, allPlayerCharacters.Count, segment));
			foreach (Character character in allPlayerCharacters)
			{
				if (playersToTeleport.Contains(character.photonView.Owner.ActorNumber))
				{
					character.photonView.RPC("WarpPlayerRPC", RpcTarget.All, new object[]
					{
						vector,
						false
					});
				}
			}
		}
		if (sendToEveryone)
		{
			CustomCommands<CustomCommandType>.SendPackage(new SyncMapHandlerDebugCommandPackage(segment, Array.Empty<int>()), ReceiverGroup.Others);
		}
		if (MapHandler.CurrentScoutStatue != null)
		{
			MapHandler.CurrentScoutStatue.SegmentNumber = MapHandler.CurrentSegmentNumber;
			MapHandler.CurrentScoutStatue.ReviveUsed += delegate(RespawnChest statue)
			{
				Singleton<MapHandler>.Instance.LastRevivedSegment = (int)statue.SegmentNumber;
			};
		}
	}

	// Token: 0x06000A6A RID: 2666 RVA: 0x00037AB8 File Offset: 0x00035CB8
	private void OnPackageHandle(SyncMapHandlerDebugCommandPackage p)
	{
		Debug.Log(string.Format("Sync map debug package received! Jumping {0} players to {1}", p.PlayerToTeleport.Length, p.Segment));
		MapHandler.JumpToSegmentLogic(p.Segment, p.PlayerToTeleport.ToHashSet<int>(), false, true);
	}

	// Token: 0x170000BA RID: 186
	// (get) Token: 0x06000A6B RID: 2667 RVA: 0x00037B04 File Offset: 0x00035D04
	public static Segment CurrentSegmentNumber
	{
		get
		{
			return (Segment)Singleton<MapHandler>.Instance.currentSegment;
		}
	}

	// Token: 0x06000A6C RID: 2668 RVA: 0x00037B11 File Offset: 0x00035D11
	public Segment GetCurrentSegment()
	{
		return (Segment)this.currentSegment;
	}

	// Token: 0x06000A6D RID: 2669 RVA: 0x00037B1A File Offset: 0x00035D1A
	public static Biome.BiomeType GetBiomeForSegment(int segmentIdx)
	{
		return Singleton<MapHandler>.Instance.segments[segmentIdx].biome;
	}

	// Token: 0x06000A6E RID: 2670 RVA: 0x00037B2D File Offset: 0x00035D2D
	public Biome.BiomeType GetCurrentBiome()
	{
		return this.segments[this.currentSegment].biome;
	}

	// Token: 0x06000A6F RID: 2671 RVA: 0x00037B41 File Offset: 0x00035D41
	public bool BiomeIsPresent(Biome.BiomeType biomeType)
	{
		return this.biomes.Contains(biomeType);
	}

	// Token: 0x06000A70 RID: 2672 RVA: 0x00037B50 File Offset: 0x00035D50
	public MapHandler.MapSegment GetVariantSegmentFromBiome(Biome.BiomeType biome)
	{
		for (int i = 0; i < this.variantSegments.Length; i++)
		{
			if (this.variantSegments[i].biome == biome)
			{
				return this.variantSegments[i];
			}
		}
		Debug.LogError("COULDNT FIND SEGMENT FROM BIOME. RETURNING SHORE SEGMENT");
		return this.segments[0];
	}

	// Token: 0x0400099F RID: 2463
	public Transform globalParent;

	// Token: 0x040009A0 RID: 2464
	private int _lastRevivedSegment = int.MinValue;

	// Token: 0x040009A1 RID: 2465
	public MapHandler.MapSegment[] segments;

	// Token: 0x040009A2 RID: 2466
	public MapHandler.MapSegment[] variantSegments;

	// Token: 0x040009A3 RID: 2467
	public Transform respawnTheKiln;

	// Token: 0x040009A4 RID: 2468
	public Transform respawnThePeak;

	// Token: 0x040009A5 RID: 2469
	public LavaRising lavaRising;

	// Token: 0x040009A6 RID: 2470
	[SerializeField]
	private int currentSegment;

	// Token: 0x040009A7 RID: 2471
	private bool hasSpawnedInitialSpawners;

	// Token: 0x040009A8 RID: 2472
	private bool hasFinishedStartRoutine;

	// Token: 0x040009A9 RID: 2473
	private ListenerHandle debugCommandHandle;

	// Token: 0x040009AA RID: 2474
	private bool hasEnded;

	// Token: 0x040009AB RID: 2475
	private bool hasCutsceneEnded;

	// Token: 0x040009AC RID: 2476
	private List<PhotonView> viewsToDestoryIfNotAlreadyWhenSwitchingSegments = new List<PhotonView>();

	// Token: 0x040009AD RID: 2477
	public List<Biome.BiomeType> biomes = new List<Biome.BiomeType>();

	// Token: 0x040009AE RID: 2478
	private List<SpawnedItemTracker> _spawnTrackers;

	// Token: 0x02000485 RID: 1157
	[Serializable]
	public class MapSegment
	{
		// Token: 0x1700020F RID: 527
		// (get) Token: 0x06001CB8 RID: 7352 RVA: 0x00088A90 File Offset: 0x00086C90
		public Biome.BiomeType biome
		{
			get
			{
				if (this.hasVariant && Singleton<MapHandler>.Instance.BiomeIsPresent(this.variantBiome))
				{
					return Singleton<MapHandler>.Instance.GetVariantSegmentFromBiome(this.variantBiome).biome;
				}
				return this._biome;
			}
		}

		// Token: 0x17000210 RID: 528
		// (get) Token: 0x06001CB9 RID: 7353 RVA: 0x00088AC8 File Offset: 0x00086CC8
		public GameObject segmentParent
		{
			get
			{
				if (this.hasVariant && Singleton<MapHandler>.Instance.BiomeIsPresent(this.variantBiome))
				{
					return Singleton<MapHandler>.Instance.GetVariantSegmentFromBiome(this.variantBiome).segmentParent;
				}
				return this._segmentParent;
			}
		}

		// Token: 0x17000211 RID: 529
		// (get) Token: 0x06001CBA RID: 7354 RVA: 0x00088B00 File Offset: 0x00086D00
		public GameObject segmentCampfire
		{
			get
			{
				if (this.hasVariant && Singleton<MapHandler>.Instance.BiomeIsPresent(this.variantBiome))
				{
					return Singleton<MapHandler>.Instance.GetVariantSegmentFromBiome(this.variantBiome).segmentCampfire;
				}
				return this._segmentCampfire;
			}
		}

		// Token: 0x17000212 RID: 530
		// (get) Token: 0x06001CBB RID: 7355 RVA: 0x00088B38 File Offset: 0x00086D38
		public DayNightProfile dayNightProfile
		{
			get
			{
				if (this.hasVariant && Singleton<MapHandler>.Instance.BiomeIsPresent(this.variantBiome))
				{
					return Singleton<MapHandler>.Instance.GetVariantSegmentFromBiome(this.variantBiome).dayNightProfile;
				}
				return this._dayNightProfile;
			}
		}

		// Token: 0x040019E0 RID: 6624
		[SerializeField]
		private Biome.BiomeType _biome;

		// Token: 0x040019E1 RID: 6625
		[SerializeField]
		private GameObject _segmentParent;

		// Token: 0x040019E2 RID: 6626
		[SerializeField]
		private GameObject _segmentCampfire;

		// Token: 0x040019E3 RID: 6627
		public GameObject wallNext;

		// Token: 0x040019E4 RID: 6628
		public GameObject wallPrevious;

		// Token: 0x040019E5 RID: 6629
		public Transform reconnectSpawnPos;

		// Token: 0x040019E6 RID: 6630
		[SerializeField]
		private DayNightProfile _dayNightProfile;

		// Token: 0x040019E7 RID: 6631
		public bool hasVariant;

		// Token: 0x040019E8 RID: 6632
		public Biome.BiomeType variantBiome;

		// Token: 0x040019E9 RID: 6633
		public bool isVariant;
	}
}

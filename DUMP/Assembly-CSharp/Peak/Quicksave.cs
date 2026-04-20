using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Peak.Network;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zorro.Core;

namespace Peak
{
	// Token: 0x020003C4 RID: 964
	public static class Quicksave
	{
		// Token: 0x17000182 RID: 386
		// (get) Token: 0x0600197D RID: 6525 RVA: 0x00081404 File Offset: 0x0007F604
		public static Quicksave.RunProgress SavedRun
		{
			get
			{
				return Quicksave._loadedData.run;
			}
		}

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x0600197E RID: 6526 RVA: 0x00081410 File Offset: 0x0007F610
		// (set) Token: 0x0600197F RID: 6527 RVA: 0x00081417 File Offset: 0x0007F617
		public static ReconnectData SavedHostState { get; private set; }

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x06001980 RID: 6528 RVA: 0x0008141F File Offset: 0x0007F61F
		private static string SavePath
		{
			get
			{
				return Path.Join(Application.persistentDataPath, Quicksave.s_SaveName);
			}
		}

		// Token: 0x17000185 RID: 389
		// (get) Token: 0x06001981 RID: 6529 RVA: 0x0008143A File Offset: 0x0007F63A
		public static bool Exists
		{
			get
			{
				return File.Exists(Quicksave.SavePath);
			}
		}

		// Token: 0x17000186 RID: 390
		// (get) Token: 0x06001982 RID: 6530 RVA: 0x00081446 File Offset: 0x0007F646
		private static Quicksave.SaveData LoadedData
		{
			get
			{
				if (!Quicksave._hasLoadedData)
				{
					Quicksave.LoadSaveData();
				}
				return Quicksave._loadedData;
			}
		}

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x06001983 RID: 6531 RVA: 0x00081459 File Offset: 0x0007F659
		public static FileInfo SaveFile
		{
			get
			{
				FileInfo fileInfo = new FileInfo(Quicksave.SavePath);
				DirectoryInfo directory = fileInfo.Directory;
				if (directory == null)
				{
					return fileInfo;
				}
				directory.Create();
				return fileInfo;
			}
		}

		// Token: 0x06001984 RID: 6532 RVA: 0x00081475 File Offset: 0x0007F675
		private static void LoadSaveData()
		{
			Quicksave._loadedData = JsonUtility.FromJson<Quicksave.SaveData>(File.ReadAllText(Quicksave.SaveFile.FullName));
			Quicksave._hasLoadedData = true;
		}

		// Token: 0x06001985 RID: 6533 RVA: 0x00081498 File Offset: 0x0007F698
		public static void SaveNow()
		{
			Debug.Log("Saving game to app data folder as " + Quicksave.s_SaveName);
			try
			{
				Singleton<ReconnectHandler>.Instance.UpdateForAllCharacters();
				string contents = JsonUtility.ToJson(Quicksave.SaveData.Create());
				File.WriteAllText(Quicksave.SaveFile.FullName, contents);
			}
			catch (Exception arg)
			{
				Console.WriteLine(string.Format("Failed to create save file: {0}", arg));
				throw;
			}
		}

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x06001986 RID: 6534 RVA: 0x00081508 File Offset: 0x0007F708
		public static string SavedRunId
		{
			get
			{
				return Quicksave.LoadedData.run.runId;
			}
		}

		// Token: 0x06001987 RID: 6535 RVA: 0x0008151C File Offset: 0x0007F71C
		public static bool TryLoadSave()
		{
			try
			{
				Quicksave.LoadSaveData();
				if (!Quicksave.<TryLoadSave>g__CheckVersion|26_0())
				{
					Debug.LogWarning("Save data was from a previous incompatible version! Have to just destroy it.");
					Quicksave.DestroySaveData();
					return false;
				}
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
				return false;
			}
			return true;
		}

		// Token: 0x06001988 RID: 6536 RVA: 0x00081568 File Offset: 0x0007F768
		private static void PopulateHostData(Quicksave.PlayerRunData hostRecord)
		{
			Singleton<AchievementManager>.Instance.InitRunBasedValues(hostRecord.GetAchievementProgress());
			Quicksave.SavedHostState = hostRecord.GetReconnectData();
			if (!Quicksave.SavedHostState.isValid)
			{
				Debug.LogError("Our quicksave didn't have the host player's state in it! That's messed UP");
			}
			MapHandler.JumpToSegment(Quicksave.SavedRun.biomeReached);
		}

		// Token: 0x06001989 RID: 6537 RVA: 0x000815B8 File Offset: 0x0007F7B8
		public static void PopulateMapAndPlayerStates()
		{
			if (Quicksave.LoadedData.run.LobbyType == Quicksave.LobbyMode.Offline)
			{
				if (Quicksave.LoadedData.playerSaves.Count != 1)
				{
					Debug.LogError(string.Format("We have an offline save but there are {0} player records in it.", Quicksave.LoadedData.playerSaves.Count) + "That's NOT POSSIBLE!!!!");
				}
				Quicksave.PopulateHostData(Quicksave.LoadedData.playerSaves[0]);
				return;
			}
			foreach (Quicksave.PlayerRunData hostRecord in Quicksave._loadedData.playerSaves)
			{
				if (hostRecord.UserId == NetCode.Session.AuthValues.UserId)
				{
					Quicksave.PopulateHostData(hostRecord);
				}
				else
				{
					Singleton<ReconnectHandler>.Instance.PopulateReconnectRecord(hostRecord.UserId, hostRecord.GetReconnectData(), hostRecord.GetAchievementProgress());
				}
			}
		}

		// Token: 0x0600198A RID: 6538 RVA: 0x000816B8 File Offset: 0x0007F8B8
		public static void FinalizeRunSetupAndSelfDestruct()
		{
			RunManager.SetUpFromQuicksave(Guid.Parse(Quicksave.SavedRunId), Quicksave.SavedRun.runTimer);
			DayNightManager.instance.timeOfDay = Quicksave.SavedRun.timeOfDay;
			Quicksave.DestroySaveData();
		}

		// Token: 0x0600198B RID: 6539 RVA: 0x000816EC File Offset: 0x0007F8EC
		public static void DestroySaveData()
		{
			Quicksave.ShouldUseSaveData = false;
			if (File.Exists(Quicksave.SavePath))
			{
				File.Delete(Quicksave.SavePath);
			}
			Quicksave._loadedData = default(Quicksave.SaveData);
			Quicksave._hasLoadedData = false;
		}

		// Token: 0x0600198C RID: 6540 RVA: 0x0008171C File Offset: 0x0007F91C
		public static void LoadSavedGameScene()
		{
			Ascents.currentAscent = Quicksave.LoadedData.run.ascent;
			GameUtils.ApplySerializedRunSettings(Quicksave.LoadedData.runSettings);
			if (Quicksave.LoadedData.run.LobbyType == Quicksave.LobbyMode.Offline)
			{
				Debug.Log("Attempting to load offline quicksave");
				RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Basic, null, new IEnumerator[]
				{
					Quicksave.<LoadSavedGameScene>g__OfflineLoadRoutine|31_0()
				});
				return;
			}
			Debug.Log("Attempting to recreate lobby from quicksave");
			GameHandler.AddStatus<SceneSwitchingStatus>(new SceneSwitchingStatus());
			string levelName = Quicksave._loadedData.run.levelName;
			Debug.Log("Begin scene load RPC: " + levelName);
			RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Basic, null, new IEnumerator[]
			{
				RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess(levelName, true, true, 0f)
			});
		}

		// Token: 0x0600198E RID: 6542 RVA: 0x000817ED File Offset: 0x0007F9ED
		[CompilerGenerated]
		internal static bool <TryLoadSave>g__CheckVersion|26_0()
		{
			return Quicksave._loadedData.version == 2;
		}

		// Token: 0x0600198F RID: 6543 RVA: 0x000817FF File Offset: 0x0007F9FF
		[CompilerGenerated]
		internal static IEnumerator <LoadSavedGameScene>g__OfflineLoadRoutine|31_0()
		{
			yield return MainMenu.DisconnectForOfflineMode();
			yield return RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess(Quicksave.LoadedData.run.levelName, false, true, 3f);
			yield break;
		}

		// Token: 0x0400173A RID: 5946
		private static readonly string s_SaveName = "quicksave.peak";

		// Token: 0x0400173B RID: 5947
		private static Quicksave.SaveData _loadedData;

		// Token: 0x0400173C RID: 5948
		private static bool _hasLoadedData;

		// Token: 0x0400173D RID: 5949
		public static bool ShouldUseSaveData;

		// Token: 0x02000567 RID: 1383
		public enum LobbyMode
		{
			// Token: 0x04001D42 RID: 7490
			Unknown,
			// Token: 0x04001D43 RID: 7491
			Offline,
			// Token: 0x04001D44 RID: 7492
			Online
		}

		// Token: 0x02000568 RID: 1384
		[Serializable]
		public struct RunProgress
		{
			// Token: 0x170002C9 RID: 713
			// (get) Token: 0x06001FC5 RID: 8133 RVA: 0x00090074 File Offset: 0x0008E274
			// (set) Token: 0x06001FC6 RID: 8134 RVA: 0x0009007C File Offset: 0x0008E27C
			public Quicksave.LobbyMode LobbyType { readonly get; private set; }

			// Token: 0x170002CA RID: 714
			// (get) Token: 0x06001FC7 RID: 8135 RVA: 0x00090085 File Offset: 0x0008E285
			public Dictionary<Hash128, List<SpawnedItemTracker.SpawnRecord>> SpawnHistoryLookup
			{
				get
				{
					this.EnsureLookupExists();
					return this._spawnHistoryLookup;
				}
			}

			// Token: 0x06001FC8 RID: 8136 RVA: 0x00090094 File Offset: 0x0008E294
			private RunProgress(Guid id, string scene, Segment biome, bool scoutStatueUsed)
			{
				this.runId = id.ToString();
				this.LobbyType = (NetCode.Session.IsOffline ? Quicksave.LobbyMode.Offline : Quicksave.LobbyMode.Online);
				this.runTimer = RunManager.Instance.timeSinceRunStarted;
				this.timeOfDay = DayNightManager.instance.timeOfDay;
				this.levelName = scene;
				this.biomeReached = biome;
				this.ascent = Ascents.currentAscent;
				this.spawnHistory = new List<Quicksave.RunProgress.SpawnHistory>();
				this.openLuggageViewIds = new List<int>();
				if (biome < Segment.TheKiln)
				{
					foreach (Luggage luggage in MapHandler.GetBaseCampLuggage(biome))
					{
						PhotonView photonView;
						if (luggage.IsOpen && luggage.TryGetComponent<PhotonView>(out photonView))
						{
							this.openLuggageViewIds.Add(photonView.ViewID);
						}
					}
				}
				SpawnedItemTracker[] array = Object.FindObjectsByType<SpawnedItemTracker>(FindObjectsInactive.Include, FindObjectsSortMode.None);
				Debug.Log(string.Format("Serializing spawn history from {0} spawn trackers", array.Length));
				int num = 0;
				foreach (SpawnedItemTracker spawnedItemTracker in array)
				{
					if (spawnedItemTracker.HasSpawnHistory)
					{
						num++;
						List<Quicksave.RunProgress.SpawnHistory> list = this.spawnHistory;
						Quicksave.RunProgress.SpawnHistory item = default(Quicksave.RunProgress.SpawnHistory);
						item.spawnerId = spawnedItemTracker.SpawnerId;
						item.spawnedItems = (from i in spawnedItemTracker.SpawnedItems
						select new SpawnedItemTracker.SpawnRecord(i)).ToArray<SpawnedItemTracker.SpawnRecord>();
						list.Add(item);
					}
				}
				Debug.Log(string.Format("Added {0} tracked spawns to save", num));
				this._spawnHistoryLookup = null;
			}

			// Token: 0x06001FC9 RID: 8137 RVA: 0x0009021C File Offset: 0x0008E41C
			public static Quicksave.RunProgress Create()
			{
				return new Quicksave.RunProgress(RunManager.Instance.RunId, SceneManager.GetActiveScene().name, Singleton<MapHandler>.Instance.GetCurrentSegment(), MapHandler.PreviousScoutStatue.IsSpent);
			}

			// Token: 0x06001FCA RID: 8138 RVA: 0x0009025C File Offset: 0x0008E45C
			private void EnsureLookupExists()
			{
				if (this._spawnHistoryLookup != null && this._spawnHistoryLookup.Count != 0)
				{
					return;
				}
				this._spawnHistoryLookup = new Dictionary<Hash128, List<SpawnedItemTracker.SpawnRecord>>();
				if (this.spawnHistory == null)
				{
					return;
				}
				Debug.Log(string.Format("Populating tracked spawn lookup with {0} entries.", this.spawnHistory.Count));
				foreach (Quicksave.RunProgress.SpawnHistory spawnHistory in this.spawnHistory)
				{
					this._spawnHistoryLookup[spawnHistory.spawnerId] = spawnHistory.spawnedItems.ToList<SpawnedItemTracker.SpawnRecord>();
				}
			}

			// Token: 0x06001FCB RID: 8139 RVA: 0x00090310 File Offset: 0x0008E510
			public bool TryGetSpawnHistory(Hash128 spawnerId, out List<SpawnedItemTracker.SpawnRecord> spawnedItems)
			{
				this.EnsureLookupExists();
				return this._spawnHistoryLookup.TryGetValue(spawnerId, out spawnedItems);
			}

			// Token: 0x04001D45 RID: 7493
			[SerializeField]
			public string runId;

			// Token: 0x04001D47 RID: 7495
			[SerializeField]
			internal float runTimer;

			// Token: 0x04001D48 RID: 7496
			[SerializeField]
			internal float timeOfDay;

			// Token: 0x04001D49 RID: 7497
			[SerializeField]
			internal string levelName;

			// Token: 0x04001D4A RID: 7498
			[SerializeField]
			internal Segment biomeReached;

			// Token: 0x04001D4B RID: 7499
			[SerializeField]
			internal int ascent;

			// Token: 0x04001D4C RID: 7500
			[SerializeField]
			internal List<Quicksave.RunProgress.SpawnHistory> spawnHistory;

			// Token: 0x04001D4D RID: 7501
			[SerializeField]
			internal List<int> openLuggageViewIds;

			// Token: 0x04001D4E RID: 7502
			private Dictionary<Hash128, List<SpawnedItemTracker.SpawnRecord>> _spawnHistoryLookup;

			// Token: 0x02000596 RID: 1430
			[Serializable]
			internal struct SpawnHistory
			{
				// Token: 0x04001DF8 RID: 7672
				[SerializeField]
				internal Hash128 spawnerId;

				// Token: 0x04001DF9 RID: 7673
				[SerializeField]
				internal SpawnedItemTracker.SpawnRecord[] spawnedItems;
			}
		}

		// Token: 0x02000569 RID: 1385
		[Serializable]
		private struct PlayerRunData
		{
			// Token: 0x170002CB RID: 715
			// (get) Token: 0x06001FCC RID: 8140 RVA: 0x00090325 File Offset: 0x0008E525
			// (set) Token: 0x06001FCD RID: 8141 RVA: 0x0009032D File Offset: 0x0008E52D
			public string UserId { readonly get; private set; }

			// Token: 0x06001FCE RID: 8142 RVA: 0x00090336 File Offset: 0x0008E536
			private PlayerRunData(string id, ReconnectData state, SerializableRunBasedValues achievementProgress)
			{
				this.UserId = id;
				this.reconnectData = CustomTypeRPCSerialization.SerializeReconnectData(state);
				this.achievementData = CustomTypeRPCSerialization.SerializeAchievementProgress(achievementProgress);
			}

			// Token: 0x06001FCF RID: 8143 RVA: 0x00090361 File Offset: 0x0008E561
			public static Quicksave.PlayerRunData CreateForLocalPlayer()
			{
				return new Quicksave.PlayerRunData(Player.localPlayer.GetUserId(), ReconnectData.CreateFromCharacter(Character.localCharacter), Singleton<AchievementManager>.Instance.runBasedValueData);
			}

			// Token: 0x06001FD0 RID: 8144 RVA: 0x00090388 File Offset: 0x0008E588
			public static bool TryCreateFromReconnectData(string playerId, out Quicksave.PlayerRunData runData)
			{
				ReconnectData state;
				SerializableRunBasedValues achievementProgress;
				if (!ReconnectHandler.TryGetReconnectData(playerId, out state, out achievementProgress))
				{
					runData = default(Quicksave.PlayerRunData);
					return false;
				}
				runData = new Quicksave.PlayerRunData(playerId, state, achievementProgress);
				return true;
			}

			// Token: 0x06001FD1 RID: 8145 RVA: 0x000903B9 File Offset: 0x0008E5B9
			public ReconnectData GetReconnectData()
			{
				return (ReconnectData)CustomTypeRPCSerialization.DeserializeReconnectData(this.reconnectData);
			}

			// Token: 0x06001FD2 RID: 8146 RVA: 0x000903CB File Offset: 0x0008E5CB
			public SerializableRunBasedValues GetAchievementProgress()
			{
				return (SerializableRunBasedValues)CustomTypeRPCSerialization.DeserializeAchievementProgress(this.achievementData);
			}

			// Token: 0x04001D50 RID: 7504
			[SerializeField]
			private byte[] reconnectData;

			// Token: 0x04001D51 RID: 7505
			[SerializeField]
			private byte[] achievementData;
		}

		// Token: 0x0200056A RID: 1386
		[Serializable]
		private struct SaveData
		{
			// Token: 0x06001FD3 RID: 8147 RVA: 0x000903DD File Offset: 0x0008E5DD
			private SaveData(Quicksave.RunProgress progress, List<Quicksave.PlayerRunData> saves)
			{
				this.version = 2;
				this.run = progress;
				this.runSettings = RunSettings.GetSerializedRunSettings();
				this.playerSaves = saves;
			}

			// Token: 0x06001FD4 RID: 8148 RVA: 0x00090400 File Offset: 0x0008E600
			public static Quicksave.SaveData Create()
			{
				Quicksave.RunProgress runProgress = Quicksave.RunProgress.Create();
				List<Quicksave.PlayerRunData> list = new List<Quicksave.PlayerRunData>();
				foreach (string text in Singleton<ReconnectHandler>.Instance.PlayersIdsInRecords)
				{
					Quicksave.PlayerRunData item;
					if (!Quicksave.PlayerRunData.TryCreateFromReconnectData(text, out item))
					{
						Debug.LogError("Failed to get valid data for " + text + " - they will be excluded from the quicksave.");
					}
					else
					{
						list.Add(item);
					}
				}
				Debug.Log(string.Format("Save data created for campfire before {0} with {1} players' data.", runProgress.biomeReached, list.Count));
				return new Quicksave.SaveData(runProgress, list);
			}

			// Token: 0x04001D52 RID: 7506
			private const int k_CurrentVersion = 2;

			// Token: 0x04001D53 RID: 7507
			[SerializeField]
			internal int version;

			// Token: 0x04001D54 RID: 7508
			[SerializeField]
			internal Quicksave.RunProgress run;

			// Token: 0x04001D55 RID: 7509
			[SerializeField]
			internal byte[] runSettings;

			// Token: 0x04001D56 RID: 7510
			[SerializeField]
			internal List<Quicksave.PlayerRunData> playerSaves;
		}
	}
}

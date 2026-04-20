using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Peak;
using Peak.Dev;
using Peak.Network;
using Photon.Pun;
using Photon.Realtime;
using Unity.Multiplayer.Playmode;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.Serizalization;

// Token: 0x02000147 RID: 327
public class CharacterSpawner : MonoBehaviourPunCallbacks
{
	// Token: 0x170000C3 RID: 195
	// (get) Token: 0x06000ACC RID: 2764 RVA: 0x00039925 File Offset: 0x00037B25
	private bool CanSpawnCharacters
	{
		get
		{
			return !CurrentPlayer.ReadOnlyTags().Contains("NoCharacter") && !this.hasSpawnedPlayer;
		}
	}

	// Token: 0x06000ACD RID: 2765 RVA: 0x00039943 File Offset: 0x00037B43
	private SpawnPoint GetSpawnPoint(int actorNumber)
	{
		return SpawnPoint.GetSpawnPoint(actorNumber);
	}

	// Token: 0x06000ACE RID: 2766 RVA: 0x0003994B File Offset: 0x00037B4B
	private void Update()
	{
		if (!NetCode.Session.InRoom)
		{
			return;
		}
		if (NetCode.Session.IsHost)
		{
			this.<Update>g__HostUpdate|11_0();
			return;
		}
		CharacterSpawner.<Update>g__ClientUpdate|11_1();
	}

	// Token: 0x06000ACF RID: 2767 RVA: 0x00039974 File Offset: 0x00037B74
	private void SpawnHostCharacter()
	{
		SpawnPoint spawnPoint = this.GetSpawnPoint(NetCode.Session.SeatNumber);
		Debug.Log("Attempting to spawn host's local character -- " + NetCode.Session.NickName + " " + string.Format("[Actor #{0}] (that's me)", NetCode.Session.SeatNumber));
		if (GameHandler.IsInGameplayScene && (Quicksave.ShouldUseSaveData || RunSettings.isMiniRun))
		{
			if (!MapHandler.ExistsAndInitialized)
			{
				Debug.Log("Waiting for maphandler to initialize before we attempt spawn");
				return;
			}
			if (this._frameMapHandlerStarted < 0)
			{
				Debug.Log("MapHandler has awoken! Waiting a few more frames before attempting to spawn host.");
				this._frameMapHandlerStarted = Time.frameCount;
				return;
			}
			if (Time.frameCount - this._frameMapHandlerStarted < 3)
			{
				return;
			}
		}
		if (Quicksave.ShouldUseSaveData)
		{
			Quicksave.PopulateMapAndPlayerStates();
			Campfire previousCampfire = MapHandler.PreviousCampfire;
			Debug.Log("Picked " + previousCampfire.name + " as spawn point.", previousCampfire);
			ReconnectData savedHostState = Quicksave.SavedHostState;
			savedHostState.dead = false;
			savedHostState.deathTimer = 0f;
			Character character = this.SpawnMyPlayerCharacter(CharacterSpawner.SpawnFlavor.Poof, previousCampfire.transform, CharacterSpawner.RandomBaseCampOffset, false);
			MountainProgressHandler.DisplaySegmentTitleAfterDelay((int)savedHostState.mapSegment, 2f);
			if (savedHostState.isSkeleton)
			{
				character.data.SetSkeleton(true);
			}
			CharacterSpawner.PushReconnectData(character, savedHostState, false);
			return;
		}
		if (GameHandler.IsOnIsland)
		{
			if (!spawnPoint.startPassedOut)
			{
				Debug.LogWarning("Spawn point " + spawnPoint.name + " does NOT say to spawn passed out. Is it broken?", spawnPoint);
			}
			this.SpawnMyPlayerCharacter(spawnPoint.startPassedOut ? CharacterSpawner.SpawnFlavor.PassedOut : CharacterSpawner.SpawnFlavor.Instant, null, default(Vector3), true);
			return;
		}
		this.SpawnSelfInAirport();
	}

	// Token: 0x06000AD0 RID: 2768 RVA: 0x00039B00 File Offset: 0x00037D00
	private Character SpawnMyPlayerCharacter(CharacterSpawner.SpawnFlavor spawnFlavor = CharacterSpawner.SpawnFlavor.Instant, Transform spawnOverride = null, Vector3 spawnOffset = default(Vector3), bool initializeNewScout = false)
	{
		if (initializeNewScout)
		{
			Singleton<AchievementManager>.Instance.InitRunBasedValues();
		}
		SceneSwitchingStatus sceneSwitchingStatus;
		if (GameHandler.TryGetStatus<SceneSwitchingStatus>(out sceneSwitchingStatus))
		{
			GameHandler.ClearStatus<SceneSwitchingStatus>();
		}
		Character character = Character.localCharacter;
		if (this.CanSpawnCharacters)
		{
			this.hasSpawnedPlayer = true;
			Transform transform = spawnOverride ?? this.GetSpawnPoint(NetCode.Session.SeatNumber).transform;
			Vector3 vector = transform.position + spawnOffset;
			Debug.Log(string.Format("Spawning myself ({0} [{1}]) at {2}! (Flavor: {3})", new object[]
			{
				NetCode.Session.NickName,
				NetCode.Session.SeatNumber,
				vector,
				spawnFlavor
			}));
			character = PhotonNetwork.Instantiate("Character", vector, transform.rotation, 0, null).GetComponent<Character>();
			character.data.spawnPoint = transform;
			if (spawnFlavor == CharacterSpawner.SpawnFlavor.PassedOut)
			{
				character.StartPassedOutOnTheBeach();
			}
			else if (spawnFlavor == CharacterSpawner.SpawnFlavor.Poof)
			{
				character.PlayPoofVFX(vector);
			}
			if (spawnFlavor != CharacterSpawner.SpawnFlavor.Lobby)
			{
				Singleton<MountainProgressHandler>.Instance.CheckProgress(false);
			}
			if (initializeNewScout)
			{
				character.photonView.RPC("RPCA_InitializeScoutReport", RpcTarget.All, Array.Empty<object>());
			}
		}
		else if (!this.hasSpawnedPlayer)
		{
			Debug.Log("Skipping character spawn because of MPPM tags");
		}
		if (global::Player.localPlayer == null)
		{
			Debug.Log("Spawning local player object.");
			PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0, null);
		}
		else
		{
			Debug.LogWarning("Local player (" + global::Player.localPlayer.name + ") already exists?? That seems bad.", global::Player.localPlayer);
		}
		return character;
	}

	// Token: 0x06000AD1 RID: 2769 RVA: 0x00039C7C File Offset: 0x00037E7C
	private static void KillImmediately(Character self)
	{
		Debug.Log("Killing " + self.name + " now that they're spawned.");
		self.photonView.RPC("RPCA_Die", RpcTarget.All, new object[]
		{
			Character.localCharacter.Center
		});
		Character.localCharacter.data.deathTimer = 5f;
	}

	// Token: 0x06000AD2 RID: 2770 RVA: 0x00039CE0 File Offset: 0x00037EE0
	private static void DeathOnArrival()
	{
		if (Character.localCharacter == null)
		{
			PlayerHandler.CharacterRegistered = (Action<Character>)Delegate.Combine(PlayerHandler.CharacterRegistered, new Action<Character>(CharacterSpawner.<DeathOnArrival>g__WaitThenDie|16_0));
			return;
		}
		CharacterSpawner.KillImmediately(Character.localCharacter);
	}

	// Token: 0x170000C4 RID: 196
	// (get) Token: 0x06000AD3 RID: 2771 RVA: 0x00039D1A File Offset: 0x00037F1A
	private static Vector3 CurrentRevivePoint
	{
		get
		{
			return MapHandler.CurrentBaseCampSpawnPoint.position;
		}
	}

	// Token: 0x06000AD4 RID: 2772 RVA: 0x00039D26 File Offset: 0x00037F26
	private IEnumerator ReviveBeforeSpawn(ReconnectData reconnectData)
	{
		reconnectData.dead = false;
		reconnectData.deathTimer = 0f;
		reconnectData.fullyPassedOut = false;
		int num = Enum.GetNames(typeof(CharacterAfflictions.STATUSTYPE)).Length;
		for (int i = 0; i < num; i++)
		{
			CharacterAfflictions.STATUSTYPE statustype = (CharacterAfflictions.STATUSTYPE)i;
			if (statustype != CharacterAfflictions.STATUSTYPE.Weight && statustype != CharacterAfflictions.STATUSTYPE.Crab && statustype != CharacterAfflictions.STATUSTYPE.Curse)
			{
				reconnectData.currentStatuses[i] = 0f;
			}
		}
		Character self = this.SpawnSelfAtSpecificPosition(CharacterSpawner.SpawnFlavor.Poof, CharacterSpawner.CurrentRevivePoint + CharacterSpawner.RandomBaseCampOffset);
		MountainProgressHandler.DisplaySegmentTitleAfterDelay((int)reconnectData.mapSegment, 2f);
		yield return null;
		CharacterSpawner.PushReconnectData(self, reconnectData, true);
		Character.ApplyPostReviveStatus(self.refs.afflictions);
		yield break;
	}

	// Token: 0x170000C5 RID: 197
	// (get) Token: 0x06000AD5 RID: 2773 RVA: 0x00039D3C File Offset: 0x00037F3C
	public static Vector3 RandomBaseCampOffset
	{
		get
		{
			Vector2 vector = Random.Range(1.75f, 2.5f) * Random.insideUnitCircle.normalized;
			return new Vector3(vector.x, Random.Range(6f, 7f), vector.y);
		}
	}

	// Token: 0x170000C6 RID: 198
	// (get) Token: 0x06000AD6 RID: 2774 RVA: 0x00039D8C File Offset: 0x00037F8C
	private static bool ScoutsWereRevivedAtCurrentBaseCamp
	{
		get
		{
			switch (Singleton<MapHandler>.Instance.GetCurrentSegment())
			{
			case Segment.Beach:
			case Segment.Tropics:
			case Segment.Alpine:
			case Segment.Caldera:
				return MapHandler.BaseCampHasRevived;
			case Segment.TheKiln:
				return !Singleton<LavaRising>.Instance.started && MapHandler.BaseCampHasRevived;
			case Segment.Peak:
				return false;
			default:
				Debug.LogError("uh oh! We added more Segments to our game without telling me how to handle them");
				return false;
			}
		}
	}

	// Token: 0x06000AD7 RID: 2775 RVA: 0x00039DEC File Offset: 0x00037FEC
	private void SpawnSelfInAirport()
	{
		Debug.Log("Spawning local character in airport.");
		this.SpawnMyPlayerCharacter(CharacterSpawner.SpawnFlavor.Lobby, null, default(Vector3), false);
	}

	// Token: 0x06000AD8 RID: 2776 RVA: 0x00039E18 File Offset: 0x00038018
	private void SpawnSelfOnShore()
	{
		Debug.Log("Spawning fresh on the shore. Game hasn't started");
		this.SpawnMyPlayerCharacter(CharacterSpawner.SpawnFlavor.PassedOut, null, default(Vector3), true);
	}

	// Token: 0x06000AD9 RID: 2777 RVA: 0x00039E44 File Offset: 0x00038044
	private void SpawnDeadAtBaseCamp(bool andRevive)
	{
		CharacterSpawner.<>c__DisplayClass26_0 CS$<>8__locals1 = new CharacterSpawner.<>c__DisplayClass26_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.andRevive = andRevive;
		Singleton<AchievementManager>.Instance.InitRunBasedValues();
		base.StartCoroutine(CS$<>8__locals1.<SpawnDeadAtBaseCamp>g__LateConnectRoutine|0());
	}

	// Token: 0x06000ADA RID: 2778 RVA: 0x00039E7C File Offset: 0x0003807C
	private Character SpawnSelfAtSpecificPosition(CharacterSpawner.SpawnFlavor spawnFlavor, Vector3 position)
	{
		Vector3 spawnOffset = position - SpawnPoint.LocalSpawnPoint.transform.position;
		return this.SpawnMyPlayerCharacter(spawnFlavor, SpawnPoint.LocalSpawnPoint.transform, spawnOffset, false);
	}

	// Token: 0x06000ADB RID: 2779 RVA: 0x00039EB4 File Offset: 0x000380B4
	private static void PushReconnectData(Character self, ReconnectData data, bool wasRevivedBeforeSpawn = false)
	{
		self.photonView.RPC("RPCA_SyncScoutReport", RpcTarget.All, new object[]
		{
			data.scoutReport,
			wasRevivedBeforeSpawn
		});
		Debug.Log("Restoring state from ReconnectData: " + Pretty.Print(data));
		self.refs.afflictions.ApplyReconnectData(data);
		self.player.photonView.RPC("SyncInventoryRPC", RpcTarget.All, new object[]
		{
			IBinarySerializable.ToManagedArray<InventorySyncData>(data.inventorySyncData),
			true
		});
		if (data.inventorySyncData.tempSlot.ItemID != 65535)
		{
			self.refs.items.EquipSlot(Optionable<byte>.Some(250));
		}
		if (data.dead)
		{
			self.SetDeadAfterReconnect(data.position);
		}
	}

	// Token: 0x06000ADC RID: 2780 RVA: 0x00039F92 File Offset: 0x00038192
	[PunRPC]
	public void RPC_ReconnectingPlayerSpawn(Segment currentMapStage, ReconnectData reconnectData, SerializableRunBasedValues achievementData, bool canRevive, int lastReviveSegment, PhotonMessageInfo info)
	{
		if (info.Sender.ActorNumber != NetCode.Session.HostId)
		{
			return;
		}
		this.ReconnectingPlayerSpawn(currentMapStage, reconnectData, achievementData, canRevive, lastReviveSegment);
	}

	// Token: 0x06000ADD RID: 2781 RVA: 0x00039FBC File Offset: 0x000381BC
	private void ReconnectingPlayerSpawn(Segment currentMapStage, ReconnectData reconnectData, SerializableRunBasedValues achievementData, bool canRevive, int lastReviveSegment)
	{
		CharacterSpawner.<>c__DisplayClass30_0 CS$<>8__locals1 = new CharacterSpawner.<>c__DisplayClass30_0();
		CS$<>8__locals1.currentMapStage = currentMapStage;
		CS$<>8__locals1.lastReviveSegment = lastReviveSegment;
		CS$<>8__locals1.reconnectData = reconnectData;
		CS$<>8__locals1.canRevive = canRevive;
		CS$<>8__locals1.<>4__this = this;
		if (!GameHandler.IsOnIsland || !CS$<>8__locals1.reconnectData.isValid)
		{
			Debug.LogError("What da heck! This shouldn't happen.");
		}
		if (this.hasSpawnedPlayer)
		{
			Debug.LogWarning("Ignoring spawn request because we already spawned this player.");
			return;
		}
		Singleton<AchievementManager>.Instance.InitRunBasedValues(achievementData);
		base.StartCoroutine(CS$<>8__locals1.<ReconnectingPlayerSpawn>g__ReconnectRoutine|0());
	}

	// Token: 0x06000ADE RID: 2782 RVA: 0x0003A03D File Offset: 0x0003823D
	private static IEnumerator WaitUntilCharacterInitialized()
	{
		while (Character.localCharacter == null)
		{
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000ADF RID: 2783 RVA: 0x0003A048 File Offset: 0x00038248
	[PunRPC]
	public void RPC_MiniRunPlayerSpawn(Segment miniRunSegment, PhotonMessageInfo info)
	{
		CharacterSpawner.<>c__DisplayClass32_0 CS$<>8__locals1 = new CharacterSpawner.<>c__DisplayClass32_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.miniRunSegment = miniRunSegment;
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		if (NetCode.Session.IsHost)
		{
			Debug.LogError("What da heck! I should have already spawned myself and not gone through this code path.");
		}
		if (this.hasSpawnedPlayer)
		{
			Debug.LogWarning("Ignoring spawn request because we already spawned this player.");
			return;
		}
		if (this.waitingForIslandInit)
		{
			Debug.LogWarning("Ignoring spawn request because we're still waiting for one to go through.");
			return;
		}
		base.StartCoroutine(CS$<>8__locals1.<RPC_MiniRunPlayerSpawn>g__MiniRunRoutine|0());
	}

	// Token: 0x06000AE0 RID: 2784 RVA: 0x0003A0C0 File Offset: 0x000382C0
	[PunRPC]
	public void RPC_NewPlayerSpawn(Segment currentMapStage, bool canRevive, int lastReviveSegment, PhotonMessageInfo info)
	{
		CharacterSpawner.<>c__DisplayClass33_0 CS$<>8__locals1 = new CharacterSpawner.<>c__DisplayClass33_0();
		CS$<>8__locals1.currentMapStage = currentMapStage;
		CS$<>8__locals1.lastReviveSegment = lastReviveSegment;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.canRevive = canRevive;
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		if (NetCode.Session.IsHost)
		{
			Debug.LogError("What da heck! I should have already spawned myself and not gone through this code path.");
		}
		if (this.hasSpawnedPlayer)
		{
			Debug.LogWarning("Ignoring spawn request because we already spawned this player.");
			return;
		}
		base.StartCoroutine(CS$<>8__locals1.<RPC_NewPlayerSpawn>g__NewPlayerRoutine|0());
	}

	// Token: 0x06000AE2 RID: 2786 RVA: 0x0003A150 File Offset: 0x00038350
	[CompilerGenerated]
	private void <Update>g__HostUpdate|11_0()
	{
		Character character;
		if (!PlayerHandler.TryGetCharacter(NetCode.Session.SeatNumber, out character))
		{
			this.SpawnHostCharacter();
			return;
		}
		foreach (Photon.Realtime.Player player in this._playerList.Get())
		{
			if (!PlayerHandler.TryGetCharacter(player.ActorNumber, out character) && (!this.attemptedSpawns.ContainsKey(player.ActorNumber) || Time.realtimeSinceStartup - this.attemptedSpawns[player.ActorNumber] >= 2f))
			{
				this.attemptedSpawns[player.ActorNumber] = Time.realtimeSinceStartup;
				if (!player.IsLocal)
				{
					ReconnectData reconnectData;
					SerializableRunBasedValues serializableRunBasedValues;
					if (ReconnectHandler.TryGetReconnectData(player, out reconnectData, out serializableRunBasedValues) && GameHandler.IsOnIsland)
					{
						Segment segmentNumber = MapHandler.BaseCampScoutStatue.SegmentNumber;
						bool scoutsWereRevivedAtCurrentBaseCamp = CharacterSpawner.ScoutsWereRevivedAtCurrentBaseCamp;
						bool flag = segmentNumber != Segment.Beach && reconnectData.lastRevivedSegment == segmentNumber;
						bool flag2 = !RunSettings.isMiniRun && scoutsWereRevivedAtCurrentBaseCamp && !flag;
						Debug.Log(string.Format("Reconnect data found for {0} ({1})! ", player.UserId, player.ActorNumber) + "Sending: " + Pretty.Print(reconnectData) + string.Format("\nCurrent base camp: {0} | Revive used: {1}", segmentNumber, scoutsWereRevivedAtCurrentBaseCamp));
						OrbFogHandler.InitFogIfExists(player);
						base.photonView.RPC("RPC_ReconnectingPlayerSpawn", player, new object[]
						{
							MapHandler.CurrentSegmentNumber,
							reconnectData,
							serializableRunBasedValues,
							flag2,
							Singleton<MapHandler>.Instance.LastRevivedSegment
						});
					}
					else if (RunSettings.isMiniRun && GameHandler.IsOnIsland)
					{
						Segment segment = (Segment)RunSettings.GetValue(RunSettings.SETTINGTYPE.MiniRunBiome, false);
						if (Singleton<MapHandler>.Instance.GetCurrentSegment() != segment)
						{
							Debug.LogWarning("Would like to spawn player into mini run, but the map segment isn't set yet! Waiting a little longer...");
							return;
						}
						Debug.Log("Fresh player connected for mini run. Having them spawn at beginning of run.");
						OrbFogHandler.InitFogIfExists(player);
						base.photonView.RPC("RPC_MiniRunPlayerSpawn", player, new object[]
						{
							segment
						});
					}
					else
					{
						Debug.Log(string.Format("No reconnect data found for {0} ({1}). Requesting fresh spawn.", player.UserId, player.ActorNumber));
						bool flag3 = GameHandler.IsOnIsland && MapHandler.BaseCampHasRevived && MapHandler.LastSeenCampfireIsSafe;
						OrbFogHandler.InitFogIfExists(player);
						base.photonView.RPC("RPC_NewPlayerSpawn", player, new object[]
						{
							GameHandler.IsOnIsland ? MapHandler.CurrentSegmentNumber : Segment.Beach,
							flag3,
							GameHandler.IsOnIsland ? Singleton<MapHandler>.Instance.LastRevivedSegment : int.MinValue
						});
					}
				}
			}
		}
	}

	// Token: 0x06000AE3 RID: 2787 RVA: 0x0003A40C File Offset: 0x0003860C
	[CompilerGenerated]
	internal static void <Update>g__ClientUpdate|11_1()
	{
	}

	// Token: 0x06000AE4 RID: 2788 RVA: 0x0003A40E File Offset: 0x0003860E
	[CompilerGenerated]
	internal static void <DeathOnArrival>g__WaitThenDie|16_0(Character registeredCharacter)
	{
		if (!registeredCharacter.photonView.IsMine)
		{
			return;
		}
		Debug.Log("Player was registered and needs to die!");
		PlayerHandler.CharacterRegistered = (Action<Character>)Delegate.Remove(PlayerHandler.CharacterRegistered, new Action<Character>(CharacterSpawner.<DeathOnArrival>g__WaitThenDie|16_0));
		CharacterSpawner.KillImmediately(registeredCharacter);
	}

	// Token: 0x04000A13 RID: 2579
	private CachedPlayerList _playerList;

	// Token: 0x04000A14 RID: 2580
	private const float SpawnAttemptCooldown = 2f;

	// Token: 0x04000A15 RID: 2581
	public Item[] itemsToSpawnWith;

	// Token: 0x04000A16 RID: 2582
	private float _timeLastLocalSpawnStarted;

	// Token: 0x04000A17 RID: 2583
	private bool hasSpawnedPlayer;

	// Token: 0x04000A18 RID: 2584
	private bool waitingForIslandInit;

	// Token: 0x04000A19 RID: 2585
	private Dictionary<int, float> attemptedSpawns = new Dictionary<int, float>();

	// Token: 0x04000A1A RID: 2586
	private int _frameMapHandlerStarted = -1;

	// Token: 0x0200048C RID: 1164
	public enum SpawnFlavor
	{
		// Token: 0x040019FF RID: 6655
		Instant,
		// Token: 0x04001A00 RID: 6656
		PassedOut,
		// Token: 0x04001A01 RID: 6657
		Poof,
		// Token: 0x04001A02 RID: 6658
		Lobby
	}
}

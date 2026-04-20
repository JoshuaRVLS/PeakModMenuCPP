using System;
using System.Collections;
using Peak;
using Peak.Network;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200017F RID: 383
public class RunManager : MonoBehaviourPunCallbacks
{
	// Token: 0x170000F0 RID: 240
	// (get) Token: 0x06000C75 RID: 3189 RVA: 0x00043128 File Offset: 0x00041328
	private bool shouldCheckGameEnd
	{
		get
		{
			return this.runStarted && this.timerActive && this._timeWhenEndGameCheckNeeded < Time.realtimeSinceStartup && this._timeWhenEndGameCheckNeeded > this._timeEndGameLastChecked;
		}
	}

	// Token: 0x170000F1 RID: 241
	// (get) Token: 0x06000C76 RID: 3190 RVA: 0x00043157 File Offset: 0x00041357
	// (set) Token: 0x06000C77 RID: 3191 RVA: 0x0004315F File Offset: 0x0004135F
	public Guid RunId { get; set; }

	// Token: 0x06000C78 RID: 3192 RVA: 0x00043168 File Offset: 0x00041368
	private void Awake()
	{
		RunManager.Instance = this;
	}

	// Token: 0x06000C79 RID: 3193 RVA: 0x00043170 File Offset: 0x00041370
	private IEnumerator Start()
	{
		this.runStarted = false;
		this.timeSinceRunStarted = 0f;
		PlayerHandler.CharacterRegistered = (Action<Character>)Delegate.Combine(PlayerHandler.CharacterRegistered, new Action<Character>(this.OnCharacterRegistered));
		if (Quicksave.ShouldUseSaveData)
		{
			MapHandler.PopulateSpawnHistories(Quicksave.SavedRun.SpawnHistoryLookup);
		}
		if (MapHandler.Exists)
		{
			MapHandler.InitializeMap();
		}
		while (!NetCode.Session.InRoom || !Character.localCharacter || LoadingScreenHandler.loading)
		{
			yield return null;
		}
		if (Character.localCharacter.inAirport)
		{
			Debug.Log("We're in the airport. No need to track Run stats. Disabling self.");
			yield break;
		}
		Debug.Log("RUN STARTED");
		this.StartRun();
		if (!NetCode.Session.IsHost)
		{
			this.GetRunId();
			yield break;
		}
		yield return new WaitForSecondsRealtime(2f);
		if (Quicksave.ShouldUseSaveData)
		{
			Quicksave.RunProgress savedRun = Quicksave.SavedRun;
			MapHandler.OpenBaseCampLuggage(savedRun.biomeReached, savedRun.openLuggageViewIds);
			Quicksave.FinalizeRunSetupAndSelfDestruct();
		}
		else
		{
			this.SetRunIdForLobby(Guid.NewGuid());
			base.photonView.RPC("RPC_SyncTime", RpcTarget.All, new object[]
			{
				0f,
				true
			});
			if (RunSettings.isMiniRun)
			{
				base.StartCoroutine(this.JumpToMiniRunBiomeWhenReady());
			}
		}
		yield break;
	}

	// Token: 0x06000C7A RID: 3194 RVA: 0x0004317F File Offset: 0x0004137F
	private IEnumerator JumpToMiniRunBiomeWhenReady()
	{
		while (!Character.localCharacter || !Singleton<MapHandler>.Instance)
		{
			yield return null;
		}
		switch (RunSettings.GetValue(RunSettings.SETTINGTYPE.MiniRunBiome, false))
		{
		case 1:
			MapHandler.JumpToSegment(Segment.Tropics);
			break;
		case 2:
			MapHandler.JumpToSegment(Segment.Alpine);
			break;
		case 3:
			MapHandler.JumpToSegment(Segment.Caldera);
			break;
		}
		yield break;
	}

	// Token: 0x06000C7B RID: 3195 RVA: 0x00043187 File Offset: 0x00041387
	private void Update()
	{
		if (this.timerActive)
		{
			this.timeSinceRunStarted += Time.deltaTime;
		}
		if (this.shouldCheckGameEnd)
		{
			this.CheckForGameEnd();
		}
	}

	// Token: 0x06000C7C RID: 3196 RVA: 0x000431B1 File Offset: 0x000413B1
	private void OnDestroy()
	{
		PlayerHandler.CharacterRegistered = (Action<Character>)Delegate.Remove(PlayerHandler.CharacterRegistered, new Action<Character>(this.OnCharacterRegistered));
	}

	// Token: 0x06000C7D RID: 3197 RVA: 0x000431D4 File Offset: 0x000413D4
	public void StartRun()
	{
		base.enabled = true;
		this.runStarted = true;
		NetCode.RoomEvents.PlayerLeft += this.OnPlayersChanged;
		NetCode.RoomEvents.RoomOwnerChanged += this.OnPlayersChanged;
		this._timeEndGameLastChecked = 0f;
		this._timeWhenEndGameCheckNeeded = Time.realtimeSinceStartup + 2f;
	}

	// Token: 0x06000C7E RID: 3198 RVA: 0x00043237 File Offset: 0x00041437
	private void DebugCurrentTime()
	{
		Debug.Log(this.timeSinceRunStarted);
	}

	// Token: 0x06000C7F RID: 3199 RVA: 0x00043249 File Offset: 0x00041449
	private void ScheduleNextEndGameCheck()
	{
		if (this._timeWhenEndGameCheckNeeded < Time.realtimeSinceStartup && !this.shouldCheckGameEnd)
		{
			this._timeWhenEndGameCheckNeeded = Time.realtimeSinceStartup + 2f;
		}
	}

	// Token: 0x06000C80 RID: 3200 RVA: 0x00043271 File Offset: 0x00041471
	private void OnPlayersChanged()
	{
		this.ScheduleNextEndGameCheck();
	}

	// Token: 0x06000C81 RID: 3201 RVA: 0x00043279 File Offset: 0x00041479
	private void CheckForGameEnd()
	{
		this._timeEndGameLastChecked = Time.realtimeSinceStartup;
		if (Character.localCharacter != null)
		{
			Character.localCharacter.CheckEndGame();
		}
	}

	// Token: 0x06000C82 RID: 3202 RVA: 0x000432A0 File Offset: 0x000414A0
	internal void SyncTimeMaster()
	{
		if (NetCode.Session.IsHost)
		{
			base.photonView.RPC("RPC_SyncTime", RpcTarget.All, new object[]
			{
				this.timeSinceRunStarted,
				this.timerActive
			});
		}
	}

	// Token: 0x06000C83 RID: 3203 RVA: 0x000432EC File Offset: 0x000414EC
	internal void EndGame()
	{
		NetCode.RoomEvents.PlayerLeft -= this.OnPlayersChanged;
		NetCode.RoomEvents.RoomOwnerChanged -= this.OnPlayersChanged;
		if (NetCode.Session.IsHost)
		{
			this.SetRunIdForLobby(Guid.Empty);
			base.photonView.RPC("RPC_SyncTime", RpcTarget.All, new object[]
			{
				this.timeSinceRunStarted,
				false
			});
		}
	}

	// Token: 0x06000C84 RID: 3204 RVA: 0x0004336C File Offset: 0x0004156C
	private void GetRunId()
	{
		if (this._hasRunId)
		{
			return;
		}
		string input;
		NetCode.Matchmaking.TryGetLobbyData(NetCode.Matchmaking.LobbyId, "RunId", out input);
		this.RunId = Guid.Parse(input);
		this._hasRunId = true;
	}

	// Token: 0x06000C85 RID: 3205 RVA: 0x000433B4 File Offset: 0x000415B4
	private void SetRunIdForLobby(Guid guid)
	{
		this.RunId = guid;
		NetCode.Matchmaking.TrySetLobbyData("RunId", this.RunId.ToString());
		base.photonView.RPC("RPC_ReceiveRunId", RpcTarget.Others, new object[]
		{
			guid.ToString()
		});
		this._hasRunId = true;
	}

	// Token: 0x06000C86 RID: 3206 RVA: 0x0004341A File Offset: 0x0004161A
	[PunRPC]
	private void RPC_ReceiveRunId(string guid)
	{
		this.RunId = Guid.Parse(guid);
		this._hasRunId = true;
	}

	// Token: 0x06000C87 RID: 3207 RVA: 0x00043430 File Offset: 0x00041630
	public void OnCharacterRegistered(Character character)
	{
		if (NetCode.Session.IsHost)
		{
			base.photonView.RPC("RPC_SyncTime", character.photonView.Owner, new object[]
			{
				this.timeSinceRunStarted,
				this.timerActive
			});
		}
	}

	// Token: 0x06000C88 RID: 3208 RVA: 0x00043486 File Offset: 0x00041686
	public static void SetUpFromQuicksave(Guid runId, float newTime)
	{
		RunManager.Instance.SetRunIdForLobby(runId);
		RunManager.Instance.photonView.RPC("RPC_SyncTime", RpcTarget.All, new object[]
		{
			newTime,
			true
		});
	}

	// Token: 0x06000C89 RID: 3209 RVA: 0x000434C0 File Offset: 0x000416C0
	[PunRPC]
	private void RPC_SyncTime(float time, bool timerActive)
	{
		Debug.Log(string.Format("Time synced: {0} timer active: {1}", time, timerActive));
		this.timeSinceRunStarted = time;
		this.timerActive = timerActive;
	}

	// Token: 0x04000B6D RID: 2925
	public float timeSinceRunStarted;

	// Token: 0x04000B6E RID: 2926
	public const string RunIdLobbyDataKey = "RunId";

	// Token: 0x04000B6F RID: 2927
	private float _timeWhenEndGameCheckNeeded;

	// Token: 0x04000B70 RID: 2928
	private float _timeEndGameLastChecked = float.NegativeInfinity;

	// Token: 0x04000B71 RID: 2929
	private const float EndGameCheckDelay = 2f;

	// Token: 0x04000B72 RID: 2930
	private bool _hasRunId;

	// Token: 0x04000B73 RID: 2931
	private bool runStarted;

	// Token: 0x04000B74 RID: 2932
	private bool timerActive;

	// Token: 0x04000B76 RID: 2934
	public static RunManager Instance;
}

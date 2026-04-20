using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zorro.Core;

// Token: 0x02000137 RID: 311
public class LoadingScreenHandler : RetrievableResourceSingleton<LoadingScreenHandler>
{
	// Token: 0x170000A4 RID: 164
	// (get) Token: 0x06000A00 RID: 2560 RVA: 0x00035AA8 File Offset: 0x00033CA8
	// (set) Token: 0x06000A01 RID: 2561 RVA: 0x00035AAF File Offset: 0x00033CAF
	public static bool loading
	{
		get
		{
			return LoadingScreenHandler._loading;
		}
		private set
		{
			if (!value && RetrievableResourceSingleton<LoadingScreenHandler>.Instance._lastActiveLoadingScreen != null)
			{
				Debug.Log("Stopped loading without first destroying loading screen");
			}
			LoadingScreenHandler._loading = value;
		}
	}

	// Token: 0x06000A02 RID: 2562 RVA: 0x00035AD6 File Offset: 0x00033CD6
	public static void KillCurrentLoadingScreen()
	{
		if (RetrievableResourceSingleton<LoadingScreenHandler>.Instance._lastActiveLoadingScreen)
		{
			Object.Destroy(RetrievableResourceSingleton<LoadingScreenHandler>.Instance._lastActiveLoadingScreen.gameObject);
		}
	}

	// Token: 0x06000A03 RID: 2563 RVA: 0x00035AFD File Offset: 0x00033CFD
	private void Awake()
	{
		this.loadingScreens = new Dictionary<LoadingScreen.LoadingScreenType, LoadingScreen>
		{
			{
				LoadingScreen.LoadingScreenType.Basic,
				this.loadingScreenPrefabBasic
			},
			{
				LoadingScreen.LoadingScreenType.Plane,
				this.loadingScreenPrefabPlane
			}
		};
		Object.DontDestroyOnLoad(this);
	}

	// Token: 0x06000A04 RID: 2564 RVA: 0x00035B2A File Offset: 0x00033D2A
	public LoadingScreen GetLoadingScreenPrefab(LoadingScreen.LoadingScreenType type)
	{
		return this.loadingScreens[type];
	}

	// Token: 0x06000A05 RID: 2565 RVA: 0x00035B38 File Offset: 0x00033D38
	public void Load(LoadingScreen.LoadingScreenType type, Action runAfter, params IEnumerator[] processes)
	{
		GameHandler.ClearStatus<EndScreenStatus>();
		if (!LoadingScreenHandler.loading)
		{
			base.StartCoroutine(this.LoadingRoutine(type, runAfter, processes));
			return;
		}
		Debug.LogError("Tried to load while already loading! If this happens a lot it's likely an issue!");
	}

	// Token: 0x06000A06 RID: 2566 RVA: 0x00035B61 File Offset: 0x00033D61
	private IEnumerator LoadingRoutine(LoadingScreen.LoadingScreenType type, Action runAfter, params IEnumerator[] processes)
	{
		PhotonNetwork.IsMessageQueueRunning = false;
		LoadingScreenHandler.loading = true;
		LoadingScreen loadingScreenPrefab = this.GetLoadingScreenPrefab(type);
		this._lastActiveLoadingScreen = Object.Instantiate<LoadingScreen>(loadingScreenPrefab, Vector3.zero, Quaternion.identity);
		yield return base.StartCoroutine(this._lastActiveLoadingScreen.LoadingRoutine(runAfter, processes));
		LoadingScreenHandler.loading = false;
		PhotonNetwork.IsMessageQueueRunning = true;
		yield break;
	}

	// Token: 0x06000A07 RID: 2567 RVA: 0x00035B85 File Offset: 0x00033D85
	public IEnumerator LoadSceneProcess(string sceneName, bool networked, bool yieldForCharacterSpawn = false, float extraYieldTimeOnEnd = 3f)
	{
		if (networked)
		{
			yield return this.LoadSceneProcessNetworked(sceneName, yieldForCharacterSpawn, extraYieldTimeOnEnd);
		}
		else
		{
			yield return this.LoadSceneProcessOffline(sceneName, yieldForCharacterSpawn, extraYieldTimeOnEnd);
		}
		yield break;
	}

	// Token: 0x06000A08 RID: 2568 RVA: 0x00035BB1 File Offset: 0x00033DB1
	private IEnumerator LoadSceneProcessNetworked(string sceneName, bool yieldForCharacterSpawn, float extraYieldTimeOnEnd)
	{
		PhotonNetwork.LoadLevel(sceneName);
		float timeout = 5f;
		while ((timeout > 0f && PhotonNetwork.LevelLoadingProgress == 0f) || PhotonNetwork.LevelLoadingProgress >= 1f)
		{
			timeout -= Time.unscaledDeltaTime;
			yield return null;
		}
		Debug.Log(string.Format("Waited {0} for level to start loading. Progress: {1}", 5f - timeout, PhotonNetwork.LevelLoadingProgress));
		if (DayNightManager.instance != null)
		{
			DayNightManager.instance.specialDaySunBlend = 0f;
			DayNightManager.instance.specialDaySkyBlend = 0f;
		}
		float tic = Time.realtimeSinceStartup;
		while (PhotonNetwork.LevelLoadingProgress < 1f)
		{
			yield return null;
		}
		Debug.Log(string.Format("Level took {0} seconds to start loading.", Time.realtimeSinceStartup - tic));
		PhotonNetwork.IsMessageQueueRunning = true;
		tic = Time.realtimeSinceStartup;
		while (PhotonNetwork.NetworkClientState == ClientState.ConnectingToGameServer)
		{
			yield return null;
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (Mathf.Approximately(0f, realtimeSinceStartup - tic))
		{
			Debug.Log("We were connected to game server before level finished loading.");
		}
		Debug.Log(string.Format("Needed to wait {0} additional seconds for Photon to connect to game server.", realtimeSinceStartup - tic));
		if (yieldForCharacterSpawn)
		{
			yield return this.WaitForCharacterSpawn(120f);
		}
		yield return new WaitForSecondsRealtime(extraYieldTimeOnEnd);
		yield break;
	}

	// Token: 0x06000A09 RID: 2569 RVA: 0x00035BD5 File Offset: 0x00033DD5
	private IEnumerator LoadSceneProcessOffline(string sceneName, bool yieldForCharacterSpawn, float extraYieldTimeOnEnd)
	{
		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
		if (!operation.isDone)
		{
			Debug.Log("Waiting for scene loading...");
		}
		while (!operation.isDone)
		{
			yield return null;
		}
		if (PhotonNetwork.NetworkClientState == ClientState.ConnectingToGameServer)
		{
			Debug.Log("Waiting while connecting...");
		}
		else
		{
			Debug.Log("Scene load complete and no need to wait for Photon connect.");
		}
		while (PhotonNetwork.NetworkClientState == ClientState.ConnectingToGameServer)
		{
			yield return null;
		}
		if (yieldForCharacterSpawn)
		{
			yield return this.WaitForCharacterSpawn(120f);
		}
		yield return new WaitForSecondsRealtime(extraYieldTimeOnEnd);
		yield break;
	}

	// Token: 0x06000A0A RID: 2570 RVA: 0x00035BF9 File Offset: 0x00033DF9
	private IEnumerator WaitForCharacterSpawn(float timeout = 120f)
	{
		if (!PhotonNetwork.IsMessageQueueRunning)
		{
			Debug.LogWarning("OOPS! Message queue was disabled. We'll need that to receive our spawn request");
			PhotonNetwork.IsMessageQueueRunning = true;
		}
		Debug.Log("Level loaded and Photon connected! Just waiting for Character to spawn...");
		float tic = Time.realtimeSinceStartup;
		while (!Character.localCharacter || !PhotonNetwork.InRoom)
		{
			if (Time.realtimeSinceStartup - tic > timeout)
			{
				Debug.LogError(string.Format("Waited to spawn for {0} seconds and it didn't happen. Giving up", timeout));
				yield break;
			}
			yield return null;
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		Debug.Log(string.Format("It took {0} seconds for player's local character to spawn.", realtimeSinceStartup - tic));
		yield break;
	}

	// Token: 0x04000960 RID: 2400
	private LoadingScreen _lastActiveLoadingScreen;

	// Token: 0x04000961 RID: 2401
	public LoadingScreen loadingScreenPrefabBasic;

	// Token: 0x04000962 RID: 2402
	public LoadingScreen loadingScreenPrefabPlane;

	// Token: 0x04000963 RID: 2403
	private static bool _loading;

	// Token: 0x04000964 RID: 2404
	private Dictionary<LoadingScreen.LoadingScreenType, LoadingScreen> loadingScreens;
}

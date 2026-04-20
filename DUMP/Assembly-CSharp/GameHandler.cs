using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zorro.Core;

// Token: 0x02000084 RID: 132
[DefaultExecutionOrder(-100)]
public class GameHandler : MonoBehaviour
{
	// Token: 0x1700006E RID: 110
	// (get) Token: 0x06000599 RID: 1433 RVA: 0x0002084B File Offset: 0x0001EA4B
	public static GameHandler Instance
	{
		get
		{
			return GameHandler._instance;
		}
	}

	// Token: 0x1700006F RID: 111
	// (get) Token: 0x0600059A RID: 1434 RVA: 0x00020852 File Offset: 0x0001EA52
	// (set) Token: 0x0600059B RID: 1435 RVA: 0x0002085A File Offset: 0x0001EA5A
	public SettingsHandler SettingsHandler { get; private set; }

	// Token: 0x17000070 RID: 112
	// (get) Token: 0x0600059C RID: 1436 RVA: 0x00020863 File Offset: 0x0001EA63
	public static bool PlayersHaveLeftShore
	{
		get
		{
			return GameHandler.IsOnIsland && (Singleton<MapHandler>.Instance.GetCurrentSegment() != Segment.Beach || OrbFogHandler.IsFoggingCurrentSegment);
		}
	}

	// Token: 0x17000071 RID: 113
	// (get) Token: 0x0600059D RID: 1437 RVA: 0x00020881 File Offset: 0x0001EA81
	public static bool IsOnIsland
	{
		get
		{
			return GameHandler.Instance != null && MapHandler.Exists;
		}
	}

	// Token: 0x17000072 RID: 114
	// (get) Token: 0x0600059E RID: 1438 RVA: 0x00020897 File Offset: 0x0001EA97
	public static bool IsOnIslandAndInitialized
	{
		get
		{
			return GameHandler.Instance != null && MapHandler.ExistsAndInitialized;
		}
	}

	// Token: 0x17000073 RID: 115
	// (get) Token: 0x0600059F RID: 1439 RVA: 0x000208AD File Offset: 0x0001EAAD
	public static bool Initialized
	{
		get
		{
			return GameHandler.Instance != null && GameHandler.Instance.m_initialized;
		}
	}

	// Token: 0x060005A0 RID: 1440 RVA: 0x000208C8 File Offset: 0x0001EAC8
	public void Initialize()
	{
		Debug.Log("Game Handler Initialized");
		GameHandler._instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x060005A1 RID: 1441 RVA: 0x000208E8 File Offset: 0x0001EAE8
	private void OnDestroy()
	{
		Debug.Log("Game Handler Destroying...");
		foreach (GameService gameService in this.m_gameServices.Values)
		{
			gameService.OnDestroy();
		}
		SceneManager.sceneLoaded -= this.OnSceneLoaded;
	}

	// Token: 0x060005A2 RID: 1442 RVA: 0x00020958 File Offset: 0x0001EB58
	private bool IsGameplayScene(Scene scene)
	{
		return scene.name.Contains("Island") || scene.name.Contains("Level_");
	}

	// Token: 0x17000074 RID: 116
	// (get) Token: 0x060005A3 RID: 1443 RVA: 0x00020980 File Offset: 0x0001EB80
	public static bool IsInGameplayScene
	{
		get
		{
			return GameHandler.Instance.IsGameplayScene(SceneManager.GetActiveScene());
		}
	}

	// Token: 0x060005A4 RID: 1444 RVA: 0x00020994 File Offset: 0x0001EB94
	private void Awake()
	{
		GameHandler.<Awake>d__23 <Awake>d__;
		<Awake>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Awake>d__.<>4__this = this;
		<Awake>d__.<>1__state = -1;
		<Awake>d__.<>t__builder.Start<GameHandler.<Awake>d__23>(ref <Awake>d__);
	}

	// Token: 0x060005A5 RID: 1445 RVA: 0x000209CC File Offset: 0x0001EBCC
	private void OnNewSession()
	{
		Debug.Log("Clearing stale session data!");
		foreach (GameService gameService in this.m_gameServices.Values)
		{
			gameService.ClearSessionData();
		}
	}

	// Token: 0x060005A6 RID: 1446 RVA: 0x00020A2C File Offset: 0x0001EC2C
	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		Debug.Log("Clearing stale Scene data!");
		if (Singleton<ReconnectHandler>.Instance)
		{
			Singleton<ReconnectHandler>.Instance.Clear();
		}
		foreach (GameService gameService in this.m_gameServices.Values)
		{
			gameService.ClearSceneData();
		}
		if (scene.name == "Title")
		{
			this.OnNewSession();
		}
	}

	// Token: 0x060005A7 RID: 1447 RVA: 0x00020ABC File Offset: 0x0001ECBC
	private void RegisterService<T>(T service) where T : GameService
	{
		Type type = service.GetType();
		this.m_gameServices[type] = service;
	}

	// Token: 0x060005A8 RID: 1448 RVA: 0x00020AE7 File Offset: 0x0001ECE7
	public static T GetService<T>() where T : GameService
	{
		GameHandler instance = GameHandler.Instance;
		return ((instance != null) ? instance.m_gameServices[typeof(T)] : null) as T;
	}

	// Token: 0x060005A9 RID: 1449 RVA: 0x00020B14 File Offset: 0x0001ED14
	public static Awaitable WaitForInitialization()
	{
		GameHandler.<WaitForInitialization>d__28 <WaitForInitialization>d__;
		<WaitForInitialization>d__.<>t__builder = Awaitable.AwaitableAsyncMethodBuilder.Create();
		<WaitForInitialization>d__.<>1__state = -1;
		<WaitForInitialization>d__.<>t__builder.Start<GameHandler.<WaitForInitialization>d__28>(ref <WaitForInitialization>d__);
		return <WaitForInitialization>d__.<>t__builder.Task;
	}

	// Token: 0x060005AA RID: 1450 RVA: 0x00020B50 File Offset: 0x0001ED50
	private static T RestartService<T>(T service) where T : GameService, IDisposable
	{
		Debug.Log("Restarting Service of type: " + typeof(T).Name);
		Type type = service.GetType();
		if (GameHandler.Instance.m_gameServices.ContainsKey(type))
		{
			((T)((object)GameHandler.Instance.m_gameServices[type])).Dispose();
		}
		GameHandler.Instance.m_gameServices[type] = service;
		return service;
	}

	// Token: 0x060005AB RID: 1451 RVA: 0x00020BD0 File Offset: 0x0001EDD0
	public static void AddStatus<T>(GameStatus status) where T : GameStatus
	{
		Type type = status.GetType();
		GameHandler.Instance.m_gameStatus[type] = status;
		Debug.Log(string.Format("Add status: {0}", type));
		SceneSwitchingStatus sceneSwitchingStatus = status as SceneSwitchingStatus;
	}

	// Token: 0x060005AC RID: 1452 RVA: 0x00020C0C File Offset: 0x0001EE0C
	public static bool TryGetStatus<T>(out T status) where T : GameStatus
	{
		Type typeFromHandle = typeof(T);
		GameStatus gameStatus;
		bool flag = GameHandler.Instance.m_gameStatus.TryGetValue(typeFromHandle, out gameStatus);
		status = default(T);
		if (flag)
		{
			status = (gameStatus as T);
		}
		return flag;
	}

	// Token: 0x060005AD RID: 1453 RVA: 0x00020C54 File Offset: 0x0001EE54
	public static void ClearStatus<T>() where T : GameStatus
	{
		Type typeFromHandle = typeof(T);
		if (GameHandler.Instance.m_gameStatus.ContainsKey(typeFromHandle))
		{
			GameHandler.Instance.m_gameStatus.Remove(typeFromHandle);
			Debug.Log(string.Format("Clear status: {0}", typeFromHandle));
		}
	}

	// Token: 0x060005AE RID: 1454 RVA: 0x00020C9F File Offset: 0x0001EE9F
	public static void ClearAllStatuses()
	{
		GameHandler.Instance.m_gameStatus.Clear();
		Debug.Log("Clearing all statuses!");
	}

	// Token: 0x060005AF RID: 1455 RVA: 0x00020CBC File Offset: 0x0001EEBC
	private void Update()
	{
		this.SettingsHandler.Update();
		foreach (GameService gameService in this.m_gameServices.Values)
		{
			gameService.Update();
		}
		Debug.ClearDeveloperConsole();
	}

	// Token: 0x040005BE RID: 1470
	private static GameHandler _instance;

	// Token: 0x040005BF RID: 1471
	private Dictionary<Type, GameService> m_gameServices;

	// Token: 0x040005C1 RID: 1473
	private bool m_initialized;

	// Token: 0x040005C2 RID: 1474
	private Dictionary<Type, GameStatus> m_gameStatus;
}

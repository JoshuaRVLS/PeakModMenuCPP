using System;
using Peak;
using Peak.Network;
using Peak.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000083 RID: 131
public static class GameBooter
{
	// Token: 0x06000591 RID: 1425 RVA: 0x00020690 File Offset: 0x0001E890
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
	public static void Initialize()
	{
		IntegratedGraphicsHelper.DisableGPUCullingIfNecessary();
		Application.quitting += GameBooter.CleanUp;
		SceneManager.sceneLoaded += GameBooter.OnSceneLoaded;
		SceneManager.sceneLoaded += GameBooter.AutoBoot;
		NetCode.Matchmaking.InviteReceived += GameBooter.CheckForInviteAndJoinWhenReady;
	}

	// Token: 0x06000592 RID: 1426 RVA: 0x000206EB File Offset: 0x0001E8EB
	private static void CleanUp()
	{
		Application.quitting -= GameBooter.CleanUp;
		SceneManager.sceneLoaded -= GameBooter.OnSceneLoaded;
	}

	// Token: 0x06000593 RID: 1427 RVA: 0x0002070F File Offset: 0x0001E90F
	private static void OnSceneLoaded(Scene sceneLoaded, LoadSceneMode _)
	{
		if (sceneLoaded.name == "Title")
		{
			GameBooter.CheckForInviteAndJoinWhenReady();
		}
	}

	// Token: 0x06000594 RID: 1428 RVA: 0x00020729 File Offset: 0x0001E929
	private static void JoinLobbyAfterRelayReady()
	{
		NetCode.Session.ConnectedAndReady -= GameBooter.JoinLobbyAfterRelayReady;
		GameBooter.CheckForInviteAndJoinWhenReady();
	}

	// Token: 0x06000595 RID: 1429 RVA: 0x00020748 File Offset: 0x0001E948
	private static void CheckForInviteAndJoinWhenReady()
	{
		if (!NetCode.Session.IsConnectedAndReady)
		{
			NetCode.ConnectionEvents.ConnectedAndReady += GameBooter.JoinLobbyAfterRelayReady;
			return;
		}
		if (NetCode.Matchmaking.HasPendingInvite)
		{
			if (!Quicksave.Exists)
			{
				NetCode.Matchmaking.ConsumePendingJoin(true);
				return;
			}
			string savedRunId = Quicksave.SavedRunId;
			string a;
			if (!NetCode.Matchmaking.TryGetLobbyData(NetCode.Matchmaking.InvitedLobbyId, "RunId", out a))
			{
				Debug.LogWarning("Unable to fetch the Run ID from lobby data. Something is broken!");
			}
			if (a == savedRunId)
			{
				NetCode.Matchmaking.ConsumePendingJoin(true);
			}
			ConfirmPage.Open("SAVE_DESTROY_ON_JOIN", new Action(GameBooter.OnJoinConfirm), new Action(GameBooter.OnJoinCancel));
		}
	}

	// Token: 0x06000596 RID: 1430 RVA: 0x000207FA File Offset: 0x0001E9FA
	private static void OnJoinConfirm()
	{
		Quicksave.DestroySaveData();
		NetCode.Matchmaking.ConsumePendingJoin(true);
	}

	// Token: 0x06000597 RID: 1431 RVA: 0x0002080C File Offset: 0x0001EA0C
	private static void OnJoinCancel()
	{
		NetCode.Matchmaking.ConsumePendingJoin(false);
	}

	// Token: 0x06000598 RID: 1432 RVA: 0x00020819 File Offset: 0x0001EA19
	private static void AutoBoot(Scene _, LoadSceneMode __)
	{
		SceneManager.sceneLoaded -= GameBooter.AutoBoot;
		GameObject gameObject = new GameObject("Game");
		gameObject.AddComponent<GameHandler>().Initialize();
		gameObject.AddComponent<UIInputHandler>().Initialize();
	}

	// Token: 0x040005BD RID: 1469
	private const string DESTROY_QUICKSAVE_KEY = "SAVE_DESTROY_ON_JOIN";
}

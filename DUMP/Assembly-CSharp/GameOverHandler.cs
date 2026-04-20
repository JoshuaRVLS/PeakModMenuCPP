using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x020000BD RID: 189
public class GameOverHandler : Singleton<GameOverHandler>
{
	// Token: 0x06000719 RID: 1817 RVA: 0x00028B18 File Offset: 0x00026D18
	protected override void Awake()
	{
		base.Awake();
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x0600071A RID: 1818 RVA: 0x00028B2C File Offset: 0x00026D2C
	public void LocalPlayerHasClosedEndScreen()
	{
		this.view.RPC("PlayerHasClosedEndScreen", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x0600071B RID: 1819 RVA: 0x00028B44 File Offset: 0x00026D44
	[PunRPC]
	public void PlayerHasClosedEndScreen(PhotonMessageInfo messageInfo)
	{
		int actorNumber = messageInfo.Sender.ActorNumber;
		Player player;
		if (!PlayerHandler.TryGetPlayer(actorNumber, out player))
		{
			Debug.LogError(string.Format("Player not found: {0}", actorNumber));
			return;
		}
		player.hasClosedEndScreen = true;
		Debug.Log(string.Format("{0} Player has closed end screen", player));
	}

	// Token: 0x0600071C RID: 1820 RVA: 0x00028B94 File Offset: 0x00026D94
	public void LoadAirport()
	{
		this.view.RPC("LoadAirportMaster", RpcTarget.MasterClient, Array.Empty<object>());
	}

	// Token: 0x0600071D RID: 1821 RVA: 0x00028BAC File Offset: 0x00026DAC
	[PunRPC]
	public void LoadAirportMaster()
	{
		this.view.RPC("BeginAirportLoadRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x0600071E RID: 1822 RVA: 0x00028BC4 File Offset: 0x00026DC4
	[PunRPC]
	public void BeginAirportLoadRPC()
	{
		Debug.Log("Load Island RPC..");
		SceneSwitchingStatus sceneSwitchingStatus;
		if (GameHandler.TryGetStatus<SceneSwitchingStatus>(out sceneSwitchingStatus))
		{
			Debug.Log("Already loading... ");
			return;
		}
		GameHandler.AddStatus<SceneSwitchingStatus>(new SceneSwitchingStatus());
		RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Basic, null, new IEnumerator[]
		{
			RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess("Airport", true, true, 0f)
		});
	}

	// Token: 0x0600071F RID: 1823 RVA: 0x00028C24 File Offset: 0x00026E24
	public void ForceEveryPlayerDoneWithEndScreen()
	{
		this.view.RPC("ForceEveryPlayerDoneWithEndScreenRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06000720 RID: 1824 RVA: 0x00028C3C File Offset: 0x00026E3C
	[PunRPC]
	public void ForceEveryPlayerDoneWithEndScreenRPC()
	{
		Debug.Log("Force every player closed end screen");
		foreach (Player player in PlayerHandler.GetAllPlayers())
		{
			player.hasClosedEndScreen = true;
		}
	}

	// Token: 0x04000717 RID: 1815
	private PhotonView view;
}

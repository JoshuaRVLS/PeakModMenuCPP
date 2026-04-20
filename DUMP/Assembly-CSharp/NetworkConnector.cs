using System;
using System.Collections;
using System.Linq;
using System.Text;
using Peak.Network;
using Photon.Pun;
using Photon.Realtime;
using Unity.Multiplayer.Playmode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zorro.Core.CLI;
using Zorro.UI.Modal;

// Token: 0x0200014C RID: 332
public class NetworkConnector : MonoBehaviourPunCallbacks
{
	// Token: 0x170000C9 RID: 201
	// (get) Token: 0x06000B07 RID: 2823 RVA: 0x0003B5E8 File Offset: 0x000397E8
	private static bool UsingEditorBootstrapping
	{
		get
		{
			return CurrentPlayer.ReadOnlyTags().Contains("Client") || (CurrentPlayer.ReadOnlyTags().Contains("Player1") && !PhotonNetwork.IsConnected) || NetworkConnector.CurrentConnectionState is DefaultConnectionState;
		}
	}

	// Token: 0x170000CA RID: 202
	// (get) Token: 0x06000B08 RID: 2824 RVA: 0x0003B622 File Offset: 0x00039822
	public static ConnectionState CurrentConnectionState
	{
		get
		{
			return GameHandler.GetService<ConnectionService>().StateMachine.CurrentState;
		}
	}

	// Token: 0x06000B09 RID: 2825 RVA: 0x0003B633 File Offset: 0x00039833
	public static void ChangeConnectionState<T>() where T : ConnectionState
	{
		GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<T>(false);
	}

	// Token: 0x170000CB RID: 203
	// (get) Token: 0x06000B0A RID: 2826 RVA: 0x0003B646 File Offset: 0x00039846
	// (set) Token: 0x06000B0B RID: 2827 RVA: 0x0003B64D File Offset: 0x0003984D
	public static bool IsInitialized { get; private set; }

	// Token: 0x170000CC RID: 204
	// (get) Token: 0x06000B0C RID: 2828 RVA: 0x0003B655 File Offset: 0x00039855
	public static bool ReadyToHostOrJoinRoom
	{
		get
		{
			return NetworkConnector.IsInitialized && PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer && !(NetworkConnector.CurrentConnectionState is DefaultConnectionState);
		}
	}

	// Token: 0x06000B0D RID: 2829 RVA: 0x0003B67C File Offset: 0x0003987C
	private void Start()
	{
		ConnectionState currentState = GameHandler.GetService<ConnectionService>().StateMachine.CurrentState;
		Debug.Log("Network Connector is starting in scene: " + SceneManager.GetActiveScene().name + ". \n" + string.Format("State is currently {0}.", currentState.GetType()));
		if (currentState is InRoomState)
		{
			foreach (global::Player player in PlayerHandler.GetAllPlayers())
			{
				player.hasClosedEndScreen = false;
			}
			if (PhotonNetwork.IsMasterClient)
			{
				GameHandler.GetService<SteamLobbyHandler>().SetLobbyData();
			}
			if (this.keepSettingLobbyDataCoroutine == null)
			{
				this.keepSettingLobbyDataCoroutine = base.StartCoroutine(this.KeepSettingLobbyData());
			}
			NetworkConnector.IsInitialized = true;
			return;
		}
		NetworkConnector.IsInitialized = true;
		if (NetworkConnector.ReadyToHostOrJoinRoom)
		{
			NetworkConnector.HandleConnectionState(NetworkConnector.CurrentConnectionState);
			return;
		}
		if (PhotonNetwork.OfflineMode)
		{
			NetworkingUtilities.JoinDummyRoom();
			return;
		}
		if (!NetworkConnector.UsingEditorBootstrapping)
		{
			Debug.LogError("NetworkConnector is initialized, but it can't connect to a room! Something is wrong. Dumping Networking state.", this);
			NetworkConnector.PrintNetworkStates();
		}
	}

	// Token: 0x06000B0E RID: 2830 RVA: 0x0003B780 File Offset: 0x00039980
	private void OnDestroy()
	{
		NetworkConnector.IsInitialized = false;
	}

	// Token: 0x06000B0F RID: 2831 RVA: 0x0003B788 File Offset: 0x00039988
	private static void HandleConnectionState(ConnectionState state)
	{
		HostState hostState = state as HostState;
		if (hostState != null)
		{
			RoomOptions roomOptions = NetworkingUtilities.HostRoomOptions();
			PhotonNetwork.CreateRoom(hostState.RoomName, roomOptions, null, null);
			return;
		}
		JoinSpecificRoomState joinSpecificRoomState = state as JoinSpecificRoomState;
		if (joinSpecificRoomState == null)
		{
			Debug.LogWarning(string.Format("No logic for handling state {0}", NetworkConnector.CurrentConnectionState.GetType()));
			return;
		}
		Debug.Log(string.Concat(new string[]
		{
			"$Connecting to specific region: ",
			joinSpecificRoomState.RegionToJoin,
			" with app ID ",
			PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime,
			". Is currently connected to: ",
			PhotonNetwork.CloudRegion
		}));
		if (PhotonNetwork.CloudRegion != joinSpecificRoomState.RegionToJoin && !string.IsNullOrEmpty(joinSpecificRoomState.RegionToJoin))
		{
			Debug.Log("Changing regions from " + PhotonNetwork.CloudRegion + " and reconnecting to " + joinSpecificRoomState.RegionToJoin);
			PhotonNetwork.Disconnect();
			PhotonNetwork.ConnectToRegion(joinSpecificRoomState.RegionToJoin);
			return;
		}
		PhotonNetwork.JoinRoom(joinSpecificRoomState.RoomName, null);
	}

	// Token: 0x06000B10 RID: 2832 RVA: 0x0003B884 File Offset: 0x00039A84
	private IEnumerator KeepSettingLobbyData()
	{
		while (PhotonNetwork.InRoom)
		{
			if (GameHandler.GetService<SteamLobbyHandler>().InSteamLobby() && PhotonNetwork.IsMasterClient)
			{
				GameHandler.GetService<SteamLobbyHandler>().SetLobbyData();
				Debug.Log("IS master, is updating lobby data");
			}
			yield return new WaitForSecondsRealtime(100f);
		}
		yield break;
	}

	// Token: 0x06000B11 RID: 2833 RVA: 0x0003B88C File Offset: 0x00039A8C
	public override void OnConnectedToMaster()
	{
		if (!NetworkConnector.UsingEditorBootstrapping)
		{
			Debug.LogWarning("Reconnect detected! Dumping network state and then attempting to recover current connection state.");
			NetworkConnector.PrintNetworkStates();
		}
		NetworkConnector.HandleConnectionState(NetworkConnector.CurrentConnectionState);
	}

	// Token: 0x06000B12 RID: 2834 RVA: 0x0003B8B0 File Offset: 0x00039AB0
	[ConsoleCommand(true)]
	public static void PrintNetworkStates()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("<color=#" + ColorUtility.ToHtmlStringRGBA(Color.magenta) + ">[[[ DUMPING NETWORK STATES ]]]</color>");
		stringBuilder.AppendLine(string.Format("Services || Connection: {0} ", NetworkConnector.CurrentConnectionState.GetType()) + "| SteamLobby.LobbyID: " + NetCode.Matchmaking.LobbyId);
		stringBuilder.AppendLine(string.Format("NetworkConnector || Initialized: {0} | Ready: {1}", NetworkConnector.IsInitialized, NetworkConnector.ReadyToHostOrJoinRoom));
		stringBuilder.Append(string.Format("PhotonNetwork || Connected: {0} | Ready: {1} ", PhotonNetwork.IsConnected, PhotonNetwork.IsConnectedAndReady) + "| ClientState: " + Enum.GetName(typeof(ClientState), PhotonNetwork.NetworkClientState));
		Debug.Log(stringBuilder.ToString());
	}

	// Token: 0x06000B13 RID: 2835 RVA: 0x0003B988 File Offset: 0x00039B88
	public override void OnLeftRoom()
	{
		base.OnLeftRoom();
		SceneManager.LoadScene(NetworkConnector.rejoinScene);
		NetworkConnector.rejoinScene = "Title";
	}

	// Token: 0x06000B14 RID: 2836 RVA: 0x0003B9A4 File Offset: 0x00039BA4
	public override void OnCreatedRoom()
	{
		base.OnCreatedRoom();
		GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<InRoomState>(false);
		if (!NetCode.Matchmaking.InLobby)
		{
			return;
		}
		SteamLobbyHandler service = GameHandler.GetService<SteamLobbyHandler>();
		if (PhotonNetwork.IsMasterClient)
		{
			service.SetLobbyData();
		}
		if (this.keepSettingLobbyDataCoroutine == null)
		{
			this.keepSettingLobbyDataCoroutine = base.StartCoroutine(this.KeepSettingLobbyData());
		}
	}

	// Token: 0x06000B15 RID: 2837 RVA: 0x0003BA02 File Offset: 0x00039C02
	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		base.OnCreateRoomFailed(returnCode, message);
		Debug.LogError(string.Format("Failed to create Photon Room, code: {0}, message: {1}", returnCode, message));
	}

	// Token: 0x06000B16 RID: 2838 RVA: 0x0003BA24 File Offset: 0x00039C24
	public override void OnDisconnected(DisconnectCause cause)
	{
		base.OnDisconnected(cause);
		if (PhotonNetwork.OfflineMode)
		{
			return;
		}
		if (cause == DisconnectCause.DisconnectByClientLogic)
		{
			return;
		}
		Debug.LogError(string.Format("Disconnected from Photon Server: {0}", cause));
		NetworkConnector.ChangeConnectionState<DefaultConnectionState>();
		HeaderModalOption headerContent = new DefaultHeaderModalOption(LocalizedText.GetText("MODAL_DISCONNECTEDPHOTON_TITLE", true), LocalizedText.GetText("MODAL_DISCONNECTEDPHOTON_BODY", true).Replace("#", cause.ToString()));
		ModalButtonsOption.Option[] array = new ModalButtonsOption.Option[1];
		array[0] = new ModalButtonsOption.Option(LocalizedText.GetText("OK", true), delegate()
		{
			SceneManager.LoadScene("Title");
		});
		Modal.OpenModal(headerContent, new ModalButtonsOption(array), null);
	}

	// Token: 0x06000B17 RID: 2839 RVA: 0x0003BAD8 File Offset: 0x00039CD8
	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		if (CurrentPlayer.ReadOnlyTags().Contains("Client") && message != "KICKED")
		{
			JoinSpecificRoomState joinSpecificRoomState = NetworkConnector.CurrentConnectionState as JoinSpecificRoomState;
			if (joinSpecificRoomState != null)
			{
				Debug.Log("Failed to join " + joinSpecificRoomState.RoomName + ". Attempting again");
				return;
			}
			Debug.Log("Failed to join room and not in the right state!");
		}
		base.OnJoinRoomFailed(returnCode, message);
		if (message == "KICKED")
		{
			NetworkConnector.ChangeConnectionState<KickedState>();
			return;
		}
		Debug.LogError(string.Format("Failed to join Photon Room, code: {0}, message: {1}", returnCode, message));
		GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<DefaultConnectionState>(false);
		HeaderModalOption headerContent = new DefaultHeaderModalOption(LocalizedText.GetText("MODAL_FAILEDPHOTON_TITLE", true), string.Format("[{0}] {1}", returnCode, message));
		ModalButtonsOption.Option[] array = new ModalButtonsOption.Option[1];
		array[0] = new ModalButtonsOption.Option(LocalizedText.GetText("OK", true), delegate()
		{
			SceneManager.LoadScene("Title");
		});
		Modal.OpenModal(headerContent, new ModalButtonsOption(array), null);
	}

	// Token: 0x06000B18 RID: 2840 RVA: 0x0003BBDC File Offset: 0x00039DDC
	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		base.OnJoinRandomFailed(returnCode, message);
		Debug.LogError(string.Format("Failed to join Random Photon Room, code: {0}, message: {1}", returnCode, message));
	}

	// Token: 0x06000B19 RID: 2841 RVA: 0x0003BBFC File Offset: 0x00039DFC
	public override void OnJoinedRoom()
	{
		if (Character.localCharacter != null)
		{
			Debug.Log(string.Format("On Joined Photon Room. UserId:{0}, rejoined: {1}", Character.localCharacter.photonView.Owner.UserId, Character.localCharacter.photonView.Owner.HasRejoined));
		}
		else
		{
			Debug.Log("On Joined Photon Room. No Character");
		}
		GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<InRoomState>(false);
	}

	// Token: 0x04000A36 RID: 2614
	private Coroutine keepSettingLobbyDataCoroutine;

	// Token: 0x04000A38 RID: 2616
	private static string rejoinScene = "Title";
}

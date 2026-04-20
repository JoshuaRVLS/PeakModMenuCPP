using System;
using System.Collections;
using System.Linq;
using System.Text;
using Peak;
using Photon.Pun;
using Steamworks;
using Unity.Collections;
using Unity.Multiplayer.Playmode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zorro.Core;
using Zorro.Core.Serizalization;
using Zorro.UI.Modal;

// Token: 0x0200008E RID: 142
public class SteamLobbyHandler : GameService
{
	// Token: 0x17000075 RID: 117
	// (get) Token: 0x060005CB RID: 1483 RVA: 0x000211E4 File Offset: 0x0001F3E4
	// (set) Token: 0x060005CC RID: 1484 RVA: 0x000211EC File Offset: 0x0001F3EC
	public CSteamID LobbySteamId { get; private set; }

	// Token: 0x17000076 RID: 118
	// (get) Token: 0x060005CD RID: 1485 RVA: 0x000211F5 File Offset: 0x0001F3F5
	// (set) Token: 0x060005CE RID: 1486 RVA: 0x000211FC File Offset: 0x0001F3FC
	private static bool HasCheckedSteamLaunchCommand { get; set; }

	// Token: 0x17000077 RID: 119
	// (get) Token: 0x060005CF RID: 1487 RVA: 0x00021204 File Offset: 0x0001F404
	public string CurrentLobbyID
	{
		get
		{
			return this.LobbySteamId.ToString();
		}
	}

	// Token: 0x060005D0 RID: 1488 RVA: 0x00021228 File Offset: 0x0001F428
	public SteamLobbyHandler()
	{
		Debug.Log("Steam Lobby Handler initialized");
		Callback<LobbyCreated_t>.Create(new Callback<LobbyCreated_t>.DispatchDelegate(this.OnLobbyCreated));
		Callback<LobbyEnter_t>.Create(new Callback<LobbyEnter_t>.DispatchDelegate(this.OnLobbyEnter));
		Callback<LobbyDataUpdate_t>.Create(new Callback<LobbyDataUpdate_t>.DispatchDelegate(this.OnLobbyDataUpdate));
		Callback<LobbyChatMsg_t>.Create(new Callback<LobbyChatMsg_t>.DispatchDelegate(this.OnLobbyChat));
		this.LobbySteamId = CSteamID.Nil;
	}

	// Token: 0x060005D1 RID: 1489 RVA: 0x000212A4 File Offset: 0x0001F4A4
	public bool TryConsumeLaunchCommandInvite(out CSteamID invite)
	{
		invite = default(CSteamID);
		if (SteamLobbyHandler.HasCheckedSteamLaunchCommand)
		{
			return false;
		}
		SteamLobbyHandler.HasCheckedSteamLaunchCommand = true;
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		if (commandLineArgs.Length < 2)
		{
			return false;
		}
		for (int i = 0; i < commandLineArgs.Length - 1; i++)
		{
			ulong num;
			if (!(commandLineArgs[i].ToLower() != "+connect_lobby") && ulong.TryParse(commandLineArgs[i + 1], out num) && num > 0UL)
			{
				invite = new CSteamID(num);
				return true;
			}
		}
		return false;
	}

	// Token: 0x060005D2 RID: 1490 RVA: 0x0002131C File Offset: 0x0001F51C
	public void CheckForSteamInviteAndConnect()
	{
		if (SteamLobbyHandler.HasCheckedSteamLaunchCommand)
		{
			return;
		}
		SteamLobbyHandler.HasCheckedSteamLaunchCommand = true;
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		if (commandLineArgs.Length < 2)
		{
			return;
		}
		for (int i = 0; i < commandLineArgs.Length - 1; i++)
		{
			ulong num;
			if (!(commandLineArgs[i].ToLower() != "+connect_lobby") && ulong.TryParse(commandLineArgs[i + 1], out num) && num > 0UL)
			{
				Debug.Log(string.Format("Steam invite detected! Attempting to connect to lobby {0}", num));
				this.RequestLobbyData(new CSteamID(num));
				return;
			}
		}
	}

	// Token: 0x060005D3 RID: 1491 RVA: 0x000213A0 File Offset: 0x0001F5A0
	private void OnLobbyEnter(LobbyEnter_t param)
	{
		if (this.m_isHosting)
		{
			this.m_isHosting = false;
			return;
		}
		if (param.m_EChatRoomEnterResponse != 1U)
		{
			this.LobbySteamId = CSteamID.Nil;
			return;
		}
		this.LobbySteamId = new CSteamID(param.m_ulSteamIDLobby);
		Debug.Log("Entered Steam Lobby: " + this.LobbySteamId.ToString());
		string lobbyData = SteamMatchmaking.GetLobbyData(this.LobbySteamId, "PhotonRegion");
		string lobbyData2 = SteamMatchmaking.GetLobbyData(this.LobbySteamId, "CurrentScene");
		if (!string.IsNullOrEmpty(lobbyData))
		{
			this.tryingToFetchLobbyDataAttempts = Optionable<int>.None;
			this.m_currentlyWaitingForRoomID = Optionable<ValueTuple<CSteamID, string, string>>.Some(new ValueTuple<CSteamID, string, string>(this.LobbySteamId, lobbyData2, lobbyData));
			return;
		}
		if (this.tryingToFetchLobbyDataAttempts.IsNone)
		{
			this.tryingToFetchLobbyDataAttempts = Optionable<int>.Some(1);
		}
		else
		{
			this.tryingToFetchLobbyDataAttempts = Optionable<int>.Some(this.tryingToFetchLobbyDataAttempts.Value + 1);
		}
		Debug.LogError(string.Format("Failed to get lobby region, attempts: {0}", this.tryingToFetchLobbyDataAttempts.Value));
		if (this.tryingToFetchLobbyDataAttempts.Value < 5)
		{
			this.LeaveLobby();
			this.TryJoinLobby(new CSteamID(param.m_ulSteamIDLobby));
			return;
		}
		Debug.LogError("Failed to fetch steam lobby");
		this.LeaveLobby();
		Modal.OpenModal(new DefaultHeaderModalOption(LocalizedText.GetText("MODAL_CANTJOINLOBBY_TITLE", true), LocalizedText.GetText("MODAL_INVALIDLOBBY_BODY", true)), new ModalButtonsOption(new ModalButtonsOption.Option[]
		{
			new ModalButtonsOption.Option(LocalizedText.GetText("OK", true), null)
		}), null);
	}

	// Token: 0x060005D4 RID: 1492 RVA: 0x00021524 File Offset: 0x0001F724
	public override void Update()
	{
		base.Update();
		if (this.m_currentlyWaitingForRoomID.IsSome && this.m_currentlyRequestingRoomID.IsNone)
		{
			this.RequestPhotonRoomID();
		}
	}

	// Token: 0x060005D5 RID: 1493 RVA: 0x0002154C File Offset: 0x0001F74C
	private void RequestPhotonRoomID()
	{
		if (this.LobbySteamId == CSteamID.Nil)
		{
			Debug.LogError("Not in a lobby");
			return;
		}
		this.m_currentlyRequestingRoomID = Optionable<CSteamID>.Some(this.LobbySteamId);
		BinarySerializer binarySerializer = new BinarySerializer(256, Allocator.Temp);
		binarySerializer.WriteByte(1);
		byte[] array = binarySerializer.buffer.ToByteArray();
		binarySerializer.Dispose();
		if (!SteamMatchmaking.SendLobbyChatMsg(this.LobbySteamId, array, array.Length))
		{
			this.m_currentlyRequestingRoomID = Optionable<CSteamID>.None;
			Debug.LogError("Failed to request Room ID");
			return;
		}
		Debug.Log("Requested Room ID");
	}

	// Token: 0x060005D6 RID: 1494 RVA: 0x000215DC File Offset: 0x0001F7DC
	private void SendRoomID()
	{
		if (this.LobbySteamId == CSteamID.Nil)
		{
			Debug.LogError("Not in a lobby");
			return;
		}
		BinarySerializer binarySerializer = new BinarySerializer(256, Allocator.Temp);
		binarySerializer.WriteByte(2);
		binarySerializer.WriteString(PhotonNetwork.CurrentRoom.Name, Encoding.ASCII);
		byte[] array = binarySerializer.buffer.ToByteArray();
		binarySerializer.Dispose();
		if (!SteamMatchmaking.SendLobbyChatMsg(this.LobbySteamId, array, array.Length))
		{
			this.m_currentlyRequestingRoomID = Optionable<CSteamID>.None;
			Debug.LogError("Failed to send Room ID...");
			return;
		}
		Debug.Log("Lobby has been requested. Sending " + PhotonNetwork.CurrentRoom.Name);
	}

	// Token: 0x060005D7 RID: 1495 RVA: 0x00021680 File Offset: 0x0001F880
	private void OnLobbyChat(LobbyChatMsg_t param)
	{
		if (param.m_ulSteamIDLobby != this.LobbySteamId.m_SteamID)
		{
			Debug.LogError(string.Format("Received Chat Message from another lobby: {0}", param.m_ulSteamIDLobby));
			return;
		}
		if (param.m_ulSteamIDUser == SteamUser.GetSteamID().m_SteamID)
		{
			Debug.Log("Ignoring local chat message");
			return;
		}
		byte[] array = new byte[1024];
		CSteamID csteamID;
		EChatEntryType echatEntryType;
		if (SteamMatchmaking.GetLobbyChatEntry(this.LobbySteamId, (int)param.m_iChatID, out csteamID, array, array.Length, out echatEntryType) <= 0)
		{
			Debug.LogError("Failed to get chat message, no bytes written");
			return;
		}
		BinaryDeserializer binaryDeserializer = new BinaryDeserializer(array.ToNativeArray(Allocator.Temp));
		SteamLobbyHandler.MessageType messageType = (SteamLobbyHandler.MessageType)binaryDeserializer.ReadByte();
		if (messageType == SteamLobbyHandler.MessageType.INVALID)
		{
			Debug.LogError("Invalid message type");
		}
		else
		{
			this.HandleMessage(messageType, binaryDeserializer, new CSteamID(param.m_ulSteamIDLobby));
		}
		binaryDeserializer.Dispose();
	}

	// Token: 0x060005D8 RID: 1496 RVA: 0x0002174C File Offset: 0x0001F94C
	private void HandleMessage(SteamLobbyHandler.MessageType messageType, BinaryDeserializer deserializer, CSteamID lobbyID)
	{
		if (messageType == SteamLobbyHandler.MessageType.RequestRoomID)
		{
			if (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
			{
				this.SendRoomID();
				return;
			}
		}
		else if (messageType == SteamLobbyHandler.MessageType.RoomID)
		{
			if (this.m_currentlyRequestingRoomID.IsNone)
			{
				Debug.LogError("Not requesting room id, ignoring...");
				return;
			}
			if (this.m_currentlyRequestingRoomID.IsSome && this.m_currentlyRequestingRoomID.Value != lobbyID)
			{
				Debug.LogError("Got room id for wrong lobby");
				return;
			}
			string roomName = deserializer.ReadString(Encoding.ASCII);
			ValueTuple<CSteamID, string, string> value = this.m_currentlyWaitingForRoomID.Value;
			string text = value.Item2;
			string item = value.Item3;
			this.m_currentlyRequestingRoomID = Optionable<CSteamID>.None;
			this.m_currentlyWaitingForRoomID = Optionable<ValueTuple<CSteamID, string, string>>.None;
			if (string.IsNullOrEmpty(text))
			{
				text = "Airport";
				Debug.LogError("Failed to get scene to load, defaulting to airport");
			}
			JoinSpecificRoomState joinSpecificRoomState = GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<JoinSpecificRoomState>(false);
			joinSpecificRoomState.RoomName = roomName;
			joinSpecificRoomState.RegionToJoin = item;
			RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Basic, null, new IEnumerator[]
			{
				RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess(text, false, true, 3f)
			});
		}
	}

	// Token: 0x060005D9 RID: 1497 RVA: 0x00021858 File Offset: 0x0001FA58
	internal void RequestLobbyData(CSteamID lobbyId)
	{
		if (SteamMatchmaking.RequestLobbyData(lobbyId))
		{
			this.m_currentlyFetchingGameVersion = Optionable<CSteamID>.Some(lobbyId);
			return;
		}
		Modal.OpenModal(new DefaultHeaderModalOption(LocalizedText.GetText("MODAL_CANTFINDLOBBY_TITLE", true), LocalizedText.GetText("MODAL_FAILEDTOFETCH_BODY", true)), new ModalButtonsOption(new ModalButtonsOption.Option[]
		{
			new ModalButtonsOption.Option(LocalizedText.GetText("OK", true), null)
		}), null);
	}

	// Token: 0x060005DA RID: 1498 RVA: 0x000218BC File Offset: 0x0001FABC
	private void OnLobbyDataUpdate(LobbyDataUpdate_t param)
	{
		if (param.m_bSuccess != 1)
		{
			Debug.LogError("Failed to fetch lobby data");
			Modal.OpenModal(new DefaultHeaderModalOption(LocalizedText.GetText("MODAL_CANTFINDLOBBY_TITLE", true), LocalizedText.GetText("MODAL_CANTFINDLOBBY_BODY", true)), new ModalButtonsOption(new ModalButtonsOption.Option[]
			{
				new ModalButtonsOption.Option(LocalizedText.GetText("OK", true), null)
			}), null);
			this.m_currentlyFetchingGameVersion = Optionable<CSteamID>.None;
			return;
		}
		if (!this.m_currentlyFetchingGameVersion.IsSome || this.m_currentlyFetchingGameVersion.Value.m_SteamID != param.m_ulSteamIDLobby)
		{
			this.m_currentlyFetchingGameVersion = Optionable<CSteamID>.None;
			return;
		}
		string lobbyData = SteamMatchmaking.GetLobbyData(this.m_currentlyFetchingGameVersion.Value, "PeakVersion");
		if (lobbyData == new BuildVersion(Application.version, "???").ToMatchmaking())
		{
			if (PhotonNetwork.InRoom)
			{
				Debug.LogError("Not joining invite because your already in a room...");
				return;
			}
			this.JoinLobby(this.m_currentlyFetchingGameVersion.Value);
		}
		else
		{
			Debug.LogError("Game version mismatch: " + lobbyData);
			string subheader = LocalizedText.GetText("MODAL_MISMATCH_BODY", true).Replace("#", lobbyData).Replace("&", new BuildVersion(Application.version, "???").ToMatchmaking());
			Modal.OpenModal(new DefaultHeaderModalOption(LocalizedText.GetText("MODAL_MISMATCH_TITLE", true), subheader), new ModalButtonsOption(new ModalButtonsOption.Option[]
			{
				new ModalButtonsOption.Option(LocalizedText.GetText("OK", true), null)
			}), null);
		}
		if (this.m_currentlyFetchingGameVersion.IsSome)
		{
			this.m_currentlyFetchingGameVersion = Optionable<CSteamID>.None;
		}
	}

	// Token: 0x060005DB RID: 1499 RVA: 0x00021A49 File Offset: 0x0001FC49
	private void JoinLobby(CSteamID lobbyID)
	{
		this.LeaveLobby();
		Debug.Log(string.Format("Joining lobby: {0}", lobbyID));
		SteamMatchmaking.JoinLobby(lobbyID);
	}

	// Token: 0x060005DC RID: 1500 RVA: 0x00021A70 File Offset: 0x0001FC70
	public void TryJoinLobby(CSteamID lobbyID)
	{
		if (SteamMatchmaking.RequestLobbyData(lobbyID))
		{
			this.m_currentlyFetchingGameVersion = Optionable<CSteamID>.Some(lobbyID);
			return;
		}
		Modal.OpenModal(new DefaultHeaderModalOption(LocalizedText.GetText("MODAL_CANTJOINLOBBY_TITLE", true), LocalizedText.GetText("MODAL_FAILEDTOFETCH_BODY", true)), new ModalButtonsOption(new ModalButtonsOption.Option[]
		{
			new ModalButtonsOption.Option(LocalizedText.GetText("OK", true), null)
		}), null);
	}

	// Token: 0x060005DD RID: 1501 RVA: 0x00021AD4 File Offset: 0x0001FCD4
	private string GenerateRoomName()
	{
		if (!CurrentPlayer.ReadOnlyTags().Contains("Player1"))
		{
			return Guid.NewGuid().ToString();
		}
		return Environment.MachineName;
	}

	// Token: 0x060005DE RID: 1502 RVA: 0x00021B0C File Offset: 0x0001FD0C
	private void OnLobbyCreated(LobbyCreated_t param)
	{
		this.m_isHosting = true;
		if (param.m_eResult != EResult.k_EResultOK)
		{
			Modal.OpenModal(new DefaultHeaderModalOption(LocalizedText.GetText("MODAL_CANTCREATELOBBY_TITLE", true), string.Format("{0}", param.m_eResult)), new ModalButtonsOption(new ModalButtonsOption.Option[]
			{
				new ModalButtonsOption.Option(LocalizedText.GetText("OK", true), null)
			}), null);
			return;
		}
		Debug.Log(string.Format("Lobby Created: {0}", param.m_ulSteamIDLobby));
		this.LobbySteamId = new CSteamID(param.m_ulSteamIDLobby);
		if (!SteamMatchmaking.SetLobbyData(this.LobbySteamId, "PeakVersion", new BuildVersion(Application.version, "???").ToMatchmaking()))
		{
			Debug.LogError("Failed to assign game version to lobby");
		}
		GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<HostState>(false).RoomName = this.GenerateRoomName();
		if (Quicksave.ShouldUseSaveData)
		{
			Quicksave.LoadSavedGameScene();
			return;
		}
		RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Basic, null, new IEnumerator[]
		{
			RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess("Airport", false, true, 3f)
		});
	}

	// Token: 0x060005DF RID: 1503 RVA: 0x00021C28 File Offset: 0x0001FE28
	public void SetLobbyData()
	{
		if (PhotonNetwork.OfflineMode)
		{
			return;
		}
		if (this.LobbySteamId == CSteamID.Nil)
		{
			Debug.LogError("Failed to set lobby data, no lobby joined...");
			return;
		}
		if (!PhotonNetwork.InRoom)
		{
			Debug.LogError("Failed to set Lobby data. not in a photon room");
			return;
		}
		if (SteamMatchmaking.SetLobbyData(this.LobbySteamId, "PhotonRegion", PhotonNetwork.CloudRegion))
		{
			Debug.Log("Set Photon Region to steam lobby data: " + PhotonNetwork.CloudRegion);
		}
		else
		{
			Debug.LogError("Failed to set lobby data, returned not okay...");
		}
		string name = SceneManager.GetActiveScene().name;
		if (SteamMatchmaking.SetLobbyData(this.LobbySteamId, "CurrentScene", name))
		{
			Debug.Log("Set current scene to: " + name);
			return;
		}
		Debug.LogError("Failed to set lobby data, returned not okay...");
	}

	// Token: 0x060005E0 RID: 1504 RVA: 0x00021CE0 File Offset: 0x0001FEE0
	public void LeaveLobby()
	{
		this.m_currentlyWaitingForRoomID = Optionable<ValueTuple<CSteamID, string, string>>.None;
		this.m_currentlyRequestingRoomID = Optionable<CSteamID>.None;
		if (this.LobbySteamId != CSteamID.Nil)
		{
			Debug.Log("Leaving current lobby: " + this.LobbySteamId.ToString());
			SteamMatchmaking.LeaveLobby(this.LobbySteamId);
			this.LobbySteamId = CSteamID.Nil;
			return;
		}
		Debug.Log("Can't leave current lobby because not in a lobby");
	}

	// Token: 0x060005E1 RID: 1505 RVA: 0x00021D59 File Offset: 0x0001FF59
	public bool InSteamLobby()
	{
		return this.LobbySteamId != CSteamID.Nil;
	}

	// Token: 0x060005E2 RID: 1506 RVA: 0x00021D6B File Offset: 0x0001FF6B
	public bool InSteamLobby(out CSteamID lobbyID)
	{
		lobbyID = this.LobbySteamId;
		return this.LobbySteamId != CSteamID.Nil;
	}

	// Token: 0x040005CC RID: 1484
	private const string PHOTON_REGION_KEY = "PhotonRegion";

	// Token: 0x040005CD RID: 1485
	private const string GAME_VERSION_KEY = "PeakVersion";

	// Token: 0x040005CE RID: 1486
	private const string CURRENT_SCENE_KEY = "CurrentScene";

	// Token: 0x040005CF RID: 1487
	private const string RUN_ID_KEY = "RunId";

	// Token: 0x040005D0 RID: 1488
	private bool m_isHosting;

	// Token: 0x040005D2 RID: 1490
	private Optionable<CSteamID> m_currentlyFetchingGameVersion;

	// Token: 0x040005D3 RID: 1491
	private Optionable<CSteamID> m_currentlyRequestingRoomID;

	// Token: 0x040005D4 RID: 1492
	private Optionable<ValueTuple<CSteamID, string, string>> m_currentlyWaitingForRoomID;

	// Token: 0x040005D6 RID: 1494
	private Optionable<int> tryingToFetchLobbyDataAttempts = Optionable<int>.None;

	// Token: 0x0200043E RID: 1086
	public enum MessageType : byte
	{
		// Token: 0x04001897 RID: 6295
		INVALID,
		// Token: 0x04001898 RID: 6296
		RequestRoomID,
		// Token: 0x04001899 RID: 6297
		RoomID
	}
}

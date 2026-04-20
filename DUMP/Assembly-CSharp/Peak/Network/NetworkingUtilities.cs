using System;
using System.Linq;
using System.Text.RegularExpressions;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using Unity.Multiplayer.Playmode;
using UnityEngine;
using Zorro.Core;

namespace Peak.Network
{
	// Token: 0x020003DB RID: 987
	public static class NetworkingUtilities
	{
		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x06001A16 RID: 6678 RVA: 0x000823D9 File Offset: 0x000805D9
		public static int MAX_PLAYERS
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x06001A17 RID: 6679 RVA: 0x000823DC File Offset: 0x000805DC
		public static string Sanitize(string text)
		{
			return NetworkingUtilities._richText.Replace(text, string.Empty);
		}

		// Token: 0x06001A18 RID: 6680 RVA: 0x000823EE File Offset: 0x000805EE
		public static string GetUsername()
		{
			return NetworkingUtilities.Sanitize(SteamFriends.GetPersonaName());
		}

		// Token: 0x06001A19 RID: 6681 RVA: 0x000823FC File Offset: 0x000805FC
		private static string GetBestUserID()
		{
			string text;
			if (!CurrentPlayer.ReadOnlyTags().Contains("NoSteam") && SteamAPI.IsSteamRunning() && SteamUser.BLoggedOn())
			{
				text = SteamUser.GetSteamID().m_SteamID.ToString();
			}
			else if (PlayerPrefs.HasKey("UserID"))
			{
				text = PlayerPrefs.GetString("UserID");
			}
			else
			{
				text = Guid.NewGuid().ToString();
				PlayerPrefs.SetString("UserID", text);
			}
			return text;
		}

		// Token: 0x06001A1A RID: 6682 RVA: 0x00082476 File Offset: 0x00080676
		public static AuthenticationValues LoadUserID()
		{
			return new AuthenticationValues
			{
				AuthType = CustomAuthenticationType.None,
				UserId = NetworkingUtilities.GetBestUserID()
			};
		}

		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x06001A1B RID: 6683 RVA: 0x00082493 File Offset: 0x00080693
		public static bool IsConnectedToNetwork
		{
			get
			{
				return PhotonNetwork.IsConnected;
			}
		}

		// Token: 0x06001A1C RID: 6684 RVA: 0x0008249A File Offset: 0x0008069A
		public static RoomOptions HostRoomOptions()
		{
			return new RoomOptions
			{
				IsVisible = false,
				MaxPlayers = NetworkingUtilities.MAX_PLAYERS,
				PublishUserId = true
			};
		}

		// Token: 0x06001A1D RID: 6685 RVA: 0x000824BC File Offset: 0x000806BC
		public static void JoinDummyRoom()
		{
			if (!PhotonNetwork.OfflineMode)
			{
				Debug.LogWarning("Oy! Attempting to start a dummy room when not in offline mode. That's inappropriate");
			}
			PhotonNetwork.NickName = NetworkingUtilities.GetUsername();
			PhotonNetwork.AuthValues = NetworkingUtilities.LoadUserID();
			RoomOptions roomOptions = NetworkingUtilities.HostRoomOptions();
			PhotonNetwork.CreateRoom(Guid.NewGuid().ToString(), roomOptions, null, null);
		}

		// Token: 0x06001A1E RID: 6686 RVA: 0x00082510 File Offset: 0x00080710
		public static bool HasAuthority(this MonoBehaviourPunCallbacks self)
		{
			return self.photonView.IsMine || PhotonNetwork.IsMasterClient;
		}

		// Token: 0x06001A1F RID: 6687 RVA: 0x00082528 File Offset: 0x00080728
		public static bool ConnectToNetwork()
		{
			if (PhotonNetwork.IsConnected)
			{
				Debug.LogWarning("Attempted to connect to Photon even though we're already connected? Doing nothing.");
				return false;
			}
			PhotonNetwork.SerializationRate = 30;
			PhotonNetwork.SendRate = 30;
			BuildVersion buildVersion = new BuildVersion(Application.version, "???");
			PhotonNetwork.AutomaticallySyncScene = true;
			PhotonNetwork.GameVersion = buildVersion.ToString();
			PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = buildVersion.ToMatchmaking();
			PhotonNetwork.NickName = NetworkingUtilities.GetUsername();
			PhotonNetwork.AuthValues = NetworkingUtilities.LoadUserID();
			Debug.Log("Photon Start" + PhotonNetwork.NetworkClientState.ToString() + " using app version: " + buildVersion.ToMatchmaking());
			return PhotonNetwork.ConnectUsingSettings();
		}

		// Token: 0x06001A20 RID: 6688 RVA: 0x000825DC File Offset: 0x000807DC
		public static string GetUserId(this global::Player self)
		{
			return self.photonView.Owner.UserId;
		}

		// Token: 0x06001A21 RID: 6689 RVA: 0x000825EE File Offset: 0x000807EE
		public static int GetActorNumber(this global::Player self)
		{
			return self.photonView.OwnerActorNr;
		}

		// Token: 0x04001763 RID: 5987
		private static readonly Regex _richText = new Regex("<[^>]*>");
	}
}

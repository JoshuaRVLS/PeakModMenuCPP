using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using UnityEngine;

namespace Peak.Network
{
	// Token: 0x020003E2 RID: 994
	public class SteamLobbyAPI : IMatchmakingAPI, IMatchmakingEvents, IMatchmakingCallbacks, IDisposable
	{
		// Token: 0x1400001D RID: 29
		// (add) Token: 0x06001A65 RID: 6757 RVA: 0x00082CD4 File Offset: 0x00080ED4
		// (remove) Token: 0x06001A66 RID: 6758 RVA: 0x00082D0C File Offset: 0x00080F0C
		public event Action InviteReceived;

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x06001A67 RID: 6759 RVA: 0x00082D41 File Offset: 0x00080F41
		public string LobbyId
		{
			get
			{
				SteamLobbyHandler lobbyHandler = SteamLobbyAPI.LobbyHandler;
				return ((lobbyHandler != null) ? lobbyHandler.CurrentLobbyID : null) ?? string.Empty;
			}
		}

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x06001A68 RID: 6760 RVA: 0x00082D5D File Offset: 0x00080F5D
		public string InvitedLobbyId
		{
			get
			{
				if (!this._hasPendingJoinRequest)
				{
					return string.Empty;
				}
				return SteamLobbyAPI._pendingLobbyJoinID.ToString();
			}
		}

		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x06001A69 RID: 6761 RVA: 0x00082D7D File Offset: 0x00080F7D
		public bool InLobby
		{
			get
			{
				SteamLobbyHandler lobbyHandler = SteamLobbyAPI.LobbyHandler;
				return lobbyHandler != null && lobbyHandler.InSteamLobby();
			}
		}

		// Token: 0x170001BA RID: 442
		// (get) Token: 0x06001A6A RID: 6762 RVA: 0x00082D8F File Offset: 0x00080F8F
		private static SteamLobbyHandler LobbyHandler
		{
			get
			{
				return GameHandler.GetService<SteamLobbyHandler>();
			}
		}

		// Token: 0x06001A6B RID: 6763 RVA: 0x00082D96 File Offset: 0x00080F96
		private CSteamID ToSteamId(string idStr)
		{
			return new CSteamID(ulong.Parse(idStr));
		}

		// Token: 0x06001A6C RID: 6764 RVA: 0x00082DA3 File Offset: 0x00080FA3
		public SteamLobbyAPI()
		{
			Callback<GameLobbyJoinRequested_t>.Create(new Callback<GameLobbyJoinRequested_t>.DispatchDelegate(this.OnLobbyJoinRequested));
			PhotonNetwork.AddCallbackTarget(this);
		}

		// Token: 0x06001A6D RID: 6765 RVA: 0x00082DC3 File Offset: 0x00080FC3
		public void Dispose()
		{
			PhotonNetwork.RemoveCallbackTarget(this);
		}

		// Token: 0x06001A6E RID: 6766 RVA: 0x00082DCB File Offset: 0x00080FCB
		public void CreateLobby(LobbyTypeSetting.LobbyType lobbyType)
		{
			SteamMatchmaking.CreateLobby((lobbyType == LobbyTypeSetting.LobbyType.Friends) ? ELobbyType.k_ELobbyTypeFriendsOnly : ELobbyType.k_ELobbyTypePrivate, NetworkingUtilities.MAX_PLAYERS);
		}

		// Token: 0x06001A6F RID: 6767 RVA: 0x00082DDF File Offset: 0x00080FDF
		public void LeaveLobby()
		{
			SteamLobbyAPI.LobbyHandler.LeaveLobby();
		}

		// Token: 0x170001BB RID: 443
		// (get) Token: 0x06001A70 RID: 6768 RVA: 0x00082DEB File Offset: 0x00080FEB
		public bool HasPendingInvite
		{
			get
			{
				if (this._hasPendingJoinRequest)
				{
					return true;
				}
				if (!GameHandler.Initialized)
				{
					return false;
				}
				if (SteamLobbyAPI.LobbyHandler.TryConsumeLaunchCommandInvite(out SteamLobbyAPI._pendingLobbyJoinID))
				{
					this._hasPendingJoinRequest = true;
				}
				return this._hasPendingJoinRequest;
			}
		}

		// Token: 0x06001A71 RID: 6769 RVA: 0x00082E20 File Offset: 0x00081020
		public bool PlayerIsInLobby(string playerId)
		{
			ulong ulSteamID;
			if (!ulong.TryParse(playerId, out ulSteamID))
			{
				Debug.LogWarning("Player " + playerId + " is not a valid Steam ID. Something is fishy!");
				return false;
			}
			CSteamID lobbySteamId = SteamLobbyAPI.LobbyHandler.LobbySteamId;
			CSteamID x = new CSteamID(ulSteamID);
			int numLobbyMembers = SteamMatchmaking.GetNumLobbyMembers(lobbySteamId);
			for (int i = 0; i < numLobbyMembers; i++)
			{
				if (x == SteamMatchmaking.GetLobbyMemberByIndex(lobbySteamId, i))
				{
					return true;
				}
			}
			Debug.LogWarning("Didn't find " + playerId + " in this steam lobby. Something is fishy!");
			return false;
		}

		// Token: 0x06001A72 RID: 6770 RVA: 0x00082EA0 File Offset: 0x000810A0
		public void ConsumePendingJoin(bool andJoin)
		{
			if (!this.HasPendingInvite)
			{
				Debug.LogWarning("Attempted to consume join request but we have none pending.");
				return;
			}
			this._hasPendingJoinRequest = false;
			if (andJoin)
			{
				SteamLobbyAPI.LobbyHandler.RequestLobbyData(SteamLobbyAPI._pendingLobbyJoinID);
			}
		}

		// Token: 0x06001A73 RID: 6771 RVA: 0x00082ECE File Offset: 0x000810CE
		public bool TrySetLobbyData(string key, string value)
		{
			return SteamMatchmaking.SetLobbyData(SteamLobbyAPI.LobbyHandler.LobbySteamId, key, value);
		}

		// Token: 0x06001A74 RID: 6772 RVA: 0x00082EE1 File Offset: 0x000810E1
		public bool TryGetLobbyData(string lobbyId, string key, out string value)
		{
			value = SteamMatchmaking.GetLobbyData(this.ToSteamId(lobbyId), key);
			return value != null;
		}

		// Token: 0x06001A75 RID: 6773 RVA: 0x00082EF8 File Offset: 0x000810F8
		private void OnLobbyJoinRequested(GameLobbyJoinRequested_t param)
		{
			if (this._hasPendingJoinRequest)
			{
				Debug.LogWarning(string.Format("Received join request from {0} in lobby {1} ", param.m_steamIDFriend, param.m_steamIDLobby) + "but can't do anything with it because we already have a pending invite");
				return;
			}
			Debug.Log(string.Format("On Lobby Join Requested: {0} by {1}", param.m_steamIDLobby, param.m_steamIDFriend));
			this._hasPendingJoinRequest = true;
			SteamLobbyAPI._pendingLobbyJoinID = param.m_steamIDLobby;
			Action inviteReceived = this.InviteReceived;
			if (inviteReceived == null)
			{
				return;
			}
			inviteReceived();
		}

		// Token: 0x1400001E RID: 30
		// (add) Token: 0x06001A76 RID: 6774 RVA: 0x00082F84 File Offset: 0x00081184
		// (remove) Token: 0x06001A77 RID: 6775 RVA: 0x00082FBC File Offset: 0x000811BC
		public event Action<short, string> JoinLobbyFailed;

		// Token: 0x1400001F RID: 31
		// (add) Token: 0x06001A78 RID: 6776 RVA: 0x00082FF4 File Offset: 0x000811F4
		// (remove) Token: 0x06001A79 RID: 6777 RVA: 0x0008302C File Offset: 0x0008122C
		public event Action JoinedLobby;

		// Token: 0x14000020 RID: 32
		// (add) Token: 0x06001A7A RID: 6778 RVA: 0x00083064 File Offset: 0x00081264
		// (remove) Token: 0x06001A7B RID: 6779 RVA: 0x0008309C File Offset: 0x0008129C
		public event Action LeftLobby;

		// Token: 0x06001A7C RID: 6780 RVA: 0x000830D1 File Offset: 0x000812D1
		public void OnJoinRoomFailed(short returnCode, string message)
		{
			Action<short, string> joinLobbyFailed = this.JoinLobbyFailed;
			if (joinLobbyFailed == null)
			{
				return;
			}
			joinLobbyFailed(returnCode, message);
		}

		// Token: 0x06001A7D RID: 6781 RVA: 0x000830E5 File Offset: 0x000812E5
		public void OnFriendListUpdate(List<FriendInfo> friendList)
		{
		}

		// Token: 0x06001A7E RID: 6782 RVA: 0x000830E7 File Offset: 0x000812E7
		public void OnCreatedRoom()
		{
			Action joinedLobby = this.JoinedLobby;
			if (joinedLobby == null)
			{
				return;
			}
			joinedLobby();
		}

		// Token: 0x06001A7F RID: 6783 RVA: 0x000830F9 File Offset: 0x000812F9
		public void OnCreateRoomFailed(short returnCode, string message)
		{
		}

		// Token: 0x06001A80 RID: 6784 RVA: 0x000830FB File Offset: 0x000812FB
		public void OnJoinedRoom()
		{
			Action joinedLobby = this.JoinedLobby;
			if (joinedLobby == null)
			{
				return;
			}
			joinedLobby();
		}

		// Token: 0x06001A81 RID: 6785 RVA: 0x0008310D File Offset: 0x0008130D
		public void OnJoinRandomFailed(short returnCode, string message)
		{
		}

		// Token: 0x06001A82 RID: 6786 RVA: 0x0008310F File Offset: 0x0008130F
		public void OnLeftRoom()
		{
			Action leftLobby = this.LeftLobby;
			if (leftLobby == null)
			{
				return;
			}
			leftLobby();
		}

		// Token: 0x04001770 RID: 6000
		private static CSteamID _pendingLobbyJoinID;

		// Token: 0x04001771 RID: 6001
		private bool _hasPendingJoinRequest;
	}
}

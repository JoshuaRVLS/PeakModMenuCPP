using System;
using Photon.Pun;
using Steamworks;

// Token: 0x0200016B RID: 363
internal class SteamRichPresence : IRichPresence
{
	// Token: 0x170000E2 RID: 226
	// (get) Token: 0x06000BDC RID: 3036 RVA: 0x0003F77A File Offset: 0x0003D97A
	public RichPresenceState State
	{
		get
		{
			return this.m_currentState;
		}
	}

	// Token: 0x06000BDD RID: 3037 RVA: 0x0003F784 File Offset: 0x0003D984
	public void SetState(RichPresenceState state)
	{
		this.m_currentState = state;
		int num = 1;
		int num2 = 4;
		if (state == RichPresenceState.Status_MainMenu)
		{
			SteamFriends.ClearRichPresence();
		}
		else if (PhotonNetwork.OfflineMode)
		{
			num = 1;
			num2 = 1;
		}
		else if (PhotonNetwork.InRoom)
		{
			num = PhotonNetwork.PlayerList.Length;
			SteamFriends.SetRichPresence("steam_player_group", PhotonNetwork.CurrentRoom.Name);
			num2 = PhotonNetwork.CurrentRoom.MaxPlayers;
		}
		SteamFriends.SetRichPresence("steam_display", "#" + state.ToString());
		SteamFriends.SetRichPresence("players", num.ToString());
		SteamFriends.SetRichPresence("steam_player_group_size", num.ToString());
		SteamFriends.SetRichPresence("max_players", num2.ToString());
	}

	// Token: 0x04000ACD RID: 2765
	private RichPresenceState m_currentState;
}

using System;
using Sirenix.Utilities;
using Unity.Multiplayer.Playmode;
using UnityEngine;

namespace Peak.Network
{
	// Token: 0x020003DC RID: 988
	public class NoMatchmaking : IMatchmakingAPI, IMatchmakingEvents
	{
		// Token: 0x06001A23 RID: 6691 RVA: 0x0008260C File Offset: 0x0008080C
		public NoMatchmaking()
		{
			Debug.LogWarning("Initializing without a matchmaking API. Will be unable to receive game invites.");
		}

		// Token: 0x14000014 RID: 20
		// (add) Token: 0x06001A24 RID: 6692 RVA: 0x00082620 File Offset: 0x00080820
		// (remove) Token: 0x06001A25 RID: 6693 RVA: 0x00082658 File Offset: 0x00080858
		public event Action InviteReceived;

		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x06001A26 RID: 6694 RVA: 0x0008268D File Offset: 0x0008088D
		public string LobbyId
		{
			get
			{
				return "Disabled";
			}
		}

		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x06001A27 RID: 6695 RVA: 0x00082694 File Offset: 0x00080894
		public string InvitedLobbyId
		{
			get
			{
				return this.LobbyId;
			}
		}

		// Token: 0x170001AA RID: 426
		// (get) Token: 0x06001A28 RID: 6696 RVA: 0x0008269C File Offset: 0x0008089C
		public bool InLobby
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06001A29 RID: 6697 RVA: 0x0008269F File Offset: 0x0008089F
		public void CreateLobby(LobbyTypeSetting.LobbyType lobbyType)
		{
			Debug.LogWarning("Not actually creating a lobby.");
		}

		// Token: 0x06001A2A RID: 6698 RVA: 0x000826AB File Offset: 0x000808AB
		public void LeaveLobby()
		{
		}

		// Token: 0x06001A2B RID: 6699 RVA: 0x000826AD File Offset: 0x000808AD
		public bool PlayerIsInLobby(string playerId)
		{
			Debug.Log("There is no lobby");
			return !CurrentPlayer.ReadOnlyTags().IsNullOrEmpty<string>();
		}

		// Token: 0x06001A2C RID: 6700 RVA: 0x000826C6 File Offset: 0x000808C6
		public bool TrySetLobbyData(string key, string value)
		{
			return false;
		}

		// Token: 0x06001A2D RID: 6701 RVA: 0x000826C9 File Offset: 0x000808C9
		public bool TryGetLobbyData(string id, string key, out string value)
		{
			value = null;
			return false;
		}

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x06001A2E RID: 6702 RVA: 0x000826CF File Offset: 0x000808CF
		public bool HasPendingInvite
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06001A2F RID: 6703 RVA: 0x000826D2 File Offset: 0x000808D2
		public void ConsumePendingJoin(bool _)
		{
			Debug.LogError("Can't consume pending join! We don't have any matchmaking initialized.");
		}

		// Token: 0x14000015 RID: 21
		// (add) Token: 0x06001A30 RID: 6704 RVA: 0x000826E0 File Offset: 0x000808E0
		// (remove) Token: 0x06001A31 RID: 6705 RVA: 0x00082718 File Offset: 0x00080918
		public event Action<short, string> JoinLobbyFailed;

		// Token: 0x14000016 RID: 22
		// (add) Token: 0x06001A32 RID: 6706 RVA: 0x00082750 File Offset: 0x00080950
		// (remove) Token: 0x06001A33 RID: 6707 RVA: 0x00082788 File Offset: 0x00080988
		public event Action JoinedLobby;

		// Token: 0x14000017 RID: 23
		// (add) Token: 0x06001A34 RID: 6708 RVA: 0x000827C0 File Offset: 0x000809C0
		// (remove) Token: 0x06001A35 RID: 6709 RVA: 0x000827F8 File Offset: 0x000809F8
		public event Action LeftLobby;
	}
}

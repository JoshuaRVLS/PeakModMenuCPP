using System;

namespace Peak.Network
{
	// Token: 0x020003DA RID: 986
	public interface IMatchmakingAPI : IMatchmakingEvents
	{
		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x06001A0A RID: 6666
		bool InLobby { get; }

		// Token: 0x06001A0B RID: 6667
		void CreateLobby(LobbyTypeSetting.LobbyType lobbyType);

		// Token: 0x06001A0C RID: 6668
		void LeaveLobby();

		// Token: 0x06001A0D RID: 6669
		bool PlayerIsInLobby(string playerId);

		// Token: 0x06001A0E RID: 6670
		bool TrySetLobbyData(string key, string value);

		// Token: 0x06001A0F RID: 6671
		bool TryGetLobbyData(string lobbyId, string key, out string value);

		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x06001A10 RID: 6672
		bool HasPendingInvite { get; }

		// Token: 0x06001A11 RID: 6673
		void ConsumePendingJoin(bool andJoin);

		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x06001A12 RID: 6674
		string LobbyId { get; }

		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x06001A13 RID: 6675
		string InvitedLobbyId { get; }

		// Token: 0x14000013 RID: 19
		// (add) Token: 0x06001A14 RID: 6676
		// (remove) Token: 0x06001A15 RID: 6677
		event Action<short, string> JoinLobbyFailed;
	}
}

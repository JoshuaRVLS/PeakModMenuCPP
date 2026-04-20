using System;
using System.Linq;
using Unity.Multiplayer.Playmode;

namespace Peak.Network
{
	// Token: 0x020003D4 RID: 980
	public static class NetCode
	{
		// Token: 0x17000193 RID: 403
		// (get) Token: 0x060019E6 RID: 6630 RVA: 0x00082364 File Offset: 0x00080564
		private static bool SteamIsEnabled
		{
			get
			{
				return !CurrentPlayer.ReadOnlyTags().Contains("NoSteam");
			}
		}

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x060019E7 RID: 6631 RVA: 0x00082378 File Offset: 0x00080578
		public static ISessionAPI Session { get; } = new PhotonShim();

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x060019E8 RID: 6632 RVA: 0x0008237F File Offset: 0x0008057F
		public static IMatchmakingAPI Matchmaking { get; }

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x060019E9 RID: 6633 RVA: 0x00082386 File Offset: 0x00080586
		public static ISessionEvents ConnectionEvents
		{
			get
			{
				return NetCode.Session;
			}
		}

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x060019EA RID: 6634 RVA: 0x0008238D File Offset: 0x0008058D
		public static INetworkRoomEvents RoomEvents { get; }

		// Token: 0x060019EB RID: 6635 RVA: 0x00082394 File Offset: 0x00080594
		// Note: this type is marked as 'beforefieldinit'.
		static NetCode()
		{
			IMatchmakingAPI matchmakingAPI2;
			if (!NetCode.SteamIsEnabled)
			{
				IMatchmakingAPI matchmakingAPI = new NoMatchmaking();
				matchmakingAPI2 = matchmakingAPI;
			}
			else
			{
				IMatchmakingAPI matchmakingAPI = new SteamLobbyAPI();
				matchmakingAPI2 = matchmakingAPI;
			}
			NetCode.Matchmaking = matchmakingAPI2;
			NetCode.RoomEvents = new PhotonRoomEvents();
		}
	}
}

using System;
using Crab.Network;

namespace Peak.Network.Crab.Network
{
	// Token: 0x020003E3 RID: 995
	[Obsolete("Moved to Peak.Network.NetCode")]
	public static class Network
	{
		// Token: 0x170001BC RID: 444
		// (get) Token: 0x06001A83 RID: 6787 RVA: 0x00083121 File Offset: 0x00081321
		[Obsolete("Moved to Peak.Network.NetCode.Session")]
		public static IRelayAPI Relay
		{
			get
			{
				return NetCode.Session as IRelayAPI;
			}
		}

		// Token: 0x170001BD RID: 445
		// (get) Token: 0x06001A84 RID: 6788 RVA: 0x0008312D File Offset: 0x0008132D
		[Obsolete("Moved to Peak.Network.NetCode")]
		public static IMatchmakingAPI Matchmaking
		{
			get
			{
				return NetCode.Matchmaking;
			}
		}

		// Token: 0x170001BE RID: 446
		// (get) Token: 0x06001A85 RID: 6789 RVA: 0x00083134 File Offset: 0x00081334
		[Obsolete("Moved to Peak.Network.NetCode")]
		public static IRelayEvents ConnectionEvents
		{
			get
			{
				return Network.Relay;
			}
		}

		// Token: 0x170001BF RID: 447
		// (get) Token: 0x06001A86 RID: 6790 RVA: 0x0008313B File Offset: 0x0008133B
		[Obsolete("Moved to Peak.Network.NetCode")]
		public static INetworkRoomEvents RoomEvents
		{
			get
			{
				return NetCode.RoomEvents;
			}
		}
	}
}

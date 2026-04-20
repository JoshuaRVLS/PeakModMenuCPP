using System;

namespace Peak.Network
{
	// Token: 0x020003D8 RID: 984
	public interface IMatchmakingEvents
	{
		// Token: 0x14000010 RID: 16
		// (add) Token: 0x060019F7 RID: 6647
		// (remove) Token: 0x060019F8 RID: 6648
		event Action InviteReceived;

		// Token: 0x14000011 RID: 17
		// (add) Token: 0x060019F9 RID: 6649
		// (remove) Token: 0x060019FA RID: 6650
		event Action JoinedLobby;

		// Token: 0x14000012 RID: 18
		// (add) Token: 0x060019FB RID: 6651
		// (remove) Token: 0x060019FC RID: 6652
		event Action LeftLobby;
	}
}

using System;

namespace Peak.Network
{
	// Token: 0x020003D6 RID: 982
	public interface INetworkRoomEvents
	{
		// Token: 0x1400000B RID: 11
		// (add) Token: 0x060019ED RID: 6637
		// (remove) Token: 0x060019EE RID: 6638
		event Action PlayerEntered;

		// Token: 0x1400000C RID: 12
		// (add) Token: 0x060019EF RID: 6639
		// (remove) Token: 0x060019F0 RID: 6640
		event Action PlayerLeft;

		// Token: 0x1400000D RID: 13
		// (add) Token: 0x060019F1 RID: 6641
		// (remove) Token: 0x060019F2 RID: 6642
		event Action RoomOwnerChanged;
	}
}

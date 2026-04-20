using System;

namespace Peak.Network
{
	// Token: 0x020003D7 RID: 983
	public interface ISessionEvents
	{
		// Token: 0x1400000E RID: 14
		// (add) Token: 0x060019F3 RID: 6643
		// (remove) Token: 0x060019F4 RID: 6644
		event Action ConnectedAndReady;

		// Token: 0x1400000F RID: 15
		// (add) Token: 0x060019F5 RID: 6645
		// (remove) Token: 0x060019F6 RID: 6646
		event Action<INetworkEventData> NetworkEventReceived;
	}
}

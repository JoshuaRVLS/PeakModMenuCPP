using System;
using Photon.Realtime;

namespace Peak.Network
{
	// Token: 0x020003D9 RID: 985
	public interface ISessionAPI : ISessionEvents
	{
		// Token: 0x17000199 RID: 409
		// (get) Token: 0x060019FD RID: 6653 RVA: 0x000823D1 File Offset: 0x000805D1
		[Obsolete("Call IsHost instead")]
		bool IsMasterClient
		{
			get
			{
				return this.IsHost;
			}
		}

		// Token: 0x1700019A RID: 410
		// (get) Token: 0x060019FE RID: 6654
		bool IsHost { get; }

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x060019FF RID: 6655
		bool IsOffline { get; }

		// Token: 0x1700019C RID: 412
		// (get) Token: 0x06001A00 RID: 6656
		int HostId { get; }

		// Token: 0x1700019D RID: 413
		// (get) Token: 0x06001A01 RID: 6657
		bool IsConnectedAndReady { get; }

		// Token: 0x1700019E RID: 414
		// (get) Token: 0x06001A02 RID: 6658
		// (set) Token: 0x06001A03 RID: 6659
		string NickName { get; set; }

		// Token: 0x1700019F RID: 415
		// (get) Token: 0x06001A04 RID: 6660
		int SeatNumber { get; }

		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x06001A05 RID: 6661
		// (set) Token: 0x06001A06 RID: 6662
		AuthenticationValues AuthValues { get; set; }

		// Token: 0x06001A07 RID: 6663
		void InitializeNetcodeBackend();

		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x06001A08 RID: 6664
		bool InRoom { get; }

		// Token: 0x06001A09 RID: 6665
		void Kick(string id);
	}
}

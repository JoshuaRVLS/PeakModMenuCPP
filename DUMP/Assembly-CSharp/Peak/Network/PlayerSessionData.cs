using System;

namespace Peak.Network
{
	// Token: 0x020003E0 RID: 992
	public class PlayerSessionData
	{
		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x06001A5E RID: 6750 RVA: 0x00082C89 File Offset: 0x00080E89
		public string Id { get; }

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x06001A5F RID: 6751 RVA: 0x00082C91 File Offset: 0x00080E91
		// (set) Token: 0x06001A60 RID: 6752 RVA: 0x00082C99 File Offset: 0x00080E99
		public int ActorNumber { get; internal set; }

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x06001A61 RID: 6753 RVA: 0x00082CA2 File Offset: 0x00080EA2
		// (set) Token: 0x06001A62 RID: 6754 RVA: 0x00082CAA File Offset: 0x00080EAA
		public bool IsBanned { get; internal set; }

		// Token: 0x06001A63 RID: 6755 RVA: 0x00082CB3 File Offset: 0x00080EB3
		public PlayerSessionData(string id, int actorNumber)
		{
			this.Id = id;
			this.ActorNumber = actorNumber;
		}
	}
}

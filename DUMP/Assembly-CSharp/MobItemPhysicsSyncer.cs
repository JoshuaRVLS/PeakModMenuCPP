using System;

// Token: 0x0200015A RID: 346
public class MobItemPhysicsSyncer : ItemPhysicsSyncer
{
	// Token: 0x170000DB RID: 219
	// (get) Token: 0x06000B77 RID: 2935 RVA: 0x0003D772 File Offset: 0x0003B972
	protected override float maxAngleChangePerSecond
	{
		get
		{
			return this.maxAngleUpdatePerSecond;
		}
	}

	// Token: 0x04000A85 RID: 2693
	public float maxAngleUpdatePerSecond = 180f;
}

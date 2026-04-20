using System;

// Token: 0x02000169 RID: 361
internal interface IRichPresence
{
	// Token: 0x170000E0 RID: 224
	// (get) Token: 0x06000BD7 RID: 3031
	RichPresenceState State { get; }

	// Token: 0x06000BD8 RID: 3032
	void SetState(RichPresenceState state);
}

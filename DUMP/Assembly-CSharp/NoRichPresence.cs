using System;

// Token: 0x0200016A RID: 362
internal class NoRichPresence : IRichPresence
{
	// Token: 0x170000E1 RID: 225
	// (get) Token: 0x06000BD9 RID: 3033 RVA: 0x0003F76D File Offset: 0x0003D96D
	public RichPresenceState State
	{
		get
		{
			return RichPresenceState.Status_MainMenu;
		}
	}

	// Token: 0x06000BDA RID: 3034 RVA: 0x0003F770 File Offset: 0x0003D970
	public void SetState(RichPresenceState state)
	{
	}
}

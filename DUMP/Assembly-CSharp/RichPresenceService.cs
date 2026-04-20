using System;
using System.Linq;
using Peak.Network;
using Unity.Multiplayer.Playmode;

// Token: 0x0200016C RID: 364
public class RichPresenceService : GameService, IDisposable
{
	// Token: 0x170000E3 RID: 227
	// (get) Token: 0x06000BDF RID: 3039 RVA: 0x0003F842 File Offset: 0x0003DA42
	[Obsolete("Rich presence functionality is encapsulated in the IRichPresence interface now.")]
	public RichPresenceState m_currentState
	{
		get
		{
			return this._presence.State;
		}
	}

	// Token: 0x06000BE0 RID: 3040 RVA: 0x0003F850 File Offset: 0x0003DA50
	public RichPresenceService()
	{
		IRichPresence presence;
		if (!CurrentPlayer.ReadOnlyTags().Contains("NoSteam"))
		{
			IRichPresence richPresence = new SteamRichPresence();
			presence = richPresence;
		}
		else
		{
			IRichPresence richPresence = new NoRichPresence();
			presence = richPresence;
		}
		this._presence = presence;
		NetCode.RoomEvents.PlayerEntered += this.Dirty;
		NetCode.RoomEvents.PlayerLeft += this.Dirty;
	}

	// Token: 0x06000BE1 RID: 3041 RVA: 0x0003F8B6 File Offset: 0x0003DAB6
	public void Dispose()
	{
		NetCode.RoomEvents.PlayerEntered -= this.Dirty;
		NetCode.RoomEvents.PlayerLeft -= this.Dirty;
	}

	// Token: 0x06000BE2 RID: 3042 RVA: 0x0003F8E4 File Offset: 0x0003DAE4
	public void SetState(RichPresenceState state)
	{
		this._presence.SetState(state);
	}

	// Token: 0x06000BE3 RID: 3043 RVA: 0x0003F8F2 File Offset: 0x0003DAF2
	public void Dirty()
	{
		this._presence.SetState(this._presence.State);
	}

	// Token: 0x04000ACE RID: 2766
	private IRichPresence _presence;
}

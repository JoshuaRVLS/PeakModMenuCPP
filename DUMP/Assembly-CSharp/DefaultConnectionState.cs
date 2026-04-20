using System;
using Peak.Network;
using UnityEngine;

// Token: 0x02000082 RID: 130
public class DefaultConnectionState : ConnectionState
{
	// Token: 0x1700006D RID: 109
	// (get) Token: 0x0600058E RID: 1422 RVA: 0x00020657 File Offset: 0x0001E857
	private bool InLobby
	{
		get
		{
			return NetCode.Matchmaking.InLobby;
		}
	}

	// Token: 0x0600058F RID: 1423 RVA: 0x00020663 File Offset: 0x0001E863
	public override void Enter()
	{
		base.Enter();
		if (Time.frameCount > 3 && this.InLobby)
		{
			GameHandler.GetService<SteamLobbyHandler>().LeaveLobby();
		}
	}
}

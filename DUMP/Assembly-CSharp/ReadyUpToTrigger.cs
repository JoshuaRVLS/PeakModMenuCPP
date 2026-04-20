using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000164 RID: 356
public class ReadyUpToTrigger : MonoBehaviourPunCallbacks
{
	// Token: 0x06000BB0 RID: 2992 RVA: 0x0003EB2D File Offset: 0x0003CD2D
	public override void OnJoinedRoom()
	{
		this.readyUpStatusDict.Clear();
		this.PopulatePlayerDict();
	}

	// Token: 0x06000BB1 RID: 2993 RVA: 0x0003EB40 File Offset: 0x0003CD40
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		this.PopulatePlayerDict();
	}

	// Token: 0x06000BB2 RID: 2994 RVA: 0x0003EB48 File Offset: 0x0003CD48
	public override void OnPlayerLeftRoom(Photon.Realtime.Player leavingPlayer)
	{
		this.readyUpStatusDict.Remove(leavingPlayer);
		Debug.Log("Removing player from ready-up list: " + leavingPlayer.NickName);
	}

	// Token: 0x06000BB3 RID: 2995 RVA: 0x0003EB6C File Offset: 0x0003CD6C
	private void PopulatePlayerDict()
	{
		foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
		{
			if (!this.readyUpStatusDict.ContainsKey(player))
			{
				this.readyUpStatusDict.Add(player, false);
				Debug.Log("Adding player to ready-up list: " + player.NickName);
			}
		}
	}

	// Token: 0x04000AC2 RID: 2754
	public Dictionary<Photon.Realtime.Player, bool> readyUpStatusDict = new Dictionary<Photon.Realtime.Player, bool>();
}

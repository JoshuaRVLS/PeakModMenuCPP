using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000152 RID: 338
public class ScoutmasterSpawner : MonoBehaviourPunCallbacks
{
	// Token: 0x06000B31 RID: 2865 RVA: 0x0003C5F7 File Offset: 0x0003A7F7
	private void Awake()
	{
		if (PhotonNetwork.InRoom)
		{
			this.SpawnScoutmaster();
		}
	}

	// Token: 0x06000B32 RID: 2866 RVA: 0x0003C606 File Offset: 0x0003A806
	public override void OnJoinedRoom()
	{
		this.SpawnScoutmaster();
	}

	// Token: 0x06000B33 RID: 2867 RVA: 0x0003C610 File Offset: 0x0003A810
	private void SpawnScoutmaster()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		Debug.Log("SPAWN SCOUTMASTER");
		PhotonNetwork.InstantiateRoomObject("Character_Scoutmaster", base.transform.position, base.transform.rotation, 0, null).GetComponent<Character>().data.spawnPoint = base.transform;
	}
}

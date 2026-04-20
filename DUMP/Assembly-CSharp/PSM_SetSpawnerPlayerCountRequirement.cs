using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002E7 RID: 743
public class PSM_SetSpawnerPlayerCountRequirement : PropSpawnerMod
{
	// Token: 0x060014AB RID: 5291 RVA: 0x00068A54 File Offset: 0x00066C54
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		if (spawned.GetComponent<PhotonView>())
		{
			Spawner component = spawned.GetComponent<Spawner>();
			if (this.onePerPlayer)
			{
				component.playersInRoomRequirement = spawnData.spawnCount + 1;
			}
		}
	}

	// Token: 0x040012C9 RID: 4809
	public bool onePerPlayer;
}

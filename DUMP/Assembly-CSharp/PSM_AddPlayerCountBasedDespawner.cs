using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002E6 RID: 742
public class PSM_AddPlayerCountBasedDespawner : PropSpawnerMod
{
	// Token: 0x060014A9 RID: 5289 RVA: 0x00068A08 File Offset: 0x00066C08
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		if (spawned.GetComponent<PhotonView>())
		{
			DestroyBasedOnPlayerCount destroyBasedOnPlayerCount = spawned.AddComponent<DestroyBasedOnPlayerCount>();
			if (this.onePerPlayer)
			{
				destroyBasedOnPlayerCount.destroyIfPlayerCountIsLessThan = spawnData.spawnCount + 1;
				return;
			}
			destroyBasedOnPlayerCount.destroyIfPlayerCountIsLessThan = this.destroyAllIfLessThan;
		}
	}

	// Token: 0x040012C7 RID: 4807
	public bool onePerPlayer;

	// Token: 0x040012C8 RID: 4808
	public int destroyAllIfLessThan;
}

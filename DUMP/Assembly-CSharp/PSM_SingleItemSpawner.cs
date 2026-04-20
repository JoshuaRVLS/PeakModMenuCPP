using System;
using UnityEngine;

// Token: 0x020002EA RID: 746
public class PSM_SingleItemSpawner : PropSpawnerMod
{
	// Token: 0x060014B1 RID: 5297 RVA: 0x00068B3E File Offset: 0x00066D3E
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		spawned.GetComponentInChildren<SingleItemSpawner>().prefab = this.objToSpawn;
		spawned.gameObject.name = this.objToSpawn.gameObject.name + " (spawner)";
	}

	// Token: 0x040012CE RID: 4814
	public GameObject objToSpawn;
}

using System;
using UnityEngine;

// Token: 0x020002F9 RID: 761
public class PSM_BlastVine : PropSpawnerMod
{
	// Token: 0x060014CF RID: 5327 RVA: 0x000693F6 File Offset: 0x000675F6
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		spawned.GetComponent<VinePlane>().Blast();
	}
}

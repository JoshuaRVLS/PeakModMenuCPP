using System;
using UnityEngine;

// Token: 0x020002EF RID: 751
public class PSM_PitonNormal : PropSpawnerMod
{
	// Token: 0x060014BB RID: 5307 RVA: 0x00068DFA File Offset: 0x00066FFA
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		spawned.transform.rotation = Quaternion.LookRotation(-spawnData.hit.normal, Vector3.up);
	}
}

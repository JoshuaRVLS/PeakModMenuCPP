using System;
using UnityEngine;

// Token: 0x020002FE RID: 766
public class PSM_SetRandomMaterial : PropSpawnerMod
{
	// Token: 0x060014D8 RID: 5336 RVA: 0x00069598 File Offset: 0x00067798
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		Renderer[] componentsInChildren = spawned.GetComponentsInChildren<Renderer>();
		Material sharedMaterial = this.mats[Random.Range(0, this.mats.Length)];
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].sharedMaterial = sharedMaterial;
		}
	}

	// Token: 0x04001302 RID: 4866
	public Material[] mats;
}

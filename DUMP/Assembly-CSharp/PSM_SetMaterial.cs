using System;
using UnityEngine;

// Token: 0x020002FA RID: 762
public class PSM_SetMaterial : PropSpawnerMod
{
	// Token: 0x060014D1 RID: 5329 RVA: 0x0006940C File Offset: 0x0006760C
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		Renderer[] componentsInChildren = spawned.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].sharedMaterial = this.mat;
		}
	}

	// Token: 0x040012FB RID: 4859
	public Material mat;
}

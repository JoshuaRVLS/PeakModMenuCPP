using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

// Token: 0x020002FB RID: 763
public class PSM_SetMaterialOnChild : PropSpawnerMod
{
	// Token: 0x060014D3 RID: 5331 RVA: 0x00069444 File Offset: 0x00067644
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		List<Renderer> rends = new List<Renderer>();
		spawned.transform.FindChildrenRecursive(this.childName).ForEach(delegate(Transform c)
		{
			rends.AddRange(c.GetComponentsInChildren<Renderer>());
		});
		for (int i = 0; i < rends.Count; i++)
		{
			rends[i].sharedMaterial = this.mat;
		}
	}

	// Token: 0x040012FC RID: 4860
	public string childName;

	// Token: 0x040012FD RID: 4861
	public Material mat;
}

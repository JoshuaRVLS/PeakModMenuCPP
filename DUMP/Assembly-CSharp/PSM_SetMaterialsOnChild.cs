using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

// Token: 0x020002FC RID: 764
public class PSM_SetMaterialsOnChild : PropSpawnerMod
{
	// Token: 0x060014D5 RID: 5333 RVA: 0x000694BC File Offset: 0x000676BC
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		List<Renderer> rends = new List<Renderer>();
		spawned.transform.FindChildrenRecursive(this.childName).ForEach(delegate(Transform c)
		{
			rends.AddRange(c.GetComponentsInChildren<Renderer>());
		});
		for (int i = 0; i < rends.Count; i++)
		{
			Material[] sharedMaterials = rends[i].sharedMaterials;
			for (int j = 0; j < sharedMaterials.Length; j++)
			{
				foreach (MatAndID matAndID in this.edits)
				{
					if (matAndID.id == j)
					{
						sharedMaterials[j] = matAndID.mat;
					}
				}
			}
			rends[i].sharedMaterials = sharedMaterials;
		}
	}

	// Token: 0x040012FE RID: 4862
	public string childName;

	// Token: 0x040012FF RID: 4863
	public MatAndID[] edits;
}

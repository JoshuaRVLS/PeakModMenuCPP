using System;
using UnityEngine;

// Token: 0x020002F6 RID: 758
public class PSM_ReplaceMaterialWithRayTargetMaterial : PropSpawnerMod
{
	// Token: 0x060014C9 RID: 5321 RVA: 0x00069224 File Offset: 0x00067424
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		if (spawnData.hit.transform == null)
		{
			return;
		}
		MeshRenderer[] componentsInChildren = spawnData.hit.transform.GetComponentsInChildren<MeshRenderer>();
		MeshRenderer meshRenderer = null;
		foreach (MeshRenderer meshRenderer2 in componentsInChildren)
		{
			if (meshRenderer2.enabled)
			{
				meshRenderer = meshRenderer2;
				break;
			}
		}
		if (meshRenderer == null)
		{
			return;
		}
		foreach (Renderer renderer in spawned.GetComponentsInChildren<Renderer>())
		{
			Material[] sharedMaterials = renderer.sharedMaterials;
			for (int j = 0; j < sharedMaterials.Length; j++)
			{
				if (sharedMaterials[j] == this.replaceThis)
				{
					sharedMaterials[j] = meshRenderer.sharedMaterial;
				}
			}
			renderer.sharedMaterials = sharedMaterials;
		}
	}

	// Token: 0x040012F2 RID: 4850
	public Material replaceThis;
}

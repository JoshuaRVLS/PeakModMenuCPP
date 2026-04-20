using System;
using UnityEngine;

// Token: 0x020002F5 RID: 757
public class PSM_ReplaceMaterial : PropSpawnerMod
{
	// Token: 0x060014C7 RID: 5319 RVA: 0x000691B8 File Offset: 0x000673B8
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		foreach (Renderer renderer in spawned.GetComponentsInChildren<Renderer>())
		{
			Material[] sharedMaterials = renderer.sharedMaterials;
			for (int j = 0; j < sharedMaterials.Length; j++)
			{
				if (sharedMaterials[j] == this.replaceThis)
				{
					sharedMaterials[j] = this.withThis;
				}
			}
			renderer.sharedMaterials = sharedMaterials;
		}
	}

	// Token: 0x040012F0 RID: 4848
	public Material replaceThis;

	// Token: 0x040012F1 RID: 4849
	public Material withThis;
}

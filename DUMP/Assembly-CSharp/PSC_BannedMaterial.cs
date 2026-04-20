using System;
using UnityEngine;

// Token: 0x02000303 RID: 771
public class PSC_BannedMaterial : PropSpawnerConstraint
{
	// Token: 0x060014E7 RID: 5351 RVA: 0x000699F8 File Offset: 0x00067BF8
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		this.returnVal = true;
		MeshRenderer[] componentsInChildren = spawnData.hit.transform.GetComponentsInChildren<MeshRenderer>();
		foreach (Material y in this.bannedMaterial)
		{
			foreach (MeshRenderer meshRenderer in componentsInChildren)
			{
				if (meshRenderer != null && meshRenderer.sharedMaterial == y)
				{
					this.returnVal = false;
					break;
				}
			}
		}
		return this.returnVal;
	}

	// Token: 0x04001311 RID: 4881
	public Material[] bannedMaterial;

	// Token: 0x04001312 RID: 4882
	private bool returnVal = true;
}

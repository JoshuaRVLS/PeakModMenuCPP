using System;
using UnityEngine;

// Token: 0x02000304 RID: 772
public class PSC_RequiredMaterial : PropSpawnerConstraint
{
	// Token: 0x060014E9 RID: 5353 RVA: 0x00069A8C File Offset: 0x00067C8C
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		MeshRenderer componentInChildren = spawnData.hit.transform.GetComponentInChildren<MeshRenderer>();
		foreach (Material y in this.RequiredMaterial)
		{
			if (componentInChildren != null && componentInChildren.sharedMaterial == y)
			{
				return true;
			}
		}
		return this.RequiredMaterial.Length == 0;
	}

	// Token: 0x04001313 RID: 4883
	public Material[] RequiredMaterial;
}

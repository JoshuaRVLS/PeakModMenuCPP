using System;
using UnityEngine;

// Token: 0x0200030F RID: 783
public class PSCP_Custom : PropSpawnerConstraintPost
{
	// Token: 0x06001501 RID: 5377 RVA: 0x00069F6C File Offset: 0x0006816C
	public override bool CheckConstraint(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		CustomSpawnCondition[] components = spawned.GetComponents<CustomSpawnCondition>();
		for (int i = 0; i < components.Length; i++)
		{
			if (!components[i].CheckCondition(spawnData))
			{
				return false;
			}
		}
		return true;
	}
}

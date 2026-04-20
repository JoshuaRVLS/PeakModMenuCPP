using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000310 RID: 784
public class PSCP_ConnectTreePlatforms : PropSpawnerConstraintPost
{
	// Token: 0x06001503 RID: 5379 RVA: 0x00069FA4 File Offset: 0x000681A4
	public override bool CheckConstraint(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		List<TreePlatform> list = new List<TreePlatform>();
		list.AddRange(this.treePlatformParent.GetComponentsInChildren<TreePlatform>());
		JungleVine[] components = spawned.GetComponents<JungleVine>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].PickTreePlatforms(list);
		}
		return true;
	}

	// Token: 0x04001329 RID: 4905
	public GameObject treePlatformParent;
}

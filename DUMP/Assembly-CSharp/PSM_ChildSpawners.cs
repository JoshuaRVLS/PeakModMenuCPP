using System;
using UnityEngine;

// Token: 0x020002E9 RID: 745
public class PSM_ChildSpawners : PropSpawnerMod
{
	// Token: 0x060014AF RID: 5295 RVA: 0x00068B0C File Offset: 0x00066D0C
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData _)
	{
		LevelGenStep[] componentsInChildren = spawned.GetComponentsInChildren<LevelGenStep>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Execute();
		}
	}
}

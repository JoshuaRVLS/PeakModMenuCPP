using System;
using UnityEngine;

// Token: 0x020002E5 RID: 741
public class Campfire_Set_Segment : PropSpawnerMod
{
	// Token: 0x060014A7 RID: 5287 RVA: 0x000689EA File Offset: 0x00066BEA
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		spawned.GetComponentInChildren<Campfire>().advanceToSegment = this.Segment;
	}

	// Token: 0x040012C6 RID: 4806
	public Segment Segment;
}

using System;
using UnityEngine;

// Token: 0x02000309 RID: 777
public class PSC_CircleMask : PropSpawnerConstraint
{
	// Token: 0x060014F3 RID: 5363 RVA: 0x00069D00 File Offset: 0x00067F00
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		float num = Vector2.Distance(new Vector2(spawnData.pos.x, spawnData.pos.z), new Vector2(spawnData.spawnerTransform.position.x, spawnData.spawnerTransform.position.z));
		if (this.inverted)
		{
			return num > this.circleSize;
		}
		return num < this.circleSize;
	}

	// Token: 0x04001320 RID: 4896
	public float circleSize = 10f;

	// Token: 0x04001321 RID: 4897
	public bool inverted;
}

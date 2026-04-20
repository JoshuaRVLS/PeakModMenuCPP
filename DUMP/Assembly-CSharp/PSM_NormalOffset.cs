using System;
using UnityEngine;

// Token: 0x020002F1 RID: 753
public class PSM_NormalOffset : PropSpawnerMod
{
	// Token: 0x060014BF RID: 5311 RVA: 0x00068F5C File Offset: 0x0006715C
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		spawned.transform.position += spawnData.normal * Mathf.Lerp(this.minOffset, this.maxOffset, Mathf.Pow(Random.value, this.randomPow));
	}

	// Token: 0x040012E2 RID: 4834
	public float minOffset;

	// Token: 0x040012E3 RID: 4835
	public float maxOffset = 2f;

	// Token: 0x040012E4 RID: 4836
	public float randomPow = 1f;
}

using System;
using UnityEngine;

// Token: 0x020002F2 RID: 754
public class PSM_RayDirectionOffset : PropSpawnerMod
{
	// Token: 0x060014C1 RID: 5313 RVA: 0x00068FCC File Offset: 0x000671CC
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		spawned.transform.position += spawnData.rayDir * Mathf.Lerp(this.minOffset, this.maxOffset, Mathf.Pow(Random.value, this.randomPow));
	}

	// Token: 0x040012E5 RID: 4837
	public float minOffset;

	// Token: 0x040012E6 RID: 4838
	public float maxOffset = 5f;

	// Token: 0x040012E7 RID: 4839
	public float randomPow = 1f;
}

using System;
using UnityEngine;

// Token: 0x020002F7 RID: 759
public class PSM_RandomScale : PropSpawnerMod
{
	// Token: 0x060014CB RID: 5323 RVA: 0x000692E7 File Offset: 0x000674E7
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		spawned.transform.localScale *= Mathf.Lerp(this.minScaleMult, this.maxScaleMult, Mathf.Pow(Random.value, this.randomPow));
	}

	// Token: 0x040012F3 RID: 4851
	public float minScaleMult;

	// Token: 0x040012F4 RID: 4852
	public float maxScaleMult = 2f;

	// Token: 0x040012F5 RID: 4853
	public float randomPow = 1f;
}

using System;
using UnityEngine;

// Token: 0x020002EB RID: 747
public class PSM_RandomOffset : PropSpawnerMod
{
	// Token: 0x060014B3 RID: 5299 RVA: 0x00068B80 File Offset: 0x00066D80
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		float d = Mathf.Lerp(this.minOffset, this.maxOffset, Mathf.Pow(Random.value, this.randomPow));
		spawned.transform.position += Random.onUnitSphere * d;
	}

	// Token: 0x040012CF RID: 4815
	public float minOffset;

	// Token: 0x040012D0 RID: 4816
	public float maxOffset = 0.5f;

	// Token: 0x040012D1 RID: 4817
	public float randomPow = 1f;
}

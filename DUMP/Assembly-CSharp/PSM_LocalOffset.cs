using System;
using UnityEngine;

// Token: 0x020002F0 RID: 752
public class PSM_LocalOffset : PropSpawnerMod
{
	// Token: 0x060014BD RID: 5309 RVA: 0x00068E2C File Offset: 0x0006702C
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		Vector3 vector = Vector3.zero;
		vector += spawned.transform.right * Mathf.Lerp(-this.offset.x, this.offset.x, Random.value) * Mathf.Pow(Random.value, this.randomPow);
		vector += spawned.transform.up * Mathf.Lerp(-this.offset.y, this.offset.y, Random.value) * Mathf.Pow(Random.value, this.randomPow);
		vector += spawned.transform.forward * Mathf.Lerp(-this.offset.z, this.offset.z, Random.value) * Mathf.Pow(Random.value, this.randomPow);
		spawned.transform.position += vector;
	}

	// Token: 0x040012DE RID: 4830
	public Vector3 offset;

	// Token: 0x040012DF RID: 4831
	[Range(0f, 1f)]
	public float minEffect;

	// Token: 0x040012E0 RID: 4832
	[Range(0f, 1f)]
	public float maxEffect = 1f;

	// Token: 0x040012E1 RID: 4833
	public float randomPow = 1f;
}

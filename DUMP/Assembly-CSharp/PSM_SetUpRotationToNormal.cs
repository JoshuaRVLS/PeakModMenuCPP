using System;
using UnityEngine;

// Token: 0x020002ED RID: 749
public class PSM_SetUpRotationToNormal : PropSpawnerMod
{
	// Token: 0x060014B7 RID: 5303 RVA: 0x00068CFC File Offset: 0x00066EFC
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		Vector3 normal = this.flipNormal ? (-spawnData.normal) : spawnData.normal;
		spawned.transform.rotation = Quaternion.Lerp(spawned.transform.rotation, HelperFunctions.GetRandomRotationWithUp(normal), Mathf.Lerp(this.minEffect, this.maxEffect, Mathf.Pow(Random.value, this.randomPow)));
	}

	// Token: 0x040012D7 RID: 4823
	public bool flipNormal;

	// Token: 0x040012D8 RID: 4824
	[Range(0f, 1f)]
	public float minEffect;

	// Token: 0x040012D9 RID: 4825
	[Range(0f, 1f)]
	public float maxEffect = 1f;

	// Token: 0x040012DA RID: 4826
	public float randomPow = 1f;
}

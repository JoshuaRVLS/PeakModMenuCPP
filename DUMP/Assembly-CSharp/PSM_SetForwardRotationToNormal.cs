using System;
using UnityEngine;

// Token: 0x020002EE RID: 750
public class PSM_SetForwardRotationToNormal : PropSpawnerMod
{
	// Token: 0x060014B9 RID: 5305 RVA: 0x00068D88 File Offset: 0x00066F88
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		spawned.transform.rotation = Quaternion.Lerp(spawned.transform.rotation, Quaternion.LookRotation(spawnData.normal), Mathf.Lerp(this.minEffect, this.maxEffect, Mathf.Pow(Random.value, this.randomPow)));
	}

	// Token: 0x040012DB RID: 4827
	[Range(0f, 1f)]
	public float minEffect;

	// Token: 0x040012DC RID: 4828
	[Range(0f, 1f)]
	public float maxEffect = 1f;

	// Token: 0x040012DD RID: 4829
	public float randomPow = 1f;
}

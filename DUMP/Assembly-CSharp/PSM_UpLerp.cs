using System;
using UnityEngine;

// Token: 0x020002F4 RID: 756
public class PSM_UpLerp : PropSpawnerMod
{
	// Token: 0x060014C5 RID: 5317 RVA: 0x00069134 File Offset: 0x00067334
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		float t = Mathf.Pow(Random.value, this.randomPow);
		float t2 = Mathf.Lerp(this.minUpLerp, this.maxUpLerp, t);
		Vector3 vector = spawned.transform.up;
		vector = Vector3.Lerp(vector, Vector3.up, t2);
		spawned.transform.rotation = HelperFunctions.GetRotationWithUp(spawned.transform.forward, vector);
	}

	// Token: 0x040012ED RID: 4845
	[Range(0f, 1f)]
	public float minUpLerp;

	// Token: 0x040012EE RID: 4846
	[Range(0f, 1f)]
	public float maxUpLerp = 1f;

	// Token: 0x040012EF RID: 4847
	public float randomPow = 1f;
}

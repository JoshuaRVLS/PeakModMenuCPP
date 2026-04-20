using System;
using UnityEngine;

// Token: 0x020002F8 RID: 760
public class PSM_SpecificRotation : PropSpawnerMod
{
	// Token: 0x060014CD RID: 5325 RVA: 0x00069340 File Offset: 0x00067540
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		Vector3 vector = this.eulerAngles;
		if (this.random > 0f)
		{
			vector = Vector3.Lerp(vector, this.eulerAnglesRandom, Random.value * this.random);
		}
		Quaternion rotation = Quaternion.Lerp(spawned.transform.rotation, Quaternion.Euler(vector), this.blend);
		if (this.addRotation)
		{
			spawned.transform.localRotation = Quaternion.Lerp(spawned.transform.localRotation, Quaternion.Euler(vector) * spawned.transform.localRotation, this.blend);
			return;
		}
		spawned.transform.rotation = rotation;
	}

	// Token: 0x040012F6 RID: 4854
	public bool addRotation;

	// Token: 0x040012F7 RID: 4855
	public Vector3 eulerAngles;

	// Token: 0x040012F8 RID: 4856
	[Range(0f, 1f)]
	public float random;

	// Token: 0x040012F9 RID: 4857
	public Vector3 eulerAnglesRandom;

	// Token: 0x040012FA RID: 4858
	[Range(0f, 1f)]
	public float blend = 1f;
}

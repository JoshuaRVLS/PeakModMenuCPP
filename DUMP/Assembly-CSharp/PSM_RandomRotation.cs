using System;
using UnityEngine;

// Token: 0x020002EC RID: 748
public class PSM_RandomRotation : PropSpawnerMod
{
	// Token: 0x060014B5 RID: 5301 RVA: 0x00068BF0 File Offset: 0x00066DF0
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		spawned.transform.rotation = Quaternion.Lerp(spawned.transform.rotation, Random.rotation, Mathf.Lerp(this.minRotation, this.maxRotation, Mathf.Pow(Random.value, this.randomPow)));
		float x = Mathf.Round(spawned.transform.eulerAngles.x / this.increment) * this.increment;
		float y = Mathf.Round(spawned.transform.eulerAngles.y / this.increment) * this.increment;
		float z = Mathf.Round(spawned.transform.eulerAngles.z / this.increment) * this.increment;
		spawned.transform.eulerAngles = Vector3.Lerp(spawned.transform.eulerAngles, new Vector3(x, y, z), this.snapToIncrement);
	}

	// Token: 0x040012D2 RID: 4818
	[Range(0f, 1f)]
	public float minRotation;

	// Token: 0x040012D3 RID: 4819
	[Range(0f, 1f)]
	public float maxRotation = 0.5f;

	// Token: 0x040012D4 RID: 4820
	public float randomPow = 1f;

	// Token: 0x040012D5 RID: 4821
	[Range(0f, 1f)]
	public float snapToIncrement;

	// Token: 0x040012D6 RID: 4822
	public float increment = 90f;
}

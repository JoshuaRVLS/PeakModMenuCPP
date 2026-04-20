using System;
using UnityEngine;

// Token: 0x020002F3 RID: 755
public class PSM_PlacementOffset : PropSpawnerMod
{
	// Token: 0x060014C3 RID: 5315 RVA: 0x0006903C File Offset: 0x0006723C
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		float num = Mathf.Lerp(this.minHeight.x, this.maxHeight.x, spawnData.placement.x);
		float num2 = Mathf.Lerp(this.minHeight.y, this.maxHeight.y, spawnData.placement.y);
		spawned.transform.position += Vector3.right * (num + num2) * this.xMult;
		spawned.transform.position += Vector3.up * (num + num2) * this.yMult;
		spawned.transform.position += Vector3.forward * (num + num2) * this.zMult;
	}

	// Token: 0x040012E8 RID: 4840
	public float xMult;

	// Token: 0x040012E9 RID: 4841
	public float yMult = 1f;

	// Token: 0x040012EA RID: 4842
	public float zMult;

	// Token: 0x040012EB RID: 4843
	public Vector2 minHeight;

	// Token: 0x040012EC RID: 4844
	public Vector2 maxHeight;
}

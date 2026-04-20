using System;
using UnityEngine;

// Token: 0x02000302 RID: 770
public class PSC_Normal : PropSpawnerConstraint
{
	// Token: 0x060014E5 RID: 5349 RVA: 0x000699B0 File Offset: 0x00067BB0
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		float num = Vector3.Angle(Vector3.up, spawnData.normal);
		return num < this.maxAngle && num > this.minAngle;
	}

	// Token: 0x0400130F RID: 4879
	public float minAngle;

	// Token: 0x04001310 RID: 4880
	public float maxAngle = 50f;
}

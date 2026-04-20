using System;
using UnityEngine;

// Token: 0x02000308 RID: 776
public class PSC_Perlin : PropSpawnerConstraint
{
	// Token: 0x060014F1 RID: 5361 RVA: 0x00069C5C File Offset: 0x00067E5C
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		float num = Mathf.PerlinNoise((spawnData.pos.x + 500f + this.perlinOffset) * this.perlinSize * 0.1f, (spawnData.pos.z + 500f + this.perlinOffset) * this.perlinSize * 0.1f);
		return num > this.minMax.x && num < this.minMax.y;
	}

	// Token: 0x0400131D RID: 4893
	public float perlinSize = 10f;

	// Token: 0x0400131E RID: 4894
	public float perlinOffset;

	// Token: 0x0400131F RID: 4895
	public Vector2 minMax = new Vector2(0f, 0.5f);
}

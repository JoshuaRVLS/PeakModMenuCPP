using System;
using UnityEngine;

// Token: 0x0200030B RID: 779
public class PSC_VolumeLight : PropSpawnerConstraint, IValidationConstraint
{
	// Token: 0x060014F7 RID: 5367 RVA: 0x00069E64 File Offset: 0x00068064
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		Color color = LightVolume.Instance().SamplePosition(spawnData.pos);
		return color.a > this.minMax.x && color.a < this.minMax.y;
	}

	// Token: 0x04001325 RID: 4901
	public Vector2 minMax = new Vector2(0f, 0.5f);
}

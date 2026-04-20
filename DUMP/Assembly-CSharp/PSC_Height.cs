using System;

// Token: 0x02000305 RID: 773
public class PSC_Height : PropSpawnerConstraint
{
	// Token: 0x060014EB RID: 5355 RVA: 0x00069AF0 File Offset: 0x00067CF0
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		return spawnData.pos.y > this.minHeight && spawnData.pos.y < this.maxHeight;
	}

	// Token: 0x04001314 RID: 4884
	public float maxHeight = 10000f;

	// Token: 0x04001315 RID: 4885
	public float minHeight = -10000f;
}

using System;
using System.Linq;
using UnityEngine;

// Token: 0x02000307 RID: 775
public class PSC_SameTypeDistance : PropSpawnerConstraint
{
	// Token: 0x060014EF RID: 5359 RVA: 0x00069BD0 File Offset: 0x00067DD0
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		PSC_SameTypeDistance.<>c__DisplayClass3_0 CS$<>8__locals1 = new PSC_SameTypeDistance.<>c__DisplayClass3_0();
		CS$<>8__locals1.spawnData = spawnData;
		CS$<>8__locals1.<>4__this = this;
		if (this.findAllSpawners)
		{
			if ((from levenGenStep in Object.FindObjectsByType<LevelGenStep>(FindObjectsSortMode.None)
			where levenGenStep.name == CS$<>8__locals1.spawnData.spawnerTransform.name
			select levenGenStep).Any((LevelGenStep spawner) => base.<CheckConstraint>g__AnyChildrenTooClose|0(spawner.transform)))
			{
				return false;
			}
		}
		else if (CS$<>8__locals1.<CheckConstraint>g__AnyChildrenTooClose|0(CS$<>8__locals1.spawnData.spawnerTransform))
		{
			return false;
		}
		return true;
	}

	// Token: 0x0400131A RID: 4890
	public float minDistance = 5f;

	// Token: 0x0400131B RID: 4891
	public bool findAllSpawners;

	// Token: 0x0400131C RID: 4892
	public Vector3 axisMultipliers = Vector3.one;
}

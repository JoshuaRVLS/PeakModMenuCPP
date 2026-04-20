using System;
using UnityEngine;

// Token: 0x0200030E RID: 782
public class PSCP_LineCheck : PropSpawnerConstraintPost, IValidationConstraint
{
	// Token: 0x060014FF RID: 5375 RVA: 0x00069EE4 File Offset: 0x000680E4
	public override bool CheckConstraint(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		return !HelperFunctions.LineCheck(spawned.transform.TransformPoint(this.localStart), spawned.transform.TransformPoint(this.localEnd), HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform;
	}

	// Token: 0x04001327 RID: 4903
	public Vector3 localStart = new Vector3(0f, 0.1f, 0f);

	// Token: 0x04001328 RID: 4904
	public Vector3 localEnd = new Vector3(0f, 5f, 0f);
}

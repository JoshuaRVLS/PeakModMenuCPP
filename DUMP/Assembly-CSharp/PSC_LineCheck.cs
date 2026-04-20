using System;
using UnityEngine;

// Token: 0x02000300 RID: 768
public class PSC_LineCheck : PropSpawnerConstraint
{
	// Token: 0x060014DE RID: 5342 RVA: 0x000695FC File Offset: 0x000677FC
	public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
	{
		Vector3 vector = spawnData.hit.point + Vector3.Scale(spawnData.spawnerTransform.lossyScale, this.localStart);
		Vector3 vector2 = vector + Vector3.Scale(spawnData.spawnerTransform.localScale, this.localEnd);
		bool flag = !HelperFunctions.LineCheck(vector, vector2, HelperFunctions.LayerType.TerrainMap, this.radius, QueryTriggerInteraction.Ignore).transform;
		if (this.invert)
		{
			flag = !flag;
		}
		Debug.DrawLine(vector, vector2, flag ? Color.green : Color.red, 10f);
		return flag;
	}

	// Token: 0x04001304 RID: 4868
	public bool invert;

	// Token: 0x04001305 RID: 4869
	public float radius;

	// Token: 0x04001306 RID: 4870
	public Vector3 localStart = new Vector3(0f, 0f, 0f);

	// Token: 0x04001307 RID: 4871
	public Vector3 localEnd = new Vector3(0f, 5f, 0f);
}

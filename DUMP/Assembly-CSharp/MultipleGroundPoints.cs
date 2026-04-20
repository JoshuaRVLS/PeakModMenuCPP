using System;
using UnityEngine;

// Token: 0x020002B6 RID: 694
public class MultipleGroundPoints : CustomSpawnCondition
{
	// Token: 0x060013A7 RID: 5031 RVA: 0x00063C78 File Offset: 0x00061E78
	public override bool CheckCondition(PropSpawner.SpawnData data)
	{
		Transform transform = base.transform.Find("GroundPoints");
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			RaycastHit raycastHit = HelperFunctions.LineCheck(child.position + Vector3.up * this.checkHeight, child.position + Vector3.down * this.checkRange, this.layerType, 0f, QueryTriggerInteraction.Ignore);
			if (!raycastHit.transform)
			{
				return false;
			}
			if (Vector3.Angle(Vector3.up, raycastHit.normal) > this.maxAngle)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x040011F6 RID: 4598
	public HelperFunctions.LayerType layerType;

	// Token: 0x040011F7 RID: 4599
	public float maxAngle = 30f;

	// Token: 0x040011F8 RID: 4600
	public float checkRange = 5f;

	// Token: 0x040011F9 RID: 4601
	public float checkHeight;
}

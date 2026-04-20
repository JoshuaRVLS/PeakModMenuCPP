using System;
using UnityEngine;

// Token: 0x02000317 RID: 791
public class RadiusCheck : CustomSpawnCondition
{
	// Token: 0x06001536 RID: 5430 RVA: 0x0006B208 File Offset: 0x00069408
	public override bool CheckCondition(PropSpawner.SpawnData data)
	{
		LayerMask mask = HelperFunctions.GetMask(this.layerType);
		Collider[] array = Physics.OverlapSphere(base.transform.position, this.radius, mask);
		return array == null || array.Length == 0;
	}

	// Token: 0x0400134B RID: 4939
	public HelperFunctions.LayerType layerType;

	// Token: 0x0400134C RID: 4940
	public float radius = 5f;
}

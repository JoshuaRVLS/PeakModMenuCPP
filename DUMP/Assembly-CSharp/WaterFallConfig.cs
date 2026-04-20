using System;
using UnityEngine;

// Token: 0x0200037E RID: 894
public class WaterFallConfig : CustomSpawnCondition
{
	// Token: 0x0600176D RID: 5997 RVA: 0x00078D88 File Offset: 0x00076F88
	public override bool CheckCondition(PropSpawner.SpawnData data)
	{
		RaycastHit raycastHit = HelperFunctions.LineCheck(this.rayStart.position, this.rayEnd.position, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore);
		if (raycastHit.transform)
		{
			this.endRock.transform.position = raycastHit.point;
			MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
			this.mesh.GetPropertyBlock(materialPropertyBlock);
			materialPropertyBlock.SetFloat("_WorldPositionY", raycastHit.point.y);
			this.mesh.SetPropertyBlock(materialPropertyBlock);
		}
		return true;
	}

	// Token: 0x040015E0 RID: 5600
	public MeshRenderer mesh;

	// Token: 0x040015E1 RID: 5601
	public Transform endRock;

	// Token: 0x040015E2 RID: 5602
	public Transform rayStart;

	// Token: 0x040015E3 RID: 5603
	public Transform rayEnd;
}

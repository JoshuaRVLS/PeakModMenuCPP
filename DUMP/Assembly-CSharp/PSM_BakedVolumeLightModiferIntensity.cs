using System;
using UnityEngine;

// Token: 0x020002E8 RID: 744
public class PSM_BakedVolumeLightModiferIntensity : PropSpawnerMod
{
	// Token: 0x060014AD RID: 5293 RVA: 0x00068A94 File Offset: 0x00066C94
	public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
	{
		BakedVolumeLight component = spawned.GetComponent<BakedVolumeLight>();
		if (!component)
		{
			return;
		}
		if (this.customIntensity)
		{
			component.intensity = this.intensity;
		}
		if (this.customColor)
		{
			component.color = this.color;
		}
	}

	// Token: 0x040012CA RID: 4810
	public bool customColor;

	// Token: 0x040012CB RID: 4811
	public Color color = new Color(0.86f, 0.56f, 0.04f, 0.87f);

	// Token: 0x040012CC RID: 4812
	public bool customIntensity;

	// Token: 0x040012CD RID: 4813
	public float intensity = 0.5f;
}

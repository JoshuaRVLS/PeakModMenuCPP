using System;
using UnityEngine;

// Token: 0x020001B2 RID: 434
public class SpawnConnectingBridge : CustomSpawnCondition
{
	// Token: 0x06000E02 RID: 3586 RVA: 0x00046880 File Offset: 0x00044A80
	public override bool CheckCondition(PropSpawner.SpawnData data)
	{
		SpawnConnectingBridge.parent = base.transform.parent;
		this.treePlatforms = SpawnConnectingBridge.parent.GetComponentsInChildren<TreePlatform>();
		Debug.Log("Checking: " + this.treePlatforms.Length.ToString());
		return true;
	}

	// Token: 0x04000BD6 RID: 3030
	public static Transform parent;

	// Token: 0x04000BD7 RID: 3031
	public TreePlatform[] treePlatforms;
}

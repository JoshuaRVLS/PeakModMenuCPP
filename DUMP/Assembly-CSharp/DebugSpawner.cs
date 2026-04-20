using System;
using UnityEngine;

// Token: 0x0200024F RID: 591
public class DebugSpawner : MonoBehaviour
{
	// Token: 0x060011EA RID: 4586 RVA: 0x0005A40C File Offset: 0x0005860C
	private void Update()
	{
		if (Application.isEditor && Input.GetKeyDown(KeyCode.Alpha0))
		{
			this.Spawn();
		}
	}

	// Token: 0x060011EB RID: 4587 RVA: 0x0005A424 File Offset: 0x00058624
	private void Spawn()
	{
		Object.Instantiate<GameObject>(this.objToSpawn, HelperFunctions.LineCheck(MainCamera.instance.transform.position, MainCamera.instance.transform.TransformPoint(Vector3.forward * 5000f), HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).point, Quaternion.identity);
	}

	// Token: 0x04000FF8 RID: 4088
	public GameObject objToSpawn;
}

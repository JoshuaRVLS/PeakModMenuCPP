using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002E2 RID: 738
public class PropPostSpawnModifiers : MonoBehaviour
{
	// Token: 0x0600148A RID: 5258 RVA: 0x00067C30 File Offset: 0x00065E30
	public void ApplyModifiers()
	{
		PropSpawner[] componentsInChildren = base.GetComponentsInChildren<PropSpawner>();
		List<GameObject> list = new List<GameObject>();
		PropSpawner[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Transform transform = array[i].transform;
			int childCount = transform.childCount;
			for (int j = 0; j < childCount; j++)
			{
				list.Add(transform.GetChild(j).gameObject);
			}
		}
		foreach (GameObject gameObject in list)
		{
			foreach (PropSpawnerMod propSpawnerMod in this.modifiers)
			{
				propSpawnerMod.ModifyObject(gameObject, new PropSpawner.SpawnData
				{
					hit = default(RaycastHit),
					normal = Vector3.up,
					placement = Vector2.zero,
					pos = gameObject.transform.position,
					rayDir = Vector3.zero,
					spawnCount = 0,
					spawnerTransform = null
				});
			}
		}
	}

	// Token: 0x040012AD RID: 4781
	[SerializeReference]
	public List<PropSpawnerMod> modifiers = new List<PropSpawnerMod>();

	// Token: 0x02000528 RID: 1320
	public enum PropModTiming
	{
		// Token: 0x04001C76 RID: 7286
		Early,
		// Token: 0x04001C77 RID: 7287
		Late
	}
}

using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200034C RID: 844
public class SpineCheck : CustomSpawnCondition
{
	// Token: 0x0600166E RID: 5742 RVA: 0x000723EC File Offset: 0x000705EC
	public override bool CheckCondition(PropSpawner.SpawnData data)
	{
		Transform transform = base.transform.Find("Spine");
		for (int i = 0; i < transform.childCount - 1; i++)
		{
			Transform child = transform.GetChild(i);
			Transform child2 = transform.GetChild(i + 1);
			if (HelperFunctions.LineCheck(child.position, child2.position, this.layerType, 0f, QueryTriggerInteraction.Ignore).transform)
			{
				return false;
			}
		}
		this.successEvent.Invoke();
		return true;
	}

	// Token: 0x040014A5 RID: 5285
	public HelperFunctions.LayerType layerType;

	// Token: 0x040014A6 RID: 5286
	public UnityEvent successEvent;
}

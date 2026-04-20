using System;
using UnityEngine;

// Token: 0x0200004B RID: 75
public class CookingBehavior_EnableScripts : AdditionalCookingBehavior
{
	// Token: 0x06000445 RID: 1093 RVA: 0x0001B00C File Offset: 0x0001920C
	protected override void TriggerBehaviour(int cookedAmount)
	{
		foreach (MonoBehaviour monoBehaviour in this.scriptsToEnable)
		{
			if (monoBehaviour != null)
			{
				monoBehaviour.enabled = true;
			}
		}
	}

	// Token: 0x040004B8 RID: 1208
	public MonoBehaviour[] scriptsToEnable;
}

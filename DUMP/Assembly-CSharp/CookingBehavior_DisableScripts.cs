using System;
using UnityEngine;

// Token: 0x0200004A RID: 74
public class CookingBehavior_DisableScripts : AdditionalCookingBehavior
{
	// Token: 0x06000443 RID: 1091 RVA: 0x0001AFCC File Offset: 0x000191CC
	protected override void TriggerBehaviour(int cookedAmount)
	{
		foreach (MonoBehaviour monoBehaviour in this.scriptsToDisable)
		{
			if (monoBehaviour != null)
			{
				monoBehaviour.enabled = false;
			}
		}
	}

	// Token: 0x040004B7 RID: 1207
	public MonoBehaviour[] scriptsToDisable;
}

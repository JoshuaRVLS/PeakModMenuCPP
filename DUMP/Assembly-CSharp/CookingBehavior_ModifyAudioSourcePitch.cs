using System;
using UnityEngine;

// Token: 0x02000051 RID: 81
public class CookingBehavior_ModifyAudioSourcePitch : AdditionalCookingBehavior
{
	// Token: 0x06000451 RID: 1105 RVA: 0x0001B168 File Offset: 0x00019368
	protected override void TriggerBehaviour(int cookedAmount)
	{
		for (int i = 0; i < cookedAmount; i++)
		{
			this.source.pitch += this.changePerCooking;
		}
	}

	// Token: 0x040004C0 RID: 1216
	public float changePerCooking;

	// Token: 0x040004C1 RID: 1217
	public AudioSource source;
}

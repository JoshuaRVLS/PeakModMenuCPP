using System;
using UnityEngine;

// Token: 0x0200004D RID: 77
public class CookingBehavior_MessUpAudio : AdditionalCookingBehavior
{
	// Token: 0x06000449 RID: 1097 RVA: 0x0001B086 File Offset: 0x00019286
	protected override void TriggerBehaviour(int cookedAmount)
	{
		this.source.pitch -= this.pitchReductionPerCooking * (float)cookedAmount;
		this.source.volume -= this.volumeReductionPerCooking * (float)cookedAmount;
	}

	// Token: 0x040004BA RID: 1210
	public AudioSource source;

	// Token: 0x040004BB RID: 1211
	public float pitchReductionPerCooking;

	// Token: 0x040004BC RID: 1212
	public float volumeReductionPerCooking;
}

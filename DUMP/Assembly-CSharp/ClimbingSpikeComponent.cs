using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000019 RID: 25
public class ClimbingSpikeComponent : ItemComponent
{
	// Token: 0x06000232 RID: 562 RVA: 0x00010FD5 File Offset: 0x0000F1D5
	public override void OnInstanceDataSet()
	{
	}

	// Token: 0x06000233 RID: 563 RVA: 0x00010FD7 File Offset: 0x0000F1D7
	private void Start()
	{
		this.item.overrideUsability = Optionable<bool>.Some(false);
	}

	// Token: 0x04000208 RID: 520
	public GameObject hammeredVersionPrefab;

	// Token: 0x04000209 RID: 521
	public GameObject climbingSpikePreviewPrefab;

	// Token: 0x0400020A RID: 522
	public float climbingSpikeStartDistance;

	// Token: 0x0400020B RID: 523
	public float climbingSpikePreviewDisableDistance;

	// Token: 0x0400020C RID: 524
	public float climbingSpikeStartDistanceGrounded;

	// Token: 0x0400020D RID: 525
	public float climbingSpikePreviewDisableDistanceGrounded;
}

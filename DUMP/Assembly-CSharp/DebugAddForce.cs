using System;
using UnityEngine;

// Token: 0x02000096 RID: 150
public class DebugAddForce : ItemComponent
{
	// Token: 0x06000604 RID: 1540 RVA: 0x00022A1D File Offset: 0x00020C1D
	public override void OnInstanceDataSet()
	{
	}

	// Token: 0x06000605 RID: 1541 RVA: 0x00022A20 File Offset: 0x00020C20
	private void FixedUpdate()
	{
		if (this.item.itemState == ItemState.Ground && this.item.photonView.IsMine && !this.item.rig.isKinematic)
		{
			this.item.rig.linearVelocity = Vector3.right * this.force;
		}
	}

	// Token: 0x04000620 RID: 1568
	public float force;
}

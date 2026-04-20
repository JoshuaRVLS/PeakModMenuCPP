using System;
using UnityEngine;

// Token: 0x02000233 RID: 563
public class ChessStabilizer : MonoBehaviour
{
	// Token: 0x06001164 RID: 4452 RVA: 0x00057876 File Offset: 0x00055A76
	private void Start()
	{
		this.startingPos = base.transform.position;
		this.startingRot = base.transform.rotation;
		if (this.startKinematic)
		{
			this.item.SetKinematic(true);
		}
	}

	// Token: 0x06001165 RID: 4453 RVA: 0x000578B0 File Offset: 0x00055AB0
	private void FixedUpdate()
	{
		if (this.item.itemState == ItemState.Ground && !this.item.rig.isKinematic)
		{
			Vector3 up = base.transform.up;
			Vector3 normalized = Vector3.Cross(up, Vector3.up).normalized;
			float d = Vector3.Angle(up, Vector3.up);
			Vector3 a = normalized * d * this.torqueStrength;
			Vector3 b = -this.item.rig.angularVelocity * this.dampingStrength;
			this.item.rig.AddTorque(a + b, ForceMode.Acceleration);
			this.groundTimer += Time.fixedDeltaTime;
			if (this.groundTimer > 2f && this.item.rig.linearVelocity.sqrMagnitude < 0.5f && this.item.rig.angularVelocity.sqrMagnitude < 0.5f && Vector3.Angle(base.transform.up, Vector3.up) < 2f)
			{
				this.item.SetKinematic(true);
			}
		}
	}

	// Token: 0x04000F31 RID: 3889
	public bool startKinematic;

	// Token: 0x04000F32 RID: 3890
	public Item item;

	// Token: 0x04000F33 RID: 3891
	private Vector3 startingPos;

	// Token: 0x04000F34 RID: 3892
	private Quaternion startingRot;

	// Token: 0x04000F35 RID: 3893
	private float groundTimer;

	// Token: 0x04000F36 RID: 3894
	public float torqueStrength = 10f;

	// Token: 0x04000F37 RID: 3895
	public float dampingStrength = 5f;
}

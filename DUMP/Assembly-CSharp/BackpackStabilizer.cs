using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000210 RID: 528
public class BackpackStabilizer : MonoBehaviour
{
	// Token: 0x06001045 RID: 4165 RVA: 0x00050D3A File Offset: 0x0004EF3A
	private void Start()
	{
		this.startingPos = base.transform.position;
		this.startingRot = base.transform.rotation;
	}

	// Token: 0x06001046 RID: 4166 RVA: 0x00050D60 File Offset: 0x0004EF60
	private void FixedUpdate()
	{
		if (this.backpack.photonView.IsMine && this.backpack.itemState == ItemState.Ground && this.backpack.lastThrownAmount < 0.15f && this.timeSinceSpawned < 0.5f)
		{
			this.timeSinceSpawned += Time.fixedDeltaTime;
			if (this.timeSinceSpawned >= 0.5f)
			{
				Debug.Log("Distance moved: " + Vector3.Distance(base.transform.position, this.startingPos).ToString());
			}
			if (Vector3.Distance(base.transform.position, this.startingPos) > 5f)
			{
				this.ResetPosition();
			}
		}
		if (this.backpack.itemState == ItemState.Ground)
		{
			Vector3 up = base.transform.up;
			Vector3 normalized = Vector3.Cross(up, Vector3.up).normalized;
			float d = Vector3.Angle(up, Vector3.up);
			Vector3 torque = normalized * d * this.torqueStrength;
			this.backpack.rig.AddTorque(torque, ForceMode.Acceleration);
		}
	}

	// Token: 0x06001047 RID: 4167 RVA: 0x00050E80 File Offset: 0x0004F080
	private void ResetPosition()
	{
		this.backpack.photonView.RPC("SetKinematicAndResetSyncData", RpcTarget.All, new object[]
		{
			true,
			this.startingPos,
			this.startingRot
		});
	}

	// Token: 0x06001048 RID: 4168 RVA: 0x00050ECE File Offset: 0x0004F0CE
	private void Teleport()
	{
		base.transform.position += Vector3.up * 6f;
	}

	// Token: 0x04000E2F RID: 3631
	public Backpack backpack;

	// Token: 0x04000E30 RID: 3632
	private Vector3 startingPos;

	// Token: 0x04000E31 RID: 3633
	private Quaternion startingRot;

	// Token: 0x04000E32 RID: 3634
	private float timeSinceSpawned;

	// Token: 0x04000E33 RID: 3635
	private const float failsafeTime = 0.5f;

	// Token: 0x04000E34 RID: 3636
	private const float failsafeDistance = 5f;

	// Token: 0x04000E35 RID: 3637
	public float torqueStrength = 10f;
}

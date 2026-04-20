using System;
using UnityEngine;

// Token: 0x020002C0 RID: 704
public class PackpackHover : MonoBehaviour
{
	// Token: 0x060013EC RID: 5100 RVA: 0x00064D88 File Offset: 0x00062F88
	private void Start()
	{
		this.forward = base.transform.forward;
		this.up = base.transform.up;
		this.item = base.GetComponent<Item>();
		this.rig = base.GetComponent<Rigidbody>();
		this.hit = HelperFunctions.LineCheck(base.transform.position, base.transform.position + Vector3.down * 2f, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore);
	}

	// Token: 0x060013ED RID: 5101 RVA: 0x00064E0C File Offset: 0x0006300C
	private void FixedUpdate()
	{
		if (this.rig == null)
		{
			return;
		}
		if (!this.hit.transform)
		{
			return;
		}
		if (this.item.itemState != ItemState.Ground)
		{
			return;
		}
		if (!this.item.photonView.IsMine)
		{
			return;
		}
		Vector3 a = this.hit.point + this.hit.normal * 1f;
		this.rig.AddForce((a - base.transform.position) * 60f, ForceMode.Acceleration);
		Vector3 a2 = Vector3.Cross(base.transform.forward, this.forward).normalized * Vector3.Angle(base.transform.forward, this.forward);
		a2 += Vector3.Cross(base.transform.up, this.up).normalized * Vector3.Angle(base.transform.up, this.up);
		this.rig.AddTorque(a2 * 100f, ForceMode.Acceleration);
		this.rig.linearVelocity *= 0.8f;
		this.rig.angularVelocity *= 0.8f;
	}

	// Token: 0x04001222 RID: 4642
	private Rigidbody rig;

	// Token: 0x04001223 RID: 4643
	private RaycastHit hit;

	// Token: 0x04001224 RID: 4644
	private Item item;

	// Token: 0x04001225 RID: 4645
	private Vector3 forward;

	// Token: 0x04001226 RID: 4646
	private Vector3 up;
}

using System;
using UnityEngine;

// Token: 0x0200022A RID: 554
public class CenterOfMass : MonoBehaviour
{
	// Token: 0x060010EA RID: 4330 RVA: 0x0005405C File Offset: 0x0005225C
	private void Start()
	{
		if (this.onlyOnGround)
		{
			this.item = base.GetComponent<Item>();
			if (this.item.itemState != ItemState.Ground)
			{
				return;
			}
		}
		this.rb = base.GetComponent<Rigidbody>();
		this.rb.centerOfMass = this.localCenterOfMass;
		this.rb.angularDamping = this.angularDamping;
		if (this.centerOfMassTransform)
		{
			this.rb.centerOfMass = this.centerOfMassTransform.localPosition;
		}
	}

	// Token: 0x060010EB RID: 4331 RVA: 0x000540DC File Offset: 0x000522DC
	private void FixedUpdate()
	{
		if (this.onlyOnGround && this.item.itemState != ItemState.Ground)
		{
			return;
		}
		if (this.centerOfMassTransform)
		{
			this.rb.centerOfMass = this.centerOfMassTransform.localPosition;
		}
		else
		{
			this.rb.centerOfMass = this.localCenterOfMass;
		}
		this.rb.angularDamping = this.angularDamping;
	}

	// Token: 0x060010EC RID: 4332 RVA: 0x00054146 File Offset: 0x00052346
	private void OnDrawGizmosSelected()
	{
		if (this.rb)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(this.rb.worldCenterOfMass, 0.5f);
		}
	}

	// Token: 0x04000EED RID: 3821
	public bool onlyOnGround;

	// Token: 0x04000EEE RID: 3822
	private Item item;

	// Token: 0x04000EEF RID: 3823
	public Transform centerOfMassTransform;

	// Token: 0x04000EF0 RID: 3824
	public Vector3 localCenterOfMass;

	// Token: 0x04000EF1 RID: 3825
	public float angularDamping = 3f;

	// Token: 0x04000EF2 RID: 3826
	private Rigidbody rb;
}

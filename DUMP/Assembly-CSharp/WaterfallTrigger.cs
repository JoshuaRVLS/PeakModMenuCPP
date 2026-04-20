using System;
using UnityEngine;

// Token: 0x02000380 RID: 896
public class WaterfallTrigger : MonoBehaviour
{
	// Token: 0x06001771 RID: 6001 RVA: 0x00078F38 File Offset: 0x00077138
	private void OnTriggerStay(Collider other)
	{
		Character componentInParent = other.gameObject.GetComponentInParent<Character>();
		if (!componentInParent)
		{
			return;
		}
		if (this.ragdoll)
		{
			componentInParent.Fall(this.ragdollLength, 0f);
		}
		Rigidbody attachedRigidbody = other.attachedRigidbody;
		if (!attachedRigidbody)
		{
			return;
		}
		Vector3 a = other.transform.position - base.transform.position;
		attachedRigidbody.AddForce(a * this.force, ForceMode.Impulse);
		attachedRigidbody.AddForce(base.transform.forward * this.force / 4f, ForceMode.Acceleration);
		attachedRigidbody.linearVelocity *= this.drag;
	}

	// Token: 0x040015EA RID: 5610
	public float force = 5f;

	// Token: 0x040015EB RID: 5611
	public float drag = 0.9f;

	// Token: 0x040015EC RID: 5612
	public bool ragdoll;

	// Token: 0x040015ED RID: 5613
	public float ragdollLength = 1f;
}

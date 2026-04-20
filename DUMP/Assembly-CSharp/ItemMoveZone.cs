using System;
using UnityEngine;

// Token: 0x02000286 RID: 646
public class ItemMoveZone : MonoBehaviour
{
	// Token: 0x060012A6 RID: 4774 RVA: 0x0005DC54 File Offset: 0x0005BE54
	private void OnTriggerStay(Collider other)
	{
		if (other.attachedRigidbody.GetComponent<Item>() != null)
		{
			other.attachedRigidbody.MovePosition(other.attachedRigidbody.position + base.transform.forward * this.forceMultiplier * Time.fixedDeltaTime);
		}
	}

	// Token: 0x040010BE RID: 4286
	public float forceMultiplier = 1f;
}

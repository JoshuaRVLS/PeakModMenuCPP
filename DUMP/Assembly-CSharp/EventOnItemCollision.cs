using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000B8 RID: 184
public class EventOnItemCollision : ItemComponent
{
	// Token: 0x060006F0 RID: 1776 RVA: 0x00027F26 File Offset: 0x00026126
	private new void Awake()
	{
		this.rb = base.GetComponent<Rigidbody>();
	}

	// Token: 0x060006F1 RID: 1777 RVA: 0x00027F34 File Offset: 0x00026134
	public override void OnInstanceDataSet()
	{
	}

	// Token: 0x060006F2 RID: 1778 RVA: 0x00027F38 File Offset: 0x00026138
	private void OnCollisionEnter(Collision collision)
	{
		if (this.onlyOnce && this.triggered)
		{
			return;
		}
		if (this.onlyWhenImKinematic && this.rb != null && !this.rb.isKinematic)
		{
			return;
		}
		Item componentInParent = collision.gameObject.GetComponentInParent<Item>();
		if (componentInParent == null || componentInParent.itemState != ItemState.Ground)
		{
			return;
		}
		Debug.Log(string.Format("{0} collided with {1} at velocity {2}", base.gameObject.name, componentInParent.gameObject.name, collision.relativeVelocity.magnitude));
		if (collision.relativeVelocity.magnitude > this.minCollisionVelocity)
		{
			this.TriggerEvent();
		}
	}

	// Token: 0x060006F3 RID: 1779 RVA: 0x00027FEC File Offset: 0x000261EC
	internal void TriggerEvent()
	{
		if (this.onlyOnce && this.triggered)
		{
			return;
		}
		this.triggered = true;
		UnityEvent unityEvent = this.eventOnCollided;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		if (this.bonkSFX != null)
		{
			SFX_Player.instance.PlaySFX(this.bonkSFX, base.transform.position, null, null, 1f, false);
		}
	}

	// Token: 0x040006FD RID: 1789
	public bool onlyWhenImKinematic;

	// Token: 0x040006FE RID: 1790
	public UnityEvent eventOnCollided;

	// Token: 0x040006FF RID: 1791
	private Rigidbody rb;

	// Token: 0x04000700 RID: 1792
	public float minCollisionVelocity;

	// Token: 0x04000701 RID: 1793
	public bool onlyOnce;

	// Token: 0x04000702 RID: 1794
	public SFX_Instance bonkSFX;

	// Token: 0x04000703 RID: 1795
	private bool triggered;
}

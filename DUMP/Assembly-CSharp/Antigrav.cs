using System;
using UnityEngine;

// Token: 0x02000104 RID: 260
[RequireComponent(typeof(Item))]
public class Antigrav : MonoBehaviour
{
	// Token: 0x060008C9 RID: 2249 RVA: 0x000303C8 File Offset: 0x0002E5C8
	private void Start()
	{
		this.item = base.GetComponent<Item>();
	}

	// Token: 0x060008CA RID: 2250 RVA: 0x000303D6 File Offset: 0x0002E5D6
	private void FixedUpdate()
	{
		if (this.item.itemState == ItemState.Ground)
		{
			this.item.rig.AddForce(-Physics.gravity * this.intensity, ForceMode.Acceleration);
		}
	}

	// Token: 0x0400085D RID: 2141
	private Item item;

	// Token: 0x0400085E RID: 2142
	public float intensity = 1f;
}

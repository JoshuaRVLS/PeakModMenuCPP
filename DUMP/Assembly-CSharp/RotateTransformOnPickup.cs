using System;
using UnityEngine;

// Token: 0x0200012C RID: 300
[RequireComponent(typeof(Item))]
public class RotateTransformOnPickup : MonoBehaviour
{
	// Token: 0x060009C1 RID: 2497 RVA: 0x00033ED4 File Offset: 0x000320D4
	private void Start()
	{
		if (this.item.itemState == ItemState.Held)
		{
			this.transformToRotate.localEulerAngles += this.rotation;
		}
	}

	// Token: 0x04000906 RID: 2310
	public Vector3 rotation;

	// Token: 0x04000907 RID: 2311
	public Transform transformToRotate;

	// Token: 0x04000908 RID: 2312
	public Item item;
}

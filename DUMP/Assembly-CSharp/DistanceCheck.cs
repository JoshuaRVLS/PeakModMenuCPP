using System;
using UnityEngine;

// Token: 0x0200025B RID: 603
[ExecuteAlways]
public class DistanceCheck : MonoBehaviour
{
	// Token: 0x0600120A RID: 4618 RVA: 0x0005A8FB File Offset: 0x00058AFB
	private void Update()
	{
		if (this.object1 == null || this.object2 == null)
		{
			return;
		}
		this.distance = Vector3.Distance(this.object1.position, this.object2.position);
	}

	// Token: 0x0400100B RID: 4107
	public Transform object1;

	// Token: 0x0400100C RID: 4108
	public Transform object2;

	// Token: 0x0400100D RID: 4109
	public float distance;
}

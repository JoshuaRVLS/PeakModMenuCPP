using System;
using UnityEngine;

// Token: 0x02000271 RID: 625
public class FollowTransform : MonoBehaviour
{
	// Token: 0x0600125A RID: 4698 RVA: 0x0005BEB1 File Offset: 0x0005A0B1
	private void LateUpdate()
	{
		if (this.t)
		{
			base.transform.position = this.t.position;
		}
	}

	// Token: 0x0400106C RID: 4204
	public Transform t;
}

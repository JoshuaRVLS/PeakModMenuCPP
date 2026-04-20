using System;
using UnityEngine;

// Token: 0x020002B4 RID: 692
public class MoveTransform : MonoBehaviour
{
	// Token: 0x0600139F RID: 5023 RVA: 0x00063A71 File Offset: 0x00061C71
	private void Update()
	{
		base.transform.position += this.move * Time.deltaTime;
	}

	// Token: 0x040011F0 RID: 4592
	public Vector3 move;
}

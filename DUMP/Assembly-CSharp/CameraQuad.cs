using System;
using UnityEngine;

// Token: 0x0200006E RID: 110
[ExecuteInEditMode]
[DefaultExecutionOrder(100000)]
public class CameraQuad : MonoBehaviour
{
	// Token: 0x0600051F RID: 1311 RVA: 0x0001E384 File Offset: 0x0001C584
	private void LateUpdate()
	{
		if (!this.cam)
		{
			this.cam = Camera.main;
		}
		float num = this.cam.nearClipPlane + this.distance;
		Vector3 a = this.cam.ViewportToWorldPoint(new Vector3(0f, 0f, num));
		Vector3 b = this.cam.ViewportToWorldPoint(new Vector3(0f, 1f, num));
		Vector3 b2 = this.cam.ViewportToWorldPoint(new Vector3(1f, 0f, num));
		this.cam.ViewportToWorldPoint(new Vector3(1f, 1f, num));
		float y = Vector3.Distance(a, b);
		float x = Vector3.Distance(a, b2);
		base.transform.localScale = new Vector3(x, y, 1f);
		base.transform.position = this.cam.transform.position + this.cam.transform.forward * num;
		base.transform.rotation = this.cam.transform.rotation;
	}

	// Token: 0x04000572 RID: 1394
	public float distance = 0.01f;

	// Token: 0x04000573 RID: 1395
	private Camera cam;
}

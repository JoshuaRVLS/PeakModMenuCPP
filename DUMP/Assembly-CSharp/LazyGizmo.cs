using System;
using UnityEngine;

// Token: 0x02000134 RID: 308
public class LazyGizmo : MonoBehaviour
{
	// Token: 0x060009E6 RID: 2534 RVA: 0x00034AF4 File Offset: 0x00032CF4
	private void DrawGizmos()
	{
		Gizmos.color = this.color;
		if (this.useTop)
		{
			Gizmos.DrawSphere(base.transform.position - Vector3.up * this.radius, this.radius);
			return;
		}
		Gizmos.DrawSphere(base.transform.position, this.radius);
	}

	// Token: 0x060009E7 RID: 2535 RVA: 0x00034B56 File Offset: 0x00032D56
	private void OnDrawGizmos()
	{
		if (!this.onSelected)
		{
			this.DrawGizmos();
		}
	}

	// Token: 0x060009E8 RID: 2536 RVA: 0x00034B68 File Offset: 0x00032D68
	private void OnDrawGizmosSelected()
	{
		if (this.onSelected)
		{
			this.DrawGizmos();
		}
		if (this.showArrows)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(base.transform.position, base.transform.position + base.transform.up * (this.radius + 1f));
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(base.transform.position, base.transform.position + base.transform.forward * (this.radius + 1f));
			Gizmos.color = Color.red;
			Gizmos.DrawLine(base.transform.position, base.transform.position + base.transform.right * (this.radius + 1f));
		}
	}

	// Token: 0x04000941 RID: 2369
	public bool onSelected = true;

	// Token: 0x04000942 RID: 2370
	public bool useTop;

	// Token: 0x04000943 RID: 2371
	public Color color;

	// Token: 0x04000944 RID: 2372
	public float radius;

	// Token: 0x04000945 RID: 2373
	public bool showArrows;
}

using System;
using UnityEngine;

// Token: 0x02000098 RID: 152
public class DebugFogPoints : MonoBehaviour
{
	// Token: 0x06000609 RID: 1545 RVA: 0x00022AF7 File Offset: 0x00020CF7
	private void Start()
	{
	}

	// Token: 0x0600060A RID: 1546 RVA: 0x00022AF9 File Offset: 0x00020CF9
	private void Update()
	{
		this.fogRenderer.material.SetVector("_FogCenter", this.fogPoint.position);
	}

	// Token: 0x04000623 RID: 1571
	public Transform fogPoint;

	// Token: 0x04000624 RID: 1572
	public Renderer fogRenderer;
}

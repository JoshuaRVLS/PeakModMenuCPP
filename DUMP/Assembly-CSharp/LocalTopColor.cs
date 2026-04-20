using System;
using UnityEngine;

// Token: 0x020000AE RID: 174
public class LocalTopColor : MonoBehaviour
{
	// Token: 0x060006A4 RID: 1700 RVA: 0x00026082 File Offset: 0x00024282
	private void Start()
	{
		this.setTopVector();
	}

	// Token: 0x060006A5 RID: 1701 RVA: 0x0002608C File Offset: 0x0002428C
	private void setTopVector()
	{
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		Vector3 v = base.transform.InverseTransformDirection(Vector3.up);
		materialPropertyBlock.SetVector("_LocalTopDirection", v);
		this.renderer.SetPropertyBlock(materialPropertyBlock);
	}

	// Token: 0x04000694 RID: 1684
	public MeshRenderer renderer;
}

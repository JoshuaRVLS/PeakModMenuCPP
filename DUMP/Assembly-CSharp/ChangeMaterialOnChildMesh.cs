using System;
using UnityEngine;

// Token: 0x0200022B RID: 555
public class ChangeMaterialOnChildMesh : MonoBehaviour
{
	// Token: 0x060010EE RID: 4334 RVA: 0x00054188 File Offset: 0x00052388
	public void Go()
	{
		MeshRenderer[] componentsInChildren = base.GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].material = this.material;
		}
	}

	// Token: 0x04000EF3 RID: 3827
	public Material material;
}

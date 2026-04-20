using System;
using UnityEngine;

// Token: 0x0200013D RID: 317
public class MaterialSetter : MonoBehaviour
{
	// Token: 0x06000A72 RID: 2674 RVA: 0x00037BC4 File Offset: 0x00035DC4
	public void setMaterial()
	{
		Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].sharedMaterial = this.material;
		}
	}

	// Token: 0x040009AF RID: 2479
	public Material material;
}

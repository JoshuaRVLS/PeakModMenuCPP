using System;
using UnityEngine;

// Token: 0x02000322 RID: 802
public class RockMaterialSwapper : MonoBehaviour
{
	// Token: 0x0600156C RID: 5484 RVA: 0x0006C5E0 File Offset: 0x0006A7E0
	private void Start()
	{
		Transform[] array = this.parents;
		for (int i = 0; i < array.Length; i++)
		{
			MeshRenderer[] componentsInChildren = array[i].GetComponentsInChildren<MeshRenderer>(true);
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].sharedMaterial = this.mat;
			}
		}
	}

	// Token: 0x0400138D RID: 5005
	public Transform[] parents;

	// Token: 0x0400138E RID: 5006
	public Material mat;
}

using System;
using UnityEngine;

// Token: 0x02000337 RID: 823
public class SetRockColors : MonoBehaviour
{
	// Token: 0x060015D4 RID: 5588 RVA: 0x0006E8FC File Offset: 0x0006CAFC
	private void Start()
	{
		foreach (Material material in this.matsToEdit)
		{
			material.SetColor("_TopColor", this.topColor);
			material.SetColor("_Tint", this.tint);
		}
	}

	// Token: 0x040013D9 RID: 5081
	public Color topColor;

	// Token: 0x040013DA RID: 5082
	[ColorUsage(false, true)]
	public Color tint;

	// Token: 0x040013DB RID: 5083
	public Material[] matsToEdit;
}

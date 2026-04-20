using System;
using UnityEngine;

// Token: 0x02000335 RID: 821
public class SetMaterialProperty : MonoBehaviour
{
	// Token: 0x060015CD RID: 5581 RVA: 0x0006E881 File Offset: 0x0006CA81
	public void Go()
	{
		this.SetVal(this.propertyValue);
	}

	// Token: 0x060015CE RID: 5582 RVA: 0x0006E890 File Offset: 0x0006CA90
	public void SetVal(float val)
	{
		Renderer component = base.GetComponent<Renderer>();
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		component.GetPropertyBlock(materialPropertyBlock);
		materialPropertyBlock.SetFloat(this.propertyName, val);
		component.SetPropertyBlock(materialPropertyBlock);
	}

	// Token: 0x040013D5 RID: 5077
	public string propertyName;

	// Token: 0x040013D6 RID: 5078
	public float propertyValue;
}

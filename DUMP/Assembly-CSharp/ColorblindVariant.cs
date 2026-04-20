using System;
using UnityEngine;

// Token: 0x0200023E RID: 574
public class ColorblindVariant : MonoBehaviour
{
	// Token: 0x0600119B RID: 4507 RVA: 0x00058DA4 File Offset: 0x00056FA4
	private void Awake()
	{
		if (GUIManager.instance && GUIManager.instance.colorblindness)
		{
			if (this.matIndex == 0)
			{
				this.rend.material = this.colorblindMaterial;
			}
			else
			{
				Material[] materials = this.rend.materials;
				materials[this.matIndex] = this.colorblindMaterial;
				this.rend.materials = materials;
			}
			Item item;
			if (base.TryGetComponent<Item>(out item))
			{
				item.UIData.icon = item.UIData.altIcon;
			}
		}
	}

	// Token: 0x04000F83 RID: 3971
	public Renderer rend;

	// Token: 0x04000F84 RID: 3972
	public Material colorblindMaterial;

	// Token: 0x04000F85 RID: 3973
	public int matIndex;
}

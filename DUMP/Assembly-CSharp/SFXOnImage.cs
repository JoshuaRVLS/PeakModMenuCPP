using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000339 RID: 825
public class SFXOnImage : MonoBehaviour
{
	// Token: 0x060015D9 RID: 5593 RVA: 0x0006E9C4 File Offset: 0x0006CBC4
	private void Update()
	{
		if (this.image.texture != this.tex)
		{
			for (int i = 0; i < this.equipSound.Length; i++)
			{
				this.equipSound[i].Play(default(Vector3));
			}
		}
		this.tex = this.image.texture;
	}

	// Token: 0x040013DE RID: 5086
	public RawImage image;

	// Token: 0x040013DF RID: 5087
	private Texture tex;

	// Token: 0x040013E0 RID: 5088
	public SFX_Instance[] equipSound;
}

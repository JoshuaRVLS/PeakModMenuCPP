using System;
using UnityEngine;

// Token: 0x020001DE RID: 478
public class PassportTab : MonoBehaviour
{
	// Token: 0x06000F3D RID: 3901 RVA: 0x0004B2B3 File Offset: 0x000494B3
	public void SetTab()
	{
		if (!this.opened)
		{
			this.manager.OpenTab(this.type);
		}
	}

	// Token: 0x06000F3E RID: 3902 RVA: 0x0004B2CE File Offset: 0x000494CE
	public void Open()
	{
		this.anim.SetBool("Open", true);
		this.opened = true;
	}

	// Token: 0x06000F3F RID: 3903 RVA: 0x0004B2E8 File Offset: 0x000494E8
	public void Close()
	{
		this.anim.SetBool("Open", false);
		this.opened = false;
	}

	// Token: 0x04000CD1 RID: 3281
	public PassportManager manager;

	// Token: 0x04000CD2 RID: 3282
	public Customization.Type type;

	// Token: 0x04000CD3 RID: 3283
	public Animator anim;

	// Token: 0x04000CD4 RID: 3284
	private bool opened;
}

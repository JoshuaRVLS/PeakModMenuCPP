using System;
using UnityEngine;

// Token: 0x0200021E RID: 542
public class BiomeMusicCheck : MonoBehaviour
{
	// Token: 0x06001094 RID: 4244 RVA: 0x000528A8 File Offset: 0x00050AA8
	private void Update()
	{
		if (!this.tornado)
		{
			this.regularMusic.SetActive(true);
			this.mesaMusic.SetActive(false);
			return;
		}
		if (this.tornado.active)
		{
			this.regularMusic.SetActive(false);
			this.mesaMusic.SetActive(true);
			return;
		}
		this.regularMusic.SetActive(true);
		this.mesaMusic.SetActive(false);
	}

	// Token: 0x04000E83 RID: 3715
	public GameObject tornado;

	// Token: 0x04000E84 RID: 3716
	public GameObject regularMusic;

	// Token: 0x04000E85 RID: 3717
	public GameObject mesaMusic;
}

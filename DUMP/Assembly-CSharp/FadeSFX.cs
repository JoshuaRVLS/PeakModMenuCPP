using System;
using UnityEngine;

// Token: 0x02000265 RID: 613
public class FadeSFX : MonoBehaviour
{
	// Token: 0x06001229 RID: 4649 RVA: 0x0005B146 File Offset: 0x00059346
	private void Update()
	{
		AudioListener.volume = this.f;
	}

	// Token: 0x04001034 RID: 4148
	public float f;
}

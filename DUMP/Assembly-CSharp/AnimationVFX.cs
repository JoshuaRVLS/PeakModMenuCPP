using System;
using UnityEngine;

// Token: 0x02000206 RID: 518
public class AnimationVFX : MonoBehaviour
{
	// Token: 0x06001017 RID: 4119 RVA: 0x0004F7BD File Offset: 0x0004D9BD
	public void PlayVFX(int x)
	{
		this.vfx[x].Play();
	}

	// Token: 0x04000DEF RID: 3567
	public ParticleSystem[] vfx;
}

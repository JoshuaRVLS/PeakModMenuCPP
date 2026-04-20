using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200037D RID: 893
public class WarpSFX : MonoBehaviour
{
	// Token: 0x0600176B RID: 5995 RVA: 0x00078D3F File Offset: 0x00076F3F
	private void Update()
	{
		this.warpSFX.volume = this.vol.weight / 2f;
		this.warpSFX.pitch = 1f + this.vol.weight * 2f;
	}

	// Token: 0x040015DE RID: 5598
	public Volume vol;

	// Token: 0x040015DF RID: 5599
	public AudioSource warpSFX;
}

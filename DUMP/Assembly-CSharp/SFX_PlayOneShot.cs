using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020001AD RID: 429
public class SFX_PlayOneShot : MonoBehaviour
{
	// Token: 0x06000DED RID: 3565 RVA: 0x00046362 File Offset: 0x00044562
	public void Start()
	{
		if (this.playOnStart)
		{
			this.Play();
		}
	}

	// Token: 0x06000DEE RID: 3566 RVA: 0x00046372 File Offset: 0x00044572
	public void OnEnable()
	{
		if (this.playOnEnable)
		{
			base.StartCoroutine(this.<OnEnable>g__PlayAfterAnim|6_0());
		}
	}

	// Token: 0x06000DEF RID: 3567 RVA: 0x00046389 File Offset: 0x00044589
	public void Play()
	{
		this.PlayOneShot();
	}

	// Token: 0x06000DF0 RID: 3568 RVA: 0x00046394 File Offset: 0x00044594
	public void PlayOneShot()
	{
		Action action = this.beforePlayAction;
		if (action != null)
		{
			action();
		}
		if (this.sfx != null)
		{
			SFX_Player.instance.PlaySFX(this.sfx, base.transform.position, this.followTransform ? base.transform : null, null, 1f, false);
		}
		for (int i = 0; i < this.sfxs.Length; i++)
		{
			SFX_Player.instance.PlaySFX(this.sfxs[i], base.transform.position, this.followTransform ? base.transform : null, null, 1f, false);
		}
		Action action2 = this.afterPlayAction;
		if (action2 == null)
		{
			return;
		}
		action2();
	}

	// Token: 0x06000DF2 RID: 3570 RVA: 0x0004645C File Offset: 0x0004465C
	[CompilerGenerated]
	private IEnumerator <OnEnable>g__PlayAfterAnim|6_0()
	{
		yield return new WaitForEndOfFrame();
		this.Play();
		yield break;
	}

	// Token: 0x04000BC0 RID: 3008
	public Action beforePlayAction;

	// Token: 0x04000BC1 RID: 3009
	public Action afterPlayAction;

	// Token: 0x04000BC2 RID: 3010
	public bool playOnStart;

	// Token: 0x04000BC3 RID: 3011
	public bool playOnEnable;

	// Token: 0x04000BC4 RID: 3012
	public bool followTransform = true;

	// Token: 0x04000BC5 RID: 3013
	public SFX_Instance sfx;

	// Token: 0x04000BC6 RID: 3014
	public SFX_Instance[] sfxs;
}

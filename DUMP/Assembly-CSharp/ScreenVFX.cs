using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

// Token: 0x0200032F RID: 815
public class ScreenVFX : MonoBehaviour
{
	// Token: 0x060015B9 RID: 5561 RVA: 0x0006E4F4 File Offset: 0x0006C6F4
	private void Start()
	{
		if (GUIManager.instance.photosensitivity)
		{
			this.duration *= 2f;
		}
	}

	// Token: 0x060015BA RID: 5562 RVA: 0x0006E514 File Offset: 0x0006C714
	public void Test()
	{
		this.Play(1f);
	}

	// Token: 0x060015BB RID: 5563 RVA: 0x0006E524 File Offset: 0x0006C724
	public void Play(float amount)
	{
		base.gameObject.SetActive(true);
		amount = 1f;
		if (GUIManager.instance.photosensitivity)
		{
			amount *= 0.3f;
		}
		if (this.tween != null)
		{
			this.tween.Kill(false);
		}
		if (GUIManager.instance.photosensitivity)
		{
			if (this.sequence != null)
			{
				this.sequence.Kill(false);
			}
			this.sequence = DOTween.Sequence();
			this.sequence.Append(this.renderer.material.DOFloat(amount, ScreenVFX.INTENSITY, this.sequenceInitialDuration)).Append(this.renderer.material.DOFloat(0f, ScreenVFX.INTENSITY, this.duration)).SetDelay(this.delay).OnComplete(new TweenCallback(this.Disable));
			return;
		}
		this.renderer.material.SetFloat(ScreenVFX.INTENSITY, amount);
		this.tween = this.renderer.material.DOFloat(0f, ScreenVFX.INTENSITY, this.duration).SetDelay(this.delay).OnComplete(new TweenCallback(this.Disable));
	}

	// Token: 0x060015BC RID: 5564 RVA: 0x0006E660 File Offset: 0x0006C860
	public void StartFX(float photosensitive = 0.5f)
	{
		base.gameObject.SetActive(true);
		float num = 1f;
		float num2 = this.duration;
		if (GUIManager.instance.photosensitivity)
		{
			num *= photosensitive;
		}
		this.renderer.material.SetFloat(ScreenVFX.INTENSITY, 0f);
		this.renderer.material.DOFloat(num, ScreenVFX.INTENSITY, this.duration);
	}

	// Token: 0x060015BD RID: 5565 RVA: 0x0006E6CD File Offset: 0x0006C8CD
	public void EndFX()
	{
		this.renderer.material.DOFloat(0f, ScreenVFX.INTENSITY, this.duration).OnComplete(new TweenCallback(this.Disable));
	}

	// Token: 0x060015BE RID: 5566 RVA: 0x0006E701 File Offset: 0x0006C901
	private void Disable()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x040013C6 RID: 5062
	private static readonly int INTENSITY = Shader.PropertyToID("_Intensity");

	// Token: 0x040013C7 RID: 5063
	public Renderer renderer;

	// Token: 0x040013C8 RID: 5064
	public float sequenceInitialDuration = 0.25f;

	// Token: 0x040013C9 RID: 5065
	public float duration = 0.5f;

	// Token: 0x040013CA RID: 5066
	public float delay = 0.25f;

	// Token: 0x040013CB RID: 5067
	private Tweener tween;

	// Token: 0x040013CC RID: 5068
	private Sequence sequence;
}

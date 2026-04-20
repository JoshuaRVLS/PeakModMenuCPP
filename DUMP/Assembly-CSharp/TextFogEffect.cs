using System;
using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;

// Token: 0x020001EF RID: 495
public class TextFogEffect : MonoBehaviour
{
	// Token: 0x17000121 RID: 289
	// (get) Token: 0x06000FAD RID: 4013 RVA: 0x0004D225 File Offset: 0x0004B425
	public virtual float colorSpeedMult
	{
		get
		{
			return 1f;
		}
	}

	// Token: 0x06000FAE RID: 4014 RVA: 0x0004D22C File Offset: 0x0004B42C
	private void Awake()
	{
		this.m_TextComponent = base.GetComponent<TMP_Text>();
		this.DTanimator = new DOTweenTMPAnimator(this.m_TextComponent);
	}

	// Token: 0x06000FAF RID: 4015 RVA: 0x0004D24B File Offset: 0x0004B44B
	private void Start()
	{
		this.Init();
	}

	// Token: 0x06000FB0 RID: 4016 RVA: 0x0004D253 File Offset: 0x0004B453
	private void OnEnable()
	{
		base.StartCoroutine(this.TextEffectRoutine());
	}

	// Token: 0x06000FB1 RID: 4017 RVA: 0x0004D262 File Offset: 0x0004B462
	private IEnumerator TextEffectRoutine()
	{
		this.textInfo = this.m_TextComponent.textInfo;
		int characterCount = this.textInfo.characterCount;
		for (;;)
		{
			this.UpdateCharacter(Random.Range(0, characterCount));
			yield return new WaitForSeconds(this.period);
		}
		yield break;
	}

	// Token: 0x06000FB2 RID: 4018 RVA: 0x0004D271 File Offset: 0x0004B471
	public virtual void Init()
	{
		this.textInfo = this.m_TextComponent.textInfo;
		int characterCount = this.textInfo.characterCount;
	}

	// Token: 0x06000FB3 RID: 4019 RVA: 0x0004D290 File Offset: 0x0004B490
	private void TryDestroy()
	{
		this.destroyed = true;
		Object.Destroy(this);
	}

	// Token: 0x06000FB4 RID: 4020 RVA: 0x0004D29F File Offset: 0x0004B49F
	private void LateUpdate()
	{
		bool flag = this.destroyed;
	}

	// Token: 0x06000FB5 RID: 4021 RVA: 0x0004D2A8 File Offset: 0x0004B4A8
	protected virtual void EffectRoutine()
	{
		this.textInfo = this.m_TextComponent.textInfo;
		int characterCount = this.textInfo.characterCount;
	}

	// Token: 0x06000FB6 RID: 4022 RVA: 0x0004D2C8 File Offset: 0x0004B4C8
	public void UpdateCharacter(int index)
	{
		if (this.period == 0f)
		{
			return;
		}
		float num = this.offset * (float)index;
		float num2 = Mathf.Sin((Time.time + num) / this.period);
		float num3 = 1f + num2 * this.amplitude;
		if (this.roundSin)
		{
			num3 = Mathf.Round(num3 * this.chunkiness) / this.chunkiness;
		}
		num3 = this.amplitude;
		this.DTanimator.DOOffsetChar(index, Random.insideUnitSphere * num3, this.shiftTime).SetEase(Ease.InOutCubic);
		float time = (Mathf.Sin((Time.time + num) / (this.period / this.colorSpeedMult)) + 1f) * 0.5f;
		this.DTanimator.SetCharColor(index, this.colorGradient.Evaluate(time));
	}

	// Token: 0x04000D43 RID: 3395
	public bool abs;

	// Token: 0x04000D44 RID: 3396
	public float amplitude = 0.2f;

	// Token: 0x04000D45 RID: 3397
	public float period = 0.5f;

	// Token: 0x04000D46 RID: 3398
	public float offset = 0.1f;

	// Token: 0x04000D47 RID: 3399
	public Gradient colorGradient;

	// Token: 0x04000D48 RID: 3400
	public bool skewXtop = true;

	// Token: 0x04000D49 RID: 3401
	public float skewX;

	// Token: 0x04000D4A RID: 3402
	public bool skewYtop = true;

	// Token: 0x04000D4B RID: 3403
	public float skewY;

	// Token: 0x04000D4C RID: 3404
	public bool roundSin;

	// Token: 0x04000D4D RID: 3405
	public float chunkiness = 1f;

	// Token: 0x04000D4E RID: 3406
	public float updateChance = 0.1f;

	// Token: 0x04000D4F RID: 3407
	public float shiftTime = 0.5f;

	// Token: 0x04000D50 RID: 3408
	protected TMP_Text m_TextComponent;

	// Token: 0x04000D51 RID: 3409
	protected TMP_TextInfo textInfo;

	// Token: 0x04000D52 RID: 3410
	public DOTweenTMPAnimator DTanimator;

	// Token: 0x04000D53 RID: 3411
	private bool destroyed;
}

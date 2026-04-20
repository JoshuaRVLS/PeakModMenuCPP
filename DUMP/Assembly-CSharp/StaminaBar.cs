using System;
using System.Collections;
using System.Runtime.CompilerServices;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001EE RID: 494
public class StaminaBar : MonoBehaviour
{
	// Token: 0x06000FA2 RID: 4002 RVA: 0x0004C9C0 File Offset: 0x0004ABC0
	private void Start()
	{
		this.afflictions = base.GetComponentsInChildren<BarAffliction>();
		this.TAU = 6.2831855f;
		BarAffliction[] array = this.afflictions;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(false);
		}
	}

	// Token: 0x06000FA3 RID: 4003 RVA: 0x0004CA08 File Offset: 0x0004AC08
	public void ChangeBar()
	{
		for (int i = 0; i < this.afflictions.Length; i++)
		{
			this.afflictions[i].ChangeAffliction(this);
		}
	}

	// Token: 0x06000FA4 RID: 4004 RVA: 0x0004CA38 File Offset: 0x0004AC38
	private void Update()
	{
		if (Character.observedCharacter == null)
		{
			return;
		}
		for (int i = 0; i < this.afflictions.Length; i++)
		{
			this.afflictions[i].UpdateAffliction(this);
		}
		this.desiredStaminaSize = Mathf.Max(0f, Character.observedCharacter.data.currentStamina * this.fullBar.sizeDelta.x + this.staminaBarOffset);
		if (Character.observedCharacter.data.currentStamina <= 0.005f)
		{
			if (!this.outOfStamina)
			{
				this.outOfStamina = true;
				this.OutOfStaminaPulse();
			}
		}
		else
		{
			this.outOfStamina = false;
		}
		this.staminaBar.sizeDelta = new Vector2(Mathf.Lerp(this.staminaBar.sizeDelta.x, this.desiredStaminaSize, Time.deltaTime * 10f), this.staminaBar.sizeDelta.y);
		Color color = this.staminaGlow.color;
		float num = Mathf.Clamp01((this.staminaBar.sizeDelta.x - this.desiredStaminaSize) * 0.5f);
		this.sinTime += Time.deltaTime * 10f * num;
		color.a = num * 0.4f - Mathf.Abs(Mathf.Sin(this.sinTime)) * 0.2f;
		this.staminaGlow.color = color;
		this.desiredMaxStaminaSize = Mathf.Max(0f, Character.observedCharacter.GetMaxStamina() * this.fullBar.sizeDelta.x + this.staminaBarOffset);
		this.maxStaminaBar.sizeDelta = new Vector2(Mathf.Lerp(this.maxStaminaBar.sizeDelta.x, this.desiredMaxStaminaSize, Time.deltaTime * 10f), this.maxStaminaBar.sizeDelta.y);
		float statusSum = Character.observedCharacter.refs.afflictions.statusSum;
		this.staminaBarOutline.sizeDelta = new Vector2(14f + Mathf.Max(1f, statusSum) * this.fullBar.sizeDelta.x, this.staminaBarOutline.sizeDelta.y);
		this.staminaBarOutlineOverflowBar.gameObject.SetActive((double)statusSum > 1.005);
		this.staminaBar.gameObject.SetActive(this.staminaBar.sizeDelta.x > this.minStaminaBarWidth);
		this.maxStaminaBar.gameObject.SetActive(this.maxStaminaBar.sizeDelta.x > this.minStaminaBarWidth);
		bool flag = Character.observedCharacter.data.extraStamina > 0f;
		if (!this.extraBar.gameObject.activeSelf && flag)
		{
			this.extraBar.sizeDelta = Vector2.zero;
			this.extraBar.DOKill(false);
			this.extraBar.DOSizeDelta(new Vector2(45f, 45f), 0.25f, false).SetEase(Ease.OutCubic);
			this.extraBar.gameObject.SetActive(true);
			this.desiredExtraStaminaSize = Mathf.Max(0f, Character.observedCharacter.data.extraStamina * this.fullBar.sizeDelta.x);
			this.extraBarOutline.sizeDelta = new Vector2(Mathf.Max(20f, this.desiredExtraStaminaSize + 12f), this.extraBarOutline.sizeDelta.y);
			this.extraBarStamina.sizeDelta = new Vector2(this.desiredExtraStaminaSize, this.extraBarStamina.sizeDelta.y);
			this.cachedExtraStam = this.desiredExtraStaminaSize;
		}
		if (this.extraBar.gameObject.activeSelf)
		{
			this.desiredExtraStaminaSize = Mathf.Max(0f, Character.observedCharacter.data.extraStamina * this.fullBar.sizeDelta.x);
			this.cachedExtraStam = Mathf.Lerp(this.cachedExtraStam, this.desiredExtraStaminaSize, Time.deltaTime * 10f);
			if (Mathf.Abs(this.desiredExtraStaminaSize - this.cachedExtraStam) < 0.05f)
			{
				this.extraBarOutline.sizeDelta = new Vector2(Mathf.Lerp(this.extraBarOutline.sizeDelta.x, Mathf.Max(20f, this.desiredExtraStaminaSize + 12f), Time.deltaTime * 10f), this.extraBarOutline.sizeDelta.y);
			}
			else if (this.desiredExtraStaminaSize + 12f > this.extraBarOutline.sizeDelta.x)
			{
				this.extraBarOutline.sizeDelta = new Vector2(Mathf.Max(20f, this.desiredExtraStaminaSize + 12f), this.extraBarOutline.sizeDelta.y);
			}
			Color color2 = this.extraStaminaGlow.color;
			float num2 = Mathf.Clamp01((this.extraBar.sizeDelta.x - this.desiredExtraStaminaSize) * 0.5f);
			this.sinTime += Time.deltaTime * 10f * num2;
			color2.a = num2 * 0.4f - Mathf.Abs(Mathf.Sin(this.sinTime)) * 0.2f;
			this.extraStaminaGlow.color = color2;
			this.extraBarStamina.sizeDelta = new Vector2(Mathf.Max(6f, this.cachedExtraStam), this.extraBarStamina.sizeDelta.y);
			if (!flag && !this.sequencingExtraBar)
			{
				this.sequencingExtraBar = true;
				Sequence sequence = DOTween.Sequence();
				sequence.Append(this.extraBar.DOSizeDelta(new Vector2(this.extraBar.sizeDelta.x, 0f), 0.2f, false));
				sequence.OnComplete(new TweenCallback(this.DisableExtraBar));
			}
		}
		this.shield.gameObject.SetActive(Character.observedCharacter.data.isInvincible);
		this.campfire.gameObject.SetActive(!Character.observedCharacter.refs.afflictions.canGetHungry);
		if (this.sinTime > this.TAU)
		{
			this.sinTime -= this.TAU;
		}
	}

	// Token: 0x06000FA5 RID: 4005 RVA: 0x0004D09C File Offset: 0x0004B29C
	public void OutOfStaminaPulse()
	{
		this.backing.color = this.outOfStaminaBackingColor;
		this.backing.DOColor(this.defaultBackingColor, 0.5f);
		this.noStaminaSFX.Play(default(Vector3));
	}

	// Token: 0x06000FA6 RID: 4006 RVA: 0x0004D0E5 File Offset: 0x0004B2E5
	private void DisableExtraBar()
	{
		this.extraBar.gameObject.SetActive(false);
		this.sequencingExtraBar = false;
	}

	// Token: 0x06000FA7 RID: 4007 RVA: 0x0004D100 File Offset: 0x0004B300
	public void AddRainbow()
	{
		if (this.rainbowRoutine != null)
		{
			base.StopCoroutine(this.rainbowRoutine);
		}
		this.rainbowStamina.enabled = true;
		this.rainbowStamina.color = (GUIManager.instance.photosensitivity ? new Color(0.5f, 0.5f, 0.5f, 0f) : new Color(1f, 1f, 1f, 0f));
		this.rainbowStamina.DOFade(1f, 0.5f);
	}

	// Token: 0x06000FA8 RID: 4008 RVA: 0x0004D18E File Offset: 0x0004B38E
	public void RemoveRainbow()
	{
		this.rainbowStamina.DOFade(0f, 0.5f);
		this.rainbowRoutine = base.StartCoroutine(this.<RemoveRainbow>g__RemoveRainbowRoutine|39_0());
	}

	// Token: 0x06000FA9 RID: 4009 RVA: 0x0004D1B8 File Offset: 0x0004B3B8
	public void PlayMoraleBoost(int scoutCount)
	{
		this.moraleBoostText.enabled = true;
		this.moraleBoostText.text = LocalizedText.GetText("MORALEBOOST", true);
		base.StartCoroutine(this.MoraleBoostRoutine());
	}

	// Token: 0x06000FAA RID: 4010 RVA: 0x0004D1E9 File Offset: 0x0004B3E9
	private IEnumerator MoraleBoostRoutine()
	{
		if (this.animator == null)
		{
			this.animator = new DOTweenTMPAnimator(this.moraleBoostText);
		}
		this.animator.Refresh();
		this.moraleBoostAnimator.Play("Boost", 0, 0f);
		for (int j = 0; j < this.animator.textInfo.characterCount; j++)
		{
			this.animator.SetCharScale(j, Vector3.zero);
		}
		yield return null;
		int num;
		for (int i = 0; i < this.animator.textInfo.characterCount; i = num + 1)
		{
			this.animator.DOScaleChar(i, Vector3.one, 0.2f).SetEase(Ease.OutBack);
			yield return new WaitForSeconds(0.033f);
			num = i;
		}
		yield return new WaitForSeconds(2f);
		yield return new WaitForSeconds(0.5f);
		this.moraleBoostText.enabled = false;
		yield break;
	}

	// Token: 0x06000FAC RID: 4012 RVA: 0x0004D216 File Offset: 0x0004B416
	[CompilerGenerated]
	private IEnumerator <RemoveRainbow>g__RemoveRainbowRoutine|39_0()
	{
		yield return new WaitForSeconds(0.5f);
		this.rainbowStamina.enabled = false;
		yield break;
	}

	// Token: 0x04000D21 RID: 3361
	public Image backing;

	// Token: 0x04000D22 RID: 3362
	public RectTransform fullBar;

	// Token: 0x04000D23 RID: 3363
	public RectTransform staminaBar;

	// Token: 0x04000D24 RID: 3364
	public Image staminaGlow;

	// Token: 0x04000D25 RID: 3365
	public Image extraStaminaGlow;

	// Token: 0x04000D26 RID: 3366
	public RectTransform maxStaminaBar;

	// Token: 0x04000D27 RID: 3367
	public RectTransform staminaBarOutline;

	// Token: 0x04000D28 RID: 3368
	public RectTransform staminaBarOutlineOverflowBar;

	// Token: 0x04000D29 RID: 3369
	public RectTransform extraBar;

	// Token: 0x04000D2A RID: 3370
	public RectTransform extraBarStamina;

	// Token: 0x04000D2B RID: 3371
	public RectTransform extraBarOutline;

	// Token: 0x04000D2C RID: 3372
	public Image rainbowStamina;

	// Token: 0x04000D2D RID: 3373
	[HideInInspector]
	public BarAffliction[] afflictions;

	// Token: 0x04000D2E RID: 3374
	public float staminaBarOffset;

	// Token: 0x04000D2F RID: 3375
	private float desiredStaminaSize;

	// Token: 0x04000D30 RID: 3376
	private float desiredMaxStaminaSize;

	// Token: 0x04000D31 RID: 3377
	private float desiredExtraStaminaSize;

	// Token: 0x04000D32 RID: 3378
	public float minAfflictionWidth = 60f;

	// Token: 0x04000D33 RID: 3379
	public float minStaminaBarWidth = 20f;

	// Token: 0x04000D34 RID: 3380
	public TextMeshProUGUI moraleBoostText;

	// Token: 0x04000D35 RID: 3381
	public Animator moraleBoostAnimator;

	// Token: 0x04000D36 RID: 3382
	public GameObject shield;

	// Token: 0x04000D37 RID: 3383
	public GameObject campfire;

	// Token: 0x04000D38 RID: 3384
	public Color defaultBackingColor;

	// Token: 0x04000D39 RID: 3385
	public Color outOfStaminaBackingColor;

	// Token: 0x04000D3A RID: 3386
	private float TAU;

	// Token: 0x04000D3B RID: 3387
	public SFX_Instance noStaminaSFX;

	// Token: 0x04000D3C RID: 3388
	private float cachedExtraStam;

	// Token: 0x04000D3D RID: 3389
	private float allAfflictionSizes;

	// Token: 0x04000D3E RID: 3390
	private bool outOfStamina;

	// Token: 0x04000D3F RID: 3391
	private float sinTime;

	// Token: 0x04000D40 RID: 3392
	private bool sequencingExtraBar;

	// Token: 0x04000D41 RID: 3393
	private Coroutine rainbowRoutine;

	// Token: 0x04000D42 RID: 3394
	private DOTweenTMPAnimator animator;
}

using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001C3 RID: 451
public class BarAffliction : MonoBehaviour
{
	// Token: 0x17000107 RID: 263
	// (get) Token: 0x06000E72 RID: 3698 RVA: 0x0004887D File Offset: 0x00046A7D
	// (set) Token: 0x06000E73 RID: 3699 RVA: 0x0004888F File Offset: 0x00046A8F
	public float width
	{
		get
		{
			return this.rtf.sizeDelta.x;
		}
		set
		{
			this.rtf.sizeDelta = new Vector2(value, this.rtf.sizeDelta.y);
		}
	}

	// Token: 0x06000E74 RID: 3700 RVA: 0x000488B2 File Offset: 0x00046AB2
	public void OnEnable()
	{
		this.icon.transform.localScale = Vector3.zero;
		this.icon.transform.DOScale(1f, 0.5f).SetEase(Ease.OutElastic);
	}

	// Token: 0x06000E75 RID: 3701 RVA: 0x000488EC File Offset: 0x00046AEC
	public void ChangeAffliction(StaminaBar bar)
	{
		if (Character.observedCharacter == null)
		{
			return;
		}
		float currentStatus = Character.observedCharacter.refs.afflictions.GetCurrentStatus(this.afflictionType);
		this.size = bar.fullBar.sizeDelta.x * currentStatus;
		if (currentStatus > 0.01f)
		{
			if (this.size < bar.minAfflictionWidth)
			{
				this.size = bar.minAfflictionWidth;
			}
			base.gameObject.SetActive(true);
			return;
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x06000E76 RID: 3702 RVA: 0x00048975 File Offset: 0x00046B75
	public void UpdateAffliction(StaminaBar bar)
	{
		this.width = Mathf.Lerp(this.width, this.size, Mathf.Min(Time.deltaTime * 10f, 0.1f));
	}

	// Token: 0x04000C25 RID: 3109
	public RectTransform rtf;

	// Token: 0x04000C26 RID: 3110
	public Image icon;

	// Token: 0x04000C27 RID: 3111
	public float size;

	// Token: 0x04000C28 RID: 3112
	public CharacterAfflictions.STATUSTYPE afflictionType;
}

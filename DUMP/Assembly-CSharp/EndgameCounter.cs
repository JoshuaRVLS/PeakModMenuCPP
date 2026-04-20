using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;

// Token: 0x02000260 RID: 608
public class EndgameCounter : MonoBehaviour
{
	// Token: 0x06001216 RID: 4630 RVA: 0x0005AB18 File Offset: 0x00058D18
	public void UpdateCounter(int value)
	{
		this.counterGroup.gameObject.SetActive(true);
		this.counterGroup.DOFade(1f, 0.25f);
		this.counter.text = (value.ToString() ?? "");
		this.counter.transform.localScale = Vector3.one * 2f;
		this.counter.alpha = 0f;
		this.counter.DOScale(1f, 0.25f).SetEase(Ease.OutCubic);
		this.counter.DOFade(1f, 0.25f).SetEase(Ease.OutCubic);
	}

	// Token: 0x06001217 RID: 4631 RVA: 0x0005ABD0 File Offset: 0x00058DD0
	public void Win()
	{
		this.winGroup.gameObject.SetActive(true);
		this.winGroup.alpha = 0f;
		this.winGroup.DOFade(1f, 1f);
	}

	// Token: 0x06001218 RID: 4632 RVA: 0x0005AC09 File Offset: 0x00058E09
	public void Lose()
	{
		this.loseGroup.gameObject.SetActive(true);
		this.loseGroup.alpha = 0f;
		this.loseGroup.DOFade(1f, 1f);
	}

	// Token: 0x06001219 RID: 4633 RVA: 0x0005AC42 File Offset: 0x00058E42
	public void Disable()
	{
		this.counterGroup.gameObject.SetActive(false);
	}

	// Token: 0x04001017 RID: 4119
	public CanvasGroup counterGroup;

	// Token: 0x04001018 RID: 4120
	public CanvasGroup winGroup;

	// Token: 0x04001019 RID: 4121
	public CanvasGroup loseGroup;

	// Token: 0x0400101A RID: 4122
	public TextMeshProUGUI counter;
}

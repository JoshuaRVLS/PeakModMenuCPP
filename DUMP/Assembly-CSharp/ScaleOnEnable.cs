using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

// Token: 0x0200032C RID: 812
public class ScaleOnEnable : MonoBehaviour
{
	// Token: 0x0600158A RID: 5514 RVA: 0x0006CFC4 File Offset: 0x0006B1C4
	private void OnEnable()
	{
		base.transform.localScale = Vector3.zero;
		base.transform.DOScale(Vector3.one, this.time).SetEase(this.easeType);
		if (this.canvasGroup)
		{
			this.canvasGroup.alpha = 0f;
			this.canvasGroup.DOFade(1f, this.time).SetEase(this.easeType);
		}
	}

	// Token: 0x040013AB RID: 5035
	public float time = 0.25f;

	// Token: 0x040013AC RID: 5036
	public Ease easeType = Ease.OutBounce;

	// Token: 0x040013AD RID: 5037
	public CanvasGroup canvasGroup;
}

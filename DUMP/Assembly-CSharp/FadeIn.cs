using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000264 RID: 612
public class FadeIn : MonoBehaviour
{
	// Token: 0x06001226 RID: 4646 RVA: 0x0005B0D8 File Offset: 0x000592D8
	private void Awake()
	{
		Color color = this.fade.color;
		color.a = 1f;
		this.fade.color = color;
		this.fade.DOFade(0f, 2f).OnComplete(new TweenCallback(this.Disable));
	}

	// Token: 0x06001227 RID: 4647 RVA: 0x0005B130 File Offset: 0x00059330
	private void Disable()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04001033 RID: 4147
	public Image fade;
}

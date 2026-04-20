using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000371 RID: 881
public class UI_UseItemProgress : MonoBehaviour
{
	// Token: 0x1700016C RID: 364
	// (get) Token: 0x06001729 RID: 5929 RVA: 0x000771B7 File Offset: 0x000753B7
	private bool constantUseInteractableExists
	{
		get
		{
			return Interaction.instance.currentHeldInteractible != null;
		}
	}

	// Token: 0x0600172A RID: 5930 RVA: 0x000771C8 File Offset: 0x000753C8
	private void Update()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		Character.localCharacter.data.currentItem != null;
		bool flag = this.UpdateFillAmount();
		if (!this.fill.enabled && flag)
		{
			base.transform.DOKill(false);
			base.transform.localScale = Vector3.zero;
			base.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack);
		}
		this.fill.enabled = flag;
		this.empty.enabled = this.fill.enabled;
	}

	// Token: 0x0600172B RID: 5931 RVA: 0x00077270 File Offset: 0x00075470
	private bool UpdateFillAmount()
	{
		bool flag = Character.localCharacter.data.currentItem != null;
		if (Character.localCharacter.refs.items.climbingSpikeCastProgress > 0f)
		{
			this.fill.fillAmount = Character.localCharacter.refs.items.climbingSpikeCastProgress;
			return true;
		}
		if (flag && Character.localCharacter.data.currentItem.shouldShowCastProgress)
		{
			float progress = Character.localCharacter.data.currentItem.progress;
			if (progress > 0f)
			{
				this.fill.fillAmount = progress;
				return true;
			}
		}
		else if (this.constantUseInteractableExists && Interaction.instance.constantInteractableProgress > 0f)
		{
			this.fill.fillAmount = Interaction.instance.constantInteractableProgress;
			return true;
		}
		return false;
	}

	// Token: 0x040015A5 RID: 5541
	public Image fill;

	// Token: 0x040015A6 RID: 5542
	public Image empty;
}

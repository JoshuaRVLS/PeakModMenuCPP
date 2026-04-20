using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000245 RID: 581
public class CustomOptionBase : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler
{
	// Token: 0x060011AE RID: 4526 RVA: 0x00059330 File Offset: 0x00057530
	public virtual void OnClick()
	{
		SFX_Player.instance.PlaySFX(this.sfxClick, base.transform.position, null, null, 1f, false);
	}

	// Token: 0x060011AF RID: 4527 RVA: 0x00059356 File Offset: 0x00057556
	public void OnPointerEnter(PointerEventData pointerEventData)
	{
		this.Hover();
	}

	// Token: 0x060011B0 RID: 4528 RVA: 0x0005935E File Offset: 0x0005755E
	public void Hover()
	{
		SFX_Player.instance.PlaySFX(this.sfxHover, base.transform.position, null, null, 1f, false);
	}

	// Token: 0x060011B1 RID: 4529 RVA: 0x00059384 File Offset: 0x00057584
	public virtual void RestoreDefault()
	{
	}

	// Token: 0x04000FB1 RID: 4017
	public SFX_Instance sfxClick;

	// Token: 0x04000FB2 RID: 4018
	public SFX_Instance sfxHover;
}

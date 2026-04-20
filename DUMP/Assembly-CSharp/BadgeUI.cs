using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001C2 RID: 450
public class BadgeUI : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
	// Token: 0x06000E6A RID: 3690 RVA: 0x0004879C File Offset: 0x0004699C
	public void Init(BadgeData data)
	{
		this.data = data;
		if (data)
		{
			base.gameObject.SetActive(true);
			this.icon.texture = data.icon;
			this.icon.color = new Color(1f, 1f, 1f, (float)(data.IsLocked ? 0 : 1));
			this.icon.enabled = true;
			this.blank.enabled = false;
			return;
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x06000E6B RID: 3691 RVA: 0x00048826 File Offset: 0x00046A26
	public void Hover()
	{
		this.manager.selectedBadge = this;
	}

	// Token: 0x06000E6C RID: 3692 RVA: 0x00048834 File Offset: 0x00046A34
	public void Dehover()
	{
		if (this.manager.selectedBadge == this)
		{
			this.manager.selectedBadge = null;
		}
	}

	// Token: 0x06000E6D RID: 3693 RVA: 0x00048855 File Offset: 0x00046A55
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.Hover();
	}

	// Token: 0x06000E6E RID: 3694 RVA: 0x0004885D File Offset: 0x00046A5D
	public void OnPointerExit(PointerEventData eventData)
	{
		this.Dehover();
	}

	// Token: 0x06000E6F RID: 3695 RVA: 0x00048865 File Offset: 0x00046A65
	public void OnSelect(BaseEventData eventData)
	{
		this.Hover();
	}

	// Token: 0x06000E70 RID: 3696 RVA: 0x0004886D File Offset: 0x00046A6D
	public void OnDeselect(BaseEventData eventData)
	{
		this.Dehover();
	}

	// Token: 0x04000C20 RID: 3104
	public BadgeManager manager;

	// Token: 0x04000C21 RID: 3105
	public RawImage icon;

	// Token: 0x04000C22 RID: 3106
	public RawImage blank;

	// Token: 0x04000C23 RID: 3107
	public BadgeData data;

	// Token: 0x04000C24 RID: 3108
	public CanvasGroup canvasGroup;
}

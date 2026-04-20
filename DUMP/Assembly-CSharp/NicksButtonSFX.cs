using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020002BD RID: 701
public class NicksButtonSFX : MonoBehaviour, ISelectHandler, IEventSystemHandler, IPointerEnterHandler
{
	// Token: 0x060013C4 RID: 5060 RVA: 0x00064754 File Offset: 0x00062954
	private void Start()
	{
		this.button.onClick.AddListener(new UnityAction(this.OnClick));
	}

	// Token: 0x060013C5 RID: 5061 RVA: 0x00064772 File Offset: 0x00062972
	private void OnClick()
	{
		SFX_Player.instance.PlaySFX(this.sfxClick, base.transform.position, null, null, 1f, false);
	}

	// Token: 0x060013C6 RID: 5062 RVA: 0x00064798 File Offset: 0x00062998
	public void OnPointerEnter(PointerEventData pointerEventData)
	{
		SFX_Player.instance.PlaySFX(this.sfxHover, base.transform.position, null, null, 1f, false);
	}

	// Token: 0x060013C7 RID: 5063 RVA: 0x000647BE File Offset: 0x000629BE
	public void OnSelect(BaseEventData eventData)
	{
		SFX_Player.instance.PlaySFX(this.sfxHover, base.transform.position, null, null, 1f, false);
	}

	// Token: 0x0400120D RID: 4621
	public SFX_Instance sfxClick;

	// Token: 0x0400120E RID: 4622
	public SFX_Instance sfxHover;

	// Token: 0x0400120F RID: 4623
	public Button button;
}

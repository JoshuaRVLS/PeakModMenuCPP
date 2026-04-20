using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200024A RID: 586
public class CustomOptionTooltip : MonoBehaviour, ISelectHandler, IEventSystemHandler, IPointerEnterHandler, IDeselectHandler, IPointerExitHandler, IPointerClickHandler
{
	// Token: 0x060011C6 RID: 4550 RVA: 0x0005959D File Offset: 0x0005779D
	public void OnEnable()
	{
		this.text.SetText("");
	}

	// Token: 0x060011C7 RID: 4551 RVA: 0x000595AF File Offset: 0x000577AF
	public void ShowTooltip()
	{
		this.text.SetTextLocalized(this.index[Mathf.Clamp(this.option, 0, this.index.Length - 1)]);
		this.text.gameObject.SetActive(true);
	}

	// Token: 0x060011C8 RID: 4552 RVA: 0x000595EA File Offset: 0x000577EA
	private void HideTooltip()
	{
		this.text.gameObject.SetActive(false);
	}

	// Token: 0x060011C9 RID: 4553 RVA: 0x000595FD File Offset: 0x000577FD
	public void OnSelect(BaseEventData eventData)
	{
		this.ShowTooltip();
	}

	// Token: 0x060011CA RID: 4554 RVA: 0x00059605 File Offset: 0x00057805
	public void OnDeselect(BaseEventData eventData)
	{
		this.HideTooltip();
	}

	// Token: 0x060011CB RID: 4555 RVA: 0x0005960D File Offset: 0x0005780D
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.ShowTooltip();
	}

	// Token: 0x060011CC RID: 4556 RVA: 0x00059615 File Offset: 0x00057815
	public void OnPointerExit(PointerEventData eventData)
	{
		this.HideTooltip();
	}

	// Token: 0x060011CD RID: 4557 RVA: 0x0005961D File Offset: 0x0005781D
	public void OnPointerClick(PointerEventData data)
	{
		this.ShowTooltip();
	}

	// Token: 0x04000FC3 RID: 4035
	public LocalizedText text;

	// Token: 0x04000FC4 RID: 4036
	public string[] index;

	// Token: 0x04000FC5 RID: 4037
	public int option;
}

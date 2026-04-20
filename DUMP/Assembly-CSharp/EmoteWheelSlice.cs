using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001C8 RID: 456
public class EmoteWheelSlice : UIWheelSlice, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x06000E94 RID: 3732 RVA: 0x00048E24 File Offset: 0x00047024
	public void Init(EmoteWheelData data, EmoteWheel wheel)
	{
		this.emoteWheel = wheel;
		this.emoteData = data;
		if (data == null)
		{
			this.image.enabled = false;
			this.button.interactable = false;
			return;
		}
		this.image.enabled = true;
		this.image.sprite = data.emoteSprite;
		this.button.interactable = true;
	}

	// Token: 0x06000E95 RID: 3733 RVA: 0x00048E8A File Offset: 0x0004708A
	private void Update()
	{
		if (this.emoteData != null && this.emoteData.requireGrounded)
		{
			this.button.interactable = Character.localCharacter.data.isGrounded;
		}
	}

	// Token: 0x06000E96 RID: 3734 RVA: 0x00048EC1 File Offset: 0x000470C1
	public void Hover()
	{
		if (this.button.interactable)
		{
			this.emoteWheel.Hover(this.emoteData);
		}
	}

	// Token: 0x06000E97 RID: 3735 RVA: 0x00048EE1 File Offset: 0x000470E1
	public void Dehover()
	{
		if (this.button.interactable)
		{
			this.emoteWheel.Dehover(this.emoteData);
		}
	}

	// Token: 0x06000E98 RID: 3736 RVA: 0x00048F01 File Offset: 0x00047101
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.Hover();
	}

	// Token: 0x06000E99 RID: 3737 RVA: 0x00048F09 File Offset: 0x00047109
	public void OnPointerExit(PointerEventData eventData)
	{
		this.Dehover();
	}

	// Token: 0x04000C40 RID: 3136
	private EmoteWheel emoteWheel;

	// Token: 0x04000C41 RID: 3137
	private EmoteWheelData emoteData;

	// Token: 0x04000C42 RID: 3138
	public Image image;
}

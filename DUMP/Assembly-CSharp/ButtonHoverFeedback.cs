using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000227 RID: 551
public class ButtonHoverFeedback : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x060010DD RID: 4317 RVA: 0x00053E89 File Offset: 0x00052089
	private void Start()
	{
		Button component = base.GetComponent<Button>();
		if (component == null)
		{
			return;
		}
		component.onClick.AddListener(new UnityAction(this.OnClick));
	}

	// Token: 0x060010DE RID: 4318 RVA: 0x00053EAC File Offset: 0x000520AC
	private void OnClick()
	{
		this.vel += 15f;
	}

	// Token: 0x060010DF RID: 4319 RVA: 0x00053EC0 File Offset: 0x000520C0
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.targetScale = 1.15f;
	}

	// Token: 0x060010E0 RID: 4320 RVA: 0x00053ECD File Offset: 0x000520CD
	public void OnPointerExit(PointerEventData eventData)
	{
		this.targetScale = 1f;
	}

	// Token: 0x060010E1 RID: 4321 RVA: 0x00053EDA File Offset: 0x000520DA
	private void OnEnable()
	{
		base.transform.localScale = Vector3.one;
		this.scale = 1f;
		this.vel = 0f;
		this.targetScale = 1f;
	}

	// Token: 0x060010E2 RID: 4322 RVA: 0x00053F10 File Offset: 0x00052110
	private void Update()
	{
		this.vel = FRILerp.Lerp(this.vel, (this.targetScale - this.scale) * 25f, 20f, true);
		this.scale += this.vel * Time.deltaTime;
		base.transform.localScale = Vector3.one * this.scale;
	}

	// Token: 0x04000EE5 RID: 3813
	private float scale = 1f;

	// Token: 0x04000EE6 RID: 3814
	private float vel;

	// Token: 0x04000EE7 RID: 3815
	private float targetScale = 1f;
}

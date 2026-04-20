using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.ControllerSupport;
using Zorro.UI;

// Token: 0x020001DF RID: 479
public class PauseMenuAccoladesPage : UIPage, IHaveParentPage, INavigationPage
{
	// Token: 0x06000F41 RID: 3905 RVA: 0x0004B30A File Offset: 0x0004950A
	private void Start()
	{
		this.backButton.onClick.AddListener(new UnityAction(this.BackClicked));
	}

	// Token: 0x06000F42 RID: 3906 RVA: 0x0004B328 File Offset: 0x00049528
	private void BackClicked()
	{
		this.pageHandler.TransistionToPage<PauseMenuMainPage>();
	}

	// Token: 0x06000F43 RID: 3907 RVA: 0x0004B336 File Offset: 0x00049536
	public ValueTuple<UIPage, PageTransistion> GetParentPage()
	{
		return new ValueTuple<UIPage, PageTransistion>(this.pageHandler.GetPage<PauseMenuMainPage>(), new SetActivePageTransistion());
	}

	// Token: 0x06000F44 RID: 3908 RVA: 0x0004B34D File Offset: 0x0004954D
	public GameObject GetFirstSelectedGameObject()
	{
		return this.firstBadge.gameObject;
	}

	// Token: 0x04000CD5 RID: 3285
	public Button firstBadge;

	// Token: 0x04000CD6 RID: 3286
	public Button backButton;
}

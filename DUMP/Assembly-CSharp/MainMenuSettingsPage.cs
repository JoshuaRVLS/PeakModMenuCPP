using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.ControllerSupport;
using Zorro.UI;

// Token: 0x020001D6 RID: 470
public class MainMenuSettingsPage : UIPage, IHaveParentPage, INavigationPage
{
	// Token: 0x06000EF6 RID: 3830 RVA: 0x0004A936 File Offset: 0x00048B36
	private void Start()
	{
		this.backButton.onClick.AddListener(new UnityAction(this.BackClicked));
	}

	// Token: 0x06000EF7 RID: 3831 RVA: 0x0004A954 File Offset: 0x00048B54
	private void BackClicked()
	{
		this.pageHandler.TransistionToPage<MainMenuMainPage>();
	}

	// Token: 0x06000EF8 RID: 3832 RVA: 0x0004A962 File Offset: 0x00048B62
	public ValueTuple<UIPage, PageTransistion> GetParentPage()
	{
		return new ValueTuple<UIPage, PageTransistion>(this.pageHandler.GetPage<MainMenuMainPage>(), new SetActivePageTransistion());
	}

	// Token: 0x06000EF9 RID: 3833 RVA: 0x0004A97C File Offset: 0x00048B7C
	public GameObject GetFirstSelectedGameObject()
	{
		GameObject defaultSelection = this.SharedSettingsMenu.GetDefaultSelection();
		if (defaultSelection)
		{
			return defaultSelection;
		}
		return this.backButton.gameObject;
	}

	// Token: 0x04000CB2 RID: 3250
	public SharedSettingsMenu SharedSettingsMenu;

	// Token: 0x04000CB3 RID: 3251
	public Button backButton;
}

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.ControllerSupport;
using Zorro.UI;

// Token: 0x020001E6 RID: 486
public class PauseMenuSettingsMenuPage : UIPage, IHaveParentPage, INavigationPage
{
	// Token: 0x06000F7A RID: 3962 RVA: 0x0004BFDA File Offset: 0x0004A1DA
	private void Start()
	{
		this.backButton.onClick.AddListener(new UnityAction(this.BackClicked));
	}

	// Token: 0x06000F7B RID: 3963 RVA: 0x0004BFF8 File Offset: 0x0004A1F8
	private void BackClicked()
	{
		this.pageHandler.TransistionToPage<PauseMenuMainPage>();
	}

	// Token: 0x06000F7C RID: 3964 RVA: 0x0004C006 File Offset: 0x0004A206
	public ValueTuple<UIPage, PageTransistion> GetParentPage()
	{
		return new ValueTuple<UIPage, PageTransistion>(this.pageHandler.GetPage<PauseMenuMainPage>(), new SetActivePageTransistion());
	}

	// Token: 0x06000F7D RID: 3965 RVA: 0x0004C020 File Offset: 0x0004A220
	public GameObject GetFirstSelectedGameObject()
	{
		GameObject defaultSelection = this.sharedSettingsMenu.GetDefaultSelection();
		if (defaultSelection == null)
		{
			return this.backButton.gameObject;
		}
		return defaultSelection;
	}

	// Token: 0x04000CFD RID: 3325
	public Button backButton;

	// Token: 0x04000CFE RID: 3326
	public SharedSettingsMenu sharedSettingsMenu;
}

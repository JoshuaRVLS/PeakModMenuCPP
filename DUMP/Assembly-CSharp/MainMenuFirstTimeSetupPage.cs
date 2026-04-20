using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.ControllerSupport;
using Zorro.Settings.UI;
using Zorro.UI;

// Token: 0x020001D1 RID: 465
public class MainMenuFirstTimeSetupPage : UIPage, INavigationPage
{
	// Token: 0x06000ED7 RID: 3799 RVA: 0x0004A39C File Offset: 0x0004859C
	public void Start()
	{
		SettingsHandler instance = SettingsHandler.Instance;
		MicrophoneSetting setting = instance.GetSetting<MicrophoneSetting>();
		this.MicSettingUI.Setup(setting, instance);
		this.ContinueButton.onClick.AddListener(new UnityAction(this.ContinueClicked));
	}

	// Token: 0x06000ED8 RID: 3800 RVA: 0x0004A3DF File Offset: 0x000485DF
	private void ContinueClicked()
	{
		this.pageHandler.TransistionToPage<MainMenuMainPage>();
	}

	// Token: 0x06000ED9 RID: 3801 RVA: 0x0004A3ED File Offset: 0x000485ED
	public GameObject GetFirstSelectedGameObject()
	{
		return this.MicSettingUI.dropdown.gameObject;
	}

	// Token: 0x04000CA2 RID: 3234
	public EnumSettingUI MicSettingUI;

	// Token: 0x04000CA3 RID: 3235
	public Button ContinueButton;
}

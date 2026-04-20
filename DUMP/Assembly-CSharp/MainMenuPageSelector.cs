using System;
using UnityEngine;
using Zorro.UI;

// Token: 0x020001D4 RID: 468
public class MainMenuPageSelector : UIPageHandlerStartPageSelector
{
	// Token: 0x06000EF0 RID: 3824 RVA: 0x0004A818 File Offset: 0x00048A18
	public override UIPage GetStartPage()
	{
		string key = "FirstTimeStartup2";
		if (PlayerPrefs.HasKey(key))
		{
			return this.mainPage;
		}
		PlayerPrefs.SetInt(key, 1);
		PlayerPrefs.Save();
		return this.firstTimeSetupPage;
	}

	// Token: 0x04000CAD RID: 3245
	public MainMenuMainPage mainPage;

	// Token: 0x04000CAE RID: 3246
	public MainMenuFirstTimeSetupPage firstTimeSetupPage;
}

using System;
using UnityEngine;
using UnityEngine.UI;
using Zorro.ControllerSupport;
using Zorro.UI;

// Token: 0x020001E0 RID: 480
public class PauseMenuConfirmPage : UIPage, INavigationPage
{
	// Token: 0x06000F46 RID: 3910 RVA: 0x0004B362 File Offset: 0x00049562
	public GameObject GetFirstSelectedGameObject()
	{
		return this.firstButton.gameObject;
	}

	// Token: 0x04000CD7 RID: 3287
	public Button firstButton;
}

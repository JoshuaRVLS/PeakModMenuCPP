using System;
using UnityEngine.UI;

// Token: 0x020001D8 RID: 472
public class CosmeticUnlockWindow : MenuWindow
{
	// Token: 0x1700010E RID: 270
	// (get) Token: 0x06000EFF RID: 3839 RVA: 0x0004AA97 File Offset: 0x00048C97
	public new virtual Selectable objectToSelectOnOpen
	{
		get
		{
			return this.continueButton;
		}
	}

	// Token: 0x04000CB6 RID: 3254
	public Button continueButton;
}

using System;
using UnityEngine.InputSystem;
using Zorro.UI;

// Token: 0x020001EA RID: 490
public class SettingsTABS : TABS<SettingsTABSButton>
{
	// Token: 0x06000F91 RID: 3985 RVA: 0x0004C54B File Offset: 0x0004A74B
	public override void OnSelected(SettingsTABSButton button)
	{
		this.SettingsMenu.ShowSettings(button.category);
	}

	// Token: 0x06000F92 RID: 3986 RVA: 0x0004C55E File Offset: 0x0004A75E
	private void Update()
	{
		if (this.RightAction.action.WasPressedThisFrame())
		{
			base.SelectNext();
			return;
		}
		if (this.LeftAction.action.WasPressedThisFrame())
		{
			base.SelectPrevious();
		}
	}

	// Token: 0x04000D0E RID: 3342
	public SharedSettingsMenu SettingsMenu;

	// Token: 0x04000D0F RID: 3343
	public InputActionReference RightAction;

	// Token: 0x04000D10 RID: 3344
	public InputActionReference LeftAction;
}

using System;
using UnityEngine;
using Zorro.UI;

// Token: 0x020001EB RID: 491
public class SettingsTABSButton : TAB_Button
{
	// Token: 0x06000F94 RID: 3988 RVA: 0x0004C59C File Offset: 0x0004A79C
	private void Update()
	{
		Color b = base.Selected ? Color.black : Color.white;
		this.text.color = Color.Lerp(this.text.color, b, Time.unscaledDeltaTime * 7f);
		this.SelectedGraphic.gameObject.SetActive(base.Selected);
	}

	// Token: 0x04000D11 RID: 3345
	public SettingsCategory category;

	// Token: 0x04000D12 RID: 3346
	public GameObject SelectedGraphic;
}

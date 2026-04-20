using System;
using UnityEngine;
using Zorro.Settings;

// Token: 0x02000097 RID: 151
public class DebugDisableBasedOnSetting<T> : MonoBehaviour where T : OffOnSetting
{
	// Token: 0x06000607 RID: 1543 RVA: 0x00022A88 File Offset: 0x00020C88
	private void Update()
	{
		if (this.settings == null && GameHandler.Instance != null)
		{
			this.settings = GameHandler.Instance.SettingsHandler.GetSetting<T>();
		}
		if (this.settings != null)
		{
			this.target.SetActive(this.settings.Value == OffOnMode.OFF);
		}
	}

	// Token: 0x04000621 RID: 1569
	public GameObject target;

	// Token: 0x04000622 RID: 1570
	private T settings;
}

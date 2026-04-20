using System;
using UnityEngine;

// Token: 0x0200009C RID: 156
public class DisableBasedOnRunSettings : MonoBehaviour
{
	// Token: 0x0600061B RID: 1563 RVA: 0x00022F0D File Offset: 0x0002110D
	private void OnEnable()
	{
		GlobalEvents.OnRunSettingsUpdated = (Action)Delegate.Combine(GlobalEvents.OnRunSettingsUpdated, new Action(this.UpdateEnabled));
	}

	// Token: 0x0600061C RID: 1564 RVA: 0x00022F2F File Offset: 0x0002112F
	private void OnDisable()
	{
		GlobalEvents.OnRunSettingsUpdated = (Action)Delegate.Remove(GlobalEvents.OnRunSettingsUpdated, new Action(this.UpdateEnabled));
	}

	// Token: 0x0600061D RID: 1565 RVA: 0x00022F51 File Offset: 0x00021151
	private void Awake()
	{
		this.UpdateEnabled();
	}

	// Token: 0x0600061E RID: 1566 RVA: 0x00022F59 File Offset: 0x00021159
	private void UpdateEnabled()
	{
		if (RunSettings.IsCustomRun && RunSettings.GetValue(this.disableIfSettingDisabled, false) == 0)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x04000633 RID: 1587
	public RunSettings.SETTINGTYPE disableIfSettingDisabled;
}

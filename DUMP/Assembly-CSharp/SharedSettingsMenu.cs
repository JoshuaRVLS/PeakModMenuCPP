using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zorro.Settings;

// Token: 0x020001ED RID: 493
public class SharedSettingsMenu : MonoBehaviour
{
	// Token: 0x06000F9C RID: 3996 RVA: 0x0004C77F File Offset: 0x0004A97F
	private void OnEnable()
	{
		this.RefreshSettings();
		if (this.m_tabs.selectedButton != null)
		{
			this.m_tabs.Select(this.m_tabs.selectedButton);
		}
	}

	// Token: 0x06000F9D RID: 3997 RVA: 0x0004C7B0 File Offset: 0x0004A9B0
	private void RefreshSettings()
	{
		if (GameHandler.Instance != null)
		{
			this.settings = GameHandler.Instance.SettingsHandler.GetSettingsThatImplements<IExposedSetting>();
		}
	}

	// Token: 0x06000F9E RID: 3998 RVA: 0x0004C7D4 File Offset: 0x0004A9D4
	public void ShowSettings(SettingsCategory category)
	{
		if (this.m_fadeInCoroutine != null)
		{
			base.StopCoroutine(this.m_fadeInCoroutine);
			this.m_fadeInCoroutine = null;
		}
		foreach (SettingsUICell settingsUICell in this.m_spawnedCells)
		{
			Object.Destroy(settingsUICell.gameObject);
		}
		this.m_spawnedCells.Clear();
		this.RefreshSettings();
		foreach (IExposedSetting exposedSetting in (from setting in this.settings
		where setting.GetCategory() == category.ToString()
		select setting).Where(delegate(IExposedSetting setting)
		{
			IConditionalSetting conditionalSetting2 = setting as IConditionalSetting;
			return true;
		}))
		{
			SettingsUICell component = Object.Instantiate<GameObject>(this.m_settingsCellPrefab, this.m_settingsContentParent).GetComponent<SettingsUICell>();
			IConditionalSetting conditionalSetting = exposedSetting as IConditionalSetting;
			if (conditionalSetting != null && !conditionalSetting.ShouldShow())
			{
				component.ShouldntShow();
			}
			this.m_spawnedCells.Add(component);
			component.Setup<Setting>(exposedSetting as Setting);
		}
		this.m_fadeInCoroutine = base.StartCoroutine(this.FadeInCells());
	}

	// Token: 0x06000F9F RID: 3999 RVA: 0x0004C92C File Offset: 0x0004AB2C
	private IEnumerator FadeInCells()
	{
		int i = 0;
		foreach (SettingsUICell settingsUICell in this.m_spawnedCells)
		{
			settingsUICell.FadeIn();
			yield return new WaitForSecondsRealtime(0.05f);
			int num = i;
			i = num + 1;
		}
		List<SettingsUICell>.Enumerator enumerator = default(List<SettingsUICell>.Enumerator);
		this.m_fadeInCoroutine = null;
		yield break;
		yield break;
	}

	// Token: 0x06000FA0 RID: 4000 RVA: 0x0004C93C File Offset: 0x0004AB3C
	public GameObject GetDefaultSelection()
	{
		foreach (SettingsUICell settingsUICell in this.m_spawnedCells)
		{
			if (settingsUICell.gameObject.activeSelf)
			{
				GameObject selectable = settingsUICell.GetSelectable();
				if (selectable)
				{
					return selectable;
				}
			}
		}
		return null;
	}

	// Token: 0x04000D1B RID: 3355
	[SerializeField]
	private SettingsTABS m_tabs;

	// Token: 0x04000D1C RID: 3356
	public GameObject m_settingsCellPrefab;

	// Token: 0x04000D1D RID: 3357
	public Transform m_settingsContentParent;

	// Token: 0x04000D1E RID: 3358
	private List<IExposedSetting> settings;

	// Token: 0x04000D1F RID: 3359
	private readonly List<SettingsUICell> m_spawnedCells = new List<SettingsUICell>();

	// Token: 0x04000D20 RID: 3360
	private Coroutine m_fadeInCoroutine;
}

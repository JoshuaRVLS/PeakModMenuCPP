using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zorro.Settings;

// Token: 0x020001EC RID: 492
public class SettingsUICell : MonoBehaviour
{
	// Token: 0x06000F96 RID: 3990 RVA: 0x0004C604 File Offset: 0x0004A804
	public void Setup<T>(T setting) where T : Setting
	{
		this.m_canvasGroup = base.GetComponent<CanvasGroup>();
		this.m_canvasGroup.alpha = 0f;
		IExposedSetting exposedSetting = setting as IExposedSetting;
		if (exposedSetting != null)
		{
			this.localizedText.SetIndex(exposedSetting.GetDisplayName());
		}
		SettingInputUICell component = Object.Instantiate<GameObject>(setting.GetSettingUICell(), this.m_settingsContentParent).GetComponent<SettingInputUICell>();
		component.disable = this.disable;
		if (this.disable)
		{
			this.onlyOnMainMenu.SetActive(true);
		}
		component.Setup(setting, GameHandler.Instance.SettingsHandler);
	}

	// Token: 0x06000F97 RID: 3991 RVA: 0x0004C6A0 File Offset: 0x0004A8A0
	public void FadeIn()
	{
		this.m_fadeIn = true;
		if (this.fadeInSFX)
		{
			this.fadeInSFX.Play(default(Vector3));
		}
	}

	// Token: 0x06000F98 RID: 3992 RVA: 0x0004C6D5 File Offset: 0x0004A8D5
	private void Update()
	{
		if (this.m_fadeIn)
		{
			this.m_canvasGroup.alpha = Mathf.Lerp(this.m_canvasGroup.alpha, 1f, Time.unscaledDeltaTime * 10f);
		}
	}

	// Token: 0x06000F99 RID: 3993 RVA: 0x0004C70C File Offset: 0x0004A90C
	public GameObject GetSelectable()
	{
		Button componentInChildren = this.m_settingsContentParent.GetComponentInChildren<Button>();
		if (componentInChildren != null)
		{
			return componentInChildren.gameObject;
		}
		Slider componentInChildren2 = this.m_settingsContentParent.GetComponentInChildren<Slider>();
		if (componentInChildren2 != null)
		{
			return componentInChildren2.gameObject;
		}
		TMP_Dropdown componentInChildren3 = this.m_settingsContentParent.GetComponentInChildren<TMP_Dropdown>();
		if (componentInChildren3 != null)
		{
			return componentInChildren3.gameObject;
		}
		return null;
	}

	// Token: 0x06000F9A RID: 3994 RVA: 0x0004C76E File Offset: 0x0004A96E
	public void ShouldntShow()
	{
		this.disable = true;
	}

	// Token: 0x04000D13 RID: 3347
	public Transform m_settingsContentParent;

	// Token: 0x04000D14 RID: 3348
	public TextMeshProUGUI m_text;

	// Token: 0x04000D15 RID: 3349
	public LocalizedText localizedText;

	// Token: 0x04000D16 RID: 3350
	public GameObject onlyOnMainMenu;

	// Token: 0x04000D17 RID: 3351
	private bool m_fadeIn;

	// Token: 0x04000D18 RID: 3352
	private CanvasGroup m_canvasGroup;

	// Token: 0x04000D19 RID: 3353
	public SFX_Instance fadeInSFX;

	// Token: 0x04000D1A RID: 3354
	private bool disable;
}

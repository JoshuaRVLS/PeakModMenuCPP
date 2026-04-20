using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Device;
using Zorro.Core;
using Zorro.Settings;
using Zorro.Settings.DebugUI;

// Token: 0x02000193 RID: 403
public class FullscreenEnumSetting : Setting, IEnumSetting, IExposedSetting, ICustomLocalizedEnumSetting
{
	// Token: 0x06000D35 RID: 3381 RVA: 0x0004522C File Offset: 0x0004342C
	public override void ApplyValue()
	{
	}

	// Token: 0x06000D36 RID: 3382 RVA: 0x0004522E File Offset: 0x0004342E
	public override SettingUI GetDebugUI(ISettingHandler settingHandler)
	{
		return new EnumSettingsUI(this, settingHandler);
	}

	// Token: 0x06000D37 RID: 3383 RVA: 0x00045237 File Offset: 0x00043437
	public override GameObject GetSettingUICell()
	{
		return SingletonAsset<InputCellMapper>.Instance.EnumSettingCell;
	}

	// Token: 0x06000D38 RID: 3384 RVA: 0x00045243 File Offset: 0x00043443
	public override void Load(ISettingsSaveLoad loader)
	{
	}

	// Token: 0x06000D39 RID: 3385 RVA: 0x00045245 File Offset: 0x00043445
	public override void Save(ISettingsSaveLoad saver)
	{
	}

	// Token: 0x06000D3A RID: 3386 RVA: 0x00045247 File Offset: 0x00043447
	public string GetDisplayName()
	{
		return "Window Mode";
	}

	// Token: 0x06000D3B RID: 3387 RVA: 0x0004524E File Offset: 0x0004344E
	public string GetCategory()
	{
		return "Graphics";
	}

	// Token: 0x06000D3C RID: 3388 RVA: 0x00045255 File Offset: 0x00043455
	public List<string> GetUnlocalizedChoices()
	{
		return new List<string>
		{
			"Windowed",
			"Fullscreen",
			"Windowed Fullscreen"
		};
	}

	// Token: 0x06000D3D RID: 3389 RVA: 0x00045280 File Offset: 0x00043480
	public int GetValue()
	{
		switch (UnityEngine.Device.Screen.fullScreenMode)
		{
		case FullScreenMode.ExclusiveFullScreen:
			return 1;
		case FullScreenMode.FullScreenWindow:
			return 2;
		case FullScreenMode.Windowed:
			return 0;
		}
		return 0;
	}

	// Token: 0x06000D3E RID: 3390 RVA: 0x000452B2 File Offset: 0x000434B2
	public void SetValue(int v, ISettingHandler settingHandler, bool fromUI)
	{
		switch (v)
		{
		case 0:
			UnityEngine.Device.Screen.fullScreenMode = FullScreenMode.Windowed;
			return;
		case 1:
			UnityEngine.Device.Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
			return;
		case 2:
			UnityEngine.Device.Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
			return;
		default:
			return;
		}
	}

	// Token: 0x06000D3F RID: 3391 RVA: 0x000452DB File Offset: 0x000434DB
	public List<string> GetCustomLocalizedChoices()
	{
		return (from s in this.GetUnlocalizedChoices()
		select LocalizedText.GetText(s, true)).ToList<string>();
	}

	// Token: 0x06000D40 RID: 3392 RVA: 0x0004530C File Offset: 0x0004350C
	public void DeregisterCustomLocalized(Action action)
	{
		LocalizedText.OnLangugageChanged = (Action)Delegate.Remove(LocalizedText.OnLangugageChanged, action);
	}

	// Token: 0x06000D41 RID: 3393 RVA: 0x00045323 File Offset: 0x00043523
	public void RegisterCustomLocalized(Action action)
	{
		LocalizedText.OnLangugageChanged = (Action)Delegate.Combine(LocalizedText.OnLangugageChanged, action);
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using Unity.Multiplayer.Playmode;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.CLI;
using Zorro.Settings;

// Token: 0x020001A9 RID: 425
public class SettingsHandler : ISettingHandler
{
	// Token: 0x170000FC RID: 252
	// (get) Token: 0x06000DCF RID: 3535 RVA: 0x00045B89 File Offset: 0x00043D89
	public static bool IsOnSteamDeck
	{
		get
		{
			return !CurrentPlayer.ReadOnlyTags().Contains("NoSteam") && SteamUtils.IsSteamRunningOnSteamDeck();
		}
	}

	// Token: 0x06000DD0 RID: 3536 RVA: 0x00045BA4 File Offset: 0x00043DA4
	public SettingsHandler()
	{
		this.settings = new List<Setting>(30);
		this._settingsSaveLoad = new DefaultSettingsSaveLoad();
		this.AddSetting(new LanguageSetting());
		this.AddSetting(new FovSetting());
		this.AddSetting(new ExtraFovSetting());
		this.AddSetting(new FullscreenEnumSetting());
		this.AddSetting(new ResolutionSetting());
		this.AddSetting(new FPSCapSetting());
		this.AddSetting(new VSyncSetting());
		this.AddSetting(new MicrophoneSetting());
		this.AddSetting(new RenderScaleSetting());
		this.AddSetting(new ShadowDistanceSettings());
		this.AddSetting(new TextureQualitySetting());
		this.AddSetting(new PushToTalkSetting());
		this.AddSetting(new MasterVolumeSetting(SingletonAsset<StaticReferences>.Instance.masterMixerGroup));
		this.AddSetting(new SFXVolumeSetting(SingletonAsset<StaticReferences>.Instance.masterMixerGroup));
		this.AddSetting(new MusicVolumeSetting(SingletonAsset<StaticReferences>.Instance.masterMixerGroup));
		this.AddSetting(new VoiceVolumeSetting(SingletonAsset<StaticReferences>.Instance.masterMixerGroup));
		this.AddSetting(new MouseSensitivitySetting());
		this.AddSetting(new ControllerSensitivitySetting());
		this.AddSetting(new LodQuality());
		this.AddSetting(new AOSetting());
		this.AddSetting(new ControllerIconSetting());
		this.AddSetting(new InvertXSetting());
		this.AddSetting(new InvertYSetting());
		this.AddSetting(new JumpToClimbSetting());
		this.AddSetting(new LobbyTypeSetting());
		this.AddSetting(new HeadBobSetting());
		this.AddSetting(new CannibalismSetting());
		this.AddSetting(new BugPhobiaSetting());
		this.AddSetting(new ZombiePhobiaSetting());
		this.AddSetting(new PhotosensitiveSetting());
		this.AddSetting(new ColorblindSetting());
		this.AddSetting(new LookerSetting());
		DebugUIHandler instance = Singleton<DebugUIHandler>.Instance;
		if (instance != null)
		{
			instance.RegisterPage("Settings", () => new SettingsPage(this.settings, this));
		}
		SettingsHandler.Instance = this;
		Debug.Log("Settings Initlaized");
	}

	// Token: 0x06000DD1 RID: 3537 RVA: 0x00045D88 File Offset: 0x00043F88
	public void AddSetting(Setting setting)
	{
		this.settings.Add(setting);
		setting.Load(this._settingsSaveLoad);
		setting.ApplyValue();
	}

	// Token: 0x06000DD2 RID: 3538 RVA: 0x00045DA8 File Offset: 0x00043FA8
	public void SaveSetting(Setting setting)
	{
		setting.Save(this._settingsSaveLoad);
		this._settingsSaveLoad.WriteToDisk();
	}

	// Token: 0x06000DD3 RID: 3539 RVA: 0x00045DC4 File Offset: 0x00043FC4
	public T GetSetting<T>() where T : Setting
	{
		foreach (Setting setting in this.settings)
		{
			T t = setting as T;
			if (t != null)
			{
				return t;
			}
		}
		return default(T);
	}

	// Token: 0x06000DD4 RID: 3540 RVA: 0x00045E34 File Offset: 0x00044034
	public IEnumerable<Setting> GetAllSettings()
	{
		return this.settings;
	}

	// Token: 0x06000DD5 RID: 3541 RVA: 0x00045E3C File Offset: 0x0004403C
	public void Update()
	{
		foreach (Setting setting in this.settings)
		{
			setting.Update();
		}
	}

	// Token: 0x04000BAB RID: 2987
	private List<Setting> settings;

	// Token: 0x04000BAC RID: 2988
	private ISettingsSaveLoad _settingsSaveLoad;

	// Token: 0x04000BAD RID: 2989
	public static SettingsHandler Instance;
}

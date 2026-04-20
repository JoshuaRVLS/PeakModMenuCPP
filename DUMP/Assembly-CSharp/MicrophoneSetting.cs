using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zorro.Core;
using Zorro.Settings;
using Zorro.Settings.DebugUI;

// Token: 0x0200019D RID: 413
public class MicrophoneSetting : Setting, IEnumSetting, IExposedSetting
{
	// Token: 0x06000D81 RID: 3457 RVA: 0x000455CC File Offset: 0x000437CC
	public List<MicrophoneSetting.MicrophoneInfo> GetChoices()
	{
		string[] devices = Microphone.devices;
		List<MicrophoneSetting.MicrophoneInfo> list = new List<MicrophoneSetting.MicrophoneInfo>();
		foreach (string text in devices)
		{
			list.Add(new MicrophoneSetting.MicrophoneInfo
			{
				id = text,
				name = text
			});
		}
		return list;
	}

	// Token: 0x06000D82 RID: 3458 RVA: 0x00045618 File Offset: 0x00043818
	public override void Load(ISettingsSaveLoad loader)
	{
		string value;
		if (loader.TryLoadString(base.GetType(), out value))
		{
			List<MicrophoneSetting.MicrophoneInfo> choices = this.GetChoices();
			this.Value = choices.Find((MicrophoneSetting.MicrophoneInfo x) => x.id == value);
			if (string.IsNullOrEmpty(this.Value.id))
			{
				Debug.LogWarning("Failed to load setting of type " + base.GetType().FullName + " from PlayerPrefs. Value not found in choices.");
				this.Value = this.GetDefaultValue();
				return;
			}
		}
		else
		{
			Debug.LogWarning("Failed to load setting of type " + base.GetType().FullName + " from PlayerPrefs.");
			this.Value = this.GetDefaultValue();
		}
	}

	// Token: 0x06000D83 RID: 3459 RVA: 0x000456C8 File Offset: 0x000438C8
	private MicrophoneSetting.MicrophoneInfo GetDefaultValue()
	{
		if (this.GetChoices().Count == 0)
		{
			Debug.LogError("No voice devices found.");
			return default(MicrophoneSetting.MicrophoneInfo);
		}
		return this.GetChoices().First<MicrophoneSetting.MicrophoneInfo>();
	}

	// Token: 0x06000D84 RID: 3460 RVA: 0x00045701 File Offset: 0x00043901
	public override void Save(ISettingsSaveLoad saver)
	{
		saver.SaveString(base.GetType(), this.Value.id);
	}

	// Token: 0x06000D85 RID: 3461 RVA: 0x0004571C File Offset: 0x0004391C
	public override void ApplyValue()
	{
		string str = "Voice setting applied: ";
		MicrophoneSetting.MicrophoneInfo value = this.Value;
		Debug.Log(str + value.ToString());
	}

	// Token: 0x06000D86 RID: 3462 RVA: 0x0004574C File Offset: 0x0004394C
	public override SettingUI GetDebugUI(ISettingHandler settingHandler)
	{
		return new EnumSettingsUI(this, settingHandler);
	}

	// Token: 0x06000D87 RID: 3463 RVA: 0x00045755 File Offset: 0x00043955
	public override GameObject GetSettingUICell()
	{
		return SingletonAsset<InputCellMapper>.Instance.EnumSettingCell;
	}

	// Token: 0x06000D88 RID: 3464 RVA: 0x00045761 File Offset: 0x00043961
	public List<string> GetUnlocalizedChoices()
	{
		return (from info in this.GetChoices()
		select info.name).ToList<string>();
	}

	// Token: 0x06000D89 RID: 3465 RVA: 0x00045794 File Offset: 0x00043994
	public int GetValue()
	{
		return (from info in this.GetChoices()
		select info.id).ToList<string>().IndexOf(this.Value.id);
	}

	// Token: 0x06000D8A RID: 3466 RVA: 0x000457E0 File Offset: 0x000439E0
	public void SetValue(int v, ISettingHandler settingHandler, bool fromUI)
	{
		MicrophoneSetting.MicrophoneInfo value = this.GetChoices()[v];
		this.Value = value;
		this.ApplyValue();
		settingHandler.SaveSetting(this);
	}

	// Token: 0x06000D8B RID: 3467 RVA: 0x0004580E File Offset: 0x00043A0E
	public string GetDisplayName()
	{
		return "Microphone";
	}

	// Token: 0x06000D8C RID: 3468 RVA: 0x00045815 File Offset: 0x00043A15
	public string GetCategory()
	{
		return "Audio";
	}

	// Token: 0x04000BAA RID: 2986
	public MicrophoneSetting.MicrophoneInfo Value;

	// Token: 0x020004BB RID: 1211
	public struct MicrophoneInfo
	{
		// Token: 0x06001D5A RID: 7514 RVA: 0x00089C7D File Offset: 0x00087E7D
		public override string ToString()
		{
			return this.id + " (" + this.name + ")";
		}

		// Token: 0x04001ACF RID: 6863
		public string id;

		// Token: 0x04001AD0 RID: 6864
		public string name;
	}
}

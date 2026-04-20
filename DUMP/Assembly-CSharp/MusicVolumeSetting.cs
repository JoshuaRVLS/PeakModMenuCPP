using System;
using UnityEngine.Audio;
using Zorro.Settings;

// Token: 0x0200019F RID: 415
public class MusicVolumeSetting : VolumeSetting, IExposedSetting
{
	// Token: 0x06000D94 RID: 3476 RVA: 0x00045854 File Offset: 0x00043A54
	public MusicVolumeSetting(AudioMixerGroup mixerGroup) : base(mixerGroup)
	{
	}

	// Token: 0x06000D95 RID: 3477 RVA: 0x0004585D File Offset: 0x00043A5D
	public override string GetParameterName()
	{
		return "MusicVolume";
	}

	// Token: 0x06000D96 RID: 3478 RVA: 0x00045864 File Offset: 0x00043A64
	public string GetDisplayName()
	{
		return "Music Volume";
	}

	// Token: 0x06000D97 RID: 3479 RVA: 0x0004586B File Offset: 0x00043A6B
	public string GetCategory()
	{
		return "Audio";
	}
}

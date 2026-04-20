using System;
using UnityEngine.Audio;
using Zorro.Settings;

// Token: 0x020001A6 RID: 422
public class VoiceVolumeSetting : VolumeSetting, IExposedSetting
{
	// Token: 0x06000DBD RID: 3517 RVA: 0x00045AF9 File Offset: 0x00043CF9
	public VoiceVolumeSetting(AudioMixerGroup mixerGroup) : base(mixerGroup)
	{
	}

	// Token: 0x06000DBE RID: 3518 RVA: 0x00045B02 File Offset: 0x00043D02
	public override string GetParameterName()
	{
		return "VoiceVolume";
	}

	// Token: 0x06000DBF RID: 3519 RVA: 0x00045B09 File Offset: 0x00043D09
	public string GetDisplayName()
	{
		return "Voices Volume";
	}

	// Token: 0x06000DC0 RID: 3520 RVA: 0x00045B10 File Offset: 0x00043D10
	public string GetCategory()
	{
		return "Audio";
	}
}

using System;
using UnityEngine.Audio;
using Zorro.Settings;

// Token: 0x020001A3 RID: 419
public class SFXVolumeSetting : VolumeSetting, IExposedSetting
{
	// Token: 0x06000DAD RID: 3501 RVA: 0x000459BD File Offset: 0x00043BBD
	public SFXVolumeSetting(AudioMixerGroup mixerGroup) : base(mixerGroup)
	{
	}

	// Token: 0x06000DAE RID: 3502 RVA: 0x000459C6 File Offset: 0x00043BC6
	public override string GetParameterName()
	{
		return "SFXVolume";
	}

	// Token: 0x06000DAF RID: 3503 RVA: 0x000459CD File Offset: 0x00043BCD
	public string GetDisplayName()
	{
		return "SFX Volume";
	}

	// Token: 0x06000DB0 RID: 3504 RVA: 0x000459D4 File Offset: 0x00043BD4
	public string GetCategory()
	{
		return "Audio";
	}
}

using System;
using UnityEngine.Audio;
using Zorro.Settings;

// Token: 0x0200019C RID: 412
public class MasterVolumeSetting : VolumeSetting, IExposedSetting
{
	// Token: 0x06000D7D RID: 3453 RVA: 0x000455AB File Offset: 0x000437AB
	public MasterVolumeSetting(AudioMixerGroup mixerGroup) : base(mixerGroup)
	{
	}

	// Token: 0x06000D7E RID: 3454 RVA: 0x000455B4 File Offset: 0x000437B4
	public override string GetParameterName()
	{
		return "MasterVolume";
	}

	// Token: 0x06000D7F RID: 3455 RVA: 0x000455BB File Offset: 0x000437BB
	public string GetDisplayName()
	{
		return "Master Volume";
	}

	// Token: 0x06000D80 RID: 3456 RVA: 0x000455C2 File Offset: 0x000437C2
	public string GetCategory()
	{
		return "Audio";
	}
}

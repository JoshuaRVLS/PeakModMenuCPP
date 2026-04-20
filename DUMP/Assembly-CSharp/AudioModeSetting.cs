using System;
using System.Collections.Generic;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x02000188 RID: 392
public class AudioModeSetting : EnumSetting<AudioModeSetting.AudioMode>, IExposedSetting
{
	// Token: 0x06000CF4 RID: 3316 RVA: 0x00044FDA File Offset: 0x000431DA
	public override void ApplyValue()
	{
	}

	// Token: 0x06000CF5 RID: 3317 RVA: 0x00044FDC File Offset: 0x000431DC
	protected override AudioModeSetting.AudioMode GetDefaultValue()
	{
		return AudioModeSetting.AudioMode.Stereo;
	}

	// Token: 0x06000CF6 RID: 3318 RVA: 0x00044FDF File Offset: 0x000431DF
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000CF7 RID: 3319 RVA: 0x00044FE2 File Offset: 0x000431E2
	public string GetDisplayName()
	{
		return "Audio Mode";
	}

	// Token: 0x06000CF8 RID: 3320 RVA: 0x00044FE9 File Offset: 0x000431E9
	public string GetCategory()
	{
		return "Audio";
	}

	// Token: 0x020004B3 RID: 1203
	public enum AudioMode
	{
		// Token: 0x04001AAC RID: 6828
		Stereo,
		// Token: 0x04001AAD RID: 6829
		Mono
	}
}

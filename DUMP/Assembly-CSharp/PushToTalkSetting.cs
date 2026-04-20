using System;
using System.Collections.Generic;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x020001A1 RID: 417
public class PushToTalkSetting : CustomLocalizedEnumSetting<PushToTalkSetting.PushToTalkType>, IExposedSetting
{
	// Token: 0x06000D9F RID: 3487 RVA: 0x0004589A File Offset: 0x00043A9A
	public override void ApplyValue()
	{
	}

	// Token: 0x06000DA0 RID: 3488 RVA: 0x0004589C File Offset: 0x00043A9C
	protected override PushToTalkSetting.PushToTalkType GetDefaultValue()
	{
		return PushToTalkSetting.PushToTalkType.VoiceActivation;
	}

	// Token: 0x06000DA1 RID: 3489 RVA: 0x0004589F File Offset: 0x00043A9F
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000DA2 RID: 3490 RVA: 0x000458A2 File Offset: 0x00043AA2
	public string GetDisplayName()
	{
		return "Microphone mode";
	}

	// Token: 0x06000DA3 RID: 3491 RVA: 0x000458A9 File Offset: 0x00043AA9
	public string GetCategory()
	{
		return "Audio";
	}

	// Token: 0x06000DA4 RID: 3492 RVA: 0x000458B0 File Offset: 0x00043AB0
	public override List<string> GetUnlocalizedChoices()
	{
		return new List<string>
		{
			"Voice Activation",
			"Push To Talk",
			"Push To Mute"
		};
	}

	// Token: 0x020004BE RID: 1214
	public enum PushToTalkType
	{
		// Token: 0x04001AD6 RID: 6870
		VoiceActivation,
		// Token: 0x04001AD7 RID: 6871
		PushToTalk,
		// Token: 0x04001AD8 RID: 6872
		PushToMute
	}
}

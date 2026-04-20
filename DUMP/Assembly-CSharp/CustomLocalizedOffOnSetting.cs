using System;
using System.Collections.Generic;
using System.Linq;
using Zorro.Settings;

// Token: 0x0200018F RID: 399
public abstract class CustomLocalizedOffOnSetting : OffOnSetting, ICustomLocalizedEnumSetting
{
	// Token: 0x06000D1F RID: 3359 RVA: 0x00045125 File Offset: 0x00043325
	public List<string> GetCustomLocalizedChoices()
	{
		return (from s in this.GetUnlocalizedChoices()
		select LocalizedText.GetText(s, true)).ToList<string>();
	}

	// Token: 0x06000D20 RID: 3360 RVA: 0x00045156 File Offset: 0x00043356
	public void DeregisterCustomLocalized(Action action)
	{
		LocalizedText.OnLangugageChanged = (Action)Delegate.Remove(LocalizedText.OnLangugageChanged, action);
	}

	// Token: 0x06000D21 RID: 3361 RVA: 0x0004516D File Offset: 0x0004336D
	public void RegisterCustomLocalized(Action action)
	{
		LocalizedText.OnLangugageChanged = (Action)Delegate.Combine(LocalizedText.OnLangugageChanged, action);
	}
}

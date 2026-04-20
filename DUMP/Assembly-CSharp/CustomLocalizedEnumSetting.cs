using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Zorro.Settings;

// Token: 0x0200018E RID: 398
public abstract class CustomLocalizedEnumSetting<[IsUnmanaged] T> : EnumSetting<T>, ICustomLocalizedEnumSetting where T : struct, ValueType, Enum
{
	// Token: 0x06000D1B RID: 3355 RVA: 0x000450BE File Offset: 0x000432BE
	public virtual List<string> GetCustomLocalizedChoices()
	{
		return (from s in this.GetUnlocalizedChoices()
		select LocalizedText.GetText(s, true)).ToList<string>();
	}

	// Token: 0x06000D1C RID: 3356 RVA: 0x000450EF File Offset: 0x000432EF
	public void DeregisterCustomLocalized(Action action)
	{
		LocalizedText.OnLangugageChanged = (Action)Delegate.Remove(LocalizedText.OnLangugageChanged, action);
	}

	// Token: 0x06000D1D RID: 3357 RVA: 0x00045106 File Offset: 0x00043306
	public void RegisterCustomLocalized(Action action)
	{
		LocalizedText.OnLangugageChanged = (Action)Delegate.Combine(LocalizedText.OnLangugageChanged, action);
	}
}

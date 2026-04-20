using System;
using System.Collections.Generic;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x02000198 RID: 408
public class LanguageSetting : CustomLocalizedEnumSetting<LanguageSetting.Language>, IExposedSetting
{
	// Token: 0x06000D5D RID: 3421 RVA: 0x000453C8 File Offset: 0x000435C8
	public LocalizedText.Language ValueToLanguage(int val)
	{
		if (val == 0)
		{
			return LocalizedText.Language.English;
		}
		if (val == 1)
		{
			return LocalizedText.Language.French;
		}
		if (val == 2)
		{
			return LocalizedText.Language.Italian;
		}
		if (val == 3)
		{
			return LocalizedText.Language.German;
		}
		if (val == 4)
		{
			return LocalizedText.Language.SpanishSpain;
		}
		if (val == 5)
		{
			return LocalizedText.Language.SpanishLatam;
		}
		if (val == 6)
		{
			return LocalizedText.Language.BRPortuguese;
		}
		if (val == 7)
		{
			return LocalizedText.Language.Russian;
		}
		if (val == 8)
		{
			return LocalizedText.Language.Ukrainian;
		}
		if (val == 9)
		{
			return LocalizedText.Language.SimplifiedChinese;
		}
		if (val == 10)
		{
			return LocalizedText.Language.Japanese;
		}
		if (val == 11)
		{
			return LocalizedText.Language.Korean;
		}
		if (val == 12)
		{
			return LocalizedText.Language.Polish;
		}
		if (val == 13)
		{
			return LocalizedText.Language.Turkish;
		}
		return LocalizedText.Language.English;
	}

	// Token: 0x06000D5E RID: 3422 RVA: 0x00045433 File Offset: 0x00043633
	public override void ApplyValue()
	{
		LocalizedText.SetLanguage((int)this.ValueToLanguage((int)base.Value));
	}

	// Token: 0x06000D5F RID: 3423 RVA: 0x00045446 File Offset: 0x00043646
	protected override LanguageSetting.Language GetDefaultValue()
	{
		return (LanguageSetting.Language)LocalizedText.GetSystemLanguage();
	}

	// Token: 0x06000D60 RID: 3424 RVA: 0x0004544D File Offset: 0x0004364D
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000D61 RID: 3425 RVA: 0x00045450 File Offset: 0x00043650
	public string GetDisplayName()
	{
		return "Language";
	}

	// Token: 0x06000D62 RID: 3426 RVA: 0x00045458 File Offset: 0x00043658
	public override List<string> GetCustomLocalizedChoices()
	{
		List<string> list = new List<string>();
		int num = base.Value.GetType().GetEnumNames().Length;
		for (int i = 0; i < num; i++)
		{
			list.Add(LocalizedText.GetText("CURRENT_LANGUAGE", this.ValueToLanguage(i)));
		}
		return list;
	}

	// Token: 0x06000D63 RID: 3427 RVA: 0x000454A7 File Offset: 0x000436A7
	public string GetCategory()
	{
		return "General";
	}

	// Token: 0x020004B8 RID: 1208
	public enum Language
	{
		// Token: 0x04001ABA RID: 6842
		English,
		// Token: 0x04001ABB RID: 6843
		French,
		// Token: 0x04001ABC RID: 6844
		Italian,
		// Token: 0x04001ABD RID: 6845
		German,
		// Token: 0x04001ABE RID: 6846
		SpanishSpain,
		// Token: 0x04001ABF RID: 6847
		SpanishLatam,
		// Token: 0x04001AC0 RID: 6848
		BRPortuguese,
		// Token: 0x04001AC1 RID: 6849
		Russian,
		// Token: 0x04001AC2 RID: 6850
		Ukrainian,
		// Token: 0x04001AC3 RID: 6851
		SimplifiedChinese,
		// Token: 0x04001AC4 RID: 6852
		Japanese = 11,
		// Token: 0x04001AC5 RID: 6853
		Korean,
		// Token: 0x04001AC6 RID: 6854
		Polish,
		// Token: 0x04001AC7 RID: 6855
		Turkish
	}
}

using System;
using System.Collections.Generic;
using Zorro.Core.CLI;

// Token: 0x02000077 RID: 119
[TypeParser(typeof(RunSettings.SETTINGTYPE))]
public class RunSettingCLIParser : CLITypeParser
{
	// Token: 0x06000568 RID: 1384 RVA: 0x00020089 File Offset: 0x0001E289
	public override object Parse(string str)
	{
		return Enum.Parse(typeof(RunSettings.SETTINGTYPE), str);
	}

	// Token: 0x06000569 RID: 1385 RVA: 0x0002009B File Offset: 0x0001E29B
	public override List<ParameterAutocomplete> FindAutocomplete(string parameterText)
	{
		return RunSettings.GetAutocompleteOptions(parameterText);
	}
}

using System;
using System.Collections.Generic;
using Zorro.Core.CLI;

// Token: 0x02000046 RID: 70
[TypeParser(typeof(ACHIEVEMENTTYPE))]
public class AchievementCLIParser : CLITypeParser
{
	// Token: 0x06000437 RID: 1079 RVA: 0x0001AD08 File Offset: 0x00018F08
	public override object Parse(string str)
	{
		ACHIEVEMENTTYPE achievementtype;
		if (Enum.TryParse<ACHIEVEMENTTYPE>(str, out achievementtype))
		{
			return achievementtype;
		}
		return ACHIEVEMENTTYPE.NONE;
	}

	// Token: 0x06000438 RID: 1080 RVA: 0x0001AD2C File Offset: 0x00018F2C
	public override List<ParameterAutocomplete> FindAutocomplete(string parameterText)
	{
		List<ParameterAutocomplete> list = new List<ParameterAutocomplete>();
		foreach (ACHIEVEMENTTYPE achievementtype in (ACHIEVEMENTTYPE[])Enum.GetValues(typeof(ACHIEVEMENTTYPE)))
		{
			list.Add(new ParameterAutocomplete(achievementtype.ToString()));
		}
		return list;
	}
}

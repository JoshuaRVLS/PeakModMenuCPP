using System;
using System.Collections.Generic;
using Zorro.Core;
using Zorro.Core.CLI;

// Token: 0x02000076 RID: 118
[TypeParser(typeof(Item))]
public class ItemCLIParser : CLITypeParser
{
	// Token: 0x06000565 RID: 1381 RVA: 0x0002006C File Offset: 0x0001E26C
	public override object Parse(string str)
	{
		return ObjectDatabaseAsset<ItemDatabase, Item>.GetObjectFromString(str);
	}

	// Token: 0x06000566 RID: 1382 RVA: 0x00020074 File Offset: 0x0001E274
	public override List<ParameterAutocomplete> FindAutocomplete(string parameterText)
	{
		return SingletonAsset<ItemDatabase>.Instance.GetAutocompleteOptions(parameterText);
	}
}

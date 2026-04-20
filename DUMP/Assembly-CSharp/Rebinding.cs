using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Token: 0x02000165 RID: 357
public static class Rebinding
{
	// Token: 0x06000BB5 RID: 2997 RVA: 0x0003EBD4 File Offset: 0x0003CDD4
	public static void LoadRebindingsFromFile(InputActionAsset actions = null)
	{
		if (actions == null)
		{
			actions = InputSystem.actions;
		}
		string @string = PlayerPrefs.GetString("rebinds");
		if (!string.IsNullOrEmpty(@string))
		{
			actions.LoadBindingOverridesFromJson(@string, true);
		}
	}

	// Token: 0x06000BB6 RID: 2998 RVA: 0x0003EC0C File Offset: 0x0003CE0C
	public static void SaveRebindingsToFile(InputActionAsset actions = null)
	{
		if (actions == null)
		{
			actions = InputSystem.actions;
		}
		string value = actions.SaveBindingOverridesAsJson();
		PlayerPrefs.SetString("rebinds", value);
	}
}

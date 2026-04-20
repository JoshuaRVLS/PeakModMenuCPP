using System;
using System.Collections.Generic;
using UnityEngine.Localization;
using Zorro.Settings;

// Token: 0x0200018C RID: 396
public class ControllerIconSetting : CustomLocalizedEnumSetting<ControllerIconSetting.IconMode>, IExposedSetting
{
	// Token: 0x06000D0F RID: 3343 RVA: 0x00045070 File Offset: 0x00043270
	public override void ApplyValue()
	{
	}

	// Token: 0x06000D10 RID: 3344 RVA: 0x00045072 File Offset: 0x00043272
	protected override ControllerIconSetting.IconMode GetDefaultValue()
	{
		return ControllerIconSetting.IconMode.Auto;
	}

	// Token: 0x06000D11 RID: 3345 RVA: 0x00045075 File Offset: 0x00043275
	public override List<LocalizedString> GetLocalizedChoices()
	{
		return null;
	}

	// Token: 0x06000D12 RID: 3346 RVA: 0x00045078 File Offset: 0x00043278
	public string GetDisplayName()
	{
		return "INPUTICONS";
	}

	// Token: 0x06000D13 RID: 3347 RVA: 0x0004507F File Offset: 0x0004327F
	public string GetCategory()
	{
		return "General";
	}

	// Token: 0x020004B4 RID: 1204
	public enum IconMode
	{
		// Token: 0x04001AAF RID: 6831
		Auto,
		// Token: 0x04001AB0 RID: 6832
		Style1,
		// Token: 0x04001AB1 RID: 6833
		Style2,
		// Token: 0x04001AB2 RID: 6834
		KBM
	}
}

using System;
using Unity.Mathematics;
using Zorro.Settings;

// Token: 0x0200019E RID: 414
public class MouseSensitivitySetting : FloatSetting, IExposedSetting
{
	// Token: 0x06000D8E RID: 3470 RVA: 0x00045824 File Offset: 0x00043A24
	public override void ApplyValue()
	{
	}

	// Token: 0x06000D8F RID: 3471 RVA: 0x00045826 File Offset: 0x00043A26
	protected override float GetDefaultValue()
	{
		return 2f;
	}

	// Token: 0x06000D90 RID: 3472 RVA: 0x0004582D File Offset: 0x00043A2D
	protected override float2 GetMinMaxValue()
	{
		return new float2(0.1f, 5f);
	}

	// Token: 0x06000D91 RID: 3473 RVA: 0x0004583E File Offset: 0x00043A3E
	public string GetDisplayName()
	{
		return "Mouse Sensitivity";
	}

	// Token: 0x06000D92 RID: 3474 RVA: 0x00045845 File Offset: 0x00043A45
	public string GetCategory()
	{
		return "General";
	}
}

using System;
using Unity.Mathematics;
using Zorro.Settings;

// Token: 0x02000190 RID: 400
public class ExtraFovSetting : FloatSetting, IExposedSetting
{
	// Token: 0x06000D23 RID: 3363 RVA: 0x0004518C File Offset: 0x0004338C
	public override void ApplyValue()
	{
	}

	// Token: 0x06000D24 RID: 3364 RVA: 0x0004518E File Offset: 0x0004338E
	protected override float GetDefaultValue()
	{
		return 40f;
	}

	// Token: 0x06000D25 RID: 3365 RVA: 0x00045195 File Offset: 0x00043395
	protected override float2 GetMinMaxValue()
	{
		return new float2(0f, 50f);
	}

	// Token: 0x06000D26 RID: 3366 RVA: 0x000451A6 File Offset: 0x000433A6
	public string GetDisplayName()
	{
		return "Climbing extra field of view";
	}

	// Token: 0x06000D27 RID: 3367 RVA: 0x000451AD File Offset: 0x000433AD
	public string GetCategory()
	{
		return "General";
	}
}

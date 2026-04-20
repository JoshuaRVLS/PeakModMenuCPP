using System;
using Unity.Mathematics;
using Zorro.Settings;

// Token: 0x0200018D RID: 397
public class ControllerSensitivitySetting : FloatSetting, IExposedSetting
{
	// Token: 0x06000D15 RID: 3349 RVA: 0x0004508E File Offset: 0x0004328E
	public override void ApplyValue()
	{
	}

	// Token: 0x06000D16 RID: 3350 RVA: 0x00045090 File Offset: 0x00043290
	protected override float GetDefaultValue()
	{
		return 2f;
	}

	// Token: 0x06000D17 RID: 3351 RVA: 0x00045097 File Offset: 0x00043297
	protected override float2 GetMinMaxValue()
	{
		return new float2(0.1f, 5f);
	}

	// Token: 0x06000D18 RID: 3352 RVA: 0x000450A8 File Offset: 0x000432A8
	public string GetDisplayName()
	{
		return "Controller Sensitivity";
	}

	// Token: 0x06000D19 RID: 3353 RVA: 0x000450AF File Offset: 0x000432AF
	public string GetCategory()
	{
		return "General";
	}
}

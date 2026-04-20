using System;
using Unity.Mathematics;
using Zorro.Settings;

// Token: 0x02000191 RID: 401
public class FovSetting : FloatSetting, IExposedSetting
{
	// Token: 0x06000D29 RID: 3369 RVA: 0x000451BC File Offset: 0x000433BC
	public override void ApplyValue()
	{
	}

	// Token: 0x06000D2A RID: 3370 RVA: 0x000451BE File Offset: 0x000433BE
	protected override float GetDefaultValue()
	{
		return 70f;
	}

	// Token: 0x06000D2B RID: 3371 RVA: 0x000451C5 File Offset: 0x000433C5
	protected override float2 GetMinMaxValue()
	{
		return new float2(60f, 100f);
	}

	// Token: 0x06000D2C RID: 3372 RVA: 0x000451D6 File Offset: 0x000433D6
	public string GetDisplayName()
	{
		return "Field of view";
	}

	// Token: 0x06000D2D RID: 3373 RVA: 0x000451DD File Offset: 0x000433DD
	public string GetCategory()
	{
		return "General";
	}
}

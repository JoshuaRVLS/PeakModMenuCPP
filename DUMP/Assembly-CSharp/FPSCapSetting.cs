using System;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Settings;

// Token: 0x02000192 RID: 402
public class FPSCapSetting : FloatSetting, IExposedSetting
{
	// Token: 0x06000D2F RID: 3375 RVA: 0x000451EC File Offset: 0x000433EC
	public override void ApplyValue()
	{
		Application.targetFrameRate = Mathf.RoundToInt(base.Value);
	}

	// Token: 0x06000D30 RID: 3376 RVA: 0x000451FE File Offset: 0x000433FE
	public string GetDisplayName()
	{
		return "Max Framerate";
	}

	// Token: 0x06000D31 RID: 3377 RVA: 0x00045205 File Offset: 0x00043405
	public string GetCategory()
	{
		return "Graphics";
	}

	// Token: 0x06000D32 RID: 3378 RVA: 0x0004520C File Offset: 0x0004340C
	protected override float GetDefaultValue()
	{
		return 400f;
	}

	// Token: 0x06000D33 RID: 3379 RVA: 0x00045213 File Offset: 0x00043413
	protected override float2 GetMinMaxValue()
	{
		return new float2(30f, 600f);
	}
}

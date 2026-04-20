using System;
using UnityEngine;

// Token: 0x02000092 RID: 146
[CreateAssetMenu(fileName = "DayNightProfile", menuName = "Scriptable Objects/DayNightProfile")]
public class DayNightProfile : ScriptableObject
{
	// Token: 0x040005FA RID: 1530
	public float sunIntensity;

	// Token: 0x040005FB RID: 1531
	public float moonIntensity;

	// Token: 0x040005FC RID: 1532
	public Gradient sunGradient;

	// Token: 0x040005FD RID: 1533
	public Gradient skyTopGradient;

	// Token: 0x040005FE RID: 1534
	public Gradient skyMidGradient;

	// Token: 0x040005FF RID: 1535
	public Gradient skyBottomGradient;

	// Token: 0x04000600 RID: 1536
	public Gradient fogGradient;

	// Token: 0x04000601 RID: 1537
	public ShaderParameters[] globalShaderFloats;

	// Token: 0x04000602 RID: 1538
	public AnimatedShaderParameters[] animatedGlobalShaderFloats;
}

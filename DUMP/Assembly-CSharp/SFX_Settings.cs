using System;
using UnityEngine;

// Token: 0x020001AB RID: 427
[Serializable]
public class SFX_Settings
{
	// Token: 0x04000BB1 RID: 2993
	[Range(0f, 1f)]
	public float volume = 0.5f;

	// Token: 0x04000BB2 RID: 2994
	[Range(0f, 1f)]
	[Tooltip("0.2 variation means random between 80% of specified volume and 100% of specified volume")]
	public float volume_Variation = 0.2f;

	// Token: 0x04000BB3 RID: 2995
	public float pitch = 1f;

	// Token: 0x04000BB4 RID: 2996
	[Range(0f, 1f)]
	[Tooltip("0.1 variation means random between 95% of specified volume and 105% of specified volume")]
	public float pitch_Variation = 0.1f;

	// Token: 0x04000BB5 RID: 2997
	[Range(0f, 1f)]
	public float spatialBlend = 1f;

	// Token: 0x04000BB6 RID: 2998
	[Range(0f, 1f)]
	public float dopplerLevel = 1f;

	// Token: 0x04000BB7 RID: 2999
	public float range = 150f;

	// Token: 0x04000BB8 RID: 3000
	public float cooldown = 0.02f;

	// Token: 0x04000BB9 RID: 3001
	public int maxInstances_NOT_IMPLEMENTED = 5;
}

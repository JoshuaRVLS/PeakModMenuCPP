using System;
using UnityEngine;

// Token: 0x02000235 RID: 565
[Serializable]
public class ThemeWithRarity
{
	// Token: 0x04000F3C RID: 3900
	[Tooltip("Parallel slot array. Index i should correspond to the same logical slot across all themes.")]
	public Material[] mats;

	// Token: 0x04000F3D RID: 3901
	[Tooltip("Weighted chance to pick this theme.")]
	public float rarity = 1f;

	// Token: 0x04000F3E RID: 3902
	public string name;
}

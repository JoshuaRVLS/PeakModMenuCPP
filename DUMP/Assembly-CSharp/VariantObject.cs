using System;
using UnityEngine;

// Token: 0x02000373 RID: 883
public class VariantObject : MonoBehaviour
{
	// Token: 0x040015A8 RID: 5544
	[Range(0f, 1f)]
	public float spawnChance = 0.5f;

	// Token: 0x040015A9 RID: 5545
	public VariantObject.Group group;

	// Token: 0x0200054F RID: 1359
	public enum Group
	{
		// Token: 0x04001D03 RID: 7427
		Default,
		// Token: 0x04001D04 RID: 7428
		One,
		// Token: 0x04001D05 RID: 7429
		Two,
		// Token: 0x04001D06 RID: 7430
		Three
	}
}

using System;
using UnityEngine;

// Token: 0x0200016F RID: 367
[Serializable]
public class SpawnObject
{
	// Token: 0x04000AE2 RID: 2786
	public int maxCount;

	// Token: 0x04000AE3 RID: 2787
	public GameObject prefab;

	// Token: 0x04000AE4 RID: 2788
	public Vector3 inversion;

	// Token: 0x04000AE5 RID: 2789
	public Vector3 randomRot;

	// Token: 0x04000AE6 RID: 2790
	public Vector3 randomScale;

	// Token: 0x04000AE7 RID: 2791
	public float uniformScale;

	// Token: 0x04000AE8 RID: 2792
	public float scaleMultiplier = 1f;

	// Token: 0x04000AE9 RID: 2793
	public Vector3 posJitter;
}

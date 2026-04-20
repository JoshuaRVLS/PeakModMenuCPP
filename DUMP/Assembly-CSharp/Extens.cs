using System;
using UnityEngine;

// Token: 0x0200002F RID: 47
public static class Extens
{
	// Token: 0x06000380 RID: 896 RVA: 0x00018084 File Offset: 0x00016284
	public static Vector3 EulerRescaled(this Quaternion quaternion)
	{
		Vector3 eulerAngles = quaternion.eulerAngles;
		return new Vector3(Mathf.Repeat(eulerAngles.x + 180f, 360f) - 180f, Mathf.Repeat(eulerAngles.y + 180f, 360f) - 180f, Mathf.Repeat(eulerAngles.z + 180f, 360f) - 180f);
	}

	// Token: 0x06000381 RID: 897 RVA: 0x000180F2 File Offset: 0x000162F2
	public static Quaternion Inverse(this Quaternion quaterion)
	{
		return Quaternion.Inverse(quaterion);
	}
}

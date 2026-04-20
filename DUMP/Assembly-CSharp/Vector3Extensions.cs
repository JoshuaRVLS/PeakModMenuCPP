using System;
using UnityEngine;

// Token: 0x020000C9 RID: 201
public static class Vector3Extensions
{
	// Token: 0x0600080E RID: 2062 RVA: 0x0002CEFD File Offset: 0x0002B0FD
	public static Vector2 XZ(this Vector3 vector)
	{
		return new Vector2(vector.x, vector.z);
	}

	// Token: 0x0600080F RID: 2063 RVA: 0x0002CF10 File Offset: 0x0002B110
	public static Vector3 Flat(this Vector3 vector)
	{
		return new Vector3(vector.x, 0f, vector.z);
	}

	// Token: 0x06000810 RID: 2064 RVA: 0x0002CF28 File Offset: 0x0002B128
	public static bool Same(this Vector3 v1, Vector3 v2, float threshold = 0.01f)
	{
		return Mathf.Abs(v1.x - v2.x) < threshold && Mathf.Abs(v1.y - v2.y) < threshold && Mathf.Abs(v1.z - v2.z) < threshold;
	}
}

using System;
using UnityEngine;

namespace Peak.Math
{
	// Token: 0x020003E7 RID: 999
	public static class VectorExtensions
	{
		// Token: 0x06001A8C RID: 6796 RVA: 0x000831A6 File Offset: 0x000813A6
		public static bool NearZero(this Vector3 self)
		{
			return self.sqrMagnitude <= 0.0001f;
		}
	}
}

using System;
using UnityEngine;

// Token: 0x020000C7 RID: 199
public static class HelperExtensions
{
	// Token: 0x060007C9 RID: 1993 RVA: 0x0002BCF3 File Offset: 0x00029EF3
	public static LayerMask ToLayerMask(this HelperFunctions.LayerType me)
	{
		return HelperFunctions.GetMask(me);
	}
}

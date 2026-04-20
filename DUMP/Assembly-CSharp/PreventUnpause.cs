using System;
using UnityEngine;

// Token: 0x020001E7 RID: 487
public class PreventUnpause : MonoBehaviour
{
	// Token: 0x06000F7F RID: 3967 RVA: 0x0004C057 File Offset: 0x0004A257
	private void OnEnable()
	{
		PreventUnpause.UnpausePreventionActive = true;
	}

	// Token: 0x06000F80 RID: 3968 RVA: 0x0004C05F File Offset: 0x0004A25F
	private void OnDisable()
	{
		PreventUnpause.UnpausePreventionActive = false;
	}

	// Token: 0x04000CFF RID: 3327
	public static bool UnpausePreventionActive;
}

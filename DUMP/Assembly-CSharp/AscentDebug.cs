using System;
using UnityEngine;

// Token: 0x0200005A RID: 90
public class AscentDebug : MonoBehaviour
{
	// Token: 0x06000493 RID: 1171 RVA: 0x0001C066 File Offset: 0x0001A266
	private void Awake()
	{
		Ascents.currentAscent = this.testAscent;
	}

	// Token: 0x04000511 RID: 1297
	public int testAscent;
}

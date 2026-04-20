using System;
using UnityEngine;

// Token: 0x0200020E RID: 526
public class AvaragePosition : MonoBehaviour
{
	// Token: 0x06001040 RID: 4160 RVA: 0x00050C24 File Offset: 0x0004EE24
	private void Update()
	{
		base.transform.position = (this.p1.position + this.p2.position) / 2f;
	}

	// Token: 0x04000E28 RID: 3624
	public Transform p1;

	// Token: 0x04000E29 RID: 3625
	public Transform p2;
}

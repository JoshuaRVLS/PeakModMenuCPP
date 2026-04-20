using System;
using UnityEngine;

// Token: 0x0200003A RID: 58
public struct RigidbodySyncData
{
	// Token: 0x060003C0 RID: 960 RVA: 0x0001908F File Offset: 0x0001728F
	public RigidbodySyncData(Rigidbody rig)
	{
		this.position = rig.position;
		this.rotation = rig.rotation;
	}

	// Token: 0x04000407 RID: 1031
	public Vector3 position;

	// Token: 0x04000408 RID: 1032
	public Quaternion rotation;
}

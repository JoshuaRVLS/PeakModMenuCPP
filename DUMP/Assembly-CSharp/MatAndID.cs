using System;
using UnityEngine;

// Token: 0x020002FD RID: 765
[Serializable]
public class MatAndID
{
	// Token: 0x060014D7 RID: 5335 RVA: 0x00069582 File Offset: 0x00067782
	public MatAndID(Material mat, int id)
	{
		this.mat = mat;
		this.id = id;
	}

	// Token: 0x04001300 RID: 4864
	public Material mat;

	// Token: 0x04001301 RID: 4865
	public int id;
}

using System;
using UnityEngine;

// Token: 0x020000C3 RID: 195
public abstract class GrassDataProvider : MonoBehaviour
{
	// Token: 0x0600076D RID: 1901
	public abstract bool IsDirty();

	// Token: 0x0600076E RID: 1902
	public abstract ComputeBuffer GetData();
}

using System;
using UnityEngine;

// Token: 0x02000253 RID: 595
public class DestroyAfterTime : MonoBehaviour
{
	// Token: 0x060011F7 RID: 4599 RVA: 0x0005A6B9 File Offset: 0x000588B9
	private void Start()
	{
		Object.Destroy(base.gameObject, this.time);
	}

	// Token: 0x04000FFF RID: 4095
	public float time = 3f;
}

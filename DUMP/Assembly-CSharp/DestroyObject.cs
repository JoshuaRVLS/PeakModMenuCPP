using System;
using UnityEngine;

// Token: 0x02000255 RID: 597
public class DestroyObject : MonoBehaviour
{
	// Token: 0x060011FB RID: 4603 RVA: 0x0005A6F6 File Offset: 0x000588F6
	public void DestroySelf()
	{
		Object.Destroy(base.gameObject);
	}
}

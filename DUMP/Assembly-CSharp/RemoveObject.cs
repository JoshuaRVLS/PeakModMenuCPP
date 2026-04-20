using System;
using UnityEngine;

// Token: 0x0200031C RID: 796
public class RemoveObject : MonoBehaviour
{
	// Token: 0x06001543 RID: 5443 RVA: 0x0006B412 File Offset: 0x00069612
	public void DestroySelf()
	{
		Object.Destroy(base.gameObject);
	}
}

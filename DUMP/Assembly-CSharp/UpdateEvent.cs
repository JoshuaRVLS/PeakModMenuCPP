using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000372 RID: 882
public class UpdateEvent : MonoBehaviour
{
	// Token: 0x0600172D RID: 5933 RVA: 0x0007734C File Offset: 0x0007554C
	private void Update()
	{
		this.updateEvent.Invoke();
	}

	// Token: 0x040015A7 RID: 5543
	public UnityEvent updateEvent;
}

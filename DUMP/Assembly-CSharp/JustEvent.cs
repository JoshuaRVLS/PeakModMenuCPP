using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200028A RID: 650
public class JustEvent : MonoBehaviour
{
	// Token: 0x060012CC RID: 4812 RVA: 0x0005ECFB File Offset: 0x0005CEFB
	private void CallEvent1()
	{
		this.event1.Invoke();
	}

	// Token: 0x040010E0 RID: 4320
	public UnityEvent event1;
}

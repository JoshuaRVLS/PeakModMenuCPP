using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002A2 RID: 674
public class LocalPlayerEvent : MonoBehaviour
{
	// Token: 0x06001335 RID: 4917 RVA: 0x00060EA1 File Offset: 0x0005F0A1
	public void Start()
	{
		if (base.GetComponentInParent<Character>().IsLocal)
		{
			this.isLocalEvent.Invoke();
		}
	}

	// Token: 0x0400114A RID: 4426
	public UnityEvent isLocalEvent;
}

using System;
using UnityEngine;

// Token: 0x0200010A RID: 266
public class BugleEventProc : MonoBehaviour
{
	// Token: 0x060008EB RID: 2283 RVA: 0x00030D37 File Offset: 0x0002EF37
	private void Awake()
	{
		this.item = base.GetComponent<Item>();
		Item item = this.item;
		item.OnPrimaryStarted = (Action)Delegate.Combine(item.OnPrimaryStarted, new Action(this.ThrowBugleEvent));
	}

	// Token: 0x060008EC RID: 2284 RVA: 0x00030D6C File Offset: 0x0002EF6C
	private void OnDestroy()
	{
		Item item = this.item;
		item.OnPrimaryStarted = (Action)Delegate.Remove(item.OnPrimaryStarted, new Action(this.ThrowBugleEvent));
	}

	// Token: 0x060008ED RID: 2285 RVA: 0x00030D95 File Offset: 0x0002EF95
	private void ThrowBugleEvent()
	{
		GlobalEvents.TriggerBugleTooted(this.item);
	}

	// Token: 0x04000882 RID: 2178
	private Item item;
}

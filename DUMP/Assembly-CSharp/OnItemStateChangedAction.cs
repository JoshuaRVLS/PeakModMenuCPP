using System;

// Token: 0x02000129 RID: 297
public class OnItemStateChangedAction : ItemActionBase
{
	// Token: 0x060009B5 RID: 2485 RVA: 0x00033A90 File Offset: 0x00031C90
	protected override void Subscribe()
	{
		Item item = this.item;
		item.OnStateChange = (Action<ItemState>)Delegate.Combine(item.OnStateChange, new Action<ItemState>(this.RunAction));
	}

	// Token: 0x060009B6 RID: 2486 RVA: 0x00033ABA File Offset: 0x00031CBA
	protected override void Unsubscribe()
	{
		Item item = this.item;
		item.OnStateChange = (Action<ItemState>)Delegate.Remove(item.OnStateChange, new Action<ItemState>(this.RunAction));
	}

	// Token: 0x060009B7 RID: 2487 RVA: 0x00033AE4 File Offset: 0x00031CE4
	public virtual void RunAction(ItemState state)
	{
	}
}

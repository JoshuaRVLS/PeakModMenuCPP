using System;
using UnityEngine;

// Token: 0x020000D8 RID: 216
[RequireComponent(typeof(Constructable))]
public class Actions_DefaultConstructActions : ItemActionBase
{
	// Token: 0x0600084F RID: 2127 RVA: 0x0002EC8C File Offset: 0x0002CE8C
	private void Awake()
	{
		this.constructable = base.GetComponent<Constructable>();
	}

	// Token: 0x06000850 RID: 2128 RVA: 0x0002EC9C File Offset: 0x0002CE9C
	protected override void Subscribe()
	{
		Item item = this.item;
		item.OnPrimaryStarted = (Action)Delegate.Combine(item.OnPrimaryStarted, new Action(this.StartConstruction));
		Item item2 = this.item;
		item2.OnPrimaryFinishedCast = (Action)Delegate.Combine(item2.OnPrimaryFinishedCast, new Action(this.RunAction));
		Item item3 = this.item;
		item3.OnPrimaryCancelled = (Action)Delegate.Combine(item3.OnPrimaryCancelled, new Action(this.CancelConstruction));
	}

	// Token: 0x06000851 RID: 2129 RVA: 0x0002ED24 File Offset: 0x0002CF24
	protected override void Unsubscribe()
	{
		Item item = this.item;
		item.OnPrimaryStarted = (Action)Delegate.Remove(item.OnPrimaryStarted, new Action(this.StartConstruction));
		Item item2 = this.item;
		item2.OnPrimaryFinishedCast = (Action)Delegate.Remove(item2.OnPrimaryFinishedCast, new Action(this.RunAction));
		Item item3 = this.item;
		item3.OnPrimaryCancelled = (Action)Delegate.Remove(item3.OnPrimaryCancelled, new Action(this.CancelConstruction));
	}

	// Token: 0x06000852 RID: 2130 RVA: 0x0002EDA9 File Offset: 0x0002CFA9
	public virtual void StartConstruction()
	{
		this.constructable.StartConstruction();
	}

	// Token: 0x06000853 RID: 2131 RVA: 0x0002EDB6 File Offset: 0x0002CFB6
	public virtual void CancelConstruction()
	{
		this.constructable.DestroyPreview();
	}

	// Token: 0x06000854 RID: 2132 RVA: 0x0002EDC3 File Offset: 0x0002CFC3
	public override void RunAction()
	{
		this.constructable.FinishConstruction();
	}

	// Token: 0x04000809 RID: 2057
	public Constructable constructable;
}

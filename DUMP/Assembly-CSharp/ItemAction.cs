using System;
using UnityEngine;

// Token: 0x0200011D RID: 285
public class ItemAction : ItemActionBase
{
	// Token: 0x06000954 RID: 2388 RVA: 0x00032380 File Offset: 0x00030580
	protected override void Subscribe()
	{
		if (this.OnPressed)
		{
			Item item = this.item;
			item.OnPrimaryStarted = (Action)Delegate.Combine(item.OnPrimaryStarted, new Action(this.RunAction));
		}
		if (this.OnHeld)
		{
			Item item2 = this.item;
			item2.OnPrimaryHeld = (Action)Delegate.Combine(item2.OnPrimaryHeld, new Action(this.RunAction));
		}
		if (this.OnCastFinished)
		{
			Item item3 = this.item;
			item3.OnPrimaryFinishedCast = (Action)Delegate.Combine(item3.OnPrimaryFinishedCast, new Action(this.RunAction));
		}
		if (this.OnCancelled)
		{
			Item item4 = this.item;
			item4.OnPrimaryCancelled = (Action)Delegate.Combine(item4.OnPrimaryCancelled, new Action(this.RunAction));
		}
		if (this.OnConsumed)
		{
			Item item5 = this.item;
			item5.OnConsumed = (Action)Delegate.Combine(item5.OnConsumed, new Action(this.RunAction));
		}
	}

	// Token: 0x06000955 RID: 2389 RVA: 0x00032480 File Offset: 0x00030680
	protected override void Unsubscribe()
	{
		if (this.OnPressed)
		{
			Item item = this.item;
			item.OnPrimaryStarted = (Action)Delegate.Remove(item.OnPrimaryStarted, new Action(this.RunAction));
		}
		if (this.OnHeld)
		{
			Item item2 = this.item;
			item2.OnPrimaryHeld = (Action)Delegate.Remove(item2.OnPrimaryHeld, new Action(this.RunAction));
		}
		if (this.OnCastFinished)
		{
			Item item3 = this.item;
			item3.OnPrimaryFinishedCast = (Action)Delegate.Remove(item3.OnPrimaryFinishedCast, new Action(this.RunAction));
		}
		if (this.OnCancelled)
		{
			Item item4 = this.item;
			item4.OnPrimaryCancelled = (Action)Delegate.Remove(item4.OnPrimaryCancelled, new Action(this.RunAction));
		}
		if (this.OnConsumed)
		{
			Item item5 = this.item;
			item5.OnConsumed = (Action)Delegate.Remove(item5.OnConsumed, new Action(this.RunAction));
		}
	}

	// Token: 0x040008BC RID: 2236
	[SerializeField]
	public bool OnPressed;

	// Token: 0x040008BD RID: 2237
	[SerializeField]
	public bool OnHeld;

	// Token: 0x040008BE RID: 2238
	[SerializeField]
	public bool OnReleased;

	// Token: 0x040008BF RID: 2239
	[SerializeField]
	public bool OnCastFinished;

	// Token: 0x040008C0 RID: 2240
	[SerializeField]
	public bool OnCancelled;

	// Token: 0x040008C1 RID: 2241
	public bool OnConsumed;
}

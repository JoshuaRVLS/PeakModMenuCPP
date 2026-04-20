using System;
using UnityEngine;

// Token: 0x020000E3 RID: 227
public class Action_ConstructableScoutCannonScroll : ItemActionBase
{
	// Token: 0x0600086D RID: 2157 RVA: 0x0002F210 File Offset: 0x0002D410
	protected override void Subscribe()
	{
		Item item = this.item;
		item.OnScrolledMouseOnly = (Action<float>)Delegate.Combine(item.OnScrolledMouseOnly, new Action<float>(this.Scrolled));
		Item item2 = this.item;
		item2.OnScrollBackwardPressed = (Action)Delegate.Combine(item2.OnScrollBackwardPressed, new Action(this.ScrollLeft));
		Item item3 = this.item;
		item3.OnScrollForwardPressed = (Action)Delegate.Combine(item3.OnScrollForwardPressed, new Action(this.ScrollRight));
	}

	// Token: 0x0600086E RID: 2158 RVA: 0x0002F294 File Offset: 0x0002D494
	protected override void Unsubscribe()
	{
		Item item = this.item;
		item.OnScrolledMouseOnly = (Action<float>)Delegate.Remove(item.OnScrolledMouseOnly, new Action<float>(this.Scrolled));
		Item item2 = this.item;
		item2.OnScrollBackwardPressed = (Action)Delegate.Remove(item2.OnScrollBackwardPressed, new Action(this.ScrollLeft));
		Item item3 = this.item;
		item3.OnScrollForwardPressed = (Action)Delegate.Remove(item3.OnScrollForwardPressed, new Action(this.ScrollRight));
	}

	// Token: 0x0600086F RID: 2159 RVA: 0x0002F316 File Offset: 0x0002D516
	private void ScrollLeft()
	{
		this.Scrolled(-1f);
	}

	// Token: 0x06000870 RID: 2160 RVA: 0x0002F323 File Offset: 0x0002D523
	private void ScrollRight()
	{
		this.Scrolled(1f);
	}

	// Token: 0x06000871 RID: 2161 RVA: 0x0002F330 File Offset: 0x0002D530
	private void Scrolled(float value)
	{
		if (this.constructable != null && this.constructable.currentPreview != null)
		{
			this.constructable.angleOffset += value * this.angleAmount;
			this.constructable.angleOffset = Mathf.Clamp(this.constructable.angleOffset, -this.maxAngle, this.maxAngle);
			this.constructable.UpdateAngle();
		}
	}

	// Token: 0x04000819 RID: 2073
	public Constructable constructable;

	// Token: 0x0400081A RID: 2074
	public float angleAmount = 5f;

	// Token: 0x0400081B RID: 2075
	public float maxAngle;
}

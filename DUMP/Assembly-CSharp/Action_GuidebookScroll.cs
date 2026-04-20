using System;

// Token: 0x020000EA RID: 234
public class Action_GuidebookScroll : ItemActionBase
{
	// Token: 0x06000881 RID: 2177 RVA: 0x0002F508 File Offset: 0x0002D708
	private void Awake()
	{
		this.guidebook = base.GetComponent<Guidebook>();
	}

	// Token: 0x06000882 RID: 2178 RVA: 0x0002F518 File Offset: 0x0002D718
	protected override void Subscribe()
	{
		Item item = this.item;
		item.OnScrolledMouseOnly = (Action<float>)Delegate.Combine(item.OnScrolledMouseOnly, new Action<float>(this.Scrolled));
		Item item2 = this.item;
		item2.OnScrollBackwardPressed = (Action)Delegate.Combine(item2.OnScrollBackwardPressed, new Action(this.ScrollLeft));
		Item item3 = this.item;
		item3.OnScrollForwardPressed = (Action)Delegate.Combine(item3.OnScrollForwardPressed, new Action(this.ScrollRight));
	}

	// Token: 0x06000883 RID: 2179 RVA: 0x0002F59C File Offset: 0x0002D79C
	protected override void Unsubscribe()
	{
		Item item = this.item;
		item.OnScrolledMouseOnly = (Action<float>)Delegate.Remove(item.OnScrolledMouseOnly, new Action<float>(this.Scrolled));
		Item item2 = this.item;
		item2.OnScrollBackwardPressed = (Action)Delegate.Remove(item2.OnScrollBackwardPressed, new Action(this.ScrollLeft));
		Item item3 = this.item;
		item3.OnScrollForwardPressed = (Action)Delegate.Remove(item3.OnScrollForwardPressed, new Action(this.ScrollRight));
	}

	// Token: 0x06000884 RID: 2180 RVA: 0x0002F61E File Offset: 0x0002D81E
	private void ScrollLeft()
	{
		this.Scrolled(-1f);
	}

	// Token: 0x06000885 RID: 2181 RVA: 0x0002F62B File Offset: 0x0002D82B
	private void ScrollRight()
	{
		this.Scrolled(1f);
	}

	// Token: 0x06000886 RID: 2182 RVA: 0x0002F638 File Offset: 0x0002D838
	private void Scrolled(float value)
	{
		if (this.guidebook && this.guidebook.isOpen)
		{
			if (value < 0f)
			{
				this.guidebook.FlipPageLeft();
				return;
			}
			if (value > 0f)
			{
				this.guidebook.FlipPageRight();
			}
		}
	}

	// Token: 0x04000822 RID: 2082
	private Guidebook guidebook;
}

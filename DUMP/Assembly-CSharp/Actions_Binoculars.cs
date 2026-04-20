using System;
using UnityEngine;

// Token: 0x020000D7 RID: 215
public class Actions_Binoculars : ItemActionBase
{
	// Token: 0x06000849 RID: 2121 RVA: 0x0002EAF8 File Offset: 0x0002CCF8
	protected override void Subscribe()
	{
		Item item = this.item;
		item.OnScrolledMouseOnly = (Action<float>)Delegate.Combine(item.OnScrolledMouseOnly, new Action<float>(this.Scrolled));
		Item item2 = this.item;
		item2.OnScrollForwardHeld = (Action)Delegate.Combine(item2.OnScrollForwardHeld, new Action(this.ScrollForwardHeld));
		Item item3 = this.item;
		item3.OnScrollBackwardHeld = (Action)Delegate.Combine(item3.OnScrollBackwardHeld, new Action(this.ScrollBackwardHeld));
	}

	// Token: 0x0600084A RID: 2122 RVA: 0x0002EB7C File Offset: 0x0002CD7C
	protected override void Unsubscribe()
	{
		Item item = this.item;
		item.OnScrolledMouseOnly = (Action<float>)Delegate.Remove(item.OnScrolledMouseOnly, new Action<float>(this.Scrolled));
		Item item2 = this.item;
		item2.OnScrollForwardHeld = (Action)Delegate.Remove(item2.OnScrollForwardHeld, new Action(this.ScrollForwardHeld));
		Item item3 = this.item;
		item3.OnScrollBackwardHeld = (Action)Delegate.Remove(item3.OnScrollBackwardHeld, new Action(this.ScrollBackwardHeld));
	}

	// Token: 0x0600084B RID: 2123 RVA: 0x0002EBFE File Offset: 0x0002CDFE
	private void ScrollForwardHeld()
	{
		if (this.binocOverlay.binocularsActive)
		{
			this.cameraOverride.AdjustFOV(-this.scrollSpeedButton * Time.deltaTime);
		}
	}

	// Token: 0x0600084C RID: 2124 RVA: 0x0002EC25 File Offset: 0x0002CE25
	private void ScrollBackwardHeld()
	{
		if (this.binocOverlay.binocularsActive)
		{
			this.cameraOverride.AdjustFOV(this.scrollSpeedButton * Time.deltaTime);
		}
	}

	// Token: 0x0600084D RID: 2125 RVA: 0x0002EC4B File Offset: 0x0002CE4B
	private void Scrolled(float value)
	{
		if (this.binocOverlay.binocularsActive)
		{
			this.cameraOverride.AdjustFOV(-value * this.scrollSpeed);
		}
	}

	// Token: 0x04000805 RID: 2053
	public Action_ShowBinocularOverlay binocOverlay;

	// Token: 0x04000806 RID: 2054
	public CameraOverride_Binoculars cameraOverride;

	// Token: 0x04000807 RID: 2055
	public float scrollSpeed = 2f;

	// Token: 0x04000808 RID: 2056
	public float scrollSpeedButton = 2f;
}

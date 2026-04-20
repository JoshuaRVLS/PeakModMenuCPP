using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityStandardAssets.CrossPlatformInput
{
	// Token: 0x02000388 RID: 904
	public class AxisTouchButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		// Token: 0x06001799 RID: 6041 RVA: 0x00079744 File Offset: 0x00077944
		private void OnEnable()
		{
			if (!CrossPlatformInputManager.AxisExists(this.axisName))
			{
				this.m_Axis = new CrossPlatformInputManager.VirtualAxis(this.axisName);
				CrossPlatformInputManager.RegisterVirtualAxis(this.m_Axis);
			}
			else
			{
				this.m_Axis = CrossPlatformInputManager.VirtualAxisReference(this.axisName);
			}
			this.FindPairedButton();
		}

		// Token: 0x0600179A RID: 6042 RVA: 0x00079794 File Offset: 0x00077994
		private void FindPairedButton()
		{
			AxisTouchButton[] array = Object.FindObjectsOfType(typeof(AxisTouchButton)) as AxisTouchButton[];
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].axisName == this.axisName && array[i] != this)
					{
						this.m_PairedWith = array[i];
					}
				}
			}
		}

		// Token: 0x0600179B RID: 6043 RVA: 0x000797F0 File Offset: 0x000779F0
		private void OnDisable()
		{
			this.m_Axis.Remove();
		}

		// Token: 0x0600179C RID: 6044 RVA: 0x00079800 File Offset: 0x00077A00
		public void OnPointerDown(PointerEventData data)
		{
			if (this.m_PairedWith == null)
			{
				this.FindPairedButton();
			}
			this.m_Axis.Update(Mathf.MoveTowards(this.m_Axis.GetValue, this.axisValue, this.responseSpeed * Time.deltaTime));
		}

		// Token: 0x0600179D RID: 6045 RVA: 0x0007984E File Offset: 0x00077A4E
		public void OnPointerUp(PointerEventData data)
		{
			this.m_Axis.Update(Mathf.MoveTowards(this.m_Axis.GetValue, 0f, this.responseSpeed * Time.deltaTime));
		}

		// Token: 0x040015FF RID: 5631
		public string axisName = "Horizontal";

		// Token: 0x04001600 RID: 5632
		public float axisValue = 1f;

		// Token: 0x04001601 RID: 5633
		public float responseSpeed = 3f;

		// Token: 0x04001602 RID: 5634
		public float returnToCentreSpeed = 3f;

		// Token: 0x04001603 RID: 5635
		private AxisTouchButton m_PairedWith;

		// Token: 0x04001604 RID: 5636
		private CrossPlatformInputManager.VirtualAxis m_Axis;
	}
}

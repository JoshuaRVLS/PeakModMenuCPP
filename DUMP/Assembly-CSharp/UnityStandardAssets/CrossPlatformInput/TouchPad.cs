using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityStandardAssets.CrossPlatformInput
{
	// Token: 0x0200038E RID: 910
	[RequireComponent(typeof(Image))]
	public class TouchPad : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		// Token: 0x060017D1 RID: 6097 RVA: 0x00079DAC File Offset: 0x00077FAC
		private void OnEnable()
		{
			this.CreateVirtualAxes();
		}

		// Token: 0x060017D2 RID: 6098 RVA: 0x00079DB4 File Offset: 0x00077FB4
		private void Start()
		{
			this.m_Image = base.GetComponent<Image>();
			this.m_Center = this.m_Image.transform.position;
		}

		// Token: 0x060017D3 RID: 6099 RVA: 0x00079DD8 File Offset: 0x00077FD8
		private void CreateVirtualAxes()
		{
			this.m_UseX = (this.axesToUse == TouchPad.AxisOption.Both || this.axesToUse == TouchPad.AxisOption.OnlyHorizontal);
			this.m_UseY = (this.axesToUse == TouchPad.AxisOption.Both || this.axesToUse == TouchPad.AxisOption.OnlyVertical);
			if (this.m_UseX)
			{
				this.m_HorizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(this.horizontalAxisName);
				CrossPlatformInputManager.RegisterVirtualAxis(this.m_HorizontalVirtualAxis);
			}
			if (this.m_UseY)
			{
				this.m_VerticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(this.verticalAxisName);
				CrossPlatformInputManager.RegisterVirtualAxis(this.m_VerticalVirtualAxis);
			}
		}

		// Token: 0x060017D4 RID: 6100 RVA: 0x00079E61 File Offset: 0x00078061
		private void UpdateVirtualAxes(Vector3 value)
		{
			value = value.normalized;
			if (this.m_UseX)
			{
				this.m_HorizontalVirtualAxis.Update(value.x);
			}
			if (this.m_UseY)
			{
				this.m_VerticalVirtualAxis.Update(value.y);
			}
		}

		// Token: 0x060017D5 RID: 6101 RVA: 0x00079E9E File Offset: 0x0007809E
		public void OnPointerDown(PointerEventData data)
		{
			this.m_Dragging = true;
			this.m_Id = data.pointerId;
			if (this.controlStyle != TouchPad.ControlStyle.Absolute)
			{
				this.m_Center = data.position;
			}
		}

		// Token: 0x060017D6 RID: 6102 RVA: 0x00079ECC File Offset: 0x000780CC
		private void Update()
		{
			if (!this.m_Dragging)
			{
				return;
			}
			if (Input.touchCount >= this.m_Id + 1 && this.m_Id != -1)
			{
				if (this.controlStyle == TouchPad.ControlStyle.Swipe)
				{
					this.m_Center = this.m_PreviousTouchPos;
					this.m_PreviousTouchPos = Input.touches[this.m_Id].position;
				}
				Vector2 normalized = new Vector2(Input.touches[this.m_Id].position.x - this.m_Center.x, Input.touches[this.m_Id].position.y - this.m_Center.y).normalized;
				normalized.x *= this.Xsensitivity;
				normalized.y *= this.Ysensitivity;
				this.UpdateVirtualAxes(new Vector3(normalized.x, normalized.y, 0f));
			}
		}

		// Token: 0x060017D7 RID: 6103 RVA: 0x00079FCD File Offset: 0x000781CD
		public void OnPointerUp(PointerEventData data)
		{
			this.m_Dragging = false;
			this.m_Id = -1;
			this.UpdateVirtualAxes(Vector3.zero);
		}

		// Token: 0x060017D8 RID: 6104 RVA: 0x00079FE8 File Offset: 0x000781E8
		private void OnDisable()
		{
			if (CrossPlatformInputManager.AxisExists(this.horizontalAxisName))
			{
				CrossPlatformInputManager.UnRegisterVirtualAxis(this.horizontalAxisName);
			}
			if (CrossPlatformInputManager.AxisExists(this.verticalAxisName))
			{
				CrossPlatformInputManager.UnRegisterVirtualAxis(this.verticalAxisName);
			}
		}

		// Token: 0x04001613 RID: 5651
		public TouchPad.AxisOption axesToUse;

		// Token: 0x04001614 RID: 5652
		public TouchPad.ControlStyle controlStyle;

		// Token: 0x04001615 RID: 5653
		public string horizontalAxisName = "Horizontal";

		// Token: 0x04001616 RID: 5654
		public string verticalAxisName = "Vertical";

		// Token: 0x04001617 RID: 5655
		public float Xsensitivity = 1f;

		// Token: 0x04001618 RID: 5656
		public float Ysensitivity = 1f;

		// Token: 0x04001619 RID: 5657
		private Vector3 m_StartPos;

		// Token: 0x0400161A RID: 5658
		private Vector2 m_PreviousDelta;

		// Token: 0x0400161B RID: 5659
		private Vector3 m_JoytickOutput;

		// Token: 0x0400161C RID: 5660
		private bool m_UseX;

		// Token: 0x0400161D RID: 5661
		private bool m_UseY;

		// Token: 0x0400161E RID: 5662
		private CrossPlatformInputManager.VirtualAxis m_HorizontalVirtualAxis;

		// Token: 0x0400161F RID: 5663
		private CrossPlatformInputManager.VirtualAxis m_VerticalVirtualAxis;

		// Token: 0x04001620 RID: 5664
		private bool m_Dragging;

		// Token: 0x04001621 RID: 5665
		private int m_Id = -1;

		// Token: 0x04001622 RID: 5666
		private Vector2 m_PreviousTouchPos;

		// Token: 0x04001623 RID: 5667
		private Vector3 m_Center;

		// Token: 0x04001624 RID: 5668
		private Image m_Image;

		// Token: 0x02000556 RID: 1366
		public enum AxisOption
		{
			// Token: 0x04001D20 RID: 7456
			Both,
			// Token: 0x04001D21 RID: 7457
			OnlyHorizontal,
			// Token: 0x04001D22 RID: 7458
			OnlyVertical
		}

		// Token: 0x02000557 RID: 1367
		public enum ControlStyle
		{
			// Token: 0x04001D24 RID: 7460
			Absolute,
			// Token: 0x04001D25 RID: 7461
			Relative,
			// Token: 0x04001D26 RID: 7462
			Swipe
		}
	}
}

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityStandardAssets.CrossPlatformInput
{
	// Token: 0x0200038C RID: 908
	public class Joystick : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IDragHandler
	{
		// Token: 0x060017C3 RID: 6083 RVA: 0x00079A92 File Offset: 0x00077C92
		private void OnEnable()
		{
			this.CreateVirtualAxes();
		}

		// Token: 0x060017C4 RID: 6084 RVA: 0x00079A9A File Offset: 0x00077C9A
		private void Start()
		{
			this.m_StartPos = base.transform.position;
		}

		// Token: 0x060017C5 RID: 6085 RVA: 0x00079AB0 File Offset: 0x00077CB0
		private void UpdateVirtualAxes(Vector3 value)
		{
			Vector3 vector = this.m_StartPos - value;
			vector.y = -vector.y;
			vector /= (float)this.MovementRange;
			if (this.m_UseX)
			{
				this.m_HorizontalVirtualAxis.Update(-vector.x);
			}
			if (this.m_UseY)
			{
				this.m_VerticalVirtualAxis.Update(vector.y);
			}
		}

		// Token: 0x060017C6 RID: 6086 RVA: 0x00079B1C File Offset: 0x00077D1C
		private void CreateVirtualAxes()
		{
			this.m_UseX = (this.axesToUse == Joystick.AxisOption.Both || this.axesToUse == Joystick.AxisOption.OnlyHorizontal);
			this.m_UseY = (this.axesToUse == Joystick.AxisOption.Both || this.axesToUse == Joystick.AxisOption.OnlyVertical);
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

		// Token: 0x060017C7 RID: 6087 RVA: 0x00079BA8 File Offset: 0x00077DA8
		public void OnDrag(PointerEventData data)
		{
			Vector3 zero = Vector3.zero;
			if (this.m_UseX)
			{
				int num = (int)(data.position.x - this.m_StartPos.x);
				num = Mathf.Clamp(num, -this.MovementRange, this.MovementRange);
				zero.x = (float)num;
			}
			if (this.m_UseY)
			{
				int num2 = (int)(data.position.y - this.m_StartPos.y);
				num2 = Mathf.Clamp(num2, -this.MovementRange, this.MovementRange);
				zero.y = (float)num2;
			}
			base.transform.position = new Vector3(this.m_StartPos.x + zero.x, this.m_StartPos.y + zero.y, this.m_StartPos.z + zero.z);
			this.UpdateVirtualAxes(base.transform.position);
		}

		// Token: 0x060017C8 RID: 6088 RVA: 0x00079C8E File Offset: 0x00077E8E
		public void OnPointerUp(PointerEventData data)
		{
			base.transform.position = this.m_StartPos;
			this.UpdateVirtualAxes(this.m_StartPos);
		}

		// Token: 0x060017C9 RID: 6089 RVA: 0x00079CAD File Offset: 0x00077EAD
		public void OnPointerDown(PointerEventData data)
		{
		}

		// Token: 0x060017CA RID: 6090 RVA: 0x00079CAF File Offset: 0x00077EAF
		private void OnDisable()
		{
			if (this.m_UseX)
			{
				this.m_HorizontalVirtualAxis.Remove();
			}
			if (this.m_UseY)
			{
				this.m_VerticalVirtualAxis.Remove();
			}
		}

		// Token: 0x0400160A RID: 5642
		public int MovementRange = 100;

		// Token: 0x0400160B RID: 5643
		public Joystick.AxisOption axesToUse;

		// Token: 0x0400160C RID: 5644
		public string horizontalAxisName = "Horizontal";

		// Token: 0x0400160D RID: 5645
		public string verticalAxisName = "Vertical";

		// Token: 0x0400160E RID: 5646
		private Vector3 m_StartPos;

		// Token: 0x0400160F RID: 5647
		private bool m_UseX;

		// Token: 0x04001610 RID: 5648
		private bool m_UseY;

		// Token: 0x04001611 RID: 5649
		private CrossPlatformInputManager.VirtualAxis m_HorizontalVirtualAxis;

		// Token: 0x04001612 RID: 5650
		private CrossPlatformInputManager.VirtualAxis m_VerticalVirtualAxis;

		// Token: 0x02000555 RID: 1365
		public enum AxisOption
		{
			// Token: 0x04001D1C RID: 7452
			Both,
			// Token: 0x04001D1D RID: 7453
			OnlyHorizontal,
			// Token: 0x04001D1E RID: 7454
			OnlyVertical
		}
	}
}

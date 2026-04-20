using System;
using UnityEngine;

namespace UnityStandardAssets.CrossPlatformInput.PlatformSpecific
{
	// Token: 0x02000390 RID: 912
	public class MobileInput : VirtualInput
	{
		// Token: 0x060017F2 RID: 6130 RVA: 0x0007A23F File Offset: 0x0007843F
		private void AddButton(string name)
		{
			CrossPlatformInputManager.RegisterVirtualButton(new CrossPlatformInputManager.VirtualButton(name));
		}

		// Token: 0x060017F3 RID: 6131 RVA: 0x0007A24C File Offset: 0x0007844C
		private void AddAxes(string name)
		{
			CrossPlatformInputManager.RegisterVirtualAxis(new CrossPlatformInputManager.VirtualAxis(name));
		}

		// Token: 0x060017F4 RID: 6132 RVA: 0x0007A259 File Offset: 0x00078459
		public override float GetAxis(string name, bool raw)
		{
			if (!this.m_VirtualAxes.ContainsKey(name))
			{
				this.AddAxes(name);
			}
			return this.m_VirtualAxes[name].GetValue;
		}

		// Token: 0x060017F5 RID: 6133 RVA: 0x0007A281 File Offset: 0x00078481
		public override void SetButtonDown(string name)
		{
			if (!this.m_VirtualButtons.ContainsKey(name))
			{
				this.AddButton(name);
			}
			this.m_VirtualButtons[name].Pressed();
		}

		// Token: 0x060017F6 RID: 6134 RVA: 0x0007A2A9 File Offset: 0x000784A9
		public override void SetButtonUp(string name)
		{
			if (!this.m_VirtualButtons.ContainsKey(name))
			{
				this.AddButton(name);
			}
			this.m_VirtualButtons[name].Released();
		}

		// Token: 0x060017F7 RID: 6135 RVA: 0x0007A2D1 File Offset: 0x000784D1
		public override void SetAxisPositive(string name)
		{
			if (!this.m_VirtualAxes.ContainsKey(name))
			{
				this.AddAxes(name);
			}
			this.m_VirtualAxes[name].Update(1f);
		}

		// Token: 0x060017F8 RID: 6136 RVA: 0x0007A2FE File Offset: 0x000784FE
		public override void SetAxisNegative(string name)
		{
			if (!this.m_VirtualAxes.ContainsKey(name))
			{
				this.AddAxes(name);
			}
			this.m_VirtualAxes[name].Update(-1f);
		}

		// Token: 0x060017F9 RID: 6137 RVA: 0x0007A32B File Offset: 0x0007852B
		public override void SetAxisZero(string name)
		{
			if (!this.m_VirtualAxes.ContainsKey(name))
			{
				this.AddAxes(name);
			}
			this.m_VirtualAxes[name].Update(0f);
		}

		// Token: 0x060017FA RID: 6138 RVA: 0x0007A358 File Offset: 0x00078558
		public override void SetAxis(string name, float value)
		{
			if (!this.m_VirtualAxes.ContainsKey(name))
			{
				this.AddAxes(name);
			}
			this.m_VirtualAxes[name].Update(value);
		}

		// Token: 0x060017FB RID: 6139 RVA: 0x0007A381 File Offset: 0x00078581
		public override bool GetButtonDown(string name)
		{
			if (this.m_VirtualButtons.ContainsKey(name))
			{
				return this.m_VirtualButtons[name].GetButtonDown;
			}
			this.AddButton(name);
			return this.m_VirtualButtons[name].GetButtonDown;
		}

		// Token: 0x060017FC RID: 6140 RVA: 0x0007A3BB File Offset: 0x000785BB
		public override bool GetButtonUp(string name)
		{
			if (this.m_VirtualButtons.ContainsKey(name))
			{
				return this.m_VirtualButtons[name].GetButtonUp;
			}
			this.AddButton(name);
			return this.m_VirtualButtons[name].GetButtonUp;
		}

		// Token: 0x060017FD RID: 6141 RVA: 0x0007A3F5 File Offset: 0x000785F5
		public override bool GetButton(string name)
		{
			if (this.m_VirtualButtons.ContainsKey(name))
			{
				return this.m_VirtualButtons[name].GetButton;
			}
			this.AddButton(name);
			return this.m_VirtualButtons[name].GetButton;
		}

		// Token: 0x060017FE RID: 6142 RVA: 0x0007A42F File Offset: 0x0007862F
		public override Vector3 MousePosition()
		{
			return base.virtualMousePosition;
		}
	}
}

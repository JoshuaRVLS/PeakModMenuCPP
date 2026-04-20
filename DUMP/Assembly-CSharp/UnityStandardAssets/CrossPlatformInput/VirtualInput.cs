using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.CrossPlatformInput
{
	// Token: 0x0200038F RID: 911
	public abstract class VirtualInput
	{
		// Token: 0x17000176 RID: 374
		// (get) Token: 0x060017DA RID: 6106 RVA: 0x0007A055 File Offset: 0x00078255
		// (set) Token: 0x060017DB RID: 6107 RVA: 0x0007A05D File Offset: 0x0007825D
		public Vector3 virtualMousePosition { get; private set; }

		// Token: 0x060017DC RID: 6108 RVA: 0x0007A066 File Offset: 0x00078266
		public bool AxisExists(string name)
		{
			return this.m_VirtualAxes.ContainsKey(name);
		}

		// Token: 0x060017DD RID: 6109 RVA: 0x0007A074 File Offset: 0x00078274
		public bool ButtonExists(string name)
		{
			return this.m_VirtualButtons.ContainsKey(name);
		}

		// Token: 0x060017DE RID: 6110 RVA: 0x0007A084 File Offset: 0x00078284
		public void RegisterVirtualAxis(CrossPlatformInputManager.VirtualAxis axis)
		{
			if (this.m_VirtualAxes.ContainsKey(axis.name))
			{
				Debug.LogError("There is already a virtual axis named " + axis.name + " registered.");
				return;
			}
			this.m_VirtualAxes.Add(axis.name, axis);
			if (!axis.matchWithInputManager)
			{
				this.m_AlwaysUseVirtual.Add(axis.name);
			}
		}

		// Token: 0x060017DF RID: 6111 RVA: 0x0007A0EC File Offset: 0x000782EC
		public void RegisterVirtualButton(CrossPlatformInputManager.VirtualButton button)
		{
			if (this.m_VirtualButtons.ContainsKey(button.name))
			{
				Debug.LogError("There is already a virtual button named " + button.name + " registered.");
				return;
			}
			this.m_VirtualButtons.Add(button.name, button);
			if (!button.matchWithInputManager)
			{
				this.m_AlwaysUseVirtual.Add(button.name);
			}
		}

		// Token: 0x060017E0 RID: 6112 RVA: 0x0007A152 File Offset: 0x00078352
		public void UnRegisterVirtualAxis(string name)
		{
			if (this.m_VirtualAxes.ContainsKey(name))
			{
				this.m_VirtualAxes.Remove(name);
			}
		}

		// Token: 0x060017E1 RID: 6113 RVA: 0x0007A16F File Offset: 0x0007836F
		public void UnRegisterVirtualButton(string name)
		{
			if (this.m_VirtualButtons.ContainsKey(name))
			{
				this.m_VirtualButtons.Remove(name);
			}
		}

		// Token: 0x060017E2 RID: 6114 RVA: 0x0007A18C File Offset: 0x0007838C
		public CrossPlatformInputManager.VirtualAxis VirtualAxisReference(string name)
		{
			if (!this.m_VirtualAxes.ContainsKey(name))
			{
				return null;
			}
			return this.m_VirtualAxes[name];
		}

		// Token: 0x060017E3 RID: 6115 RVA: 0x0007A1AA File Offset: 0x000783AA
		public void SetVirtualMousePositionX(float f)
		{
			this.virtualMousePosition = new Vector3(f, this.virtualMousePosition.y, this.virtualMousePosition.z);
		}

		// Token: 0x060017E4 RID: 6116 RVA: 0x0007A1CE File Offset: 0x000783CE
		public void SetVirtualMousePositionY(float f)
		{
			this.virtualMousePosition = new Vector3(this.virtualMousePosition.x, f, this.virtualMousePosition.z);
		}

		// Token: 0x060017E5 RID: 6117 RVA: 0x0007A1F2 File Offset: 0x000783F2
		public void SetVirtualMousePositionZ(float f)
		{
			this.virtualMousePosition = new Vector3(this.virtualMousePosition.x, this.virtualMousePosition.y, f);
		}

		// Token: 0x060017E6 RID: 6118
		public abstract float GetAxis(string name, bool raw);

		// Token: 0x060017E7 RID: 6119
		public abstract bool GetButton(string name);

		// Token: 0x060017E8 RID: 6120
		public abstract bool GetButtonDown(string name);

		// Token: 0x060017E9 RID: 6121
		public abstract bool GetButtonUp(string name);

		// Token: 0x060017EA RID: 6122
		public abstract void SetButtonDown(string name);

		// Token: 0x060017EB RID: 6123
		public abstract void SetButtonUp(string name);

		// Token: 0x060017EC RID: 6124
		public abstract void SetAxisPositive(string name);

		// Token: 0x060017ED RID: 6125
		public abstract void SetAxisNegative(string name);

		// Token: 0x060017EE RID: 6126
		public abstract void SetAxisZero(string name);

		// Token: 0x060017EF RID: 6127
		public abstract void SetAxis(string name, float value);

		// Token: 0x060017F0 RID: 6128
		public abstract Vector3 MousePosition();

		// Token: 0x04001626 RID: 5670
		protected Dictionary<string, CrossPlatformInputManager.VirtualAxis> m_VirtualAxes = new Dictionary<string, CrossPlatformInputManager.VirtualAxis>();

		// Token: 0x04001627 RID: 5671
		protected Dictionary<string, CrossPlatformInputManager.VirtualButton> m_VirtualButtons = new Dictionary<string, CrossPlatformInputManager.VirtualButton>();

		// Token: 0x04001628 RID: 5672
		protected List<string> m_AlwaysUseVirtual = new List<string>();
	}
}

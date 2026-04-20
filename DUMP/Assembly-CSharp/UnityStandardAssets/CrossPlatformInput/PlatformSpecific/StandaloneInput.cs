using System;
using UnityEngine;

namespace UnityStandardAssets.CrossPlatformInput.PlatformSpecific
{
	// Token: 0x02000391 RID: 913
	public class StandaloneInput : VirtualInput
	{
		// Token: 0x06001800 RID: 6144 RVA: 0x0007A43F File Offset: 0x0007863F
		public override float GetAxis(string name, bool raw)
		{
			if (!raw)
			{
				return Input.GetAxis(name);
			}
			return Input.GetAxisRaw(name);
		}

		// Token: 0x06001801 RID: 6145 RVA: 0x0007A451 File Offset: 0x00078651
		public override bool GetButton(string name)
		{
			return Input.GetButton(name);
		}

		// Token: 0x06001802 RID: 6146 RVA: 0x0007A459 File Offset: 0x00078659
		public override bool GetButtonDown(string name)
		{
			return Input.GetButtonDown(name);
		}

		// Token: 0x06001803 RID: 6147 RVA: 0x0007A461 File Offset: 0x00078661
		public override bool GetButtonUp(string name)
		{
			return Input.GetButtonUp(name);
		}

		// Token: 0x06001804 RID: 6148 RVA: 0x0007A469 File Offset: 0x00078669
		public override void SetButtonDown(string name)
		{
			throw new Exception(" This is not possible to be called for standalone input. Please check your platform and code where this is called");
		}

		// Token: 0x06001805 RID: 6149 RVA: 0x0007A475 File Offset: 0x00078675
		public override void SetButtonUp(string name)
		{
			throw new Exception(" This is not possible to be called for standalone input. Please check your platform and code where this is called");
		}

		// Token: 0x06001806 RID: 6150 RVA: 0x0007A481 File Offset: 0x00078681
		public override void SetAxisPositive(string name)
		{
			throw new Exception(" This is not possible to be called for standalone input. Please check your platform and code where this is called");
		}

		// Token: 0x06001807 RID: 6151 RVA: 0x0007A48D File Offset: 0x0007868D
		public override void SetAxisNegative(string name)
		{
			throw new Exception(" This is not possible to be called for standalone input. Please check your platform and code where this is called");
		}

		// Token: 0x06001808 RID: 6152 RVA: 0x0007A499 File Offset: 0x00078699
		public override void SetAxisZero(string name)
		{
			throw new Exception(" This is not possible to be called for standalone input. Please check your platform and code where this is called");
		}

		// Token: 0x06001809 RID: 6153 RVA: 0x0007A4A5 File Offset: 0x000786A5
		public override void SetAxis(string name, float value)
		{
			throw new Exception(" This is not possible to be called for standalone input. Please check your platform and code where this is called");
		}

		// Token: 0x0600180A RID: 6154 RVA: 0x0007A4B1 File Offset: 0x000786B1
		public override Vector3 MousePosition()
		{
			return Input.mousePosition;
		}
	}
}

using System;
using UnityEngine;

namespace UnityStandardAssets.CrossPlatformInput
{
	// Token: 0x02000389 RID: 905
	public class ButtonHandler : MonoBehaviour
	{
		// Token: 0x0600179F RID: 6047 RVA: 0x000798B0 File Offset: 0x00077AB0
		private void OnEnable()
		{
		}

		// Token: 0x060017A0 RID: 6048 RVA: 0x000798B2 File Offset: 0x00077AB2
		public void SetDownState()
		{
			CrossPlatformInputManager.SetButtonDown(this.Name);
		}

		// Token: 0x060017A1 RID: 6049 RVA: 0x000798BF File Offset: 0x00077ABF
		public void SetUpState()
		{
			CrossPlatformInputManager.SetButtonUp(this.Name);
		}

		// Token: 0x060017A2 RID: 6050 RVA: 0x000798CC File Offset: 0x00077ACC
		public void SetAxisPositiveState()
		{
			CrossPlatformInputManager.SetAxisPositive(this.Name);
		}

		// Token: 0x060017A3 RID: 6051 RVA: 0x000798D9 File Offset: 0x00077AD9
		public void SetAxisNeutralState()
		{
			CrossPlatformInputManager.SetAxisZero(this.Name);
		}

		// Token: 0x060017A4 RID: 6052 RVA: 0x000798E6 File Offset: 0x00077AE6
		public void SetAxisNegativeState()
		{
			CrossPlatformInputManager.SetAxisNegative(this.Name);
		}

		// Token: 0x060017A5 RID: 6053 RVA: 0x000798F3 File Offset: 0x00077AF3
		public void Update()
		{
		}

		// Token: 0x04001605 RID: 5637
		public string Name;
	}
}

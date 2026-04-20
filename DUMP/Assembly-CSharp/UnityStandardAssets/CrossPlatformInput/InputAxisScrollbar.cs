using System;
using UnityEngine;

namespace UnityStandardAssets.CrossPlatformInput
{
	// Token: 0x0200038B RID: 907
	public class InputAxisScrollbar : MonoBehaviour
	{
		// Token: 0x060017C0 RID: 6080 RVA: 0x00079A6E File Offset: 0x00077C6E
		private void Update()
		{
		}

		// Token: 0x060017C1 RID: 6081 RVA: 0x00079A70 File Offset: 0x00077C70
		public void HandleInput(float value)
		{
			CrossPlatformInputManager.SetAxis(this.axis, value * 2f - 1f);
		}

		// Token: 0x04001609 RID: 5641
		public string axis;
	}
}

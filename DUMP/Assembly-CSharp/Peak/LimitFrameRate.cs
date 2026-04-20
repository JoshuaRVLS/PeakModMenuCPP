using System;
using UnityEngine;

namespace Peak
{
	// Token: 0x020003CB RID: 971
	public class LimitFrameRate : MonoBehaviour
	{
		// Token: 0x060019AD RID: 6573 RVA: 0x00081D2C File Offset: 0x0007FF2C
		private void Start()
		{
			Application.targetFrameRate = this.FrameRate;
		}

		// Token: 0x060019AE RID: 6574 RVA: 0x00081D39 File Offset: 0x0007FF39
		private void OnValidate()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			Application.targetFrameRate = this.FrameRate;
		}

		// Token: 0x04001748 RID: 5960
		public int FrameRate = 144;
	}
}

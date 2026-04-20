using System;
using UnityEngine;

namespace Peak
{
	// Token: 0x020003C9 RID: 969
	public class BreakableHelper : MonoBehaviour
	{
		// Token: 0x0600199F RID: 6559 RVA: 0x000819D5 File Offset: 0x0007FBD5
		private void FixedUpdate()
		{
			if (!this.breakable.rig.isKinematic)
			{
				this.breakable.lastVelocity = this.breakable.rig.linearVelocity;
			}
		}

		// Token: 0x04001743 RID: 5955
		public Breakable breakable;
	}
}

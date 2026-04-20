using System;
using UnityEngine;

namespace Peak
{
	// Token: 0x020003C8 RID: 968
	public class ItemOptimizer : MonoBehaviour
	{
		// Token: 0x0600199C RID: 6556 RVA: 0x0008199D File Offset: 0x0007FB9D
		private void OnEnable()
		{
			if (this.item == null)
			{
				base.enabled = false;
				return;
			}
			ItemOptimizationManager.RegisterItem(this.item);
		}

		// Token: 0x0600199D RID: 6557 RVA: 0x000819C0 File Offset: 0x0007FBC0
		private void OnDisable()
		{
			ItemOptimizationManager.DeregisterItem(this.item);
		}

		// Token: 0x04001742 RID: 5954
		public Item item;
	}
}

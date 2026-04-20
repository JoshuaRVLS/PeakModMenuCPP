using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Peak.UI
{
	// Token: 0x020003D0 RID: 976
	public class SelectableEvents : MonoBehaviour, ISubmitHandler, IEventSystemHandler
	{
		// Token: 0x1400000A RID: 10
		// (add) Token: 0x060019C8 RID: 6600 RVA: 0x00082060 File Offset: 0x00080260
		// (remove) Token: 0x060019C9 RID: 6601 RVA: 0x00082098 File Offset: 0x00080298
		public event Action<BaseEventData> Submitted;

		// Token: 0x060019CA RID: 6602 RVA: 0x000820CD File Offset: 0x000802CD
		public void OnSubmit(BaseEventData eventData)
		{
			Action<BaseEventData> submitted = this.Submitted;
			if (submitted == null)
			{
				return;
			}
			submitted(eventData);
		}
	}
}

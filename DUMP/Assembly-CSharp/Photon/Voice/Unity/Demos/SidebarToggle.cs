using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Photon.Voice.Unity.Demos
{
	// Token: 0x0200039C RID: 924
	public class SidebarToggle : MonoBehaviour
	{
		// Token: 0x06001865 RID: 6245 RVA: 0x0007BF42 File Offset: 0x0007A142
		private void Awake()
		{
			this.sidebarButton.onClick.RemoveAllListeners();
			this.sidebarButton.onClick.AddListener(new UnityAction(this.ToggleSidebar));
			this.ToggleSidebar(this.sidebarOpen);
		}

		// Token: 0x06001866 RID: 6246 RVA: 0x0007BF7C File Offset: 0x0007A17C
		[ContextMenu("ToggleSidebar")]
		private void ToggleSidebar()
		{
			this.sidebarOpen = !this.sidebarOpen;
			this.ToggleSidebar(this.sidebarOpen);
		}

		// Token: 0x06001867 RID: 6247 RVA: 0x0007BF99 File Offset: 0x0007A199
		private void ToggleSidebar(bool open)
		{
			if (!open)
			{
				this.panelsHolder.SetPosX(0f);
				return;
			}
			this.panelsHolder.SetPosX(this.sidebarWidth);
		}

		// Token: 0x04001671 RID: 5745
		[SerializeField]
		private Button sidebarButton;

		// Token: 0x04001672 RID: 5746
		[SerializeField]
		private RectTransform panelsHolder;

		// Token: 0x04001673 RID: 5747
		private float sidebarWidth = 300f;

		// Token: 0x04001674 RID: 5748
		private bool sidebarOpen = true;
	}
}

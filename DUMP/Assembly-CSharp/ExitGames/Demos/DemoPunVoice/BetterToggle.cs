using System;
using UnityEngine;
using UnityEngine.UI;

namespace ExitGames.Demos.DemoPunVoice
{
	// Token: 0x02000393 RID: 915
	[RequireComponent(typeof(Toggle))]
	[DisallowMultipleComponent]
	public class BetterToggle : MonoBehaviour
	{
		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06001818 RID: 6168 RVA: 0x0007A660 File Offset: 0x00078860
		// (remove) Token: 0x06001819 RID: 6169 RVA: 0x0007A694 File Offset: 0x00078894
		public static event BetterToggle.OnToggle ToggleValueChanged;

		// Token: 0x0600181A RID: 6170 RVA: 0x0007A6C7 File Offset: 0x000788C7
		private void Start()
		{
			this.toggle = base.GetComponent<Toggle>();
			this.toggle.onValueChanged.AddListener(delegate(bool <p0>)
			{
				this.OnToggleValueChanged();
			});
		}

		// Token: 0x0600181B RID: 6171 RVA: 0x0007A6F1 File Offset: 0x000788F1
		public void OnToggleValueChanged()
		{
			if (BetterToggle.ToggleValueChanged != null)
			{
				BetterToggle.ToggleValueChanged(this.toggle);
			}
		}

		// Token: 0x04001631 RID: 5681
		private Toggle toggle;

		// Token: 0x02000558 RID: 1368
		// (Invoke) Token: 0x06001FA2 RID: 8098
		public delegate void OnToggle(Toggle toggle);
	}
}

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityStandardAssets.CrossPlatformInput
{
	// Token: 0x0200038D RID: 909
	[ExecuteInEditMode]
	public class MobileControlRig : MonoBehaviour
	{
		// Token: 0x060017CC RID: 6092 RVA: 0x00079CFD File Offset: 0x00077EFD
		private void OnEnable()
		{
			this.CheckEnableControlRig();
		}

		// Token: 0x060017CD RID: 6093 RVA: 0x00079D05 File Offset: 0x00077F05
		private void Start()
		{
			if (Object.FindObjectOfType<EventSystem>() == null)
			{
				GameObject gameObject = new GameObject("EventSystem");
				gameObject.AddComponent<EventSystem>();
				gameObject.AddComponent<StandaloneInputModule>();
			}
		}

		// Token: 0x060017CE RID: 6094 RVA: 0x00079D2B File Offset: 0x00077F2B
		private void CheckEnableControlRig()
		{
			this.EnableControlRig(false);
		}

		// Token: 0x060017CF RID: 6095 RVA: 0x00079D34 File Offset: 0x00077F34
		private void EnableControlRig(bool enabled)
		{
			try
			{
				foreach (object obj in base.transform)
				{
					((Transform)obj).gameObject.SetActive(enabled);
				}
			}
			catch (Exception)
			{
			}
		}
	}
}

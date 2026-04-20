using System;
using UnityEngine;

namespace Peak
{
	// Token: 0x020003CD RID: 973
	public class DisableWhenUsingSteam : MonoBehaviour
	{
		// Token: 0x060019B2 RID: 6578 RVA: 0x00081D71 File Offset: 0x0007FF71
		private void Start()
		{
			Debug.Log("Disabling self because Steam is on.");
			base.gameObject.SetActive(false);
		}
	}
}

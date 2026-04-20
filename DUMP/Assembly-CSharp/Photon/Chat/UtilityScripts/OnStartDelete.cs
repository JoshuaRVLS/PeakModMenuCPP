using System;
using UnityEngine;

namespace Photon.Chat.UtilityScripts
{
	// Token: 0x020003AE RID: 942
	public class OnStartDelete : MonoBehaviour
	{
		// Token: 0x06001932 RID: 6450 RVA: 0x0007F244 File Offset: 0x0007D444
		private void Start()
		{
			Object.Destroy(base.gameObject);
		}
	}
}

using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace Peak
{
	// Token: 0x020003CC RID: 972
	public class LoadRunSettingsOnStart : MonoBehaviour
	{
		// Token: 0x060019B0 RID: 6576 RVA: 0x00081D61 File Offset: 0x0007FF61
		private IEnumerator Start()
		{
			while (!PhotonNetwork.InRoom || !Character.localCharacter)
			{
				yield return null;
			}
			RunSettings.LoadFromDisk();
			yield break;
		}
	}
}

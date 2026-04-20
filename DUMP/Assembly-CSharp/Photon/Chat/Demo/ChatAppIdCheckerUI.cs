using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Chat.Demo
{
	// Token: 0x020003A8 RID: 936
	[ExecuteInEditMode]
	public class ChatAppIdCheckerUI : MonoBehaviour
	{
		// Token: 0x06001902 RID: 6402 RVA: 0x0007E3CC File Offset: 0x0007C5CC
		public void Update()
		{
			string text = string.Empty;
			if (string.IsNullOrEmpty(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat))
			{
				text = "<Color=Red>WARNING:</Color>\nPlease setup a Chat AppId in the PhotonServerSettings file.";
			}
			this.Description.text = text;
		}

		// Token: 0x040016DB RID: 5851
		public Text Description;

		// Token: 0x040016DC RID: 5852
		public bool WizardOpenedOnce;
	}
}

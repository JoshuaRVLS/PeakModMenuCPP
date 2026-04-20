using System;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Chat.Demo
{
	// Token: 0x020003AA RID: 938
	public class FriendItem : MonoBehaviour
	{
		// Token: 0x1700017B RID: 379
		// (get) Token: 0x06001926 RID: 6438 RVA: 0x0007F08E File Offset: 0x0007D28E
		// (set) Token: 0x06001925 RID: 6437 RVA: 0x0007F080 File Offset: 0x0007D280
		[HideInInspector]
		public string FriendId
		{
			get
			{
				return this.NameLabel.text;
			}
			set
			{
				this.NameLabel.text = value;
			}
		}

		// Token: 0x06001927 RID: 6439 RVA: 0x0007F09B File Offset: 0x0007D29B
		public void Awake()
		{
			this.Health.text = string.Empty;
		}

		// Token: 0x06001928 RID: 6440 RVA: 0x0007F0B0 File Offset: 0x0007D2B0
		public void OnFriendStatusUpdate(int status, bool gotMessage, object message)
		{
			string text;
			switch (status)
			{
			case 1:
				text = "Invisible";
				break;
			case 2:
				text = "Online";
				break;
			case 3:
				text = "Away";
				break;
			case 4:
				text = "Do not disturb";
				break;
			case 5:
				text = "Looking For Game/Group";
				break;
			case 6:
				text = "Playing";
				break;
			default:
				text = "Offline";
				break;
			}
			this.StatusLabel.text = text;
			if (gotMessage)
			{
				string text2 = string.Empty;
				if (message != null)
				{
					string[] array = message as string[];
					if (array != null && array.Length >= 2)
					{
						text2 = array[1] + "%";
					}
				}
				this.Health.text = text2;
			}
		}

		// Token: 0x040016F5 RID: 5877
		public Text NameLabel;

		// Token: 0x040016F6 RID: 5878
		public Text StatusLabel;

		// Token: 0x040016F7 RID: 5879
		public Text Health;
	}
}

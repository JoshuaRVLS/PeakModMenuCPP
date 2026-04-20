using System;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Chat.Demo
{
	// Token: 0x020003AC RID: 940
	[RequireComponent(typeof(ChatGui))]
	public class NamePickGui : MonoBehaviour
	{
		// Token: 0x0600192C RID: 6444 RVA: 0x0007F174 File Offset: 0x0007D374
		public void Start()
		{
			this.chatNewComponent = Object.FindFirstObjectByType<ChatGui>();
			string @string = PlayerPrefs.GetString("NamePickUserName");
			if (!string.IsNullOrEmpty(@string))
			{
				this.idInput.text = @string;
			}
		}

		// Token: 0x0600192D RID: 6445 RVA: 0x0007F1AB File Offset: 0x0007D3AB
		public void EndEditOnEnter()
		{
			if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
			{
				this.StartChat();
			}
		}

		// Token: 0x0600192E RID: 6446 RVA: 0x0007F1C8 File Offset: 0x0007D3C8
		public void StartChat()
		{
			ChatGui chatGui = Object.FindFirstObjectByType<ChatGui>();
			chatGui.UserName = this.idInput.text.Trim();
			chatGui.Connect();
			base.enabled = false;
			PlayerPrefs.SetString("NamePickUserName", chatGui.UserName);
		}

		// Token: 0x040016F8 RID: 5880
		private const string UserNamePlayerPref = "NamePickUserName";

		// Token: 0x040016F9 RID: 5881
		public ChatGui chatNewComponent;

		// Token: 0x040016FA RID: 5882
		public InputField idInput;
	}
}

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Photon.Chat.Demo
{
	// Token: 0x020003A7 RID: 935
	public class ChannelSelector : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x060018FF RID: 6399 RVA: 0x0007E395 File Offset: 0x0007C595
		public void SetChannel(string channel)
		{
			this.Channel = channel;
			base.GetComponentInChildren<Text>().text = this.Channel;
		}

		// Token: 0x06001900 RID: 6400 RVA: 0x0007E3AF File Offset: 0x0007C5AF
		public void OnPointerClick(PointerEventData eventData)
		{
			Object.FindFirstObjectByType<ChatGui>().ShowChannel(this.Channel);
		}

		// Token: 0x040016DA RID: 5850
		public string Channel;
	}
}

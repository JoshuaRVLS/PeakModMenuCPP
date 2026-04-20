using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Photon.Chat.UtilityScripts
{
	// Token: 0x020003AD RID: 941
	public class EventSystemSpawner : MonoBehaviour
	{
		// Token: 0x06001930 RID: 6448 RVA: 0x0007F216 File Offset: 0x0007D416
		private void OnEnable()
		{
			if (Object.FindFirstObjectByType<EventSystem>() == null)
			{
				GameObject gameObject = new GameObject("EventSystem");
				gameObject.AddComponent<EventSystem>();
				gameObject.AddComponent<StandaloneInputModule>();
			}
		}
	}
}

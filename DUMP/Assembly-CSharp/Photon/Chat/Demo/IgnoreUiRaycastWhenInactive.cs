using System;
using UnityEngine;

namespace Photon.Chat.Demo
{
	// Token: 0x020003AB RID: 939
	public class IgnoreUiRaycastWhenInactive : MonoBehaviour, ICanvasRaycastFilter
	{
		// Token: 0x0600192A RID: 6442 RVA: 0x0007F15F File Offset: 0x0007D35F
		public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
		{
			return base.gameObject.activeInHierarchy;
		}
	}
}

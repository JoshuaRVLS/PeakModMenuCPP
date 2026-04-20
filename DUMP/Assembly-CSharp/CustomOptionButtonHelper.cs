using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000246 RID: 582
public class CustomOptionButtonHelper : MonoBehaviour, ISelectHandler, IEventSystemHandler
{
	// Token: 0x060011B3 RID: 4531 RVA: 0x0005938E File Offset: 0x0005758E
	public void OnSelect(BaseEventData eventData)
	{
		this.option.Hover();
	}

	// Token: 0x04000FB3 RID: 4019
	public CustomOptionBase option;
}

using System;
using TMPro;
using UnityEngine;

// Token: 0x0200036E RID: 878
public class UI_Notifications : MonoBehaviour
{
	// Token: 0x06001721 RID: 5921 RVA: 0x00076EA4 File Offset: 0x000750A4
	public void AddNotification(string text)
	{
		Transform child = base.transform.GetChild(0);
		Object.Instantiate<GameObject>(this.prefab, child.position, child.rotation, child).GetComponentInChildren<TextMeshProUGUI>().text = text;
	}

	// Token: 0x04001593 RID: 5523
	public GameObject prefab;
}

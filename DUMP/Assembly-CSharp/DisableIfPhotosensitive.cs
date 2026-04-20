using System;
using UnityEngine;
using Zorro.Settings;

// Token: 0x02000256 RID: 598
public class DisableIfPhotosensitive : MonoBehaviour
{
	// Token: 0x060011FD RID: 4605 RVA: 0x0005A70B File Offset: 0x0005890B
	private void Start()
	{
		if (GameHandler.Instance.SettingsHandler.GetSetting<PhotosensitiveSetting>().Value == OffOnMode.ON)
		{
			this.objectToDisable.SetActive(false);
			if (this.objectToReplace)
			{
				this.objectToReplace.SetActive(true);
			}
		}
	}

	// Token: 0x04001001 RID: 4097
	public GameObject objectToDisable;

	// Token: 0x04001002 RID: 4098
	public GameObject objectToReplace;
}

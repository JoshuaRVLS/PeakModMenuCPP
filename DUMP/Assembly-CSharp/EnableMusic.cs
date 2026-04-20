using System;
using UnityEngine;

// Token: 0x0200025E RID: 606
public class EnableMusic : MonoBehaviour
{
	// Token: 0x06001212 RID: 4626 RVA: 0x0005AAD8 File Offset: 0x00058CD8
	private void Update()
	{
		if (this.enable)
		{
			this.music.SetActive(true);
		}
	}

	// Token: 0x04001014 RID: 4116
	public bool enable;

	// Token: 0x04001015 RID: 4117
	public GameObject music;
}

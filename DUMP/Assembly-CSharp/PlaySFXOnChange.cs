using System;
using UnityEngine;

// Token: 0x020002D6 RID: 726
public class PlaySFXOnChange : MonoBehaviour
{
	// Token: 0x0600144D RID: 5197 RVA: 0x00066EB4 File Offset: 0x000650B4
	private void Update()
	{
		if (this.refObj.active && !this.t)
		{
			this.t = true;
			for (int i = 0; i < this.sfxOn.Length; i++)
			{
				this.sfxOn[i].Play(default(Vector3));
			}
		}
		if (!this.refObj.active && this.t)
		{
			this.t = false;
			for (int j = 0; j < this.sfxOff.Length; j++)
			{
				this.sfxOff[j].Play(default(Vector3));
			}
		}
	}

	// Token: 0x04001286 RID: 4742
	public SFX_Instance[] sfxOn;

	// Token: 0x04001287 RID: 4743
	public SFX_Instance[] sfxOff;

	// Token: 0x04001288 RID: 4744
	private bool t;

	// Token: 0x04001289 RID: 4745
	public GameObject refObj;
}

using System;
using UnityEngine;

// Token: 0x02000217 RID: 535
public class BingBongAudioSwitch : MonoBehaviour
{
	// Token: 0x06001065 RID: 4197 RVA: 0x000517D8 File Offset: 0x0004F9D8
	public void Init()
	{
		if (this.refObject.activeSelf)
		{
			for (int i = 0; i < this.enableLoop.Length; i++)
			{
				this.enableLoop[i].SetActive(true);
			}
			for (int j = 0; j < this.disableLoop.Length; j++)
			{
				this.disableLoop[j].SetActive(false);
			}
			for (int k = 0; k < this.clipOriginal.Length; k++)
			{
				this.clipOriginal[k].enabled = false;
			}
			for (int l = 0; l < this.clipReplace.Length; l++)
			{
				this.clipReplace[l].enabled = true;
			}
		}
		if (!this.refObject.activeSelf)
		{
			for (int m = 0; m < this.enableLoop.Length; m++)
			{
				this.enableLoop[m].SetActive(false);
			}
			for (int n = 0; n < this.disableLoop.Length; n++)
			{
				this.disableLoop[n].SetActive(true);
			}
			for (int num = 0; num < this.clipOriginal.Length; num++)
			{
				this.clipOriginal[num].enabled = true;
			}
			for (int num2 = 0; num2 < this.clipReplace.Length; num2++)
			{
				this.clipReplace[num2].enabled = false;
			}
		}
	}

	// Token: 0x04000E55 RID: 3669
	public GameObject refObject;

	// Token: 0x04000E56 RID: 3670
	public SFX_PlayOneShot[] clipOriginal;

	// Token: 0x04000E57 RID: 3671
	public SFX_PlayOneShot[] clipReplace;

	// Token: 0x04000E58 RID: 3672
	public GameObject[] enableLoop;

	// Token: 0x04000E59 RID: 3673
	public GameObject[] disableLoop;
}

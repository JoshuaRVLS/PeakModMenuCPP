using System;
using UnityEngine;

// Token: 0x02000240 RID: 576
public class CoverageToVolume : MonoBehaviour
{
	// Token: 0x060011A0 RID: 4512 RVA: 0x00058E40 File Offset: 0x00057040
	private void Update()
	{
		if (this.aM && this.sound)
		{
			if (this.aM.obstruction <= 0.6f)
			{
				this.vol = this.max;
			}
			if (this.aM.obstruction > 0.6f)
			{
				this.vol = this.mid;
			}
			if (this.aM.obstruction >= 0.8f)
			{
				this.vol = this.min;
			}
			this.sound.volume = Mathf.Lerp(this.sound.volume, this.vol * this.mod, 0.5f * Time.deltaTime);
		}
	}

	// Token: 0x04000F86 RID: 3974
	public float mod;

	// Token: 0x04000F87 RID: 3975
	public AudioSource sound;

	// Token: 0x04000F88 RID: 3976
	public AmbienceAudio aM;

	// Token: 0x04000F89 RID: 3977
	public float max = 0.1f;

	// Token: 0x04000F8A RID: 3978
	public float mid = 0.05f;

	// Token: 0x04000F8B RID: 3979
	public float min = 0.025f;

	// Token: 0x04000F8C RID: 3980
	private float vol;
}

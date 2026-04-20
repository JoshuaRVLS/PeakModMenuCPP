using System;
using UnityEngine;

// Token: 0x0200020C RID: 524
public class AudioLoop : MonoBehaviour
{
	// Token: 0x0600103C RID: 4156 RVA: 0x00050B10 File Offset: 0x0004ED10
	private void Update()
	{
		this.loop.volume = Mathf.Lerp(this.loop.volume, this.volume, 2f * Time.deltaTime);
		this.loop.pitch = Mathf.Lerp(this.loop.pitch, this.pitch, 2f * Time.deltaTime);
	}

	// Token: 0x04000E23 RID: 3619
	public AudioSource loop;

	// Token: 0x04000E24 RID: 3620
	public float volume;

	// Token: 0x04000E25 RID: 3621
	public float pitch = 1f;
}

using System;
using UnityEngine;

// Token: 0x0200031F RID: 799
public class RescueHookSFX : MonoBehaviour
{
	// Token: 0x06001558 RID: 5464 RVA: 0x0006C268 File Offset: 0x0006A468
	private void Update()
	{
		if (this.hookPoint.localPosition.x != 0f)
		{
			this.t = true;
		}
		if (this.t && this.hookPoint.localPosition.x == 0f)
		{
			this.t = false;
			for (int i = 0; i < this.reAttatch.Length; i++)
			{
				this.reAttatch[i].Play(base.transform.position);
			}
		}
		if (this.lR.enabled)
		{
			this.loop.volume = 0.45f;
			this.loop.pitch += Time.deltaTime / 2f;
			if (this.loop.pitch > 2f)
			{
				this.loop.pitch = 2f;
				return;
			}
		}
		else
		{
			this.loop.volume = 0f;
			this.loop.pitch = 1f;
		}
	}

	// Token: 0x04001385 RID: 4997
	public LineRenderer lR;

	// Token: 0x04001386 RID: 4998
	public Transform hookPoint;

	// Token: 0x04001387 RID: 4999
	public SFX_Instance[] reAttatch;

	// Token: 0x04001388 RID: 5000
	public AudioSource loop;

	// Token: 0x04001389 RID: 5001
	public bool t;
}

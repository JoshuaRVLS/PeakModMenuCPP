using System;
using UnityEngine;

// Token: 0x02000325 RID: 805
public class RopeAudio : MonoBehaviour
{
	// Token: 0x06001576 RID: 5494 RVA: 0x0006C9FA File Offset: 0x0006ABFA
	private void Start()
	{
		this.prev = this.ropeSpool.segments;
	}

	// Token: 0x06001577 RID: 5495 RVA: 0x0006CA10 File Offset: 0x0006AC10
	private void Update()
	{
		this.startT -= Time.deltaTime;
		this.prev = Mathf.Lerp(this.prev, this.ropeSpool.segments, Time.deltaTime * 20f);
		if (this.startT <= 0f)
		{
			this.loop1.volume = Mathf.Lerp(this.loop1.volume, Mathf.Abs(this.prev - this.ropeSpool.segments) / 6f, 20f * Time.deltaTime);
			this.loop1.pitch = Mathf.Lerp(this.loop1.pitch, 1f + Mathf.Abs(this.prev - this.ropeSpool.segments) / 2f, 20f * Time.deltaTime);
			this.loop2.volume = Mathf.Lerp(this.loop2.volume, Mathf.Abs(this.prev - this.ropeSpool.segments) / 3f, 10f * Time.deltaTime);
			this.loop2.pitch = Mathf.Lerp(this.loop2.pitch, 0.25f + Mathf.Abs(this.prev - this.ropeSpool.segments) / 2f, 10f * Time.deltaTime);
			if (this.loop1.volume > 0.075f)
			{
				this.loop1.volume = 0.075f;
			}
			if (this.loop2.volume > 0.075f)
			{
				this.loop2.volume = 0.075f;
			}
			if (!this.t && this.ropeSpool.segments == 40f)
			{
				for (int i = 0; i < this.min.Length; i++)
				{
					this.min[i].Play(base.transform.position);
				}
				this.t = true;
			}
			if (this.t && this.ropeSpool.segments == 3f)
			{
				for (int j = 0; j < this.max.Length; j++)
				{
					this.max[j].Play(base.transform.position);
				}
				this.t = false;
			}
		}
	}

	// Token: 0x0400139B RID: 5019
	public RopeSpool ropeSpool;

	// Token: 0x0400139C RID: 5020
	public AudioSource loop1;

	// Token: 0x0400139D RID: 5021
	public AudioSource loop2;

	// Token: 0x0400139E RID: 5022
	private float prev;

	// Token: 0x0400139F RID: 5023
	public SFX_Instance[] min;

	// Token: 0x040013A0 RID: 5024
	public SFX_Instance[] max;

	// Token: 0x040013A1 RID: 5025
	private bool t;

	// Token: 0x040013A2 RID: 5026
	private float startT = 0.5f;
}

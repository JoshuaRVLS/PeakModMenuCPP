using System;
using UnityEngine;

// Token: 0x02000127 RID: 295
public class MandrakeScreamFX : MonoBehaviour
{
	// Token: 0x060009AA RID: 2474 RVA: 0x000337FA File Offset: 0x000319FA
	private void Awake()
	{
		this.tracker = base.GetComponent<TrackNetworkedObject>();
	}

	// Token: 0x060009AB RID: 2475 RVA: 0x00033808 File Offset: 0x00031A08
	private void Update()
	{
		if (this.tracker.trackedObject)
		{
			if (this.mandrake == null)
			{
				this.mandrake = this.tracker.trackedObject.GetComponent<Mandrake>();
			}
			if (this.mandrake == null || this.mandrake.item.cooking.timesCookedLocal > 0)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			if (this.screaming)
			{
				if (!this.mandrake.screaming)
				{
					this.EndScreamFX();
					return;
				}
			}
			else if (this.mandrake.screaming)
			{
				this.StartScreamFX();
			}
		}
	}

	// Token: 0x060009AC RID: 2476 RVA: 0x000338AF File Offset: 0x00031AAF
	public void StartScreamFX()
	{
		this.screaming = true;
		this.vfx.Play();
		this.sfxScream.PlayFromSource(base.transform.position, this.source);
	}

	// Token: 0x060009AD RID: 2477 RVA: 0x000338DF File Offset: 0x00031ADF
	public void EndScreamFX()
	{
		this.screaming = false;
		this.vfx.Stop();
	}

	// Token: 0x060009AE RID: 2478 RVA: 0x000338F3 File Offset: 0x00031AF3
	private void OnDestroy()
	{
		if (this.handle != null)
		{
			this.source.Stop();
		}
	}

	// Token: 0x040008ED RID: 2285
	public ParticleSystem vfx;

	// Token: 0x040008EE RID: 2286
	public SFX_Instance sfxScream;

	// Token: 0x040008EF RID: 2287
	public AudioSource source;

	// Token: 0x040008F0 RID: 2288
	private TrackNetworkedObject tracker;

	// Token: 0x040008F1 RID: 2289
	private Mandrake mandrake;

	// Token: 0x040008F2 RID: 2290
	private SFX_Player.SoundEffectHandle handle;

	// Token: 0x040008F3 RID: 2291
	private bool screaming;
}

using System;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000377 RID: 887
public class VoiceObscuranceFilter : MonoBehaviour
{
	// Token: 0x06001746 RID: 5958 RVA: 0x00077EEC File Offset: 0x000760EC
	private void Start()
	{
		this.anim = base.GetComponent<Animator>();
		if (GameObject.Find("Airport"))
		{
			this.lowPass.enabled = false;
			this.echo.enabled = false;
			this.reverb.enabled = false;
		}
		if (!this.head)
		{
			this.head = MainCamera.instance.transform;
		}
		if (this.head)
		{
			this.reverbAddition = math.saturate(LightVolume.Instance().SamplePositionAlpha(base.transform.position));
			if (Physics.Linecast(base.transform.position, this.head.position, out this.hit, this.layer))
			{
				this.lowPass.cutoffFrequency = Mathf.Lerp(this.lowPass.cutoffFrequency, 1500f, 100f * Time.deltaTime);
			}
			else
			{
				this.lowPass.cutoffFrequency = Mathf.Lerp(this.lowPass.cutoffFrequency, 7500f, 100f * Time.deltaTime);
			}
			if (Vector3.Distance(base.transform.position, this.head.position) > 60f)
			{
				if (this.anim != null)
				{
					this.anim.SetFloat("Obscurance", 1f, Time.deltaTime, 0.5f);
				}
				this.echo.wetMix = Mathf.Lerp(this.echo.wetMix, 0.35f, 5f * Time.deltaTime);
				this.echo.dryMix = Mathf.Lerp(this.echo.dryMix, 0.5f, 5f * Time.deltaTime);
				this.echo.decayRatio = Mathf.Lerp(this.echo.decayRatio, 0.3f, 5f * Time.deltaTime);
				this.echo.delay = Mathf.Lerp(this.echo.delay, 500f, 5f * Time.deltaTime);
				return;
			}
			if (this.anim != null)
			{
				this.anim.SetFloat("Obscurance", this.reverbAddition);
			}
			this.echo.wetMix = Mathf.Lerp(this.echo.wetMix, 0f, 1f * Time.deltaTime);
			this.echo.dryMix = Mathf.Lerp(this.echo.dryMix, 1f, 1f * Time.deltaTime);
			this.echo.decayRatio = Mathf.Lerp(this.echo.decayRatio, 0f, 1f * Time.deltaTime);
			this.echo.delay = Mathf.Lerp(this.echo.delay, 10f, 1f * Time.deltaTime);
		}
	}

	// Token: 0x06001747 RID: 5959 RVA: 0x000781DC File Offset: 0x000763DC
	private void Update()
	{
		if (!this.head)
		{
			this.head = MainCamera.instance.transform;
		}
		if (this.head)
		{
			this.reverbAddition = math.saturate(LightVolume.Instance().SamplePositionAlpha(base.transform.position));
			if (Physics.Linecast(base.transform.position, this.head.position, out this.hit, this.layer))
			{
				this.lowPass.cutoffFrequency = Mathf.Lerp(this.lowPass.cutoffFrequency, 1500f, 1f * Time.deltaTime);
			}
			else
			{
				this.lowPass.cutoffFrequency = Mathf.Lerp(this.lowPass.cutoffFrequency, 7500f, 1f * Time.deltaTime);
			}
			if (Vector3.Distance(base.transform.position, this.head.position) > 60f)
			{
				if (this.anim != null)
				{
					this.anim.SetFloat("Obscurance", 1f, Time.deltaTime, 0.5f);
				}
				this.echo.wetMix = Mathf.Lerp(this.echo.wetMix, 0.35f, 5f * Time.deltaTime);
				this.echo.dryMix = Mathf.Lerp(this.echo.dryMix, 0.5f, 5f * Time.deltaTime);
				this.echo.decayRatio = Mathf.Lerp(this.echo.decayRatio, 0.3f, 5f * Time.deltaTime);
				this.echo.delay = Mathf.Lerp(this.echo.delay, 500f, 5f * Time.deltaTime);
				return;
			}
			if (this.anim != null)
			{
				this.anim.SetFloat("Obscurance", this.reverbAddition);
			}
			this.echo.wetMix = Mathf.Lerp(this.echo.wetMix, 0f, 1f * Time.deltaTime);
			this.echo.dryMix = Mathf.Lerp(this.echo.dryMix, 1f, 1f * Time.deltaTime);
			this.echo.decayRatio = Mathf.Lerp(this.echo.decayRatio, 0f, 1f * Time.deltaTime);
			this.echo.delay = Mathf.Lerp(this.echo.delay, 10f, 1f * Time.deltaTime);
		}
	}

	// Token: 0x040015C3 RID: 5571
	public LayerMask layer;

	// Token: 0x040015C4 RID: 5572
	private RaycastHit hit;

	// Token: 0x040015C5 RID: 5573
	public Transform head;

	// Token: 0x040015C6 RID: 5574
	public AudioLowPassFilter lowPass;

	// Token: 0x040015C7 RID: 5575
	public AudioReverbFilter reverb;

	// Token: 0x040015C8 RID: 5576
	public AudioEchoFilter echo;

	// Token: 0x040015C9 RID: 5577
	public float reverbAddition;

	// Token: 0x040015CA RID: 5578
	private Animator anim;
}

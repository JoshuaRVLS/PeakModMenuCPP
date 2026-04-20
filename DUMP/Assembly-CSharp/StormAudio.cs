using System;
using UnityEngine;

// Token: 0x02000352 RID: 850
public class StormAudio : MonoBehaviour
{
	// Token: 0x06001682 RID: 5762 RVA: 0x00072EA0 File Offset: 0x000710A0
	private void Start()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("Storm");
		GameObject gameObject2 = GameObject.FindGameObjectWithTag("Rain");
		GameObject gameObject3 = GameObject.FindGameObjectWithTag("Wind");
		if (gameObject)
		{
			this.stormVisual = gameObject.GetComponent<StormVisual>();
		}
		if (gameObject2)
		{
			this.rainVisual = gameObject2.GetComponent<StormVisual>();
		}
		if (gameObject3)
		{
			this.windVisual = gameObject3.GetComponent<StormVisual>();
		}
	}

	// Token: 0x06001683 RID: 5763 RVA: 0x00072F0C File Offset: 0x0007110C
	private void Update()
	{
		if (this.stormVisual)
		{
			this.StormPlay(this.stormVisual, this.loopStorm, this.lPStorm, this.stormVolume);
		}
		if (this.windVisual)
		{
			this.StormPlay(this.windVisual, this.loopWind, this.lPStorm, this.rainVolume);
		}
		this.RainPlay();
	}

	// Token: 0x06001684 RID: 5764 RVA: 0x00072F78 File Offset: 0x00071178
	private void RainPlay()
	{
		this.loopRainHeavy.volume = Mathf.Lerp(this.loopRainHeavy.volume, 0f, Time.deltaTime * 0.25f);
		this.loopRainSoft.volume = Mathf.Lerp(this.loopRainSoft.volume, 0f, Time.deltaTime * 0.05f);
		if (this.rainVisual)
		{
			if (!this.rainVisual.observedPlayerInWindZone)
			{
				this.loopRainHeavy.volume = Mathf.Lerp(this.loopRainHeavy.volume, 0f, Time.deltaTime * 0.25f);
				this.loopRainSoft.volume = Mathf.Lerp(this.loopRainSoft.volume, 0f, Time.deltaTime * 0.05f);
			}
			if (this.rainVisual.observedPlayerInWindZone && this.aM)
			{
				if (this.aM.obstruction < 0.6f)
				{
					this.loopRainHeavy.volume = Mathf.Lerp(this.loopRainHeavy.volume, 0.25f, Time.deltaTime * 2f);
					this.loopRainSoft.volume = Mathf.Lerp(this.loopRainSoft.volume, 0.005f, Time.deltaTime * 2f);
				}
				if (this.aM.obstruction >= 0.6f)
				{
					this.loopRainHeavy.volume = Mathf.Lerp(this.loopRainHeavy.volume, 0.15f, Time.deltaTime * 2f);
					this.loopRainSoft.volume = Mathf.Lerp(this.loopRainSoft.volume, this.rainVolume, Time.deltaTime * 2f);
				}
			}
		}
	}

	// Token: 0x06001685 RID: 5765 RVA: 0x00073140 File Offset: 0x00071340
	private void StormPlay(StormVisual sV, AudioLoop aL, AudioLowPassFilter lFilter, float volume)
	{
		if (sV && aL && lFilter)
		{
			if (!sV.observedPlayerInWindZone)
			{
				aL.volume = Mathf.Lerp(aL.volume, 0f, Time.deltaTime * 0.25f);
				aL.pitch = Mathf.Lerp(aL.pitch, 0.25f, Time.deltaTime * 0.25f);
				lFilter.cutoffFrequency = Mathf.Lerp(lFilter.cutoffFrequency, 8000f, Time.deltaTime * 1f);
				return;
			}
			aL.pitch = Mathf.Lerp(aL.pitch, 1f, Time.deltaTime * 0.5f);
			if (this.aM.obstruction >= 0.6f)
			{
				lFilter.cutoffFrequency = Mathf.Lerp(lFilter.cutoffFrequency, 500f, Time.deltaTime * 0.25f);
				aL.volume = Mathf.Lerp(aL.volume, 0.05f, Time.deltaTime * 0.5f);
				return;
			}
			lFilter.cutoffFrequency = Mathf.Lerp(lFilter.cutoffFrequency, 8000f, Time.deltaTime * 1f);
			aL.volume = Mathf.Lerp(aL.volume, volume, Time.deltaTime * 0.5f);
		}
	}

	// Token: 0x040014D6 RID: 5334
	public AmbienceAudio aM;

	// Token: 0x040014D7 RID: 5335
	public AudioLoop loopStorm;

	// Token: 0x040014D8 RID: 5336
	public AudioLoop loopWind;

	// Token: 0x040014D9 RID: 5337
	public AudioLowPassFilter lPStorm;

	// Token: 0x040014DA RID: 5338
	public AudioLoop loopRainHeavy;

	// Token: 0x040014DB RID: 5339
	public AudioLoop loopRainSoft;

	// Token: 0x040014DC RID: 5340
	public StormVisual stormVisual;

	// Token: 0x040014DD RID: 5341
	public StormVisual rainVisual;

	// Token: 0x040014DE RID: 5342
	public StormVisual windVisual;

	// Token: 0x040014DF RID: 5343
	public float stormVolume = 0.25f;

	// Token: 0x040014E0 RID: 5344
	public float windVolume = 0.35f;

	// Token: 0x040014E1 RID: 5345
	public float rainVolume = 0.25f;
}

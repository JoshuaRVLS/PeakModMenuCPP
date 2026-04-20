using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000168 RID: 360
public class ReverbMix : MonoBehaviour
{
	// Token: 0x06000BD3 RID: 3027 RVA: 0x0003F70B File Offset: 0x0003D90B
	private void Start()
	{
		this.audioMixerGroup.audioMixer.GetFloat("EffectsStrength", out this.startReverbStrength);
		this.audioMixerGroup.audioMixer.SetFloat("EffectsStrength", this.reverbStrength);
	}

	// Token: 0x06000BD4 RID: 3028 RVA: 0x0003F745 File Offset: 0x0003D945
	private void Update()
	{
	}

	// Token: 0x06000BD5 RID: 3029 RVA: 0x0003F747 File Offset: 0x0003D947
	private void OnDisable()
	{
		this.audioMixerGroup.audioMixer.SetFloat("EffectsStrength", this.startReverbStrength);
	}

	// Token: 0x04000ACA RID: 2762
	public AudioMixerGroup audioMixerGroup;

	// Token: 0x04000ACB RID: 2763
	private float startReverbStrength;

	// Token: 0x04000ACC RID: 2764
	public float reverbStrength;
}

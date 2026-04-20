using System;
using UnityEngine;

// Token: 0x020001AA RID: 426
[CreateAssetMenu(fileName = "SoundEffectInstance", menuName = "Landfall/SoundEffectInstance")]
public class SFX_Instance : ScriptableObject
{
	// Token: 0x06000DD7 RID: 3543 RVA: 0x00045E9A File Offset: 0x0004409A
	public AudioClip GetClip()
	{
		return this.clips[Random.Range(0, this.clips.Length)];
	}

	// Token: 0x06000DD8 RID: 3544 RVA: 0x00045EB1 File Offset: 0x000440B1
	public void Play(Vector3 pos = default(Vector3))
	{
		SFX_Player.instance.PlaySFX(this, pos, null, null, 1f, false);
	}

	// Token: 0x06000DD9 RID: 3545 RVA: 0x00045EC8 File Offset: 0x000440C8
	public void Play(Vector3 position, Transform follow)
	{
		SFX_Player.instance.PlaySFX(this, position, follow, null, 1f, false);
	}

	// Token: 0x06000DDA RID: 3546 RVA: 0x00045EE0 File Offset: 0x000440E0
	public void PlayFromSource(Vector3 position, AudioSource source)
	{
		if (source.isPlaying)
		{
			source.Stop();
		}
		source.clip = this.GetClip();
		source.volume = this.GetVolume();
		source.volume = this.GetPitch();
		source.spatialBlend = this.settings.spatialBlend;
		source.dopplerLevel = this.settings.dopplerLevel;
		source.Play();
	}

	// Token: 0x06000DDB RID: 3547 RVA: 0x00045F47 File Offset: 0x00044147
	internal void OnPlayed()
	{
		this.lastTimePlayed = Time.unscaledTime;
	}

	// Token: 0x06000DDC RID: 3548 RVA: 0x00045F54 File Offset: 0x00044154
	internal bool ReadyToPlay()
	{
		return this.lastTimePlayed > Time.unscaledTime + this.settings.cooldown || this.lastTimePlayed + this.settings.cooldown < Time.unscaledTime;
	}

	// Token: 0x06000DDD RID: 3549 RVA: 0x00045F8A File Offset: 0x0004418A
	private float GetVolume()
	{
		return this.settings.volume * (1f - Random.Range(0f, this.settings.volume_Variation));
	}

	// Token: 0x06000DDE RID: 3550 RVA: 0x00045FB3 File Offset: 0x000441B3
	private float GetPitch()
	{
		return this.settings.pitch + Random.Range(-this.settings.pitch_Variation * 0.5f, this.settings.pitch_Variation * 0.5f);
	}

	// Token: 0x04000BAE RID: 2990
	public AudioClip[] clips;

	// Token: 0x04000BAF RID: 2991
	public SFX_Settings settings;

	// Token: 0x04000BB0 RID: 2992
	internal float lastTimePlayed;
}

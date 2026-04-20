using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x020001AC RID: 428
public class SFX_Player : MonoBehaviour
{
	// Token: 0x06000DE1 RID: 3553 RVA: 0x00046068 File Offset: 0x00044268
	private void Start()
	{
		this.defaultSource = base.GetComponentInChildren<AudioSource>().gameObject;
		SFX_Player.instance = this;
		for (int i = 0; i < 20; i++)
		{
			this.CreateNewSource();
		}
	}

	// Token: 0x06000DE2 RID: 3554 RVA: 0x000460A0 File Offset: 0x000442A0
	public SFX_Player.SoundEffectHandle PlaySFX(SFX_Instance SFX, Vector3 position, Transform followTransform = null, SFX_Settings overrideSettings = null, float volumeMultiplier = 1f, bool loop = false)
	{
		if (SFX == null)
		{
			return null;
		}
		if (SFX.clips.Length == 0)
		{
			return null;
		}
		if (!SFX.ReadyToPlay())
		{
			return null;
		}
		if (SFX.settings.spatialBlend > 0f && Vector3.Distance(MainCamera.instance.transform.position, position) > SFX.settings.range / 2f)
		{
			return null;
		}
		if (this.nrOfSoundsPlayed + 1 >= AudioSettings.GetConfiguration().numRealVoices)
		{
			this.StopOldest();
		}
		SFX.OnPlayed();
		SFX_Player.SoundEffectHandle soundEffectHandle = new SFX_Player.SoundEffectHandle();
		soundEffectHandle.Init(base.StartCoroutine(this.IPlaySFX(SFX, position, followTransform, overrideSettings, volumeMultiplier, loop, soundEffectHandle)));
		return soundEffectHandle;
	}

	// Token: 0x06000DE3 RID: 3555 RVA: 0x0004614C File Offset: 0x0004434C
	private void StopOldest()
	{
		this.currentlyPlayed[0].source.StopPlaying();
	}

	// Token: 0x06000DE4 RID: 3556 RVA: 0x00046164 File Offset: 0x00044364
	private IEnumerator IPlaySFX(SFX_Instance SFX, Vector3 position, Transform followTransform, SFX_Settings overrideSettings, float volumeMultiplier, bool loop, SFX_Player.SoundEffectHandle handle)
	{
		SFX_Player.SFX_Source source = this.GetAvailibleSource();
		AudioClip clip = SFX.GetClip();
		if (clip == null)
		{
			Debug.LogError("Trying to play null sound >:I");
			yield break;
		}
		SFX_Settings settings = SFX.settings;
		if (overrideSettings != null)
		{
			settings = overrideSettings;
		}
		float c = 0f;
		float t = clip.length;
		source.source.clip = clip;
		source.source.transform.position = position;
		source.source.volume = settings.volume * Random.Range(1f - settings.volume_Variation, 1f) * volumeMultiplier;
		source.source.pitch = settings.pitch + Random.Range(-settings.pitch_Variation * 0.5f, settings.pitch_Variation * 0.5f);
		source.source.maxDistance = settings.range;
		source.source.spatialBlend = settings.spatialBlend;
		source.source.dopplerLevel = settings.dopplerLevel;
		source.source.loop = loop;
		source.source.outputAudioMixerGroup = this.defaultMixerGroup;
		Vector3 relativePos = Vector3.zero;
		if (followTransform)
		{
			relativePos = followTransform.InverseTransformPoint(position);
		}
		source.StartPlaying(handle);
		while (c < t || loop)
		{
			c += Time.deltaTime * settings.pitch;
			if (followTransform)
			{
				source.source.transform.position = followTransform.TransformPoint(relativePos);
			}
			yield return null;
		}
		source.StopPlaying();
		yield break;
	}

	// Token: 0x06000DE5 RID: 3557 RVA: 0x000461B4 File Offset: 0x000443B4
	private SFX_Player.SFX_Source GetAvailibleSource()
	{
		for (int i = 0; i < this.sources.Count; i++)
		{
			if (!this.sources[i].isPlaying)
			{
				return this.sources[i];
			}
		}
		return this.CreateNewSource();
	}

	// Token: 0x06000DE6 RID: 3558 RVA: 0x00046200 File Offset: 0x00044400
	private SFX_Player.SFX_Source CreateNewSource()
	{
		SFX_Player.SFX_Source sfx_Source = new SFX_Player.SFX_Source();
		GameObject gameObject = Object.Instantiate<GameObject>(this.defaultSource, base.transform.position, base.transform.rotation, base.transform);
		sfx_Source.source = gameObject.GetComponent<AudioSource>();
		sfx_Source.player = this;
		this.sources.Add(sfx_Source);
		return sfx_Source;
	}

	// Token: 0x06000DE7 RID: 3559 RVA: 0x0004625B File Offset: 0x0004445B
	private void OnPlayed(SFX_Player.SoundEffectHandle handle)
	{
		this.nrOfSoundsPlayed++;
		this.currentlyPlayed.Add(handle);
	}

	// Token: 0x06000DE8 RID: 3560 RVA: 0x00046277 File Offset: 0x00044477
	private void OnStopped(SFX_Player.SoundEffectHandle handle)
	{
		this.nrOfSoundsPlayed--;
		this.currentlyPlayed.Remove(handle);
	}

	// Token: 0x06000DE9 RID: 3561 RVA: 0x00046294 File Offset: 0x00044494
	public static void StopPlaying(SFX_Player.SoundEffectHandle handle, float fadeTime = 0f)
	{
		SFX_Player.SFX_Source sfxsourceFromHandle = SFX_Player.GetSFXSourceFromHandle(handle);
		if (sfxsourceFromHandle != null)
		{
			if (fadeTime == 0f)
			{
				sfxsourceFromHandle.StopPlaying();
				return;
			}
			SFX_Player.instance.StartCoroutine(SFX_Player.FadeOut(sfxsourceFromHandle, fadeTime));
		}
	}

	// Token: 0x06000DEA RID: 3562 RVA: 0x000462CC File Offset: 0x000444CC
	private static IEnumerator FadeOut(SFX_Player.SFX_Source source, float fadeTime)
	{
		float c = 0f;
		float startVolume = source.source.volume;
		while (c < fadeTime)
		{
			c += Time.deltaTime;
			source.source.volume = Mathf.Lerp(startVolume, 0f, c / fadeTime);
			yield return null;
		}
		source.StopPlaying();
		yield break;
	}

	// Token: 0x06000DEB RID: 3563 RVA: 0x000462E4 File Offset: 0x000444E4
	private static SFX_Player.SFX_Source GetSFXSourceFromHandle(SFX_Player.SoundEffectHandle handle)
	{
		foreach (SFX_Player.SFX_Source sfx_Source in SFX_Player.instance.sources)
		{
			if (sfx_Source.handle == handle)
			{
				return sfx_Source;
			}
		}
		return null;
	}

	// Token: 0x04000BBA RID: 3002
	public AudioMixerGroup defaultMixerGroup;

	// Token: 0x04000BBB RID: 3003
	private GameObject defaultSource;

	// Token: 0x04000BBC RID: 3004
	public List<SFX_Player.SFX_Source> sources = new List<SFX_Player.SFX_Source>();

	// Token: 0x04000BBD RID: 3005
	private List<SFX_Player.SoundEffectHandle> currentlyPlayed = new List<SFX_Player.SoundEffectHandle>();

	// Token: 0x04000BBE RID: 3006
	public static SFX_Player instance;

	// Token: 0x04000BBF RID: 3007
	private int nrOfSoundsPlayed;

	// Token: 0x020004C3 RID: 1219
	[Serializable]
	public class SFX_Source
	{
		// Token: 0x06001D61 RID: 7521 RVA: 0x00089CDC File Offset: 0x00087EDC
		public void StopPlaying()
		{
			if (!this.isPlaying)
			{
				return;
			}
			if (this.handle.corutine != null)
			{
				this.player.StopCoroutine(this.handle.corutine);
			}
			this.player.OnStopped(this.handle);
			this.source.Stop();
			this.isPlaying = false;
			this.handle.source = null;
			this.handle = null;
		}

		// Token: 0x06001D62 RID: 7522 RVA: 0x00089D4B File Offset: 0x00087F4B
		public void StartPlaying(SFX_Player.SoundEffectHandle setHandle)
		{
			if (this.isPlaying)
			{
				return;
			}
			this.player.OnPlayed(setHandle);
			this.source.Play();
			this.isPlaying = true;
			this.handle = setHandle;
			this.handle.source = this;
		}

		// Token: 0x04001AEB RID: 6891
		public AudioSource source;

		// Token: 0x04001AEC RID: 6892
		public bool isPlaying;

		// Token: 0x04001AED RID: 6893
		public SFX_Player.SoundEffectHandle handle;

		// Token: 0x04001AEE RID: 6894
		public SFX_Player player;
	}

	// Token: 0x020004C4 RID: 1220
	public class SoundEffectHandle
	{
		// Token: 0x06001D64 RID: 7524 RVA: 0x00089D8F File Offset: 0x00087F8F
		public void Init(Coroutine c)
		{
			this.corutine = c;
		}

		// Token: 0x04001AEF RID: 6895
		public Coroutine corutine;

		// Token: 0x04001AF0 RID: 6896
		public SFX_Player.SFX_Source source;
	}
}

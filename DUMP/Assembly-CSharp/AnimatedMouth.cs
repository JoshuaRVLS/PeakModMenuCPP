using System;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.Serialization;
using Zorro.Core;

// Token: 0x02000057 RID: 87
public class AnimatedMouth : MonoBehaviour
{
	// Token: 0x0600047B RID: 1147 RVA: 0x0001B6A4 File Offset: 0x000198A4
	private void Start()
	{
		this.amplitudePeakLimiter = this.minAmplitudeThreshold;
		this.character = base.GetComponent<Character>();
		if (!this.isGhost && this.character != null && this.character.IsLocal)
		{
			Singleton<MicrophoneRelay>.Instance.RegisterMicListener(new Action<float[]>(this.OnGetMic));
		}
		this.pushToTalkSetting = GameHandler.Instance.SettingsHandler.GetSetting<PushToTalkSetting>();
	}

	// Token: 0x0600047C RID: 1148 RVA: 0x0001B718 File Offset: 0x00019918
	private void OnDestroy()
	{
		if (!this.isGhost && this.character != null && this.character.IsLocal && Singleton<MicrophoneRelay>.Instance)
		{
			Singleton<MicrophoneRelay>.Instance.UnregisterMicListener(new Action<float[]>(this.OnGetMic));
		}
	}

	// Token: 0x0600047D RID: 1149 RVA: 0x0001B76A File Offset: 0x0001996A
	public void OnGetMic(float[] buffer)
	{
		this.m_lastSentLocalBuffer = buffer;
	}

	// Token: 0x0600047E RID: 1150 RVA: 0x0001B774 File Offset: 0x00019974
	private void Update()
	{
		if (this.audioSource != null)
		{
			this.audioSource.GetSpectrumData(this.spectrum, 0, FFTWindow.Rectangular);
		}
		if (this.m_lastSentLocalBuffer != null)
		{
			this.spectrum = this.m_lastSentLocalBuffer;
		}
		this.ProcessMicData(this.spectrum);
	}

	// Token: 0x0600047F RID: 1151 RVA: 0x0001B7C4 File Offset: 0x000199C4
	public static float MicrophoneLevelMax(float[] data)
	{
		int num = 128;
		float num2 = 0f;
		for (int i = 0; i < num; i++)
		{
			float num3 = data[i] * data[i];
			if (num2 < num3)
			{
				num2 = num3;
			}
		}
		return num2;
	}

	// Token: 0x06000480 RID: 1152 RVA: 0x0001B7F8 File Offset: 0x000199F8
	public static float MicrophoneLevelMaxDecibels(float level)
	{
		return 20f * Mathf.Log10(Mathf.Abs(level));
	}

	// Token: 0x06000481 RID: 1153 RVA: 0x0001B80C File Offset: 0x00019A0C
	private void ProcessMicData(float[] buffer)
	{
		if (!this.audioSource)
		{
			return;
		}
		if (!this.isGhost && this.character != null && (this.character.data.dead || this.character.data.passedOut))
		{
			return;
		}
		float time = AnimatedMouth.MicrophoneLevelMaxDecibels(AnimatedMouth.MicrophoneLevelMax(buffer));
		if (this.character != null && this.character.IsLocal && ((this.pushToTalkSetting.Value == PushToTalkSetting.PushToTalkType.PushToTalk && !this.character.input.pushToTalkPressed) || (this.pushToTalkSetting.Value == PushToTalkSetting.PushToTalkType.PushToMute && this.character.input.pushToTalkPressed)))
		{
			time = -80f;
		}
		float num = this.decibelToAmountCurve.Evaluate(time);
		if (num > this.amplitudePeakLimiter)
		{
			this.amplitudePeakLimiter = num;
		}
		if (this.amplitudePeakLimiter > this.minAmplitudeThreshold)
		{
			this.amplitudePeakLimiter -= this.amplitudeHighestDecay * Time.deltaTime;
		}
		this.volume = num / this.amplitudePeakLimiter;
		if (this.volume > this.volumePeak)
		{
			this.volumePeak = this.volume;
		}
		this.volumePeak = Mathf.Lerp(this.volumePeak, 0f, Time.deltaTime * this.amplitudeSmoothing);
		if (this.volumePeak > this.talkThreshold)
		{
			this.mouthRenderer.material.SetInt("_UseTalkSprites", 1);
			this.isSpeaking = true;
		}
		else
		{
			this.isSpeaking = false;
			this.mouthRenderer.material.SetInt("_UseTalkSprites", 0);
		}
		this.amplitudeIndex = (int)(Mathf.Clamp01(this.volumePeak * this.amplitudeMult) * (float)(this.mouthTextures.Length - 1));
		this.mouthRenderer.material.SetTexture("_TalkSprite", this.mouthTextures[this.amplitudeIndex]);
	}

	// Token: 0x040004E2 RID: 1250
	public AnimationCurve decibelToAmountCurve = AnimationCurve.EaseInOut(-80f, 0f, 12f, 1f);

	// Token: 0x040004E3 RID: 1251
	public bool isSpeaking;

	// Token: 0x040004E4 RID: 1252
	public AudioSource audioSource;

	// Token: 0x040004E5 RID: 1253
	public Vector2 BandPassFilter;

	// Token: 0x040004E6 RID: 1254
	[FormerlySerializedAs("amplitude")]
	[Range(0f, 1f)]
	public float volume;

	// Token: 0x040004E7 RID: 1255
	[FormerlySerializedAs("amplitudeHighest")]
	public float amplitudePeakLimiter;

	// Token: 0x040004E8 RID: 1256
	public float minAmplitudeThreshold = 0.5f;

	// Token: 0x040004E9 RID: 1257
	public float amplitudeHighestDecay = 0.01f;

	// Token: 0x040004EA RID: 1258
	public float amplitudeSmoothing = 0.2f;

	// Token: 0x040004EB RID: 1259
	public float talkThreshold = 0.1f;

	// Token: 0x040004EC RID: 1260
	public float amplitudeMult;

	// Token: 0x040004ED RID: 1261
	[HideInInspector]
	public int amplitudeIndex;

	// Token: 0x040004EE RID: 1262
	[FormerlySerializedAs("textures")]
	[Header("Mouth Cards")]
	public Texture2D[] mouthTextures;

	// Token: 0x040004EF RID: 1263
	public Renderer mouthRenderer;

	// Token: 0x040004F0 RID: 1264
	public Character character;

	// Token: 0x040004F1 RID: 1265
	public bool isGhost;

	// Token: 0x040004F2 RID: 1266
	private float volumePeak;

	// Token: 0x040004F3 RID: 1267
	private PushToTalkSetting pushToTalkSetting;

	// Token: 0x040004F4 RID: 1268
	private float[] m_lastSentLocalBuffer;

	// Token: 0x040004F5 RID: 1269
	private float[] spectrum = new float[128];

	// Token: 0x040004F6 RID: 1270
	private bool checkedAudioSource;
}

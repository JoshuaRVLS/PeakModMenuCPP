using System;
using Photon.Voice;
using Photon.Voice.Unity;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;
using Zorro.Core;

// Token: 0x02000018 RID: 24
public class CharacterVoiceHandler : MonoBehaviour
{
	// Token: 0x1700002F RID: 47
	// (get) Token: 0x06000226 RID: 550 RVA: 0x00010B4A File Offset: 0x0000ED4A
	// (set) Token: 0x06000227 RID: 551 RVA: 0x00010B52 File Offset: 0x0000ED52
	internal AudioSource audioSource { get; private set; }

	// Token: 0x06000228 RID: 552 RVA: 0x00010B5B File Offset: 0x0000ED5B
	private void OnEnable()
	{
		GlobalEvents.OnCharacterAudioLevelsUpdated = (Action)Delegate.Combine(GlobalEvents.OnCharacterAudioLevelsUpdated, new Action(this.UpdateAudioLevel));
	}

	// Token: 0x06000229 RID: 553 RVA: 0x00010B7D File Offset: 0x0000ED7D
	private void OnDisable()
	{
		GlobalEvents.OnCharacterAudioLevelsUpdated = (Action)Delegate.Remove(GlobalEvents.OnCharacterAudioLevelsUpdated, new Action(this.UpdateAudioLevel));
	}

	// Token: 0x0600022A RID: 554 RVA: 0x00010BA0 File Offset: 0x0000EDA0
	private void UpdateAudioLevel()
	{
		if (AudioLevels.PlayerAudioLevels.ContainsKey(this.m_character.photonView.Owner.UserId))
		{
			float num = AudioLevels.PlayerAudioLevels[this.m_character.photonView.Owner.UserId];
			this.audioLevel = num;
			return;
		}
		this.audioLevel = 0.5f;
	}

	// Token: 0x0600022B RID: 555 RVA: 0x00010C04 File Offset: 0x0000EE04
	private void Start()
	{
		this.m_Recorder = base.GetComponent<Recorder>();
		this.m_character = base.GetComponentInParent<Character>();
		this.microphoneSetting = GameHandler.Instance.SettingsHandler.GetSetting<MicrophoneSetting>();
		this.pushToTalkSetting = GameHandler.Instance.SettingsHandler.GetSetting<PushToTalkSetting>();
		this.audioSource = base.GetComponent<AudioSource>();
		this.m_source = base.GetComponent<AudioSource>();
		if (this.m_character.IsLocal)
		{
			return;
		}
		byte b = PlayerHandler.AssignMixerGroup(this.m_character);
		if (b != 255)
		{
			this.m_source.outputAudioMixerGroup = this.GetMixerGroup(b);
			this.m_parameter = this.GetMixerGroupParameter(b);
		}
		this.m_mixer.SetFloat("VoicePitch", (RunSettings.GetValue(RunSettings.SETTINGTYPE.TimeScale, false) == 1) ? 1.5f : 1f);
		this.UpdateAudioLevel();
	}

	// Token: 0x0600022C RID: 556 RVA: 0x00010CE0 File Offset: 0x0000EEE0
	private AudioMixerGroup GetMixerGroup(byte group)
	{
		AudioMixerGroup result;
		switch (group)
		{
		case 0:
			result = this.m_mixerGroup1;
			break;
		case 1:
			result = this.m_mixerGroup2;
			break;
		case 2:
			result = this.m_mixerGroup3;
			break;
		case 3:
			result = this.m_mixerGroup4;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		return result;
	}

	// Token: 0x0600022D RID: 557 RVA: 0x00010D30 File Offset: 0x0000EF30
	private string GetMixerGroupParameter(byte group)
	{
		return "Voice" + ((int)(group + 1)).ToString() + "Effects";
	}

	// Token: 0x0600022E RID: 558 RVA: 0x00010D58 File Offset: 0x0000EF58
	private void Update()
	{
		this.m_source.volume = ((this.m_character.data.fullyConscious || this.m_character.IsGhost) ? this.audioLevel : ((this.m_character.data.passedOut && !this.m_character.data.fullyPassedOut) ? (this.audioLevel * Mathf.Clamp01(1f - this.m_character.data.passOutValue)) : 0f));
		this.PushToTalk();
		if (this.m_character.IsLocal && !this.m_character.isBot)
		{
			string id = this.microphoneSetting.Value.id;
			if (id != this.m_setMicrophoneDevice && !string.IsNullOrEmpty(id))
			{
				this.m_setMicrophoneDevice = id;
				this.m_Recorder.MicrophoneDevice = new DeviceInfo(id, null);
				Debug.Log("Setting microphone to " + id);
			}
		}
	}

	// Token: 0x0600022F RID: 559 RVA: 0x00010E54 File Offset: 0x0000F054
	private void PushToTalk()
	{
		bool flag = this.pushToTalkSetting.Value == PushToTalkSetting.PushToTalkType.VoiceActivation || (this.m_character.input.pushToTalkPressed && this.pushToTalkSetting.Value == PushToTalkSetting.PushToTalkType.PushToTalk) || (!this.m_character.input.pushToTalkPressed && this.pushToTalkSetting.Value == PushToTalkSetting.PushToTalkType.PushToMute);
		if (flag != this.m_currentlyTransmitting || this.firstTime)
		{
			this.firstTime = false;
			this.m_currentlyTransmitting = flag;
			this.m_Recorder.TransmitEnabled = flag;
		}
	}

	// Token: 0x06000230 RID: 560 RVA: 0x00010EE0 File Offset: 0x0000F0E0
	private void LateUpdate()
	{
		bool flag = false;
		if (Singleton<PeakHandler>.Instance != null && Singleton<PeakHandler>.Instance.isPlayingCinematic)
		{
			flag = true;
		}
		this.m_source.spatialBlend = (float)(flag ? 0 : 1);
		if (this.m_character.IsLocal)
		{
			return;
		}
		Vector3 position = this.m_character.refs.head.transform.position;
		if (this.m_character.Ghost != null)
		{
			position = this.m_character.Ghost.transform.position;
		}
		base.transform.position = position;
		float x = math.saturate(LightVolume.Instance().SamplePositionAlpha(position));
		x = math.saturate(1f - math.remap(0f, 0.3f, 0f, 1f, x));
		if (flag)
		{
		}
	}

	// Token: 0x040001F7 RID: 503
	private Character m_character;

	// Token: 0x040001F8 RID: 504
	[SerializeField]
	private AudioMixer m_mixer;

	// Token: 0x040001FA RID: 506
	[SerializeField]
	private AudioMixerGroup m_mixerGroup1;

	// Token: 0x040001FB RID: 507
	[SerializeField]
	private AudioMixerGroup m_mixerGroup2;

	// Token: 0x040001FC RID: 508
	[SerializeField]
	private AudioMixerGroup m_mixerGroup3;

	// Token: 0x040001FD RID: 509
	[SerializeField]
	private AudioMixerGroup m_mixerGroup4;

	// Token: 0x040001FE RID: 510
	private AudioSource m_source;

	// Token: 0x040001FF RID: 511
	private string m_parameter;

	// Token: 0x04000200 RID: 512
	private MicrophoneSetting microphoneSetting;

	// Token: 0x04000201 RID: 513
	private PushToTalkSetting pushToTalkSetting;

	// Token: 0x04000202 RID: 514
	private string m_setMicrophoneDevice;

	// Token: 0x04000203 RID: 515
	private Recorder m_Recorder;

	// Token: 0x04000204 RID: 516
	private bool m_currentlyTransmitting;

	// Token: 0x04000205 RID: 517
	private float audioLevel = 0.5f;

	// Token: 0x04000206 RID: 518
	private bool firstTime = true;

	// Token: 0x04000207 RID: 519
	public const float DEFAULT_VOICE_VOLUME = 0.5f;
}

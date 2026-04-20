using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Voice.Unity.UtilityScripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Photon.Voice.Unity.Demos.DemoVoiceUI
{
	// Token: 0x0200039F RID: 927
	[RequireComponent(typeof(UnityVoiceClient), typeof(ConnectAndJoin))]
	public class DemoVoiceUI : MonoBehaviour, IInRoomCallbacks, IMatchmakingCallbacks
	{
		// Token: 0x06001884 RID: 6276 RVA: 0x0007C4E0 File Offset: 0x0007A6E0
		private void Start()
		{
			this.connectAndJoin = base.GetComponent<ConnectAndJoin>();
			this.voiceConnection = base.GetComponent<UnityVoiceClient>();
			this.voiceAudioPreprocessor = this.voiceConnection.PrimaryRecorder.GetComponent<WebRtcAudioDsp>();
			this.compressionGainGameObject = this.agcCompressionGainSlider.transform.parent.gameObject;
			this.compressionGainText = this.compressionGainGameObject.GetComponentInChildren<Text>();
			this.targetLevelGameObject = this.agcTargetLevelSlider.transform.parent.gameObject;
			this.targetLevelText = this.targetLevelGameObject.GetComponentInChildren<Text>();
			this.aecOptionsGameObject = this.aecHighPassToggle.transform.parent.gameObject;
			this.SetDefaults();
			this.InitUiCallbacks();
			this.GetSavedNickname();
			this.voiceConnection.PrimaryRecorder.InputFactory = this.toneInputFactory;
			this.voiceConnection.SpeakerLinked += this.OnSpeakerCreated;
			this.voiceConnection.Client.AddCallbackTarget(this);
		}

		// Token: 0x06001885 RID: 6277 RVA: 0x0007C5DF File Offset: 0x0007A7DF
		protected virtual void SetDefaults()
		{
			this.muteToggle.isOn = !this.defaultTransmitEnabled;
		}

		// Token: 0x06001886 RID: 6278 RVA: 0x0007C5F5 File Offset: 0x0007A7F5
		private void OnDestroy()
		{
			this.voiceConnection.SpeakerLinked -= this.OnSpeakerCreated;
			this.voiceConnection.Client.RemoveCallbackTarget(this);
		}

		// Token: 0x06001887 RID: 6279 RVA: 0x0007C620 File Offset: 0x0007A820
		private void GetSavedNickname()
		{
			string @string = PlayerPrefs.GetString("vNick");
			if (!string.IsNullOrEmpty(@string))
			{
				this.localNicknameText.text = @string;
				this.voiceConnection.Client.NickName = @string;
			}
		}

		// Token: 0x06001888 RID: 6280 RVA: 0x0007C660 File Offset: 0x0007A860
		protected virtual void OnSpeakerCreated(Speaker speaker)
		{
			speaker.gameObject.transform.SetParent(this.RemoteVoicesPanel, false);
			speaker.GetComponent<RemoteSpeakerUI>().Init(this.voiceConnection);
			speaker.OnRemoteVoiceRemoveAction = (Action<Speaker>)Delegate.Combine(speaker.OnRemoteVoiceRemoveAction, new Action<Speaker>(this.OnRemoteVoiceRemove));
		}

		// Token: 0x06001889 RID: 6281 RVA: 0x0007C6B7 File Offset: 0x0007A8B7
		private void OnRemoteVoiceRemove(Speaker speaker)
		{
			if (speaker != null)
			{
				Object.Destroy(speaker.gameObject);
			}
		}

		// Token: 0x0600188A RID: 6282 RVA: 0x0007C6D0 File Offset: 0x0007A8D0
		private void ToggleMute(bool isOn)
		{
			this.muteToggle.targetGraphic.enabled = !isOn;
			if (isOn)
			{
				this.voiceConnection.Client.LocalPlayer.Mute();
				return;
			}
			this.voiceConnection.Client.LocalPlayer.Unmute();
		}

		// Token: 0x0600188B RID: 6283 RVA: 0x0007C721 File Offset: 0x0007A921
		protected virtual void ToggleIsRecording(bool isRecording)
		{
			this.voiceConnection.PrimaryRecorder.RecordingEnabled = isRecording;
		}

		// Token: 0x0600188C RID: 6284 RVA: 0x0007C734 File Offset: 0x0007A934
		private void ToggleDebugEcho(bool isOn)
		{
			this.voiceConnection.PrimaryRecorder.DebugEchoMode = isOn;
		}

		// Token: 0x0600188D RID: 6285 RVA: 0x0007C747 File Offset: 0x0007A947
		private void ToggleReliable(bool isOn)
		{
			this.voiceConnection.PrimaryRecorder.ReliableMode = isOn;
		}

		// Token: 0x0600188E RID: 6286 RVA: 0x0007C75A File Offset: 0x0007A95A
		private void ToggleEncryption(bool isOn)
		{
			this.voiceConnection.PrimaryRecorder.Encrypt = isOn;
		}

		// Token: 0x0600188F RID: 6287 RVA: 0x0007C76D File Offset: 0x0007A96D
		private void ToggleAEC(bool isOn)
		{
			this.voiceAudioPreprocessor.AEC = isOn;
			this.aecOptionsGameObject.SetActive(isOn);
			this.voiceConnection.Client.LocalPlayer.SetAEC(isOn);
		}

		// Token: 0x06001890 RID: 6288 RVA: 0x0007C79E File Offset: 0x0007A99E
		private void ToggleNoiseSuppression(bool isOn)
		{
			this.voiceAudioPreprocessor.NoiseSuppression = isOn;
		}

		// Token: 0x06001891 RID: 6289 RVA: 0x0007C7AC File Offset: 0x0007A9AC
		private void ToggleAGC(bool isOn)
		{
			this.voiceAudioPreprocessor.AGC = isOn;
			this.compressionGainGameObject.SetActive(isOn);
			this.targetLevelGameObject.SetActive(isOn);
			this.voiceConnection.Client.LocalPlayer.SetAGC(isOn, this.voiceAudioPreprocessor.AgcCompressionGain, this.voiceAudioPreprocessor.AgcTargetLevel);
		}

		// Token: 0x06001892 RID: 6290 RVA: 0x0007C80A File Offset: 0x0007AA0A
		private void ToggleVAD(bool isOn)
		{
			this.voiceAudioPreprocessor.VAD = isOn;
			this.voiceConnection.Client.LocalPlayer.SetWebRTCVAD(isOn);
		}

		// Token: 0x06001893 RID: 6291 RVA: 0x0007C82F File Offset: 0x0007AA2F
		private void ToggleHighPass(bool isOn)
		{
			this.voiceAudioPreprocessor.HighPass = isOn;
		}

		// Token: 0x06001894 RID: 6292 RVA: 0x0007C840 File Offset: 0x0007AA40
		private void ToggleDsp(bool isOn)
		{
			this.voiceAudioPreprocessor.enabled = isOn;
			this.voiceConnection.PrimaryRecorder.RestartRecording();
			this.webRtcDspGameObject.SetActive(isOn);
			this.voiceConnection.Client.LocalPlayer.SetWebRTCVAD(this.voiceAudioPreprocessor.VAD);
			this.voiceConnection.Client.LocalPlayer.SetAEC(this.voiceAudioPreprocessor.AEC);
			this.voiceConnection.Client.LocalPlayer.SetAGC(this.voiceAudioPreprocessor.AGC, this.voiceAudioPreprocessor.AgcCompressionGain, this.voiceAudioPreprocessor.AgcTargetLevel);
		}

		// Token: 0x06001895 RID: 6293 RVA: 0x0007C8EF File Offset: 0x0007AAEF
		private void ToggleAudioClipStreaming(bool isOn)
		{
			if (isOn)
			{
				this.audioToneToggle.SetValue(false);
				this.voiceConnection.PrimaryRecorder.SourceType = Recorder.InputSourceType.AudioClip;
				return;
			}
			if (!this.audioToneToggle.isOn)
			{
				this.microphoneSelector.SwitchToSelectedMic();
			}
		}

		// Token: 0x06001896 RID: 6294 RVA: 0x0007C92C File Offset: 0x0007AB2C
		private void ToggleAudioToneFactory(bool isOn)
		{
			if (isOn)
			{
				this.streamAudioClipToggle.SetValue(false);
				this.voiceConnection.PrimaryRecorder.SourceType = Recorder.InputSourceType.Factory;
				this.voiceConnection.PrimaryRecorder.InputFactory = this.toneInputFactory;
				return;
			}
			if (!this.streamAudioClipToggle.isOn)
			{
				this.microphoneSelector.SwitchToSelectedMic();
			}
		}

		// Token: 0x06001897 RID: 6295 RVA: 0x0007C988 File Offset: 0x0007AB88
		private void TogglePhotonVAD(bool isOn)
		{
			this.voiceConnection.PrimaryRecorder.VoiceDetection = isOn;
			this.voiceConnection.Client.LocalPlayer.SetPhotonVAD(isOn);
		}

		// Token: 0x06001898 RID: 6296 RVA: 0x0007C9B2 File Offset: 0x0007ABB2
		private void ToggleAecHighPass(bool isOn)
		{
			this.voiceAudioPreprocessor.AecHighPass = isOn;
			this.voiceConnection.Client.LocalPlayer.SetAEC(isOn);
		}

		// Token: 0x06001899 RID: 6297 RVA: 0x0007C9D8 File Offset: 0x0007ABD8
		private void OnAgcCompressionGainChanged(float agcCompressionGain)
		{
			this.voiceAudioPreprocessor.AgcCompressionGain = (int)agcCompressionGain;
			this.compressionGainText.text = "Compression Gain: " + agcCompressionGain;
			this.voiceConnection.Client.LocalPlayer.SetAGC(this.voiceAudioPreprocessor.AGC, (int)agcCompressionGain, this.voiceAudioPreprocessor.AgcTargetLevel);
		}

		// Token: 0x0600189A RID: 6298 RVA: 0x0007CA3C File Offset: 0x0007AC3C
		private void OnAgcTargetLevelChanged(float agcTargetLevel)
		{
			this.voiceAudioPreprocessor.AgcTargetLevel = (int)agcTargetLevel;
			this.targetLevelText.text = "Target Level: " + agcTargetLevel;
			this.voiceConnection.Client.LocalPlayer.SetAGC(this.voiceAudioPreprocessor.AGC, this.voiceAudioPreprocessor.AgcCompressionGain, (int)agcTargetLevel);
		}

		// Token: 0x0600189B RID: 6299 RVA: 0x0007CAA0 File Offset: 0x0007ACA0
		private void OnReverseStreamDelayChanged(string newReverseStreamString)
		{
			int num;
			if (int.TryParse(newReverseStreamString, out num) && num > 0)
			{
				this.voiceAudioPreprocessor.ReverseStreamDelayMs = num;
				return;
			}
			this.reverseStreamDelayInputField.text = this.voiceAudioPreprocessor.ReverseStreamDelayMs.ToString();
		}

		// Token: 0x0600189C RID: 6300 RVA: 0x0007CAE6 File Offset: 0x0007ACE6
		private void OnMicrophoneChanged(Recorder.MicType micType, DeviceInfo deviceInfo)
		{
			this.voiceConnection.Client.LocalPlayer.SetMic(micType);
			this.androidMicSettingGameObject.SetActive(micType == Recorder.MicType.Photon);
		}

		// Token: 0x0600189D RID: 6301 RVA: 0x0007CB0E File Offset: 0x0007AD0E
		private void OnAndroidMicSettingsChanged(bool isOn)
		{
			this.voiceConnection.PrimaryRecorder.SetAndroidNativeMicrophoneSettings(this.androidAecToggle.isOn, this.androidAgcToggle.isOn, this.androidNsToggle.isOn);
		}

		// Token: 0x0600189E RID: 6302 RVA: 0x0007CB42 File Offset: 0x0007AD42
		private void UpdateSyncedNickname(string nickname)
		{
			nickname = nickname.Trim();
			this.voiceConnection.Client.LocalPlayer.NickName = nickname;
			PlayerPrefs.SetString("vNick", nickname);
		}

		// Token: 0x0600189F RID: 6303 RVA: 0x0007CB70 File Offset: 0x0007AD70
		private void JoinOrCreateRoom(string roomName)
		{
			if (string.IsNullOrEmpty(roomName))
			{
				this.connectAndJoin.RoomName = string.Empty;
				this.connectAndJoin.RandomRoom = true;
			}
			else
			{
				this.connectAndJoin.RoomName = roomName.Trim();
				this.connectAndJoin.RandomRoom = false;
			}
			if (this.voiceConnection.Client.InRoom)
			{
				this.voiceConnection.Client.OpLeaveRoom(false, false);
				return;
			}
			if (!this.voiceConnection.Client.IsConnected)
			{
				this.voiceConnection.ConnectUsingSettings(null);
			}
		}

		// Token: 0x060018A0 RID: 6304 RVA: 0x0007CC05 File Offset: 0x0007AE05
		private void PhotonVoiceCreated(PhotonVoiceCreatedParams p)
		{
			this.InitUiValues();
		}

		// Token: 0x060018A1 RID: 6305 RVA: 0x0007CC10 File Offset: 0x0007AE10
		protected virtual void Update()
		{
			this.connectionStatusText.text = this.voiceConnection.Client.State.ToString();
			this.serverStatusText.text = string.Format("{0}/{1}", this.voiceConnection.Client.CloudRegion, this.voiceConnection.Client.CurrentServerAddress);
			if (this.voiceConnection.PrimaryRecorder.IsCurrentlyTransmitting)
			{
				float num = this.voiceConnection.PrimaryRecorder.LevelMeter.CurrentAvgAmp;
				if (num > 1f)
				{
					num /= 32768f;
				}
				if ((double)num > 0.1)
				{
					this.inputWarningText.text = "Input too loud!";
					this.inputWarningText.color = this.warningColor;
				}
				else
				{
					this.inputWarningText.text = string.Empty;
					this.ResetTextColor(this.inputWarningText);
				}
			}
			if (this.voiceConnection.FramesReceivedPerSecond > 0f)
			{
				this.packetLossWarningText.text = string.Format("{0:0.##}% Packet Loss", this.voiceConnection.FramesLostPercent);
				this.packetLossWarningText.color = ((this.voiceConnection.FramesLostPercent > 1f) ? this.warningColor : this.okColor);
			}
			else
			{
				this.packetLossWarningText.text = string.Empty;
				this.ResetTextColor(this.packetLossWarningText);
			}
			this.rttText.text = "RTT:" + this.voiceConnection.Client.LoadBalancingPeer.RoundTripTime;
			this.SetTextColor(this.voiceConnection.Client.LoadBalancingPeer.RoundTripTime, this.rttText, this.rttYellowThreshold, this.rttRedThreshold);
			this.rttVariationText.text = "VAR:" + this.voiceConnection.Client.LoadBalancingPeer.RoundTripTimeVariance;
			this.SetTextColor(this.voiceConnection.Client.LoadBalancingPeer.RoundTripTimeVariance, this.rttVariationText, this.rttVariationYellowThreshold, this.rttVariationRedThreshold);
		}

		// Token: 0x060018A2 RID: 6306 RVA: 0x0007CE37 File Offset: 0x0007B037
		private void SetTextColor(int textValue, Text text, int yellowThreshold, int redThreshold)
		{
			if (textValue > redThreshold)
			{
				text.color = this.redColor;
				return;
			}
			if (textValue > yellowThreshold)
			{
				text.color = this.warningColor;
				return;
			}
			text.color = this.okColor;
		}

		// Token: 0x060018A3 RID: 6307 RVA: 0x0007CE68 File Offset: 0x0007B068
		private void ResetTextColor(Text text)
		{
			text.color = this.defaultColor;
		}

		// Token: 0x060018A4 RID: 6308 RVA: 0x0007CE78 File Offset: 0x0007B078
		private void InitUiCallbacks()
		{
			this.muteToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleMute));
			this.debugEchoToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleDebugEcho));
			this.reliableTransmissionToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleReliable));
			this.encryptionToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleEncryption));
			this.streamAudioClipToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleAudioClipStreaming));
			this.audioToneToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleAudioToneFactory));
			this.photonVadToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.TogglePhotonVAD));
			this.vadToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleVAD));
			this.aecToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleAEC));
			this.agcToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleAGC));
			this.dspToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleDsp));
			this.highPassToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleHighPass));
			this.aecHighPassToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleAecHighPass));
			this.noiseSuppressionToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleNoiseSuppression));
			this.agcCompressionGainSlider.SetSingleOnValueChangedCallback(new UnityAction<float>(this.OnAgcCompressionGainChanged));
			this.agcTargetLevelSlider.SetSingleOnValueChangedCallback(new UnityAction<float>(this.OnAgcTargetLevelChanged));
			this.localNicknameText.SetSingleOnEndEditCallback(new UnityAction<string>(this.UpdateSyncedNickname));
			this.roomNameInputField.SetSingleOnEndEditCallback(new UnityAction<string>(this.JoinOrCreateRoom));
			this.reverseStreamDelayInputField.SetSingleOnEndEditCallback(new UnityAction<string>(this.OnReverseStreamDelayChanged));
			this.androidAgcToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.OnAndroidMicSettingsChanged));
			this.androidAecToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.OnAndroidMicSettingsChanged));
			this.androidNsToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.OnAndroidMicSettingsChanged));
		}

		// Token: 0x060018A5 RID: 6309 RVA: 0x0007D080 File Offset: 0x0007B280
		private void InitUiValues()
		{
			this.muteToggle.SetValue(this.voiceConnection.Client.LocalPlayer.IsMuted());
			this.debugEchoToggle.SetValue(this.voiceConnection.PrimaryRecorder.DebugEchoMode);
			this.reliableTransmissionToggle.SetValue(this.voiceConnection.PrimaryRecorder.ReliableMode);
			this.encryptionToggle.SetValue(this.voiceConnection.PrimaryRecorder.Encrypt);
			this.streamAudioClipToggle.SetValue(this.voiceConnection.PrimaryRecorder.SourceType == Recorder.InputSourceType.AudioClip);
			this.audioToneToggle.SetValue(this.voiceConnection.PrimaryRecorder.SourceType == Recorder.InputSourceType.Factory && this.voiceConnection.PrimaryRecorder.InputFactory == this.toneInputFactory);
			this.photonVadToggle.SetValue(this.voiceConnection.PrimaryRecorder.VoiceDetection);
			this.androidAgcToggle.SetValue(this.voiceConnection.PrimaryRecorder.AndroidMicrophoneAGC);
			this.androidAecToggle.SetValue(this.voiceConnection.PrimaryRecorder.AndroidMicrophoneAEC);
			this.androidNsToggle.SetValue(this.voiceConnection.PrimaryRecorder.AndroidMicrophoneNS);
			if (this.webRtcDspGameObject != null)
			{
				this.dspToggle.gameObject.SetActive(true);
				this.dspToggle.SetValue(this.voiceAudioPreprocessor.enabled);
				this.webRtcDspGameObject.SetActive(this.dspToggle.isOn);
				this.aecToggle.SetValue(this.voiceAudioPreprocessor.AEC);
				this.aecHighPassToggle.SetValue(this.voiceAudioPreprocessor.AecHighPass);
				this.reverseStreamDelayInputField.text = this.voiceAudioPreprocessor.ReverseStreamDelayMs.ToString();
				this.aecOptionsGameObject.SetActive(this.voiceAudioPreprocessor.AEC);
				this.noiseSuppressionToggle.isOn = this.voiceAudioPreprocessor.NoiseSuppression;
				this.agcToggle.SetValue(this.voiceAudioPreprocessor.AGC);
				this.agcCompressionGainSlider.SetValue((float)this.voiceAudioPreprocessor.AgcCompressionGain);
				this.agcTargetLevelSlider.SetValue((float)this.voiceAudioPreprocessor.AgcTargetLevel);
				this.compressionGainGameObject.SetActive(this.voiceAudioPreprocessor.AGC);
				this.targetLevelGameObject.SetActive(this.voiceAudioPreprocessor.AGC);
				this.vadToggle.SetValue(this.voiceAudioPreprocessor.VAD);
				this.highPassToggle.SetValue(this.voiceAudioPreprocessor.HighPass);
				return;
			}
			this.dspToggle.gameObject.SetActive(false);
		}

		// Token: 0x060018A6 RID: 6310 RVA: 0x0007D338 File Offset: 0x0007B538
		private void SetRoomDebugText()
		{
			string text = string.Empty;
			if (this.voiceConnection.Client.InRoom)
			{
				foreach (Photon.Realtime.Player player in this.voiceConnection.Client.CurrentRoom.Players.Values)
				{
					text += player.ToStringFull();
				}
				this.roomStatusText.text = string.Format("{0} {1}", this.voiceConnection.Client.CurrentRoom.Name, text);
			}
			else
			{
				this.roomStatusText.text = string.Empty;
			}
			this.roomStatusText.text = ((this.voiceConnection.Client.CurrentRoom == null) ? string.Empty : string.Format("{0} {1}", this.voiceConnection.Client.CurrentRoom.Name, text));
		}

		// Token: 0x060018A7 RID: 6311 RVA: 0x0007D440 File Offset: 0x0007B640
		protected virtual void OnActorPropertiesChanged(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
		{
			if (targetPlayer.IsLocal)
			{
				bool flag = targetPlayer.IsMuted();
				this.voiceConnection.PrimaryRecorder.TransmitEnabled = !flag;
				this.muteToggle.SetValue(flag);
			}
			this.SetRoomDebugText();
		}

		// Token: 0x060018A8 RID: 6312 RVA: 0x0007D482 File Offset: 0x0007B682
		protected void OnApplicationQuit()
		{
			this.voiceConnection.Client.RemoveCallbackTarget(this);
		}

		// Token: 0x060018A9 RID: 6313 RVA: 0x0007D495 File Offset: 0x0007B695
		void IInRoomCallbacks.OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
		{
			this.SetRoomDebugText();
		}

		// Token: 0x060018AA RID: 6314 RVA: 0x0007D49D File Offset: 0x0007B69D
		void IInRoomCallbacks.OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
		{
			this.SetRoomDebugText();
		}

		// Token: 0x060018AB RID: 6315 RVA: 0x0007D4A5 File Offset: 0x0007B6A5
		void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
		}

		// Token: 0x060018AC RID: 6316 RVA: 0x0007D4A7 File Offset: 0x0007B6A7
		void IInRoomCallbacks.OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
		{
			this.OnActorPropertiesChanged(targetPlayer, changedProps);
		}

		// Token: 0x060018AD RID: 6317 RVA: 0x0007D4B1 File Offset: 0x0007B6B1
		void IInRoomCallbacks.OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
		{
		}

		// Token: 0x060018AE RID: 6318 RVA: 0x0007D4B3 File Offset: 0x0007B6B3
		void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList)
		{
		}

		// Token: 0x060018AF RID: 6319 RVA: 0x0007D4B5 File Offset: 0x0007B6B5
		void IMatchmakingCallbacks.OnCreatedRoom()
		{
		}

		// Token: 0x060018B0 RID: 6320 RVA: 0x0007D4B7 File Offset: 0x0007B6B7
		void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
		{
		}

		// Token: 0x060018B1 RID: 6321 RVA: 0x0007D4B9 File Offset: 0x0007B6B9
		void IMatchmakingCallbacks.OnJoinedRoom()
		{
			this.SetRoomDebugText();
			this.voiceConnection.Client.LocalPlayer.SetMic(this.voiceConnection.PrimaryRecorder.MicrophoneType);
		}

		// Token: 0x060018B2 RID: 6322 RVA: 0x0007D4E7 File Offset: 0x0007B6E7
		void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
		{
		}

		// Token: 0x060018B3 RID: 6323 RVA: 0x0007D4E9 File Offset: 0x0007B6E9
		void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
		{
		}

		// Token: 0x060018B4 RID: 6324 RVA: 0x0007D4EB File Offset: 0x0007B6EB
		void IMatchmakingCallbacks.OnLeftRoom()
		{
			this.SetRoomDebugText();
			this.SetDefaults();
		}

		// Token: 0x0400167B RID: 5755
		[SerializeField]
		private Text connectionStatusText;

		// Token: 0x0400167C RID: 5756
		[SerializeField]
		private Text serverStatusText;

		// Token: 0x0400167D RID: 5757
		[SerializeField]
		private Text roomStatusText;

		// Token: 0x0400167E RID: 5758
		[SerializeField]
		private Text inputWarningText;

		// Token: 0x0400167F RID: 5759
		[SerializeField]
		private Text rttText;

		// Token: 0x04001680 RID: 5760
		[SerializeField]
		private Text rttVariationText;

		// Token: 0x04001681 RID: 5761
		[SerializeField]
		private Text packetLossWarningText;

		// Token: 0x04001682 RID: 5762
		[SerializeField]
		private InputField localNicknameText;

		// Token: 0x04001683 RID: 5763
		[SerializeField]
		private Toggle debugEchoToggle;

		// Token: 0x04001684 RID: 5764
		[SerializeField]
		private Toggle reliableTransmissionToggle;

		// Token: 0x04001685 RID: 5765
		[SerializeField]
		private Toggle encryptionToggle;

		// Token: 0x04001686 RID: 5766
		[SerializeField]
		private GameObject webRtcDspGameObject;

		// Token: 0x04001687 RID: 5767
		[SerializeField]
		private Toggle aecToggle;

		// Token: 0x04001688 RID: 5768
		[SerializeField]
		private Toggle aecHighPassToggle;

		// Token: 0x04001689 RID: 5769
		[SerializeField]
		private InputField reverseStreamDelayInputField;

		// Token: 0x0400168A RID: 5770
		[SerializeField]
		private Toggle noiseSuppressionToggle;

		// Token: 0x0400168B RID: 5771
		[SerializeField]
		private Toggle agcToggle;

		// Token: 0x0400168C RID: 5772
		[SerializeField]
		private Slider agcCompressionGainSlider;

		// Token: 0x0400168D RID: 5773
		[SerializeField]
		private Slider agcTargetLevelSlider;

		// Token: 0x0400168E RID: 5774
		[SerializeField]
		private Toggle vadToggle;

		// Token: 0x0400168F RID: 5775
		[SerializeField]
		private Toggle muteToggle;

		// Token: 0x04001690 RID: 5776
		[SerializeField]
		private Toggle streamAudioClipToggle;

		// Token: 0x04001691 RID: 5777
		[SerializeField]
		private Toggle audioToneToggle;

		// Token: 0x04001692 RID: 5778
		[SerializeField]
		private Toggle dspToggle;

		// Token: 0x04001693 RID: 5779
		[SerializeField]
		private Toggle highPassToggle;

		// Token: 0x04001694 RID: 5780
		[SerializeField]
		private Toggle photonVadToggle;

		// Token: 0x04001695 RID: 5781
		[SerializeField]
		private MicrophoneSelector microphoneSelector;

		// Token: 0x04001696 RID: 5782
		[SerializeField]
		private GameObject androidMicSettingGameObject;

		// Token: 0x04001697 RID: 5783
		[SerializeField]
		private Toggle androidAgcToggle;

		// Token: 0x04001698 RID: 5784
		[SerializeField]
		private Toggle androidAecToggle;

		// Token: 0x04001699 RID: 5785
		[SerializeField]
		private Toggle androidNsToggle;

		// Token: 0x0400169A RID: 5786
		[SerializeField]
		private bool defaultTransmitEnabled;

		// Token: 0x0400169B RID: 5787
		[SerializeField]
		private bool fullScreen;

		// Token: 0x0400169C RID: 5788
		[SerializeField]
		private InputField roomNameInputField;

		// Token: 0x0400169D RID: 5789
		[SerializeField]
		private int rttYellowThreshold = 100;

		// Token: 0x0400169E RID: 5790
		[SerializeField]
		private int rttRedThreshold = 160;

		// Token: 0x0400169F RID: 5791
		[SerializeField]
		private int rttVariationYellowThreshold = 25;

		// Token: 0x040016A0 RID: 5792
		[SerializeField]
		private int rttVariationRedThreshold = 50;

		// Token: 0x040016A1 RID: 5793
		private GameObject compressionGainGameObject;

		// Token: 0x040016A2 RID: 5794
		private GameObject targetLevelGameObject;

		// Token: 0x040016A3 RID: 5795
		private Text compressionGainText;

		// Token: 0x040016A4 RID: 5796
		private Text targetLevelText;

		// Token: 0x040016A5 RID: 5797
		private GameObject aecOptionsGameObject;

		// Token: 0x040016A6 RID: 5798
		public Transform RemoteVoicesPanel;

		// Token: 0x040016A7 RID: 5799
		protected UnityVoiceClient voiceConnection;

		// Token: 0x040016A8 RID: 5800
		private WebRtcAudioDsp voiceAudioPreprocessor;

		// Token: 0x040016A9 RID: 5801
		private ConnectAndJoin connectAndJoin;

		// Token: 0x040016AA RID: 5802
		private readonly Color warningColor = new Color(0.9f, 0.5f, 0f, 1f);

		// Token: 0x040016AB RID: 5803
		private readonly Color okColor = new Color(0f, 0.6f, 0.2f, 1f);

		// Token: 0x040016AC RID: 5804
		private readonly Color redColor = new Color(1f, 0f, 0f, 1f);

		// Token: 0x040016AD RID: 5805
		private readonly Color defaultColor = new Color(0f, 0f, 0f, 1f);

		// Token: 0x040016AE RID: 5806
		private Func<IAudioDesc> toneInputFactory = () => new AudioUtil.ToneAudioReader<float>(null, 440.0, 48000, 2);
	}
}

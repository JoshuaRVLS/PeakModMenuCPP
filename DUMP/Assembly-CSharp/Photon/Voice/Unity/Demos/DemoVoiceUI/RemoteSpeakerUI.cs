using System;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Photon.Voice.Unity.Demos.DemoVoiceUI
{
	// Token: 0x020003A4 RID: 932
	public class RemoteSpeakerUI : MonoBehaviour, IInRoomCallbacks
	{
		// Token: 0x17000179 RID: 377
		// (get) Token: 0x060018D7 RID: 6359 RVA: 0x0007DCF0 File Offset: 0x0007BEF0
		protected Photon.Realtime.Player Actor
		{
			get
			{
				if (this.loadBalancingClient == null || this.loadBalancingClient.CurrentRoom == null)
				{
					return null;
				}
				return this.loadBalancingClient.CurrentRoom.GetPlayer(this.speaker.RemoteVoice.PlayerId, false);
			}
		}

		// Token: 0x060018D8 RID: 6360 RVA: 0x0007DD2C File Offset: 0x0007BF2C
		protected virtual void Start()
		{
			this.speaker = base.GetComponent<Speaker>();
			this.audioSource = base.GetComponent<AudioSource>();
			this.playDelayInputField.text = this.speaker.PlayDelay.ToString();
			this.playDelayInputField.SetSingleOnEndEditCallback(new UnityAction<string>(this.OnPlayDelayChanged));
			this.SetNickname();
			this.SetMutedState();
			this.SetProperties();
			this.volumeSlider.minValue = 0f;
			this.volumeSlider.maxValue = 1f;
			this.volumeSlider.SetSingleOnValueChangedCallback(new UnityAction<float>(this.OnVolumeChanged));
			this.volumeSlider.value = 1f;
			this.OnVolumeChanged(1f);
		}

		// Token: 0x060018D9 RID: 6361 RVA: 0x0007DDEA File Offset: 0x0007BFEA
		private void OnVolumeChanged(float newValue)
		{
			this.audioSource.volume = newValue;
		}

		// Token: 0x060018DA RID: 6362 RVA: 0x0007DDF8 File Offset: 0x0007BFF8
		private void OnPlayDelayChanged(string str)
		{
			int playDelay;
			if (int.TryParse(str, out playDelay))
			{
				this.speaker.PlayDelay = playDelay;
				return;
			}
			Debug.LogErrorFormat("Failed to parse {0}", new object[]
			{
				str
			});
		}

		// Token: 0x060018DB RID: 6363 RVA: 0x0007DE30 File Offset: 0x0007C030
		private void Update()
		{
			this.remoteIsTalking.enabled = this.speaker.IsPlaying;
			if (this.speaker.IsPlaying)
			{
				int lag = this.speaker.Lag;
				this.smoothedLag = (lag + this.smoothedLag * 99) / 100;
				this.bufferLagText.text = string.Concat(new object[]
				{
					"Buffer Lag: ",
					this.smoothedLag,
					"/",
					lag
				});
				return;
			}
			this.bufferLagText.text = "Buffer Lag: " + this.smoothedLag + "/-";
		}

		// Token: 0x060018DC RID: 6364 RVA: 0x0007DEE3 File Offset: 0x0007C0E3
		private void OnDestroy()
		{
			if (this.loadBalancingClient != null)
			{
				this.loadBalancingClient.RemoveCallbackTarget(this);
			}
		}

		// Token: 0x060018DD RID: 6365 RVA: 0x0007DEFC File Offset: 0x0007C0FC
		private void SetNickname()
		{
			string text = this.speaker.name;
			if (this.Actor != null)
			{
				text = this.Actor.NickName;
				if (string.IsNullOrEmpty(text))
				{
					text = "user " + this.Actor.ActorNumber;
				}
			}
			this.nameText.text = text;
		}

		// Token: 0x060018DE RID: 6366 RVA: 0x0007DF58 File Offset: 0x0007C158
		private void SetMutedState()
		{
			this.SetMutedState(this.Actor.IsMuted());
		}

		// Token: 0x060018DF RID: 6367 RVA: 0x0007DF6C File Offset: 0x0007C16C
		private void SetProperties()
		{
			this.photonVad.enabled = this.Actor.HasPhotonVAD();
			this.webrtcVad.enabled = this.Actor.HasWebRTCVAD();
			this.aec.enabled = this.Actor.HasAEC();
			this.agc.enabled = this.Actor.HasAGC();
			this.agc.text = "AGC Gain: " + this.Actor.GetAGCGain().ToString() + " Level: " + this.Actor.GetAGCLevel().ToString();
			Recorder.MicType? micType = this.Actor.GetMic();
			this.mic.enabled = (micType != null);
			Text text = this.mic;
			string text2;
			if (micType == null)
			{
				text2 = "";
			}
			else
			{
				Recorder.MicType? micType2 = micType;
				Recorder.MicType micType3 = Recorder.MicType.Unity;
				text2 = ((micType2.GetValueOrDefault() == micType3 & micType2 != null) ? "Unity MIC" : "Photon MIC");
			}
			text.text = text2;
		}

		// Token: 0x060018E0 RID: 6368 RVA: 0x0007E06E File Offset: 0x0007C26E
		protected virtual void SetMutedState(bool isMuted)
		{
			this.remoteIsMuting.enabled = isMuted;
		}

		// Token: 0x060018E1 RID: 6369 RVA: 0x0007E07C File Offset: 0x0007C27C
		protected virtual void OnActorPropertiesChanged(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
		{
			if (this.speaker != null && this.speaker.RemoteVoice != null && targetPlayer.ActorNumber == this.speaker.RemoteVoice.PlayerId)
			{
				this.SetMutedState();
				this.SetNickname();
				this.SetProperties();
			}
		}

		// Token: 0x060018E2 RID: 6370 RVA: 0x0007E0CE File Offset: 0x0007C2CE
		public virtual void Init(VoiceConnection vC)
		{
			this.voiceConnection = vC;
			this.loadBalancingClient = this.voiceConnection.Client;
			this.loadBalancingClient.AddCallbackTarget(this);
		}

		// Token: 0x060018E3 RID: 6371 RVA: 0x0007E0F4 File Offset: 0x0007C2F4
		void IInRoomCallbacks.OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
		{
		}

		// Token: 0x060018E4 RID: 6372 RVA: 0x0007E0F6 File Offset: 0x0007C2F6
		void IInRoomCallbacks.OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
		{
		}

		// Token: 0x060018E5 RID: 6373 RVA: 0x0007E0F8 File Offset: 0x0007C2F8
		void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
		}

		// Token: 0x060018E6 RID: 6374 RVA: 0x0007E0FA File Offset: 0x0007C2FA
		void IInRoomCallbacks.OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
		{
			this.OnActorPropertiesChanged(targetPlayer, changedProps);
		}

		// Token: 0x060018E7 RID: 6375 RVA: 0x0007E104 File Offset: 0x0007C304
		void IInRoomCallbacks.OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
		{
		}

		// Token: 0x040016C6 RID: 5830
		[SerializeField]
		private Text nameText;

		// Token: 0x040016C7 RID: 5831
		[SerializeField]
		protected Image remoteIsMuting;

		// Token: 0x040016C8 RID: 5832
		[SerializeField]
		private Image remoteIsTalking;

		// Token: 0x040016C9 RID: 5833
		[SerializeField]
		private InputField playDelayInputField;

		// Token: 0x040016CA RID: 5834
		[SerializeField]
		private Text bufferLagText;

		// Token: 0x040016CB RID: 5835
		[SerializeField]
		private Slider volumeSlider;

		// Token: 0x040016CC RID: 5836
		[SerializeField]
		private Text photonVad;

		// Token: 0x040016CD RID: 5837
		[SerializeField]
		private Text webrtcVad;

		// Token: 0x040016CE RID: 5838
		[SerializeField]
		private Text aec;

		// Token: 0x040016CF RID: 5839
		[SerializeField]
		private Text agc;

		// Token: 0x040016D0 RID: 5840
		[SerializeField]
		private Text mic;

		// Token: 0x040016D1 RID: 5841
		protected Speaker speaker;

		// Token: 0x040016D2 RID: 5842
		private AudioSource audioSource;

		// Token: 0x040016D3 RID: 5843
		protected VoiceConnection voiceConnection;

		// Token: 0x040016D4 RID: 5844
		protected LoadBalancingClient loadBalancingClient;

		// Token: 0x040016D5 RID: 5845
		private int smoothedLag;
	}
}

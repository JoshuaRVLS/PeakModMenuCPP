using System;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ExitGames.Demos.DemoPunVoice
{
	// Token: 0x0200039A RID: 922
	public class VoiceDemoUI : MonoBehaviour
	{
		// Token: 0x17000178 RID: 376
		// (get) Token: 0x0600184E RID: 6222 RVA: 0x0007B325 File Offset: 0x00079525
		// (set) Token: 0x0600184F RID: 6223 RVA: 0x0007B330 File Offset: 0x00079530
		public bool DebugMode
		{
			get
			{
				return this.debugMode;
			}
			set
			{
				this.debugMode = value;
				this.debugGO.SetActive(this.debugMode);
				this.voiceDebugText.text = string.Empty;
				if (this.debugMode)
				{
					this.previousDebugLevel = this.punVoiceClient.Client.LoadBalancingPeer.DebugOut;
					this.punVoiceClient.Client.LoadBalancingPeer.DebugOut = DebugLevel.ALL;
				}
				else
				{
					this.punVoiceClient.Client.LoadBalancingPeer.DebugOut = this.previousDebugLevel;
				}
				if (VoiceDemoUI.DebugToggled != null)
				{
					VoiceDemoUI.DebugToggled(this.debugMode);
				}
			}
		}

		// Token: 0x14000009 RID: 9
		// (add) Token: 0x06001850 RID: 6224 RVA: 0x0007B3D4 File Offset: 0x000795D4
		// (remove) Token: 0x06001851 RID: 6225 RVA: 0x0007B408 File Offset: 0x00079608
		public static event VoiceDemoUI.OnDebugToggle DebugToggled;

		// Token: 0x06001852 RID: 6226 RVA: 0x0007B43B File Offset: 0x0007963B
		private void Awake()
		{
			this.punVoiceClient = PunVoiceClient.Instance;
			Debug.LogWarning("VoiceDemoUI selected a punVoiceClient.Instance", this.punVoiceClient);
		}

		// Token: 0x06001853 RID: 6227 RVA: 0x0007B458 File Offset: 0x00079658
		private void OnDestroy()
		{
			ChangePOV.CameraChanged -= this.OnCameraChanged;
			BetterToggle.ToggleValueChanged -= this.BetterToggle_ToggleValueChanged;
			CharacterInstantiation.CharacterInstantiated -= this.CharacterInstantiation_CharacterInstantiated;
			this.punVoiceClient.Client.StateChanged -= this.VoiceClientStateChanged;
			PhotonNetwork.NetworkingClient.StateChanged -= this.PunClientStateChanged;
		}

		// Token: 0x06001854 RID: 6228 RVA: 0x0007B4CC File Offset: 0x000796CC
		private void CharacterInstantiation_CharacterInstantiated(GameObject character)
		{
			PhotonVoiceView component = character.GetComponent<PhotonVoiceView>();
			if (component != null)
			{
				this.recorder = component;
			}
		}

		// Token: 0x06001855 RID: 6229 RVA: 0x0007B4F0 File Offset: 0x000796F0
		private void InitToggles(Toggle[] toggles)
		{
			if (toggles == null)
			{
				return;
			}
			foreach (Toggle toggle in toggles)
			{
				string name = toggle.name;
				if (!(name == "Mute"))
				{
					if (!(name == "VoiceDetection"))
					{
						if (!(name == "DebugVoice"))
						{
							if (!(name == "Transmit"))
							{
								if (!(name == "DebugEcho"))
								{
									if (name == "AutoConnectAndJoin")
									{
										toggle.isOn = this.punVoiceClient.AutoConnectAndJoin;
									}
								}
								else if (this.recorder != null && this.recorder.RecorderInUse != null)
								{
									toggle.isOn = this.recorder.RecorderInUse.DebugEchoMode;
								}
							}
							else if (this.recorder != null && this.recorder.RecorderInUse != null)
							{
								toggle.isOn = this.recorder.RecorderInUse.TransmitEnabled;
							}
						}
						else
						{
							toggle.isOn = this.DebugMode;
						}
					}
					else if (this.recorder != null && this.recorder.RecorderInUse != null)
					{
						toggle.isOn = this.recorder.RecorderInUse.VoiceDetection;
					}
				}
				else
				{
					toggle.isOn = (AudioListener.volume <= 0.001f);
				}
			}
		}

		// Token: 0x06001856 RID: 6230 RVA: 0x0007B670 File Offset: 0x00079870
		private void BetterToggle_ToggleValueChanged(Toggle toggle)
		{
			string name = toggle.name;
			if (!(name == "Mute"))
			{
				if (!(name == "Transmit"))
				{
					if (!(name == "VoiceDetection"))
					{
						if (!(name == "DebugEcho"))
						{
							if (name == "DebugVoice")
							{
								this.DebugMode = toggle.isOn;
								return;
							}
							if (!(name == "AutoConnectAndJoin"))
							{
								return;
							}
							this.punVoiceClient.AutoConnectAndJoin = toggle.isOn;
						}
						else if (this.recorder.RecorderInUse)
						{
							this.recorder.RecorderInUse.DebugEchoMode = toggle.isOn;
							return;
						}
					}
					else if (this.recorder.RecorderInUse)
					{
						this.recorder.RecorderInUse.VoiceDetection = toggle.isOn;
						return;
					}
				}
				else if (this.recorder.RecorderInUse)
				{
					this.recorder.RecorderInUse.TransmitEnabled = toggle.isOn;
					return;
				}
				return;
			}
			if (toggle.isOn)
			{
				this.volumeBeforeMute = AudioListener.volume;
				AudioListener.volume = 0f;
				return;
			}
			AudioListener.volume = this.volumeBeforeMute;
			this.volumeBeforeMute = 0f;
		}

		// Token: 0x06001857 RID: 6231 RVA: 0x0007B7B0 File Offset: 0x000799B0
		private void OnCameraChanged(Camera newCamera)
		{
			this.canvas.worldCamera = newCamera;
		}

		// Token: 0x06001858 RID: 6232 RVA: 0x0007B7C0 File Offset: 0x000799C0
		private void Start()
		{
			ChangePOV.CameraChanged += this.OnCameraChanged;
			BetterToggle.ToggleValueChanged += this.BetterToggle_ToggleValueChanged;
			CharacterInstantiation.CharacterInstantiated += this.CharacterInstantiation_CharacterInstantiated;
			this.punVoiceClient.Client.StateChanged += this.VoiceClientStateChanged;
			PhotonNetwork.NetworkingClient.StateChanged += this.PunClientStateChanged;
			this.canvas = base.GetComponentInChildren<Canvas>();
			if (this.punSwitch != null)
			{
				this.punSwitchText = this.punSwitch.GetComponentInChildren<Text>();
				this.punSwitch.onClick.AddListener(new UnityAction(this.PunSwitchOnClick));
			}
			if (this.voiceSwitch != null)
			{
				this.voiceSwitchText = this.voiceSwitch.GetComponentInChildren<Text>();
				this.voiceSwitch.onClick.AddListener(new UnityAction(this.VoiceSwitchOnClick));
			}
			if (this.calibrateButton != null)
			{
				this.calibrateButton.onClick.AddListener(new UnityAction(this.CalibrateButtonOnClick));
				this.calibrateText = this.calibrateButton.GetComponentInChildren<Text>();
			}
			if (this.punState != null)
			{
				this.debugGO = this.punState.transform.parent.gameObject;
			}
			this.volumeBeforeMute = AudioListener.volume;
			this.previousDebugLevel = this.punVoiceClient.Client.LoadBalancingPeer.DebugOut;
			if (this.globalSettings != null)
			{
				this.globalSettings.SetActive(true);
				this.InitToggles(this.globalSettings.GetComponentsInChildren<Toggle>());
			}
			if (this.devicesInfoText != null)
			{
				using (AudioInEnumerator audioInEnumerator = new AudioInEnumerator(this.punVoiceClient.Logger))
				{
					using (IDeviceEnumerator deviceEnumerator = Platform.CreateAudioInEnumerator(this.punVoiceClient.Logger))
					{
						if (audioInEnumerator.Count<DeviceInfo>() + deviceEnumerator.Count<DeviceInfo>() == 0)
						{
							this.devicesInfoText.enabled = true;
							this.devicesInfoText.color = Color.red;
							this.devicesInfoText.text = "No microphone device detected!";
						}
						else
						{
							this.devicesInfoText.text = "Mic Unity: " + string.Join(", ", from x in audioInEnumerator
							select x.ToString());
							Text text = this.devicesInfoText;
							text.text = text.text + "\nMic Photon: " + string.Join(", ", from x in deviceEnumerator
							select x.ToString());
						}
					}
				}
			}
			this.VoiceClientStateChanged(ClientState.PeerCreated, this.punVoiceClient.ClientState);
			this.PunClientStateChanged(ClientState.PeerCreated, PhotonNetwork.NetworkingClient.State);
		}

		// Token: 0x06001859 RID: 6233 RVA: 0x0007BAC0 File Offset: 0x00079CC0
		private void PunSwitchOnClick()
		{
			if (PhotonNetwork.NetworkClientState == ClientState.Joined)
			{
				PhotonNetwork.Disconnect();
				return;
			}
			if (PhotonNetwork.NetworkClientState == ClientState.Disconnected || PhotonNetwork.NetworkClientState == ClientState.PeerCreated)
			{
				PhotonNetwork.ConnectUsingSettings();
			}
		}

		// Token: 0x0600185A RID: 6234 RVA: 0x0007BAE8 File Offset: 0x00079CE8
		private void VoiceSwitchOnClick()
		{
			if (this.punVoiceClient.ClientState == ClientState.Joined)
			{
				this.punVoiceClient.Disconnect();
				return;
			}
			if (this.punVoiceClient.ClientState == ClientState.PeerCreated || this.punVoiceClient.ClientState == ClientState.Disconnected)
			{
				this.punVoiceClient.ConnectAndJoinRoom();
			}
		}

		// Token: 0x0600185B RID: 6235 RVA: 0x0007BB38 File Offset: 0x00079D38
		private void CalibrateButtonOnClick()
		{
			if (this.recorder.RecorderInUse && !this.recorder.RecorderInUse.VoiceDetectorCalibrating)
			{
				this.recorder.RecorderInUse.VoiceDetectorCalibrate(this.calibrationMilliSeconds, null);
			}
		}

		// Token: 0x0600185C RID: 6236 RVA: 0x0007BB78 File Offset: 0x00079D78
		private void Update()
		{
			if (this.recorder != null && this.recorder.RecorderInUse != null && this.recorder.RecorderInUse.LevelMeter != null)
			{
				this.voiceDebugText.text = string.Format("Amp: avg. {0:0.000000}, peak {1:0.000000}", this.recorder.RecorderInUse.LevelMeter.CurrentAvgAmp, this.recorder.RecorderInUse.LevelMeter.CurrentPeakAmp);
			}
		}

		// Token: 0x0600185D RID: 6237 RVA: 0x0007BC04 File Offset: 0x00079E04
		private void PunClientStateChanged(ClientState fromState, ClientState toState)
		{
			this.punState.text = string.Format("PUN: {0}", toState);
			if (toState != ClientState.PeerCreated)
			{
				if (toState == ClientState.Joined)
				{
					this.punSwitch.interactable = true;
					this.punSwitchText.text = "PUN Disconnect";
					goto IL_80;
				}
				if (toState != ClientState.Disconnected)
				{
					this.punSwitch.interactable = false;
					this.punSwitchText.text = "PUN busy";
					goto IL_80;
				}
			}
			this.punSwitch.interactable = true;
			this.punSwitchText.text = "PUN Connect";
			IL_80:
			this.UpdateUiBasedOnVoiceState(this.punVoiceClient.ClientState);
		}

		// Token: 0x0600185E RID: 6238 RVA: 0x0007BCA2 File Offset: 0x00079EA2
		private void VoiceClientStateChanged(ClientState fromState, ClientState toState)
		{
			this.UpdateUiBasedOnVoiceState(toState);
		}

		// Token: 0x0600185F RID: 6239 RVA: 0x0007BCAC File Offset: 0x00079EAC
		private void UpdateUiBasedOnVoiceState(ClientState voiceClientState)
		{
			this.voiceState.text = string.Format("PhotonVoice: {0}", voiceClientState);
			if (voiceClientState != ClientState.PeerCreated)
			{
				if (voiceClientState != ClientState.Joined)
				{
					if (voiceClientState != ClientState.Disconnected)
					{
						this.voiceSwitch.interactable = false;
						this.voiceSwitchText.text = "Voice busy";
						return;
					}
				}
				else
				{
					this.voiceSwitch.interactable = true;
					this.inGameSettings.SetActive(true);
					this.voiceSwitchText.text = "Voice Disconnect";
					this.InitToggles(this.inGameSettings.GetComponentsInChildren<Toggle>());
					if (this.recorder != null && this.recorder.RecorderInUse != null)
					{
						this.calibrateButton.interactable = !this.recorder.RecorderInUse.VoiceDetectorCalibrating;
						this.calibrateText.text = (this.recorder.RecorderInUse.VoiceDetectorCalibrating ? "Calibrating" : string.Format("Calibrate ({0}s)", this.calibrationMilliSeconds / 1000));
						return;
					}
					this.calibrateButton.interactable = false;
					this.calibrateText.text = "Unavailable";
					return;
				}
			}
			if (PhotonNetwork.InRoom)
			{
				this.voiceSwitch.interactable = true;
				this.voiceSwitchText.text = "Voice Connect";
				this.voiceDebugText.text = string.Empty;
			}
			else
			{
				this.voiceSwitch.interactable = false;
				this.voiceSwitchText.text = "Voice N/A";
				this.voiceDebugText.text = string.Empty;
			}
			this.calibrateButton.interactable = false;
			this.voiceSwitchText.text = "Voice Connect";
			this.calibrateText.text = "Unavailable";
			this.inGameSettings.SetActive(false);
		}

		// Token: 0x06001860 RID: 6240 RVA: 0x0007BE77 File Offset: 0x0007A077
		protected void OnApplicationQuit()
		{
			this.punVoiceClient.Client.StateChanged -= this.VoiceClientStateChanged;
			PhotonNetwork.NetworkingClient.StateChanged -= this.PunClientStateChanged;
		}

		// Token: 0x04001658 RID: 5720
		[SerializeField]
		private Text punState;

		// Token: 0x04001659 RID: 5721
		[SerializeField]
		private Text voiceState;

		// Token: 0x0400165A RID: 5722
		private PunVoiceClient punVoiceClient;

		// Token: 0x0400165B RID: 5723
		private Canvas canvas;

		// Token: 0x0400165C RID: 5724
		[SerializeField]
		private Button punSwitch;

		// Token: 0x0400165D RID: 5725
		private Text punSwitchText;

		// Token: 0x0400165E RID: 5726
		[SerializeField]
		private Button voiceSwitch;

		// Token: 0x0400165F RID: 5727
		private Text voiceSwitchText;

		// Token: 0x04001660 RID: 5728
		[SerializeField]
		private Button calibrateButton;

		// Token: 0x04001661 RID: 5729
		private Text calibrateText;

		// Token: 0x04001662 RID: 5730
		[SerializeField]
		private Text voiceDebugText;

		// Token: 0x04001663 RID: 5731
		private PhotonVoiceView recorder;

		// Token: 0x04001664 RID: 5732
		[SerializeField]
		private GameObject inGameSettings;

		// Token: 0x04001665 RID: 5733
		[SerializeField]
		private GameObject globalSettings;

		// Token: 0x04001666 RID: 5734
		[SerializeField]
		private Text devicesInfoText;

		// Token: 0x04001667 RID: 5735
		private GameObject debugGO;

		// Token: 0x04001668 RID: 5736
		private bool debugMode;

		// Token: 0x04001669 RID: 5737
		private float volumeBeforeMute;

		// Token: 0x0400166A RID: 5738
		private DebugLevel previousDebugLevel;

		// Token: 0x0400166C RID: 5740
		[SerializeField]
		private int calibrationMilliSeconds = 2000;

		// Token: 0x0200055C RID: 1372
		// (Invoke) Token: 0x06001FAE RID: 8110
		public delegate void OnDebugToggle(bool debugMode);
	}
}

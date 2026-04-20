using System;
using System.Collections.Generic;
using Photon.Voice.Unity.UtilityScripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Photon.Voice.Unity.Demos.DemoVoiceUI
{
	// Token: 0x020003A2 RID: 930
	public class MicrophoneSelector : VoiceComponent
	{
		// Token: 0x060018B8 RID: 6328 RVA: 0x0007D5FC File Offset: 0x0007B7FC
		protected override void Awake()
		{
			base.Awake();
			this.unityMicEnum = new AudioInEnumerator(base.Logger);
			this.photonMicEnum = Platform.CreateAudioInEnumerator(base.Logger);
			this.photonMicEnum.OnReady = delegate()
			{
				this.SetupMicDropdown();
				this.SetCurrentValue();
			};
			this.refreshButton.GetComponentInChildren<Button>().onClick.AddListener(new UnityAction(this.RefreshMicrophones));
			this.fillArea = this.micLevelSlider.fillRect.GetComponent<Image>();
			this.defaultFillColor = this.fillArea.color;
		}

		// Token: 0x060018B9 RID: 6329 RVA: 0x0007D690 File Offset: 0x0007B890
		private void Update()
		{
			if (this.recorder != null)
			{
				this.micLevelSlider.value = this.recorder.LevelMeter.CurrentPeakAmp;
				this.fillArea.color = (this.recorder.IsCurrentlyTransmitting ? this.speakingFillColor : this.defaultFillColor);
			}
		}

		// Token: 0x060018BA RID: 6330 RVA: 0x0007D6EC File Offset: 0x0007B8EC
		private void OnEnable()
		{
			MicrophonePermission.MicrophonePermissionCallback += this.OnMicrophonePermissionCallback;
		}

		// Token: 0x060018BB RID: 6331 RVA: 0x0007D6FF File Offset: 0x0007B8FF
		private void OnMicrophonePermissionCallback(bool granted)
		{
			this.RefreshMicrophones();
		}

		// Token: 0x060018BC RID: 6332 RVA: 0x0007D707 File Offset: 0x0007B907
		private void OnDisable()
		{
			MicrophonePermission.MicrophonePermissionCallback -= this.OnMicrophonePermissionCallback;
		}

		// Token: 0x060018BD RID: 6333 RVA: 0x0007D71C File Offset: 0x0007B91C
		private void SetupMicDropdown()
		{
			this.micDropdown.ClearOptions();
			this.micOptions = new List<MicRef>();
			List<string> list = new List<string>();
			this.micOptions.Add(new MicRef(MicType.Unity, DeviceInfo.Default));
			list.Add(string.Format("[Unity]\u00a0[Default]", Array.Empty<object>()));
			foreach (DeviceInfo deviceInfo in this.unityMicEnum)
			{
				this.micOptions.Add(new MicRef(MicType.Unity, deviceInfo));
				list.Add(string.Format("[Unity]\u00a0{0}", deviceInfo));
			}
			this.micOptions.Add(new MicRef(MicType.Photon, DeviceInfo.Default));
			list.Add(string.Format("[Photon]\u00a0[Default]", Array.Empty<object>()));
			foreach (DeviceInfo deviceInfo2 in this.photonMicEnum)
			{
				this.micOptions.Add(new MicRef(MicType.Photon, deviceInfo2));
				list.Add(string.Format("[Photon]\u00a0{0}", deviceInfo2));
			}
			this.micDropdown.AddOptions(list);
			this.micDropdown.onValueChanged.RemoveAllListeners();
			this.micDropdown.onValueChanged.AddListener(delegate(int x)
			{
				this.SwitchToSelectedMic();
			});
		}

		// Token: 0x060018BE RID: 6334 RVA: 0x0007D894 File Offset: 0x0007BA94
		public void SwitchToSelectedMic()
		{
			MicRef micRef = this.micOptions[this.micDropdown.value];
			MicType micType = micRef.MicType;
			if (micType != MicType.Unity)
			{
				if (micType == MicType.Photon)
				{
					this.recorder.SourceType = Recorder.InputSourceType.Microphone;
					this.recorder.MicrophoneType = Recorder.MicType.Photon;
					this.recorder.MicrophoneDevice = micRef.Device;
				}
			}
			else
			{
				this.recorder.SourceType = Recorder.InputSourceType.Microphone;
				this.recorder.MicrophoneType = Recorder.MicType.Unity;
				this.recorder.MicrophoneDevice = micRef.Device;
			}
			MicrophoneSelector.MicrophoneSelectorEvent microphoneSelectorEvent = this.onValueChanged;
			if (microphoneSelectorEvent == null)
			{
				return;
			}
			microphoneSelectorEvent.Invoke(micRef.MicType, micRef.Device);
		}

		// Token: 0x060018BF RID: 6335 RVA: 0x0007D938 File Offset: 0x0007BB38
		private void SetCurrentValue()
		{
			if (this.micOptions == null)
			{
				Debug.LogWarning("micOptions list is null");
				return;
			}
			this.micDropdown.gameObject.SetActive(true);
			this.refreshButton.SetActive(true);
			for (int i = 0; i < this.micOptions.Count; i++)
			{
				MicRef micRef = this.micOptions[i];
				if ((micRef.MicType == MicType.Unity && this.recorder.SourceType == Recorder.InputSourceType.Microphone && this.recorder.MicrophoneType == Recorder.MicType.Unity) || (micRef.MicType == MicType.Photon && this.recorder.SourceType == Recorder.InputSourceType.Microphone && this.recorder.MicrophoneType == Recorder.MicType.Photon))
				{
					this.micDropdown.value = i;
					return;
				}
			}
		}

		// Token: 0x060018C0 RID: 6336 RVA: 0x0007D9EB File Offset: 0x0007BBEB
		public void RefreshMicrophones()
		{
			this.unityMicEnum.Refresh();
			this.photonMicEnum.Refresh();
		}

		// Token: 0x060018C1 RID: 6337 RVA: 0x0007DA03 File Offset: 0x0007BC03
		private void PhotonVoiceCreated()
		{
			this.RefreshMicrophones();
		}

		// Token: 0x040016B5 RID: 5813
		public MicrophoneSelector.MicrophoneSelectorEvent onValueChanged = new MicrophoneSelector.MicrophoneSelectorEvent();

		// Token: 0x040016B6 RID: 5814
		private List<MicRef> micOptions;

		// Token: 0x040016B7 RID: 5815
		[SerializeField]
		private Dropdown micDropdown;

		// Token: 0x040016B8 RID: 5816
		[SerializeField]
		private Slider micLevelSlider;

		// Token: 0x040016B9 RID: 5817
		[SerializeField]
		private Recorder recorder;

		// Token: 0x040016BA RID: 5818
		[SerializeField]
		[FormerlySerializedAs("RefreshButton")]
		private GameObject refreshButton;

		// Token: 0x040016BB RID: 5819
		private Image fillArea;

		// Token: 0x040016BC RID: 5820
		private Color defaultFillColor = Color.white;

		// Token: 0x040016BD RID: 5821
		private Color speakingFillColor = Color.green;

		// Token: 0x040016BE RID: 5822
		private IDeviceEnumerator unityMicEnum;

		// Token: 0x040016BF RID: 5823
		private IDeviceEnumerator photonMicEnum;

		// Token: 0x0200055F RID: 1375
		public class MicrophoneSelectorEvent : UnityEvent<MicType, DeviceInfo>
		{
		}
	}
}

using System;
using Photon.Voice.PUN;
using UnityEngine;
using UnityEngine.UI;

namespace ExitGames.Demos.DemoPunVoice
{
	// Token: 0x02000397 RID: 919
	[RequireComponent(typeof(Canvas))]
	public class Highlighter : MonoBehaviour
	{
		// Token: 0x0600183F RID: 6207 RVA: 0x0007B051 File Offset: 0x00079251
		private void OnEnable()
		{
			ChangePOV.CameraChanged += this.ChangePOV_CameraChanged;
			VoiceDemoUI.DebugToggled += this.VoiceDemoUI_DebugToggled;
		}

		// Token: 0x06001840 RID: 6208 RVA: 0x0007B075 File Offset: 0x00079275
		private void OnDisable()
		{
			ChangePOV.CameraChanged -= this.ChangePOV_CameraChanged;
			VoiceDemoUI.DebugToggled -= this.VoiceDemoUI_DebugToggled;
		}

		// Token: 0x06001841 RID: 6209 RVA: 0x0007B099 File Offset: 0x00079299
		private void VoiceDemoUI_DebugToggled(bool debugMode)
		{
			this.showSpeakerLag = debugMode;
		}

		// Token: 0x06001842 RID: 6210 RVA: 0x0007B0A2 File Offset: 0x000792A2
		private void ChangePOV_CameraChanged(Camera camera)
		{
			this.canvas.worldCamera = camera;
		}

		// Token: 0x06001843 RID: 6211 RVA: 0x0007B0B0 File Offset: 0x000792B0
		private void Awake()
		{
			this.canvas = base.GetComponent<Canvas>();
			if (this.canvas != null && this.canvas.worldCamera == null)
			{
				this.canvas.worldCamera = Camera.main;
			}
			this.photonVoiceView = base.GetComponentInParent<PhotonVoiceView>();
		}

		// Token: 0x06001844 RID: 6212 RVA: 0x0007B108 File Offset: 0x00079308
		private void Update()
		{
			this.recorderSprite.enabled = this.photonVoiceView.IsRecording;
			this.speakerSprite.enabled = this.photonVoiceView.IsSpeaking;
			this.bufferLagText.enabled = (this.showSpeakerLag && this.photonVoiceView.IsSpeaking);
			if (this.bufferLagText.enabled)
			{
				this.bufferLagText.text = string.Format("{0}", this.photonVoiceView.SpeakerInUse.Lag);
			}
		}

		// Token: 0x06001845 RID: 6213 RVA: 0x0007B19C File Offset: 0x0007939C
		private void LateUpdate()
		{
			if (this.canvas == null || this.canvas.worldCamera == null)
			{
				return;
			}
			base.transform.rotation = Quaternion.Euler(0f, this.canvas.worldCamera.transform.eulerAngles.y, 0f);
		}

		// Token: 0x0400164F RID: 5711
		private Canvas canvas;

		// Token: 0x04001650 RID: 5712
		private PhotonVoiceView photonVoiceView;

		// Token: 0x04001651 RID: 5713
		[SerializeField]
		private Image recorderSprite;

		// Token: 0x04001652 RID: 5714
		[SerializeField]
		private Image speakerSprite;

		// Token: 0x04001653 RID: 5715
		[SerializeField]
		private Text bufferLagText;

		// Token: 0x04001654 RID: 5716
		private bool showSpeakerLag;
	}
}

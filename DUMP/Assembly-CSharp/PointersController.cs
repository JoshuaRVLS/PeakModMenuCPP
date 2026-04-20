using System;
using Photon.Voice.PUN;
using UnityEngine;

// Token: 0x020002CD RID: 717
[RequireComponent(typeof(PhotonVoiceView))]
public class PointersController : MonoBehaviour
{
	// Token: 0x06001427 RID: 5159 RVA: 0x00065E13 File Offset: 0x00064013
	private void Awake()
	{
		this.photonVoiceView = base.GetComponent<PhotonVoiceView>();
		this.SetActiveSafe(this.pointerUp, false);
		this.SetActiveSafe(this.pointerDown, false);
	}

	// Token: 0x06001428 RID: 5160 RVA: 0x00065E3B File Offset: 0x0006403B
	private void Update()
	{
		this.SetActiveSafe(this.pointerDown, this.photonVoiceView.IsSpeaking);
		this.SetActiveSafe(this.pointerUp, this.photonVoiceView.IsRecording);
	}

	// Token: 0x06001429 RID: 5161 RVA: 0x00065E6B File Offset: 0x0006406B
	private void SetActiveSafe(GameObject go, bool active)
	{
		if (go != null && go.activeSelf != active)
		{
			go.SetActive(active);
		}
	}

	// Token: 0x0400125C RID: 4700
	[SerializeField]
	private GameObject pointerDown;

	// Token: 0x0400125D RID: 4701
	[SerializeField]
	private GameObject pointerUp;

	// Token: 0x0400125E RID: 4702
	private PhotonVoiceView photonVoiceView;
}

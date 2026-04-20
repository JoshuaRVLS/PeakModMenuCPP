using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Photon.Voice.Unity.Demos
{
	// Token: 0x0200039B RID: 923
	public class BackgroundMusicController : MonoBehaviour
	{
		// Token: 0x06001862 RID: 6242 RVA: 0x0007BEC0 File Offset: 0x0007A0C0
		private void Awake()
		{
			this.volumeSlider.minValue = 0f;
			this.volumeSlider.maxValue = 1f;
			this.volumeSlider.SetSingleOnValueChangedCallback(new UnityAction<float>(this.OnVolumeChanged));
			this.volumeSlider.value = this.initialVolume;
			this.OnVolumeChanged(this.initialVolume);
		}

		// Token: 0x06001863 RID: 6243 RVA: 0x0007BF21 File Offset: 0x0007A121
		private void OnVolumeChanged(float newValue)
		{
			this.audioSource.volume = newValue;
		}

		// Token: 0x0400166D RID: 5741
		[SerializeField]
		private Text volumeText;

		// Token: 0x0400166E RID: 5742
		[SerializeField]
		private Slider volumeSlider;

		// Token: 0x0400166F RID: 5743
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04001670 RID: 5744
		[SerializeField]
		private float initialVolume = 0.125f;
	}
}

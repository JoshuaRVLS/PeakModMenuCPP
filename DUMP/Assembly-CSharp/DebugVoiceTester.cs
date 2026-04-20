using System;
using UnityEngine;

// Token: 0x02000251 RID: 593
public class DebugVoiceTester : MonoBehaviour
{
	// Token: 0x060011F0 RID: 4592 RVA: 0x0005A4B4 File Offset: 0x000586B4
	private void Start()
	{
		this.audioSource.clip = Microphone.Start(Microphone.devices[0], true, 10, 44100);
		this.audioSource.loop = true;
		while (Microphone.GetPosition(null) <= 0)
		{
		}
		this.audioSource.Play();
	}

	// Token: 0x060011F1 RID: 4593 RVA: 0x0005A500 File Offset: 0x00058700
	private void Update()
	{
	}

	// Token: 0x04000FFA RID: 4090
	public AudioSource audioSource;
}

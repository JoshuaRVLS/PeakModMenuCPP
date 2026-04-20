using System;
using System.Collections;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;
using Zorro.Core;

// Token: 0x020001D0 RID: 464
public class LoadingScreen : MonoBehaviour
{
	// Token: 0x06000ED1 RID: 3793 RVA: 0x0004A2AB File Offset: 0x000484AB
	private void Awake()
	{
		this.canvas.enabled = false;
		this.anim = base.GetComponent<Animator>();
		base.transform.SetParent(null, true);
		Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x06000ED2 RID: 3794 RVA: 0x0004A2DD File Offset: 0x000484DD
	public virtual IEnumerator LoadingRoutine(Action runAfter, IEnumerator[] processList)
	{
		this.canvas.enabled = true;
		this.group.blocksRaycasts = true;
		float num = 0f;
		if (this.FadeOutAudioCurve != null && this.FadeOutAudioCurve.keys.Length != 0)
		{
			num = this.FadeOutAudioCurve.GetEndTime();
		}
		float extraLoadTime = this.loadStartYieldTime - num;
		if (this.FadeOutAudioCurve != null && this.FadeOutAudioCurve.keys.Length != 0)
		{
			yield return this.FadeOutAudioCurve.YieldForCurve(delegate(float f)
			{
				if (this.Mixer != null)
				{
					this.Mixer.SetFloat("LoadingFade", math.remap(0f, 1f, -80f, 0f, f));
				}
			}, true, 1f);
		}
		if (extraLoadTime > 0f)
		{
			yield return new WaitForSecondsRealtime(extraLoadTime);
		}
		int num2;
		for (int processIndex = 0; processIndex < processList.Length; processIndex = num2 + 1)
		{
			this.currentProcess = processList[processIndex];
			base.StartCoroutine(this.RunProcess(this.currentProcess));
			while (this.runningProcess)
			{
				yield return null;
			}
			num2 = processIndex;
		}
		if (!PhotonNetwork.IsMessageQueueRunning)
		{
			Debug.Log("Restarting message queue");
			PhotonNetwork.IsMessageQueueRunning = true;
		}
		if (runAfter != null)
		{
			runAfter();
		}
		this.anim.SetTrigger("Finish");
		this.group.blocksRaycasts = false;
		Debug.Log("Loading finished.");
		if (this.FadeInAudioCurve != null && this.FadeInAudioCurve.keys.Length != 0)
		{
			Debug.Log("FADING OUT");
			yield return this.FadeInAudioCurve.YieldForCurve(delegate(float f)
			{
				if (this.Mixer != null)
				{
					this.Mixer.SetFloat("LoadingFade", math.remap(0f, 1f, -80f, 0f, f));
				}
			}, true, 1f);
			Debug.Log("DONE FADING OUT");
		}
		Debug.Log("Destroying self in 6 seconds");
		Object.Destroy(base.gameObject, 6f);
		yield break;
	}

	// Token: 0x06000ED3 RID: 3795 RVA: 0x0004A2FA File Offset: 0x000484FA
	private IEnumerator RunProcess(IEnumerator process)
	{
		Debug.Log("Process Started: process");
		this.runningProcess = true;
		yield return base.StartCoroutine(process);
		this.runningProcess = false;
		Debug.Log("Process Finished: process");
		yield break;
	}

	// Token: 0x04000C99 RID: 3225
	public AnimationCurve FadeOutAudioCurve;

	// Token: 0x04000C9A RID: 3226
	public AnimationCurve FadeInAudioCurve;

	// Token: 0x04000C9B RID: 3227
	public AudioMixer Mixer;

	// Token: 0x04000C9C RID: 3228
	public CanvasGroup group;

	// Token: 0x04000C9D RID: 3229
	public Canvas canvas;

	// Token: 0x04000C9E RID: 3230
	private Animator anim;

	// Token: 0x04000C9F RID: 3231
	public float loadStartYieldTime = 1.5f;

	// Token: 0x04000CA0 RID: 3232
	protected IEnumerator currentProcess;

	// Token: 0x04000CA1 RID: 3233
	private bool runningProcess;

	// Token: 0x020004DD RID: 1245
	public enum LoadingScreenType
	{
		// Token: 0x04001B6B RID: 7019
		Basic,
		// Token: 0x04001B6C RID: 7020
		Plane
	}
}

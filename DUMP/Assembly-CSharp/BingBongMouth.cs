using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000063 RID: 99
public class BingBongMouth : MonoBehaviour
{
	// Token: 0x060004E1 RID: 1249 RVA: 0x0001D284 File Offset: 0x0001B484
	private void Start()
	{
		this.action.OnAsk += this.SampleAudioClip;
	}

	// Token: 0x060004E2 RID: 1250 RVA: 0x0001D2A0 File Offset: 0x0001B4A0
	public void SampleAudioClip(AudioClip clip)
	{
		float[] array = new float[clip.samples * clip.channels];
		clip.GetData(array, 0);
		List<float> list = new List<float>();
		int num = clip.frequency / 30;
		for (int i = 0; i < array.Length; i += num)
		{
			float num2 = 0f;
			int num3 = 0;
			while (num3 < num && i + num3 < array.Length)
			{
				num2 += Mathf.Abs(array[i + num3]);
				num3++;
			}
			list.Add(num2 / (float)num);
		}
		this.CreateCurveMap(list);
	}

	// Token: 0x060004E3 RID: 1251 RVA: 0x0001D32C File Offset: 0x0001B52C
	public void CreateCurveMap(List<float> samples)
	{
		this.curveMap = new AnimationCurve();
		for (int i = 0; i < samples.Count; i++)
		{
			float num = (float)i / 30f;
			float value = samples[i];
			this.curveMap.AddKey(num, value);
		}
		this.maxTime = (float)(samples.Count - 1) / 30f;
		this.canPlay = true;
		this.time = this.timeOffset;
	}

	// Token: 0x060004E4 RID: 1252 RVA: 0x0001D39C File Offset: 0x0001B59C
	private void Update()
	{
		if (this.canPlay)
		{
			this.time += Time.deltaTime * this.timeMultiplier * this.audioSource.pitch;
			float b = this.curveMap.Evaluate(this.time) * this.maxMouthOpen;
			this.lerpedMouthVal = Mathf.Lerp(this.lerpedMouthVal, b, Time.deltaTime * 25f);
			this.animator.SetFloat(this.animValue, this.lerpedMouthVal);
			if (this.time > this.maxTime)
			{
				this.canPlay = false;
				this.time = this.timeOffset;
				this.lerpedMouthVal = 0f;
			}
		}
	}

	// Token: 0x04000539 RID: 1337
	public AudioSource audioSource;

	// Token: 0x0400053A RID: 1338
	public Animator animator;

	// Token: 0x0400053B RID: 1339
	public string animValue;

	// Token: 0x0400053C RID: 1340
	public AnimationCurve curveMap;

	// Token: 0x0400053D RID: 1341
	public float maxMouthOpen;

	// Token: 0x0400053E RID: 1342
	public bool isPlaying;

	// Token: 0x0400053F RID: 1343
	public bool canPlay;

	// Token: 0x04000540 RID: 1344
	public Action_AskBingBong action;

	// Token: 0x04000541 RID: 1345
	public float timeOffset = 0.25f;

	// Token: 0x04000542 RID: 1346
	private float maxTime;

	// Token: 0x04000543 RID: 1347
	private float time;

	// Token: 0x04000544 RID: 1348
	private float lerpedMouthVal;

	// Token: 0x04000545 RID: 1349
	public float timeMultiplier = 1.5f;
}

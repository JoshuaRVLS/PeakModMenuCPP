using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000368 RID: 872
public class Transition_Shader : Transition
{
	// Token: 0x06001703 RID: 5891 RVA: 0x0007646B File Offset: 0x0007466B
	private void Awake()
	{
		this.rend = base.GetComponent<MeshRenderer>();
		this.mat = Object.Instantiate<Material>(this.rend.sharedMaterial);
		this.rend.sharedMaterial = this.mat;
	}

	// Token: 0x06001704 RID: 5892 RVA: 0x000764A0 File Offset: 0x000746A0
	public override IEnumerator TransitionIn(float speed = 1f)
	{
		float c = 0f;
		float t = this.inCurve.keys[this.inCurve.keys.Length - 1].time;
		while (c < t)
		{
			c += Time.unscaledDeltaTime * speed * this.inSpeed;
			this.mat.SetFloat("_Progress", c);
			this.mat.SetInt("_In", 1);
			yield return null;
		}
		yield break;
	}

	// Token: 0x06001705 RID: 5893 RVA: 0x000764B6 File Offset: 0x000746B6
	public override IEnumerator TransitionOut(float speed = 1f)
	{
		float c = 0f;
		float t = this.outCurve.keys[this.outCurve.keys.Length - 1].time;
		while (c < t)
		{
			c += Time.unscaledDeltaTime * speed * this.outSpeed;
			this.mat.SetFloat("_Progress", c);
			this.mat.SetInt("_In", 0);
			yield return null;
		}
		yield break;
	}

	// Token: 0x04001572 RID: 5490
	private MeshRenderer rend;

	// Token: 0x04001573 RID: 5491
	private Material mat;

	// Token: 0x04001574 RID: 5492
	public float inSpeed = 1f;

	// Token: 0x04001575 RID: 5493
	public AnimationCurve inCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x04001576 RID: 5494
	public float outSpeed = 1f;

	// Token: 0x04001577 RID: 5495
	public AnimationCurve outCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
}

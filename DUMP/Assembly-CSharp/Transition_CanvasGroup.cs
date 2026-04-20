using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000367 RID: 871
public class Transition_CanvasGroup : Transition
{
	// Token: 0x060016FF RID: 5887 RVA: 0x000763CA File Offset: 0x000745CA
	private void Awake()
	{
		this.gr = base.GetComponent<CanvasGroup>();
	}

	// Token: 0x06001700 RID: 5888 RVA: 0x000763D8 File Offset: 0x000745D8
	public override IEnumerator TransitionIn(float speed = 1f)
	{
		float c = 0f;
		float t = this.inCurve.keys[this.inCurve.keys.Length - 1].time;
		while (c < t)
		{
			c += Time.unscaledDeltaTime * speed * this.inSpeed;
			this.gr.alpha = this.inCurve.Evaluate(c);
			yield return null;
		}
		yield break;
	}

	// Token: 0x06001701 RID: 5889 RVA: 0x000763EE File Offset: 0x000745EE
	public override IEnumerator TransitionOut(float speed = 1f)
	{
		float c = 0f;
		float t = this.outCurve.keys[this.outCurve.keys.Length - 1].time;
		while (c < t)
		{
			c += Time.unscaledDeltaTime * speed * this.outSpeed;
			this.gr.alpha = this.outCurve.Evaluate(c);
			yield return null;
		}
		yield break;
	}

	// Token: 0x0400156D RID: 5485
	private CanvasGroup gr;

	// Token: 0x0400156E RID: 5486
	public float inSpeed = 1f;

	// Token: 0x0400156F RID: 5487
	public AnimationCurve inCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x04001570 RID: 5488
	public float outSpeed = 1f;

	// Token: 0x04001571 RID: 5489
	public AnimationCurve outCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
}

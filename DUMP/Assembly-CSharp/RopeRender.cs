using System;
using UnityEngine;

// Token: 0x0200031E RID: 798
[Serializable]
public class RopeRender
{
	// Token: 0x06001555 RID: 5461 RVA: 0x0006C174 File Offset: 0x0006A374
	public void DisplayRope(Vector3 from, Vector3 to, float time, LineRenderer line)
	{
		line.enabled = true;
		float d = Mathf.Lerp(1f, 0f, time);
		for (int i = 0; i < line.positionCount; i++)
		{
			float num = (float)i / ((float)line.positionCount - 1f);
			Vector3 vector = Vector3.Lerp(from, to, num);
			float num2 = (float)i * this.scale;
			float num3 = time * this.scrollSpeed;
			float d2 = Mathf.Cos(num2 + num3);
			vector += d2 * line.transform.up * d * this.wobbleCurve.Evaluate(time) * this.wobbleOverLineCurve.Evaluate(num);
			line.SetPosition(i, vector);
		}
	}

	// Token: 0x06001556 RID: 5462 RVA: 0x0006C235 File Offset: 0x0006A435
	internal void StopRend(LineRenderer line)
	{
		line.enabled = false;
	}

	// Token: 0x04001380 RID: 4992
	public float wobble = 1f;

	// Token: 0x04001381 RID: 4993
	public float scrollSpeed = 1f;

	// Token: 0x04001382 RID: 4994
	public float scale = 0.3f;

	// Token: 0x04001383 RID: 4995
	public AnimationCurve wobbleCurve;

	// Token: 0x04001384 RID: 4996
	public AnimationCurve wobbleOverLineCurve;
}

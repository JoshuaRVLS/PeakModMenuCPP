using System;
using UnityEngine;

// Token: 0x02000125 RID: 293
public class MagicBeanVine : MonoBehaviour
{
	// Token: 0x0600099E RID: 2462 RVA: 0x0003341C File Offset: 0x0003161C
	private void Awake()
	{
		this.currentLength = this.initialLength;
		float time = this.currentLength / this.maxLength;
		float num = this.xzScaleCurve.Evaluate(time) * this.maxWidth;
		this.vineOriginTransform.transform.localScale = new Vector3(num, this.currentLength, num);
	}

	// Token: 0x0600099F RID: 2463 RVA: 0x00033474 File Offset: 0x00031674
	private void FixedUpdate()
	{
		if (this.currentLength < this.maxLength)
		{
			this.currentLength = Mathf.MoveTowards(this.currentLength, this.maxLength, this.growingSpeed * Time.fixedDeltaTime);
			float time = this.currentLength / this.maxLength;
			float num = this.xzScaleCurve.Evaluate(time) * this.maxWidth;
			this.vineOriginTransform.transform.localScale = new Vector3(num, this.currentLength, num);
			this.vineOriginTransform.transform.Rotate(0f, this.rotationSpeed * this.rotationSpeedCurve.Evaluate(time), 0f);
		}
	}

	// Token: 0x040008DE RID: 2270
	public Transform vineOriginTransform;

	// Token: 0x040008DF RID: 2271
	public float maxWidth = 1.5f;

	// Token: 0x040008E0 RID: 2272
	public float maxLength = 20f;

	// Token: 0x040008E1 RID: 2273
	public float initialLength = 0.5f;

	// Token: 0x040008E2 RID: 2274
	private float currentLength = 0.01f;

	// Token: 0x040008E3 RID: 2275
	public float growingSpeed = 1f;

	// Token: 0x040008E4 RID: 2276
	public float rotationSpeed = 10f;

	// Token: 0x040008E5 RID: 2277
	public AnimationCurve xzScaleCurve;

	// Token: 0x040008E6 RID: 2278
	public AnimationCurve rotationSpeedCurve;
}

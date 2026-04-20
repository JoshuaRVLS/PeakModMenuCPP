using System;
using UnityEngine;

// Token: 0x02000133 RID: 307
public class LanternLight : MonoBehaviour
{
	// Token: 0x060009E3 RID: 2531 RVA: 0x00034A6B File Offset: 0x00032C6B
	private void Start()
	{
		this.startIntensity = this.light.intensity;
	}

	// Token: 0x060009E4 RID: 2532 RVA: 0x00034A80 File Offset: 0x00032C80
	private void Update()
	{
		if (!this.light.enabled)
		{
			return;
		}
		float num = this.flickerCurve.Evaluate(Time.deltaTime * this.flickerSpeed) * this.flickerCurve.Evaluate(Time.deltaTime * this.flickerSpeed * 0.38374f);
		this.light.intensity = this.startIntensity + num * this.flickerAmount;
	}

	// Token: 0x0400093C RID: 2364
	public Light light;

	// Token: 0x0400093D RID: 2365
	public float flickerSpeed;

	// Token: 0x0400093E RID: 2366
	public float flickerAmount;

	// Token: 0x0400093F RID: 2367
	public AnimationCurve flickerCurve;

	// Token: 0x04000940 RID: 2368
	private float startIntensity;
}

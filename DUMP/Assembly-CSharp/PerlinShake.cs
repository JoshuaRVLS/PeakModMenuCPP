using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002CA RID: 714
public class PerlinShake : MonoBehaviour
{
	// Token: 0x0600141D RID: 5149 RVA: 0x00065B55 File Offset: 0x00063D55
	private void Start()
	{
	}

	// Token: 0x0600141E RID: 5150 RVA: 0x00065B58 File Offset: 0x00063D58
	private void Update()
	{
		Vector2 zero = Vector2.zero;
		for (int i = this.shakes.Count - 1; i >= 0; i--)
		{
			zero.x += (Mathf.PerlinNoise(Time.time * this.shakes[i].scale, 0f) - 0.5f) * this.shakes[i].amount * (this.shakes[i].duration / this.shakes[i].startDuration);
			zero.y += (Mathf.PerlinNoise(0f, Time.time * this.shakes[i].scale) - 0.5f) * this.shakes[i].amount * (this.shakes[i].duration / this.shakes[i].startDuration);
			this.shakes[i].duration -= Time.deltaTime;
			if (this.shakes[i].duration < 0f)
			{
				this.shakes.RemoveAt(i);
			}
		}
		base.transform.localEulerAngles = zero;
	}

	// Token: 0x0600141F RID: 5151 RVA: 0x00065CAC File Offset: 0x00063EAC
	public void AddShake(float amount = 1f, float duration = 0.2f, float scale = 15f)
	{
		PerlinShakeInstance perlinShakeInstance = new PerlinShakeInstance();
		perlinShakeInstance.amount = amount;
		perlinShakeInstance.duration = duration;
		perlinShakeInstance.startDuration = duration;
		perlinShakeInstance.scale = scale;
		this.shakes.Add(perlinShakeInstance);
	}

	// Token: 0x06001420 RID: 5152 RVA: 0x00065CE8 File Offset: 0x00063EE8
	public void AddShake(Vector3 pos, float amount = 1f, float duration = 0.2f, float scale = 15f, float range = 50f)
	{
		float num = Mathf.InverseLerp(range, 0f, Vector3.Distance(MainCamera.instance.transform.position, pos));
		if (num <= 0.001f)
		{
			return;
		}
		this.AddShake(amount * num, duration * num, scale);
	}

	// Token: 0x0400125A RID: 4698
	public List<PerlinShakeInstance> shakes = new List<PerlinShakeInstance>();
}

using System;
using UnityEngine;

// Token: 0x020000CA RID: 202
[Serializable]
public class PerlinSampler
{
	// Token: 0x06000811 RID: 2065 RVA: 0x0002CF78 File Offset: 0x0002B178
	public bool Sample(Vector2 pos, int seed = 0)
	{
		float num = this.SampleValue(pos, seed);
		return num > this.minMax.x && num < this.minMax.y;
	}

	// Token: 0x06000812 RID: 2066 RVA: 0x0002CFAC File Offset: 0x0002B1AC
	public float SampleValue(Vector2 pos, int seed = 0)
	{
		float num = 0f;
		for (int i = 0; i < this.iterations; i++)
		{
			float num2 = this.scale;
			num2 *= Mathf.Pow(this.roughness, (float)i);
			float num3 = Mathf.PerlinNoise((float)(12345 + seed) + pos.x * num2 * 0.1f, (float)(12345 + seed) + pos.y * num2 * 0.1f);
			if (i == 0)
			{
				num = num3;
			}
			else
			{
				float t = Mathf.Pow(this.roughness, (float)i);
				num = Mathf.Lerp(num, num3, t);
			}
		}
		if (!Mathf.Approximately(this.pow, 1f))
		{
			num = Mathf.Pow(num, this.pow);
		}
		return num;
	}

	// Token: 0x040007D0 RID: 2000
	public float scale = 1f;

	// Token: 0x040007D1 RID: 2001
	public int iterations = 2;

	// Token: 0x040007D2 RID: 2002
	public float scaleIncrease = 3f;

	// Token: 0x040007D3 RID: 2003
	public float roughness = 0.3f;

	// Token: 0x040007D4 RID: 2004
	public float pow = 1f;

	// Token: 0x040007D5 RID: 2005
	public Vector2 minMax = new Vector2(0f, 1f);
}

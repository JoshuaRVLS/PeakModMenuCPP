using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001F6 RID: 502
public static class Util
{
	// Token: 0x17000124 RID: 292
	// (get) Token: 0x06000FD3 RID: 4051 RVA: 0x0004D882 File Offset: 0x0004BA82
	public static Random random
	{
		get
		{
			if (Util.r == null)
			{
				Util.r = new Random();
			}
			return Util.r;
		}
	}

	// Token: 0x06000FD4 RID: 4052 RVA: 0x0004D89C File Offset: 0x0004BA9C
	public static float RangeLerp(float min, float max, float minParam, float maxParam, float param, bool clamp = true, AnimationCurve curve = null)
	{
		if (maxParam - minParam == 0f)
		{
			return min;
		}
		float num = Mathf.Clamp((param - minParam) / (maxParam - minParam), 0f, 1f);
		if (curve != null && curve.keys.Length != 0)
		{
			num = curve.Evaluate(num);
		}
		float num2 = max - min;
		return min + num2 * num;
	}

	// Token: 0x06000FD5 RID: 4053 RVA: 0x0004D8F0 File Offset: 0x0004BAF0
	public static T RandomSelection<T>(this IEnumerable<T> enumerable, Func<T, int> weightFunc)
	{
		int num = 0;
		T t = default(T);
		foreach (T t2 in enumerable)
		{
			int num2 = weightFunc(t2);
			if (Util.random.Next(num + num2) >= num)
			{
				t = t2;
			}
			num += num2;
		}
		T t3 = t;
		return t;
	}

	// Token: 0x06000FD6 RID: 4054 RVA: 0x0004D964 File Offset: 0x0004BB64
	public static Vector2 FlattenVector3(Vector3 original)
	{
		return new Vector2(original.x, original.z);
	}

	// Token: 0x06000FD7 RID: 4055 RVA: 0x0004D978 File Offset: 0x0004BB78
	public static float GenerateNormalDistribution(float mean, float stdDev)
	{
		double d = 1.0 - (double)Random.value;
		double num = 1.0 - (double)Random.value;
		double num2 = Math.Sqrt(-2.0 * Math.Log(d)) * Math.Cos(6.283185307179586 * num);
		Debug.Log(string.Concat(new string[]
		{
			"Created random distribution result:",
			num2.ToString(),
			" mean: ",
			mean.ToString(),
			" stdDev: ",
			stdDev.ToString()
		}));
		float num3 = (float)num2;
		return mean + num3 * stdDev;
	}

	// Token: 0x06000FD8 RID: 4056 RVA: 0x0004DA1C File Offset: 0x0004BC1C
	public static bool Coinflip()
	{
		return (double)Random.value > 0.5;
	}

	// Token: 0x04000D73 RID: 3443
	private static Random r;
}

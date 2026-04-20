using System;
using UnityEngine;

// Token: 0x02000356 RID: 854
[RequireComponent(typeof(HeightmapMesh), typeof(MeshFilter), typeof(MeshRenderer))]
public class SwampMeshGen : LevelGenStep
{
	// Token: 0x0600169A RID: 5786 RVA: 0x00073B13 File Offset: 0x00071D13
	private void Awake()
	{
		this._hm = base.GetComponent<HeightmapMesh>();
	}

	// Token: 0x0600169B RID: 5787 RVA: 0x00073B21 File Offset: 0x00071D21
	public override void Execute()
	{
		this.Generate();
	}

	// Token: 0x0600169C RID: 5788 RVA: 0x00073B29 File Offset: 0x00071D29
	public override void Clear()
	{
		if (this._hm == null)
		{
			this._hm = base.GetComponent<HeightmapMesh>();
		}
		this._hm.GenerateTest();
	}

	// Token: 0x0600169D RID: 5789 RVA: 0x00073B50 File Offset: 0x00071D50
	public void Generate()
	{
		if (this.resolution < 2)
		{
			Debug.LogError("Resolution must be at least 2.");
			return;
		}
		if (this._hm == null)
		{
			this._hm = base.GetComponent<HeightmapMesh>();
		}
		this._hm.cellSize = this.cellSize;
		this._hm.center = this.center;
		int num = this.resolution;
		int num2 = this.resolution;
		float[,] array = new float[num2, num];
		Vector3 zero = Vector3.zero;
		if (this.center)
		{
			float num3 = (float)(num - 1) * this.cellSize;
			float num4 = (float)(num2 - 1) * this.cellSize;
			zero = new Vector3(-num3 * 0.5f, 0f, -num4 * 0.5f);
		}
		for (int i = 0; i < num2; i++)
		{
			for (int j = 0; j < num; j++)
			{
				Vector3 b = new Vector3((float)j * this.cellSize, 0f, (float)i * this.cellSize);
				Vector3 origin = base.transform.position + zero + b;
				RaycastHit raycastHit;
				float num5;
				if (Physics.Raycast(new Ray(origin, Vector3.down), out raycastHit, this.maxRaycastLength, this.layerMask, this.triggerInteraction))
				{
					num5 = raycastHit.point.y - base.transform.position.y;
				}
				else
				{
					num5 = -this.maxRaycastLength;
				}
				array[i, j] = num5;
			}
		}
		if (this.clampHeights)
		{
			for (int k = 0; k < num2; k++)
			{
				for (int l = 0; l < num; l++)
				{
					array[k, l] = Mathf.Clamp(array[k, l], this.minHeight, this.maxHeight);
				}
			}
		}
		for (int m = 0; m < num2; m++)
		{
			for (int n = 0; n < num; n++)
			{
				array[m, n] += Mathf.PerlinNoise((float)n * this.perlinScale, (float)m * this.perlinScale) * this.perlinAmount - this.perlinAmount / 2f;
			}
		}
		if (this.blurIterations > 0 && this.blurRadius > 0)
		{
			this.BoxBlurInPlace(array, this.blurRadius, this.blurIterations);
		}
		for (int num6 = 0; num6 < num2; num6++)
		{
			for (int num7 = 0; num7 < num; num7++)
			{
				array[num6, num7] += Mathf.PerlinNoise((float)num7 * this.postPerlinScale, (float)num6 * this.postPerlinScale) * this.postPerlinAmount - this.postPerlinAmount / 2f;
				array[num6, num7] += this.heightOffset;
			}
		}
		this._hm.Generate(array);
		this._hm.GetComponent<MeshCollider>().sharedMesh = this._hm.GetComponent<MeshFilter>().sharedMesh;
	}

	// Token: 0x0600169E RID: 5790 RVA: 0x00073E3C File Offset: 0x0007203C
	private void BoxBlurInPlace(float[,] data, int radius, int iterations)
	{
		int length = data.GetLength(0);
		int length2 = data.GetLength(1);
		float[,] array = new float[length, length2];
		for (int i = 0; i < iterations; i++)
		{
			for (int j = 0; j < length; j++)
			{
				float num = 0f;
				int num2 = radius * 2 + 1;
				for (int k = -radius; k <= radius; k++)
				{
					int num3 = Mathf.Clamp(k, 0, length2 - 1);
					num += data[j, num3];
				}
				for (int l = 0; l < length2; l++)
				{
					array[j, l] = num / (float)num2;
					int num4 = l - radius;
					int num5 = l + radius + 1;
					if (num4 >= 0 && num4 < length2)
					{
						num -= data[j, num4];
					}
					else if (num4 < 0)
					{
						num -= data[j, 0];
					}
					else
					{
						num -= data[j, length2 - 1];
					}
					if (num5 >= 0 && num5 < length2)
					{
						num += data[j, num5];
					}
					else if (num5 < 0)
					{
						num += data[j, 0];
					}
					else
					{
						num += data[j, length2 - 1];
					}
				}
			}
			for (int m = 0; m < length2; m++)
			{
				float num6 = 0f;
				int num7 = radius * 2 + 1;
				for (int n = -radius; n <= radius; n++)
				{
					int num8 = Mathf.Clamp(n, 0, length - 1);
					num6 += array[num8, m];
				}
				for (int num9 = 0; num9 < length; num9++)
				{
					data[num9, m] = num6 / (float)num7;
					int num10 = num9 - radius;
					int num11 = num9 + radius + 1;
					if (num10 >= 0 && num10 < length)
					{
						num6 -= array[num10, m];
					}
					else if (num10 < 0)
					{
						num6 -= array[0, m];
					}
					else
					{
						num6 -= array[length - 1, m];
					}
					if (num11 >= 0 && num11 < length)
					{
						num6 += array[num11, m];
					}
					else if (num11 < 0)
					{
						num6 += array[0, m];
					}
					else
					{
						num6 += array[length - 1, m];
					}
				}
			}
		}
	}

	// Token: 0x0600169F RID: 5791 RVA: 0x00074080 File Offset: 0x00072280
	private void OnDrawGizmosSelected()
	{
		if (!this.drawRayGizmos && !this.drawSamplePoints)
		{
			return;
		}
		int num = Mathf.Max(2, this.resolution);
		int num2 = Mathf.Max(2, this.resolution);
		Vector3 zero = Vector3.zero;
		if (this.center)
		{
			float num3 = (float)(num - 1) * this.cellSize;
			float num4 = (float)(num2 - 1) * this.cellSize;
			zero = new Vector3(-num3 * 0.5f, 0f, -num4 * 0.5f);
		}
		for (int i = 0; i < num2; i++)
		{
			for (int j = 0; j < num; j++)
			{
				Vector3 b = new Vector3((float)j * this.cellSize, 0f, (float)i * this.cellSize);
				Vector3 vector = base.transform.position + zero + b;
				if (this.drawSamplePoints)
				{
					Gizmos.DrawSphere(vector, Mathf.Min(0.05f, this.cellSize * 0.1f));
				}
				if (this.drawRayGizmos)
				{
					Gizmos.DrawLine(vector, vector + Vector3.down * this.maxRaycastLength);
				}
			}
		}
	}

	// Token: 0x040014F6 RID: 5366
	[Header("Grid")]
	[Min(2f)]
	public int resolution = 64;

	// Token: 0x040014F7 RID: 5367
	public float cellSize = 1f;

	// Token: 0x040014F8 RID: 5368
	public bool center = true;

	// Token: 0x040014F9 RID: 5369
	[Header("Raycast")]
	public float maxRaycastLength = 10f;

	// Token: 0x040014FA RID: 5370
	public LayerMask layerMask = -1;

	// Token: 0x040014FB RID: 5371
	public QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;

	// Token: 0x040014FC RID: 5372
	[Header("Clamp (optional)")]
	public bool clampHeights;

	// Token: 0x040014FD RID: 5373
	public float minHeight = -5f;

	// Token: 0x040014FE RID: 5374
	public float maxHeight = 5f;

	// Token: 0x040014FF RID: 5375
	[Header("Perlin")]
	public float perlinScale = 0.5f;

	// Token: 0x04001500 RID: 5376
	public float perlinAmount = 0.5f;

	// Token: 0x04001501 RID: 5377
	[Header("Smoothing")]
	[Tooltip("Number of box-blur passes to apply after clamping.")]
	[Min(0f)]
	public int blurIterations = 1;

	// Token: 0x04001502 RID: 5378
	[Tooltip("Radius in cells for box blur. 0 disables blur.")]
	[Min(0f)]
	public int blurRadius = 1;

	// Token: 0x04001503 RID: 5379
	[Header("Offset")]
	public float heightOffset;

	// Token: 0x04001504 RID: 5380
	[Header("Post Perlin")]
	public float postPerlinScale = 0.5f;

	// Token: 0x04001505 RID: 5381
	public float postPerlinAmount = 0.5f;

	// Token: 0x04001506 RID: 5382
	[Header("Debug")]
	public bool drawRayGizmos;

	// Token: 0x04001507 RID: 5383
	public bool drawSamplePoints;

	// Token: 0x04001508 RID: 5384
	private HeightmapMesh _hm;
}

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

// Token: 0x02000359 RID: 857
[ExecuteInEditMode]
public class TerrainSplat : MonoBehaviour
{
	// Token: 0x060016AE RID: 5806 RVA: 0x000747E4 File Offset: 0x000729E4
	private void SetTerrainVariables()
	{
		Shader.SetGlobalFloat("TerrainTriplanarScale", this.TerrainTriplanarScale);
		Shader.SetGlobalTexture("TerrainTextureR", this.TerrainTextureR);
		Shader.SetGlobalColor("TerrainColorR", this.TerrainColorR.linear);
		Shader.SetGlobalVector("TerrainSmoothR", this.TerrainSmoothR);
		Shader.SetGlobalTexture("TerrainTextureG", this.TerrainTextureG);
		Shader.SetGlobalColor("TerrainColorG", this.TerrainColorG.linear);
		Shader.SetGlobalVector("TerrainSmoothG", this.TerrainSmoothG);
		Shader.SetGlobalTexture("TerrainTextureB", this.TerrainTextureB);
		Shader.SetGlobalColor("TerrainColorB", this.TerrainColorB.linear);
		Shader.SetGlobalVector("TerrainSmoothB", this.TerrainSmoothB);
		Shader.SetGlobalTexture("TerrainTextureA", this.TerrainTextureA);
		Shader.SetGlobalColor("TerrainColorA", this.TerrainColorA.linear);
		Shader.SetGlobalVector("TerrainSmoothA", this.TerrainSmoothA);
	}

	// Token: 0x060016AF RID: 5807 RVA: 0x000748E9 File Offset: 0x00072AE9
	private void Start()
	{
		this.Generate(TerrainBrush.BrushType.All);
	}

	// Token: 0x060016B0 RID: 5808 RVA: 0x000748F2 File Offset: 0x00072AF2
	private void GenerateAll()
	{
		this.Generate(TerrainBrush.BrushType.All);
	}

	// Token: 0x060016B1 RID: 5809 RVA: 0x000748FC File Offset: 0x00072AFC
	public void Generate(TerrainBrush.BrushType brushType)
	{
		this.SetTerrainVariables();
		this.GetBounds();
		if (brushType == TerrainBrush.BrushType.All)
		{
			this.SampleHeightMap();
			this.CreateHeighMap();
		}
		this.CreateColorData(brushType);
		this.ApplyBrushes(brushType);
		if (brushType == TerrainBrush.BrushType.All || brushType == TerrainBrush.BrushType.Splat)
		{
			this.splatMap = this.CreateTexture(this.splatMap, this.splatColors);
		}
		if (brushType == TerrainBrush.BrushType.All || brushType == TerrainBrush.BrushType.Detail)
		{
			this.detailMap = this.CreateTexture(this.detailMap, this.detailColors);
		}
		this.SetShaderData(brushType);
	}

	// Token: 0x060016B2 RID: 5810 RVA: 0x0007497C File Offset: 0x00072B7C
	private void SampleHeightMap()
	{
		this.heights = new Color[this.splatRess, this.splatRess];
		for (int i = 0; i < this.splatRess; i++)
		{
			for (int j = 0; j < this.splatRess; j++)
			{
				this.heights[i, j] = this.SampleHeight(i, j);
			}
		}
	}

	// Token: 0x060016B3 RID: 5811 RVA: 0x000749D8 File Offset: 0x00072BD8
	private Color SampleHeight(int x, int y)
	{
		return new Color(HelperFunctions.GetGroundPos(this.GetPosFromIndex(x, y) + Vector3.up * 1000f, HelperFunctions.LayerType.Terrain, 0f).y / 10f, 0f, 0f, 0f);
	}

	// Token: 0x060016B4 RID: 5812 RVA: 0x00074A2C File Offset: 0x00072C2C
	private void CreateHeighMap()
	{
		if (this.heightMap)
		{
			Object.DestroyImmediate(this.heightMap);
		}
		this.heightMap = new Texture2D(this.splatRess, this.splatRess, TextureFormat.RFloat, 0, true);
		this.heightMap.filterMode = FilterMode.Bilinear;
		this.heightMap.wrapMode = TextureWrapMode.Clamp;
		this.heightMap.SetPixels(HelperFunctions.GridToFlatArray<Color>(this.heights));
		this.heightMap.Apply();
	}

	// Token: 0x060016B5 RID: 5813 RVA: 0x00074AA8 File Offset: 0x00072CA8
	private void SetShaderData(TerrainBrush.BrushType brushType)
	{
		if (brushType == TerrainBrush.BrushType.All || brushType == TerrainBrush.BrushType.Detail)
		{
			Shader.SetGlobalTexture("TerrainDetail", this.detailMap);
		}
		if (brushType == TerrainBrush.BrushType.All || brushType == TerrainBrush.BrushType.Splat)
		{
			Shader.SetGlobalTexture("TerrainSplat", this.splatMap);
		}
		if (brushType == TerrainBrush.BrushType.All)
		{
			Shader.SetGlobalTexture("TerrainHeight", this.heightMap);
		}
		Shader.SetGlobalVector("TerrainCenter", this.bounds.center);
		Shader.SetGlobalVector("TerrainSize", this.bounds.size);
	}

	// Token: 0x060016B6 RID: 5814 RVA: 0x00074B2C File Offset: 0x00072D2C
	private void OnDestroy()
	{
		if (this.splatMap)
		{
			Object.DestroyImmediate(this.splatMap);
		}
	}

	// Token: 0x060016B7 RID: 5815 RVA: 0x00074B48 File Offset: 0x00072D48
	private void CreateColorData(TerrainBrush.BrushType brushType)
	{
		if (brushType == TerrainBrush.BrushType.All || brushType == TerrainBrush.BrushType.Splat)
		{
			this.splatColors = new Color[this.splatRess, this.splatRess];
			for (int i = 0; i < this.splatRess; i++)
			{
				for (int j = 0; j < this.splatRess; j++)
				{
					this.splatColors[i, j] = TerrainSplat.GetColor(this.baseColor);
				}
			}
		}
		if (brushType == TerrainBrush.BrushType.All || brushType == TerrainBrush.BrushType.Detail)
		{
			this.detailColors = new Color[this.splatRess, this.splatRess];
			for (int k = 0; k < this.splatRess; k++)
			{
				for (int l = 0; l < this.splatRess; l++)
				{
					this.detailColors[k, l] = new Color(0.5f, 0.5f, 0.5f, 0f);
				}
			}
		}
	}

	// Token: 0x060016B8 RID: 5816 RVA: 0x00074C14 File Offset: 0x00072E14
	private void ApplyBrushes(TerrainBrush.BrushType brushType)
	{
		foreach (TerrainBrush terrainBrush in HelperFunctions.SortBySiblingIndex<TerrainBrush>(Object.FindObjectsByType<TerrainBrush>(FindObjectsSortMode.InstanceID)).ToArray<TerrainBrush>())
		{
			if (brushType == TerrainBrush.BrushType.All || brushType == terrainBrush.brushType)
			{
				this.ApplySplatBrush(terrainBrush);
			}
		}
	}

	// Token: 0x060016B9 RID: 5817 RVA: 0x00074C58 File Offset: 0x00072E58
	private void ApplySplatBrush(TerrainBrush item)
	{
		if (item.brushType == TerrainBrush.BrushType.Splat)
		{
			item.ApplySplatData(this.splatColors, this.bounds);
			return;
		}
		item.ApplySplatData(this.detailColors, this.bounds);
	}

	// Token: 0x060016BA RID: 5818 RVA: 0x00074C88 File Offset: 0x00072E88
	private void GetBounds()
	{
		Renderer[] rends = HelperFunctions.GetComponentListFromComponentArray<TerrainSplatMesh, Renderer>(Object.FindObjectsByType<TerrainSplatMesh>(FindObjectsSortMode.None)).ToArray();
		this.bounds = HelperFunctions.GetTotalBounds(rends);
	}

	// Token: 0x060016BB RID: 5819 RVA: 0x00074CB4 File Offset: 0x00072EB4
	private Texture2D CreateTexture(Texture2D texture, Color[,] colors)
	{
		if (texture)
		{
			Object.DestroyImmediate(texture);
		}
		texture = new Texture2D(this.splatRess, this.splatRess, DefaultFormat.LDR, TextureCreationFlags.None);
		texture.filterMode = FilterMode.Bilinear;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels(HelperFunctions.GridToFlatArray<Color>(colors));
		texture.Apply();
		return texture;
	}

	// Token: 0x060016BC RID: 5820 RVA: 0x00074D05 File Offset: 0x00072F05
	private Vector3 GetPosFromIndex(int x, int y)
	{
		return this.GetPos((float)x / ((float)this.splatRess - 1f), (float)y / ((float)this.splatRess - 1f));
	}

	// Token: 0x060016BD RID: 5821 RVA: 0x00074D30 File Offset: 0x00072F30
	private Vector3 GetPos(float pX, float pY)
	{
		Vector3 b = Vector3.right * this.bounds.size.x * Mathf.Lerp(-0.5f, 0.5f, pX);
		Vector3 b2 = Vector3.forward * this.bounds.size.z * Mathf.Lerp(-0.5f, 0.5f, pY);
		return this.bounds.center + b + b2;
	}

	// Token: 0x060016BE RID: 5822 RVA: 0x00074DB4 File Offset: 0x00072FB4
	internal static Color GetColor(TerrainSplat.SplatColor color)
	{
		if (color == TerrainSplat.SplatColor.Black)
		{
			return new Color(0f, 0f, 0f, 0f);
		}
		if (color == TerrainSplat.SplatColor.Red)
		{
			return new Color(1f, 0f, 0f, 0f);
		}
		if (color == TerrainSplat.SplatColor.Green)
		{
			return new Color(0f, 1f, 0f, 0f);
		}
		if (color == TerrainSplat.SplatColor.Blue)
		{
			return new Color(0f, 0f, 1f, 0f);
		}
		if (color == TerrainSplat.SplatColor.Alpha)
		{
			return new Color(0f, 0f, 0f, 1f);
		}
		if (color == TerrainSplat.SplatColor.HalfRed)
		{
			return new Color(0.5f, 0f, 0f, 0f);
		}
		if (color == TerrainSplat.SplatColor.HalfGreen)
		{
			return new Color(0f, 0.5f, 0f, 0f);
		}
		if (color == TerrainSplat.SplatColor.HalfBlue)
		{
			return new Color(0f, 0f, 0.5f, 0f);
		}
		return new Color(0f, 0f, 0f, 0.5f);
	}

	// Token: 0x060016BF RID: 5823 RVA: 0x00074ECC File Offset: 0x000730CC
	internal Color GetSplatPixelAtWorldPos(Vector3 point)
	{
		float num = Mathf.InverseLerp(this.bounds.min.x, this.bounds.max.x, point.x);
		float num2 = Mathf.InverseLerp(this.bounds.min.z, this.bounds.max.z, point.z);
		Vector2Int vector2Int = new Vector2Int(Mathf.RoundToInt(num * (float)this.splatMap.width), Mathf.RoundToInt(num2 * (float)this.splatMap.height));
		return this.splatMap.GetPixel(vector2Int.x, vector2Int.y);
	}

	// Token: 0x04001513 RID: 5395
	public float TerrainTriplanarScale = 0.2f;

	// Token: 0x04001514 RID: 5396
	public Texture2D TerrainTextureR;

	// Token: 0x04001515 RID: 5397
	public Color TerrainColorR;

	// Token: 0x04001516 RID: 5398
	public Vector2 TerrainSmoothR = new Vector2(0f, 1f);

	// Token: 0x04001517 RID: 5399
	public Texture2D TerrainTextureG;

	// Token: 0x04001518 RID: 5400
	public Color TerrainColorG;

	// Token: 0x04001519 RID: 5401
	public Vector2 TerrainSmoothG = new Vector2(0f, 1f);

	// Token: 0x0400151A RID: 5402
	public Texture2D TerrainTextureB;

	// Token: 0x0400151B RID: 5403
	public Color TerrainColorB;

	// Token: 0x0400151C RID: 5404
	public Vector2 TerrainSmoothB = new Vector2(0f, 1f);

	// Token: 0x0400151D RID: 5405
	public Texture2D TerrainTextureA;

	// Token: 0x0400151E RID: 5406
	public Color TerrainColorA;

	// Token: 0x0400151F RID: 5407
	public Vector2 TerrainSmoothA = new Vector2(0f, 1f);

	// Token: 0x04001520 RID: 5408
	public int splatRess;

	// Token: 0x04001521 RID: 5409
	public TerrainSplat.SplatColor baseColor;

	// Token: 0x04001522 RID: 5410
	public bool displayBrushes;

	// Token: 0x04001523 RID: 5411
	public Texture2D splatMap;

	// Token: 0x04001524 RID: 5412
	public Texture2D heightMap;

	// Token: 0x04001525 RID: 5413
	public Texture2D detailMap;

	// Token: 0x04001526 RID: 5414
	private Bounds bounds;

	// Token: 0x04001527 RID: 5415
	private Color[,] splatColors;

	// Token: 0x04001528 RID: 5416
	private Color[,] detailColors;

	// Token: 0x04001529 RID: 5417
	private Color[,] heights;

	// Token: 0x02000543 RID: 1347
	public enum SplatColor
	{
		// Token: 0x04001CC8 RID: 7368
		Black,
		// Token: 0x04001CC9 RID: 7369
		Red,
		// Token: 0x04001CCA RID: 7370
		Green,
		// Token: 0x04001CCB RID: 7371
		Blue,
		// Token: 0x04001CCC RID: 7372
		Alpha,
		// Token: 0x04001CCD RID: 7373
		HalfRed,
		// Token: 0x04001CCE RID: 7374
		HalfGreen,
		// Token: 0x04001CCF RID: 7375
		HalfBlue,
		// Token: 0x04001CD0 RID: 7376
		HalfAlpha
	}
}

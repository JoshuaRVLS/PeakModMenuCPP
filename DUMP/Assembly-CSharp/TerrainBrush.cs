using System;
using UnityEngine;

// Token: 0x02000358 RID: 856
public class TerrainBrush : MonoBehaviour
{
	// Token: 0x060016A5 RID: 5797 RVA: 0x00074316 File Offset: 0x00072516
	private void Start()
	{
	}

	// Token: 0x060016A6 RID: 5798 RVA: 0x00074318 File Offset: 0x00072518
	public void Generate()
	{
		Object.FindAnyObjectByType<TerrainSplat>().Generate(this.brushType);
	}

	// Token: 0x060016A7 RID: 5799 RVA: 0x0007432C File Offset: 0x0007252C
	private Bounds GetBounds()
	{
		Bounds result = new Bounds(base.transform.position, Vector3.zero);
		result.Encapsulate(base.transform.position + base.transform.right * 0.5f * base.transform.localScale.x * 1.4f);
		result.Encapsulate(base.transform.position + base.transform.right * -0.5f * base.transform.localScale.x * 1.4f);
		result.Encapsulate(base.transform.position + base.transform.forward * 0.5f * base.transform.localScale.z * 1.4f);
		result.Encapsulate(base.transform.position + base.transform.forward * -0.5f * base.transform.localScale.z * 1.4f);
		return result;
	}

	// Token: 0x060016A8 RID: 5800 RVA: 0x00074480 File Offset: 0x00072680
	private Vector3 GetPos(float pX, float pY)
	{
		Vector3 b = base.transform.right * base.transform.localScale.x * Mathf.Lerp(-0.5f, 0.5f, pX);
		Vector3 b2 = base.transform.forward * base.transform.localScale.z * Mathf.Lerp(-0.5f, 0.5f, pY);
		return base.transform.position + b + b2;
	}

	// Token: 0x060016A9 RID: 5801 RVA: 0x00074510 File Offset: 0x00072710
	internal void ApplySplatData(Color[,] colors, Bounds totalBounds)
	{
		foreach (Vector2Int vector2Int in HelperFunctions.GetIndexesInBounds(colors.GetLength(0), colors.GetLength(1), this.GetBounds(), totalBounds))
		{
			Vector3 pos = HelperFunctions.IDToWorldPos(vector2Int.x, vector2Int.y, colors.GetLength(0), colors.GetLength(1), totalBounds);
			if (this.brushType == TerrainBrush.BrushType.Splat)
			{
				colors[vector2Int.x, vector2Int.y] = this.SampleSplatColor(pos, colors[vector2Int.x, vector2Int.y]);
			}
			else
			{
				colors[vector2Int.x, vector2Int.y] = this.SampleDetailColor(pos, colors[vector2Int.x, vector2Int.y]);
			}
		}
	}

	// Token: 0x060016AA RID: 5802 RVA: 0x00074604 File Offset: 0x00072804
	private Color SampleSplatColor(Vector3 pos, Color beforeColor)
	{
		float num = this.SampleMask(pos);
		Color color = Color.Lerp(beforeColor * 2f, TerrainSplat.GetColor(this.color) * 2f, num * this.strength);
		float magnitude = new Vector4(color.r, color.g, color.b, color.a).magnitude;
		return color / magnitude;
	}

	// Token: 0x060016AB RID: 5803 RVA: 0x00074674 File Offset: 0x00072874
	private Color SampleDetailColor(Vector3 pos, Color beforeColor)
	{
		float num = this.SampleMask(pos);
		Color color = this.detailColor;
		color.a *= num;
		Color result;
		if (beforeColor.a <= 0.01f)
		{
			result = color;
		}
		else
		{
			float t = color.a / beforeColor.a;
			Color color2 = Color.Lerp(beforeColor, color, t);
			color2.a = Mathf.Lerp(beforeColor.a, color.a, t);
			result = color2;
		}
		return result;
	}

	// Token: 0x060016AC RID: 5804 RVA: 0x000746E4 File Offset: 0x000728E4
	private float SampleMask(Vector3 pos)
	{
		Vector3 vector = base.transform.InverseTransformPoint(pos);
		float a = -0.5f;
		float b = 0.5f;
		float num = Mathf.InverseLerp(a, b, vector.x);
		float a2 = -0.5f;
		float b2 = 0.5f;
		float num2 = Mathf.InverseLerp(a2, b2, vector.z);
		float value = this.texture.GetPixel(Mathf.RoundToInt(num * (float)this.texture.width), Mathf.RoundToInt(num2 * (float)this.texture.height)).r;
		value = Mathf.InverseLerp(this.minMaxSlider.x, this.minMaxSlider.y, value);
		return Mathf.Clamp01(value);
	}

	// Token: 0x0400150C RID: 5388
	public TerrainBrush.BrushType brushType;

	// Token: 0x0400150D RID: 5389
	public Texture2D texture;

	// Token: 0x0400150E RID: 5390
	public TerrainSplat.SplatColor color;

	// Token: 0x0400150F RID: 5391
	[Range(0f, 1f)]
	public float strength = 1f;

	// Token: 0x04001510 RID: 5392
	public Color detailColor = new Color(1f, 1f, 1f, 1f);

	// Token: 0x04001511 RID: 5393
	public Vector2 minMaxSlider = new Vector2(0f, 1f);

	// Token: 0x04001512 RID: 5394
	private TerrainSplat splat;

	// Token: 0x02000542 RID: 1346
	public enum BrushType
	{
		// Token: 0x04001CC4 RID: 7364
		Splat,
		// Token: 0x04001CC5 RID: 7365
		Detail,
		// Token: 0x04001CC6 RID: 7366
		All
	}
}

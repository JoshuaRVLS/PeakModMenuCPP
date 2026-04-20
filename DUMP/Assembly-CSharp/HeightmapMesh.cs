using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000278 RID: 632
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HeightmapMesh : MonoBehaviour
{
	// Token: 0x0600127D RID: 4733 RVA: 0x0005C99C File Offset: 0x0005AB9C
	public void Generate(float[,] heights)
	{
		if (heights == null)
		{
			Debug.LogError("heights is null");
			return;
		}
		int length = heights.GetLength(0);
		int length2 = heights.GetLength(1);
		if (length2 < 2 || length < 2)
		{
			Debug.LogError("heights must be at least 2x2.");
			return;
		}
		if (this._mesh == null)
		{
			this._mesh = new Mesh();
			this._mesh.name = "Heightmap Mesh";
		}
		else
		{
			this._mesh.Clear();
		}
		int num = length2 * length;
		if (num > 65535)
		{
			this._mesh.indexFormat = IndexFormat.UInt32;
		}
		else
		{
			this._mesh.indexFormat = IndexFormat.UInt16;
		}
		Vector3[] array = new Vector3[num];
		Vector2[] array2 = new Vector2[num];
		int[] array3 = new int[(length2 - 1) * (length - 1) * 6];
		Vector3 zero = Vector3.zero;
		if (this.center)
		{
			float num2 = (float)(length2 - 1) * this.cellSize;
			float num3 = (float)(length - 1) * this.cellSize;
			zero = new Vector3(-num2 * 0.5f, 0f, -num3 * 0.5f);
		}
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				int num4 = i * length2 + j;
				float y = heights[i, j];
				array[num4] = new Vector3((float)j * this.cellSize, y, (float)i * this.cellSize) + zero;
				float x = (length2 == 1) ? 0f : ((float)j / (float)(length2 - 1));
				float y2 = (length == 1) ? 0f : ((float)i / (float)(length - 1));
				array2[num4] = new Vector2(x, y2);
			}
		}
		int num5 = 0;
		for (int k = 0; k < length - 1; k++)
		{
			for (int l = 0; l < length2 - 1; l++)
			{
				int num6 = k * length2 + l;
				int num7 = num6 + 1;
				int num8 = num6 + length2;
				int num9 = num8 + 1;
				array3[num5++] = num6;
				array3[num5++] = num8;
				array3[num5++] = num7;
				array3[num5++] = num7;
				array3[num5++] = num8;
				array3[num5++] = num9;
			}
		}
		this._mesh.SetVertices(array);
		this._mesh.SetUVs(0, array2);
		this._mesh.SetTriangles(array3, 0, true);
		this._mesh.RecalculateNormals();
		this._mesh.RecalculateBounds();
		base.GetComponent<MeshFilter>().sharedMesh = this._mesh;
	}

	// Token: 0x0600127E RID: 4734 RVA: 0x0005CC1C File Offset: 0x0005AE1C
	[ContextMenu("Generate Test Mesh (Perlin)")]
	public void GenerateTest()
	{
		int num = 129;
		int num2 = 129;
		float[,] array = new float[num2, num];
		for (int i = 0; i < num2; i++)
		{
			for (int j = 0; j < num; j++)
			{
				array[i, j] = Mathf.PerlinNoise((float)j * 0.05f, (float)i * 0.05f) * 2f - 70f;
			}
		}
		this.Generate(array);
	}

	// Token: 0x0400108A RID: 4234
	[Tooltip("World-space distance between adjacent grid points.")]
	public float cellSize = 1f;

	// Token: 0x0400108B RID: 4235
	[Tooltip("If true, the mesh is centered around the GameObject origin.")]
	public bool center;

	// Token: 0x0400108C RID: 4236
	private Mesh _mesh;
}

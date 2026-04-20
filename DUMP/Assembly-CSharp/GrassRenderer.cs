using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using Zorro.Core.Compute;

// Token: 0x020000C5 RID: 197
[ExecuteInEditMode]
public class GrassRenderer : MonoBehaviour
{
	// Token: 0x06000770 RID: 1904 RVA: 0x0002A030 File Offset: 0x00028230
	private void OnEnable()
	{
		ComputeBuffer geometryBuffer = this.GeometryBuffer;
		if (geometryBuffer != null)
		{
			geometryBuffer.Dispose();
		}
		ComputeBuffer argumentsBuffer = this.ArgumentsBuffer;
		if (argumentsBuffer != null)
		{
			argumentsBuffer.Dispose();
		}
		ComputeBuffer grassPointsBuffer = this.GrassPointsBuffer;
		if (grassPointsBuffer != null)
		{
			grassPointsBuffer.Dispose();
		}
		this.DataProvider = base.GetComponent<GrassDataProvider>();
	}

	// Token: 0x06000771 RID: 1905 RVA: 0x0002A07C File Offset: 0x0002827C
	private void OnDisable()
	{
		ComputeBuffer geometryBuffer = this.GeometryBuffer;
		if (geometryBuffer != null)
		{
			geometryBuffer.Dispose();
		}
		ComputeBuffer argumentsBuffer = this.ArgumentsBuffer;
		if (argumentsBuffer != null)
		{
			argumentsBuffer.Dispose();
		}
		ComputeBuffer grassPointsBuffer = this.GrassPointsBuffer;
		if (grassPointsBuffer != null)
		{
			grassPointsBuffer.Dispose();
		}
		this.GeometryBuffer = null;
		this.ArgumentsBuffer = null;
		this.GrassPointsBuffer = null;
	}

	// Token: 0x06000772 RID: 1906 RVA: 0x0002A0D1 File Offset: 0x000282D1
	private void Update()
	{
	}

	// Token: 0x06000773 RID: 1907 RVA: 0x0002A0D4 File Offset: 0x000282D4
	private void Render()
	{
		if (!this.DataProvider)
		{
			this.DataProvider = base.GetComponent<GrassDataProvider>();
		}
		if (this.GrassPointsBuffer == null || this.DataProvider.IsDirty())
		{
			ComputeBuffer grassPointsBuffer = this.GrassPointsBuffer;
			if (grassPointsBuffer != null)
			{
				grassPointsBuffer.Dispose();
			}
			this.GrassPointsBuffer = this.DataProvider.GetData();
		}
		Camera camera = null;
		if (Application.isPlaying)
		{
			camera = MainCamera.instance.cam;
		}
		if (!GrassChunking.ShouldDrawChunk(GrassChunking.GetChunkFromPosition(camera.transform.position), this.CurrentChunk))
		{
			return;
		}
		if (this.GeometryBuffer == null)
		{
			this.GeometryBuffer = new ComputeBuffer(10000, 148, ComputeBufferType.Append);
			this.ArgumentsBuffer = new ComputeBuffer(1, 16, ComputeBufferType.DrawIndirect);
		}
		this.GeometryBuffer.SetCounterValue(0U);
		this.ArgumentsBuffer.SetData(this.argsBufferReset);
		this.grassComputeShader.SetBuffer(this.grassGeometryKernel.kernelID, "GeometryBuffer", this.GeometryBuffer);
		this.grassComputeShader.SetBuffer(this.grassGeometryKernel.kernelID, "IndirectArgsBuffer", this.ArgumentsBuffer);
		this.grassComputeShader.SetBuffer(this.grassGeometryKernel.kernelID, "GrassPoints", this.GrassPointsBuffer);
		this.grassComputeShader.SetFloat("_Time", Time.realtimeSinceStartup);
		this.grassComputeShader.SetVector("_CameraWSPos", camera.transform.position);
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		materialPropertyBlock.SetBuffer("GeometryBuffer", this.GeometryBuffer);
		uint num;
		uint num2;
		uint num3;
		this.grassComputeShader.GetKernelThreadGroupSizes(this.grassGeometryKernel.kernelID, out num, out num2, out num3);
		this.grassGeometryKernel.Dispatch(new int3(this.GrassPointsBuffer.count, 1, 1));
		Graphics.DrawProceduralIndirect(this.m_grassRenderMaterial, new Bounds(base.transform.position, Vector3.one * 500f), MeshTopology.Triangles, this.ArgumentsBuffer, 0, null, materialPropertyBlock, ShadowCastingMode.Off, true, 0);
	}

	// Token: 0x06000774 RID: 1908 RVA: 0x0002A2D5 File Offset: 0x000284D5
	public GrassRenderer()
	{
		int[] array = new int[4];
		array[1] = 1;
		this.argsBufferReset = array;
		base..ctor();
	}

	// Token: 0x0400073F RID: 1855
	public int3 CurrentChunk = int3.zero;

	// Token: 0x04000740 RID: 1856
	public ComputeShader grassComputeShader;

	// Token: 0x04000741 RID: 1857
	private ComputeKernel grassGeometryKernel;

	// Token: 0x04000742 RID: 1858
	private ComputeBuffer GeometryBuffer;

	// Token: 0x04000743 RID: 1859
	private ComputeBuffer ArgumentsBuffer;

	// Token: 0x04000744 RID: 1860
	private ComputeBuffer GrassPointsBuffer;

	// Token: 0x04000745 RID: 1861
	private const int MAX_GRASS = 10000;

	// Token: 0x04000746 RID: 1862
	private const int DRAW_STRIDE = 148;

	// Token: 0x04000747 RID: 1863
	private const int INDIRECT_DRAW_ARGS_STIDE = 16;

	// Token: 0x04000748 RID: 1864
	private int[] argsBufferReset;

	// Token: 0x04000749 RID: 1865
	public Material m_grassRenderMaterial;

	// Token: 0x0400074A RID: 1866
	private GrassDataProvider DataProvider;
}

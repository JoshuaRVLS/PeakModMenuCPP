using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

// Token: 0x020000C1 RID: 193
public class GrassChunk : GrassDataProvider
{
	// Token: 0x06000765 RID: 1893 RVA: 0x00029EDB File Offset: 0x000280DB
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireCube(base.transform.position + Vector3.one * 50f, Vector3.one * GrassChunking.CHUNK_SIZE);
	}

	// Token: 0x06000766 RID: 1894 RVA: 0x00029F1A File Offset: 0x0002811A
	public override bool IsDirty()
	{
		return this.isDirty;
	}

	// Token: 0x06000767 RID: 1895 RVA: 0x00029F22 File Offset: 0x00028122
	public override ComputeBuffer GetData()
	{
		ComputeBuffer computeBuffer = new ComputeBuffer(this.GrassPoints.Count, UnsafeUtility.SizeOf<GrassPoint>());
		computeBuffer.SetData<GrassPoint>(this.GrassPoints);
		this.isDirty = false;
		return computeBuffer;
	}

	// Token: 0x06000768 RID: 1896 RVA: 0x00029F4C File Offset: 0x0002814C
	public void SetData(List<GrassPoint> grassPoints)
	{
		this.GrassPoints = grassPoints;
		this.isDirty = true;
	}

	// Token: 0x04000738 RID: 1848
	public List<GrassPoint> GrassPoints;

	// Token: 0x04000739 RID: 1849
	public bool isDirty = true;
}

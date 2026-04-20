using System;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x020000C2 RID: 194
public static class GrassChunking
{
	// Token: 0x0600076A RID: 1898 RVA: 0x00029F6C File Offset: 0x0002816C
	public static int3 GetChunkFromPosition(float3 p)
	{
		int x = Mathf.FloorToInt(p.x * GrassChunking.CHUNK_SIZE_INV);
		int y = Mathf.FloorToInt(p.y * GrassChunking.CHUNK_SIZE_INV);
		int z = Mathf.FloorToInt(p.z * GrassChunking.CHUNK_SIZE_INV);
		return new int3(x, y, z);
	}

	// Token: 0x0600076B RID: 1899 RVA: 0x00029FB8 File Offset: 0x000281B8
	public static bool ShouldDrawChunk(int3 cameraChunk, int3 renderChunk)
	{
		return Mathf.Abs(cameraChunk.x - renderChunk.x) <= 1 && Mathf.Abs(cameraChunk.y - renderChunk.y) <= 1 && Mathf.Abs(cameraChunk.z - renderChunk.z) <= 1;
	}

	// Token: 0x0400073A RID: 1850
	public static readonly float CHUNK_SIZE = 35f;

	// Token: 0x0400073B RID: 1851
	public static readonly float CHUNK_SIZE_INV = 1f / GrassChunking.CHUNK_SIZE;
}

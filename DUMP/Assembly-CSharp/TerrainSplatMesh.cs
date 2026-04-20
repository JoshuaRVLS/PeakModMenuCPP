using System;
using UnityEngine;

// Token: 0x0200035A RID: 858
public class TerrainSplatMesh : MonoBehaviour
{
	// Token: 0x060016C1 RID: 5825 RVA: 0x00074FEC File Offset: 0x000731EC
	private Mesh GetMesh()
	{
		if (this.mesh == null)
		{
			this.mesh = base.GetComponent<MeshFilter>().sharedMesh;
			this.verts = this.mesh.vertices;
			this.colors = this.mesh.colors;
		}
		return this.mesh;
	}

	// Token: 0x060016C2 RID: 5826 RVA: 0x00075040 File Offset: 0x00073240
	internal bool PointIsValid(Vector3 point)
	{
		if (this.vertexColorMask)
		{
			this.GetMesh();
			if (HelperFunctions.GetValue(HelperFunctions.GetVertexColorAtPoint(this.verts, this.colors, base.transform, point)) < 0.9f)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0400152A RID: 5418
	public bool vertexColorMask;

	// Token: 0x0400152B RID: 5419
	private Mesh mesh;

	// Token: 0x0400152C RID: 5420
	private Vector3[] verts;

	// Token: 0x0400152D RID: 5421
	private Color[] colors;
}

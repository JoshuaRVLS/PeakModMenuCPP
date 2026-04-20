using System;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000379 RID: 889
public class WallNavigatorTest : MonoBehaviour, ISerializationCallbackReceiver
{
	// Token: 0x06001752 RID: 5970 RVA: 0x0007880A File Offset: 0x00076A0A
	private void Start()
	{
	}

	// Token: 0x06001753 RID: 5971 RVA: 0x0007880C File Offset: 0x00076A0C
	private void Update()
	{
	}

	// Token: 0x06001754 RID: 5972 RVA: 0x00078810 File Offset: 0x00076A10
	private void TryFindValidPath()
	{
		this.color = Color.red;
		if ((from vert in this.triangulation.vertices
		where Vector3.Distance(base.transform.position, vert) < this.sphereSize
		select vert).ToList<Vector3>().Count > 0)
		{
			this.color = Color.green;
		}
	}

	// Token: 0x06001755 RID: 5973 RVA: 0x0007885C File Offset: 0x00076A5C
	private void Print()
	{
		Debug.Log(string.Format("Verts{0}, Indices{1}, Areas{2}", this.triangulation.vertices.Length, this.triangulation.indices.Length, this.triangulation.areas.Length));
	}

	// Token: 0x06001756 RID: 5974 RVA: 0x000788AE File Offset: 0x00076AAE
	private void OnDrawGizmosSelected()
	{
		this.TryFindValidPath();
		Gizmos.color = this.color;
		Gizmos.DrawWireSphere(base.transform.position, this.sphereSize);
	}

	// Token: 0x06001757 RID: 5975 RVA: 0x000788D7 File Offset: 0x00076AD7
	public void OnBeforeSerialize()
	{
		this.triangulation = NavMesh.CalculateTriangulation();
	}

	// Token: 0x06001758 RID: 5976 RVA: 0x000788E4 File Offset: 0x00076AE4
	public void OnAfterDeserialize()
	{
	}

	// Token: 0x040015CE RID: 5582
	public float fDistance = 3f;

	// Token: 0x040015CF RID: 5583
	public NavMeshSurface surface;

	// Token: 0x040015D0 RID: 5584
	private NavMeshTriangulation triangulation;

	// Token: 0x040015D1 RID: 5585
	public float sphereSize;

	// Token: 0x040015D2 RID: 5586
	private Color color;
}

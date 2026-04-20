using System;
using UnityEngine;

// Token: 0x020001AE RID: 430
public class SimpleDrawMesh : MonoBehaviour
{
	// Token: 0x06000DF3 RID: 3571 RVA: 0x0004646B File Offset: 0x0004466B
	private void Start()
	{
		this.GatherPools();
	}

	// Token: 0x06000DF4 RID: 3572 RVA: 0x00046473 File Offset: 0x00044673
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(this.distanceCheckObject.position, this.cullDistance);
	}

	// Token: 0x06000DF5 RID: 3573 RVA: 0x00046495 File Offset: 0x00044695
	private void Update()
	{
		this.drawMeshes();
	}

	// Token: 0x06000DF6 RID: 3574 RVA: 0x000464A0 File Offset: 0x000446A0
	public void drawMeshes()
	{
		if (!this.poolsGathered)
		{
			return;
		}
		if (Character.localCharacter && this.distanceCheckObject && Vector3.Distance(Character.localCharacter.Center, this.distanceCheckObject.position) > this.cullDistance)
		{
			return;
		}
		for (int i = 0; i < this.drawPools.Length; i++)
		{
			Graphics.DrawMeshInstanced(this.drawPools[i].mesh, 0, this.drawPools[i].material, this.drawPools[i].matricies, this.drawPools[i].matricies.Length);
		}
	}

	// Token: 0x06000DF7 RID: 3575 RVA: 0x00046544 File Offset: 0x00044744
	public void GatherPools()
	{
		for (int i = 0; i < this.drawPools.Length; i++)
		{
			Transform[] componentsInChildren = this.drawPools[i].transformsParent.GetComponentsInChildren<Transform>();
			this.drawPools[i].matricies = new Matrix4x4[componentsInChildren.Length];
			for (int j = 1; j < componentsInChildren.Length; j++)
			{
				this.drawPools[i].matricies[j] = Matrix4x4.TRS(componentsInChildren[j].position, componentsInChildren[j].rotation, componentsInChildren[j].localScale);
			}
		}
		this.poolsGathered = true;
	}

	// Token: 0x04000BC7 RID: 3015
	public DrawPool[] drawPools;

	// Token: 0x04000BC8 RID: 3016
	private bool poolsGathered;

	// Token: 0x04000BC9 RID: 3017
	private Matrix4x4[] matrices;

	// Token: 0x04000BCA RID: 3018
	public float cullDistance = 10f;

	// Token: 0x04000BCB RID: 3019
	public Transform distanceCheckObject;
}

using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000101 RID: 257
public class Action_WarpRandomly : ItemAction
{
	// Token: 0x060008BF RID: 2239 RVA: 0x0003003A File Offset: 0x0002E23A
	private void Awake()
	{
		this.raycastDirectionVector = new Vector3(0f, -1f, 1f);
	}

	// Token: 0x060008C0 RID: 2240 RVA: 0x00030058 File Offset: 0x0002E258
	private void TryFindNewPotentialSpot()
	{
		float x = Random.Range(this.minRaycastStartX, this.maxRaycastStartX);
		float y = base.transform.position.y + Random.Range(this.minRaycastRelativeStartY, this.maxRaycastRelativeStartY);
		float z = base.transform.position.z + this.raycastRelativeStartZ;
		Vector3 vector = new Vector3(x, y, z);
		if (Physics.Raycast(vector, this.raycastDirectionVector, out this.cachedHit, 999f, HelperFunctions.terrainMapMask))
		{
			Debug.DrawLine(vector, this.cachedHit.point, Color.red);
			Debug.Break();
			this.unvalidatedPoints.Add(this.cachedHit.point);
		}
	}

	// Token: 0x060008C1 RID: 2241 RVA: 0x00030110 File Offset: 0x0002E310
	private void ValidateRecentPoints(int numberToValidate = 1)
	{
		for (int i = 0; i < numberToValidate; i++)
		{
			if (this.unvalidatedPoints.Count == 0)
			{
				return;
			}
			Vector3 point = this.unvalidatedPoints[0];
			if (this.ValidatePoint(point))
			{
				this.validatedPoints.Add(this.unvalidatedPoints[0]);
			}
			this.unvalidatedPoints.RemoveAt(0);
		}
	}

	// Token: 0x060008C2 RID: 2242 RVA: 0x00030170 File Offset: 0x0002E370
	private bool ValidatePoint(Vector3 point)
	{
		return Vector3.Distance(point, base.transform.position) >= this.minDistanceFromCharacter;
	}

	// Token: 0x0400084D RID: 2125
	public float minimumDistance = 12f;

	// Token: 0x0400084E RID: 2126
	public bool restoreUsesOnFailure = true;

	// Token: 0x0400084F RID: 2127
	public List<Vector3> unvalidatedPoints = new List<Vector3>();

	// Token: 0x04000850 RID: 2128
	public List<Vector3> validatedPoints = new List<Vector3>();

	// Token: 0x04000851 RID: 2129
	public float minRaycastRelativeStartY;

	// Token: 0x04000852 RID: 2130
	public float maxRaycastRelativeStartY;

	// Token: 0x04000853 RID: 2131
	public float minRaycastStartX;

	// Token: 0x04000854 RID: 2132
	public float maxRaycastStartX;

	// Token: 0x04000855 RID: 2133
	public float raycastRelativeStartZ;

	// Token: 0x04000856 RID: 2134
	public float minDistanceFromCharacter = 30f;

	// Token: 0x04000857 RID: 2135
	private Vector3 raycastDirectionVector;

	// Token: 0x04000858 RID: 2136
	private RaycastHit cachedHit;
}

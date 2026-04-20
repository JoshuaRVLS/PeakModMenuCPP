using System;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x0200006D RID: 109
public class PatrolBoss : MonoBehaviour
{
	// Token: 0x0600051C RID: 1308 RVA: 0x0001E307 File Offset: 0x0001C507
	public void Awake()
	{
		PatrolBoss.me = this;
	}

	// Token: 0x0600051D RID: 1309 RVA: 0x0001E310 File Offset: 0x0001C510
	public Vector3 GetPoint()
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(this.point.transform.position + ExtMath.RandInsideUnitCircle().xoy() * 10f, Vector3.down, out raycastHit, 1000f, HelperFunctions.GetMask(HelperFunctions.LayerType.TerrainMap)))
		{
			return raycastHit.point;
		}
		Debug.LogError("This wrong");
		return Vector3.positiveInfinity;
	}

	// Token: 0x04000570 RID: 1392
	public static PatrolBoss me;

	// Token: 0x04000571 RID: 1393
	public GameObject point;
}

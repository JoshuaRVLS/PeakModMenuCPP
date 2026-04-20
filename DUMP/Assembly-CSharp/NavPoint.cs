using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002BA RID: 698
[DefaultExecutionOrder(1000)]
public class NavPoint : MonoBehaviour
{
	// Token: 0x060013B5 RID: 5045 RVA: 0x0006414C File Offset: 0x0006234C
	internal NavPoint GetNext(Vector3 targetDirection)
	{
		List<NavPoint> list = new List<NavPoint>();
		foreach (NavPoint navPoint in this.connections)
		{
			if (HelperFunctions.FlatAngle(targetDirection, navPoint.transform.position - base.transform.position) < 90f)
			{
				list.Add(navPoint);
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		return list[Random.Range(0, list.Count)];
	}

	// Token: 0x060013B6 RID: 5046 RVA: 0x000641EC File Offset: 0x000623EC
	internal void MirrorConnections()
	{
		foreach (NavPoint navPoint in this.connections)
		{
			if (!navPoint.connections.Contains(this))
			{
				navPoint.connections.Add(this);
			}
		}
	}

	// Token: 0x060013B7 RID: 5047 RVA: 0x00064254 File Offset: 0x00062454
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		foreach (NavPoint navPoint in this.connections)
		{
			Gizmos.DrawLine(base.transform.position + Vector3.up * 0.1f, navPoint.transform.position + Vector3.up * 0.1f);
		}
	}

	// Token: 0x04001203 RID: 4611
	public List<NavPoint> connections = new List<NavPoint>();
}

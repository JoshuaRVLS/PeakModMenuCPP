using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002BB RID: 699
public class NavPoints : MonoBehaviour
{
	// Token: 0x060013B9 RID: 5049 RVA: 0x00064303 File Offset: 0x00062503
	private void Awake()
	{
		NavPoints.instance = this;
		this.points = new List<NavPoint>();
		this.points.AddRange(base.GetComponentsInChildren<NavPoint>());
	}

	// Token: 0x060013BA RID: 5050 RVA: 0x00064328 File Offset: 0x00062528
	private void OnDrawGizmos()
	{
		if (!this.drawGizmos)
		{
			return;
		}
		Gizmos.color = Color.blue;
		foreach (NavPoint navPoint in this.points)
		{
			foreach (NavPoint navPoint2 in navPoint.connections)
			{
				Gizmos.DrawLine(navPoint.transform.position, navPoint2.transform.position);
			}
		}
	}

	// Token: 0x060013BB RID: 5051 RVA: 0x000643E0 File Offset: 0x000625E0
	public void ConnectPoints()
	{
		this.points = new List<NavPoint>();
		this.points.AddRange(base.GetComponentsInChildren<NavPoint>());
		foreach (NavPoint point in this.points)
		{
			this.CheckPoint(point);
		}
		foreach (NavPoint navPoint in this.points)
		{
			navPoint.MirrorConnections();
		}
	}

	// Token: 0x060013BC RID: 5052 RVA: 0x00064490 File Offset: 0x00062690
	private void CheckPoint(NavPoint point)
	{
		point.connections = new List<NavPoint>();
		float num = float.PositiveInfinity;
		List<NavPoint> list = new List<NavPoint>();
		foreach (NavPoint navPoint in this.points)
		{
			if (!(navPoint == point) && !HelperFunctions.LineCheck(point.transform.position + Vector3.up, navPoint.transform.position + Vector3.up, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform)
			{
				list.Add(navPoint);
				float num2 = Vector3.Distance(point.transform.position, navPoint.transform.position);
				if (num2 < num)
				{
					num = num2;
				}
			}
		}
		float num3 = num * 1.5f;
		foreach (NavPoint navPoint2 in list)
		{
			if (Vector3.Distance(point.transform.position, navPoint2.transform.position) < num3)
			{
				point.connections.Add(navPoint2);
			}
		}
	}

	// Token: 0x060013BD RID: 5053 RVA: 0x000645E4 File Offset: 0x000627E4
	internal NavPoint GetNavPoint(Vector3 destination, Vector3 currentPos)
	{
		NavPoint result = null;
		float num = float.PositiveInfinity;
		foreach (NavPoint navPoint in this.points)
		{
			float num2 = Vector3.Distance(currentPos, navPoint.transform.position);
			if (num2 <= num && Vector3.Angle(destination - currentPos, navPoint.transform.position - currentPos) <= 90f)
			{
				num = num2;
				result = navPoint;
			}
		}
		return result;
	}

	// Token: 0x04001204 RID: 4612
	public static NavPoints instance;

	// Token: 0x04001205 RID: 4613
	public bool drawGizmos;

	// Token: 0x04001206 RID: 4614
	private List<NavPoint> points = new List<NavPoint>();
}

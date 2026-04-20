using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000AA RID: 170
public class DebugWindThreshold : MonoBehaviour
{
	// Token: 0x06000692 RID: 1682 RVA: 0x00025C7C File Offset: 0x00023E7C
	public void GenerateMap()
	{
		this.ClearMap();
		this.min = this.zone.bounds.min;
		this.max = this.zone.bounds.max;
		Vector3 vector = this.min;
		while (vector.z < this.max.z)
		{
			while (vector.y < this.max.y)
			{
				while (vector.x < this.max.x)
				{
					this.nodes.Add(new DebugWindThreshold.WindNode(vector));
					vector.x += this.nodeSpacing;
				}
				vector.y += this.nodeSpacing;
				vector.x = this.min.x;
			}
			vector.z += this.nodeSpacing;
			vector.y = this.min.y;
			vector.x = this.min.x;
		}
	}

	// Token: 0x06000693 RID: 1683 RVA: 0x00025D80 File Offset: 0x00023F80
	public void ClearMap()
	{
		this.nodes.Clear();
	}

	// Token: 0x06000694 RID: 1684 RVA: 0x00025D90 File Offset: 0x00023F90
	private void OnDrawGizmosSelected()
	{
		for (int i = 0; i < this.nodes.Count; i++)
		{
			float amt;
			if (this.nodes[i].wind > this.lowerThreshold + this.thresholdMargin)
			{
				amt = 1f;
			}
			else if (this.nodes[i].wind < this.lowerThreshold)
			{
				amt = 0f;
			}
			else
			{
				amt = Util.RangeLerp(0f, 1f, this.lowerThreshold, this.lowerThreshold + this.thresholdMargin, this.nodes[i].wind, true, null);
			}
			this.nodes[i].DrawGizmo_HeatMap(amt);
		}
	}

	// Token: 0x04000687 RID: 1671
	[Range(0f, 1f)]
	public float lowerThreshold;

	// Token: 0x04000688 RID: 1672
	[Range(0f, 1f)]
	public float thresholdMargin;

	// Token: 0x04000689 RID: 1673
	public Collider zone;

	// Token: 0x0400068A RID: 1674
	public float nodeSpacing = 5f;

	// Token: 0x0400068B RID: 1675
	public const float MIN_NODE_SPACING = 2f;

	// Token: 0x0400068C RID: 1676
	public List<DebugWindThreshold.WindNode> nodes = new List<DebugWindThreshold.WindNode>();

	// Token: 0x0400068D RID: 1677
	public Vector3 min;

	// Token: 0x0400068E RID: 1678
	public Vector3 max;

	// Token: 0x0200044A RID: 1098
	[Serializable]
	public class WindNode
	{
		// Token: 0x06001C03 RID: 7171 RVA: 0x00086D25 File Offset: 0x00084F25
		public WindNode(Vector3 position)
		{
			this.position = position;
			this.wind = LightVolume.Instance().SamplePositionAlpha(position);
		}

		// Token: 0x06001C04 RID: 7172 RVA: 0x00086D48 File Offset: 0x00084F48
		public void DrawGizmo_HeatMap(float amt)
		{
			Color color;
			if (amt == 1f)
			{
				color = Color.red;
			}
			else if (amt == 0f)
			{
				color = Color.green;
			}
			else
			{
				color = Color.Lerp(Color.yellow, Color.red, amt);
			}
			color.a = 0.5f;
			Gizmos.color = color;
			Gizmos.DrawSphere(this.position, 1f);
		}

		// Token: 0x040018DD RID: 6365
		public float wind;

		// Token: 0x040018DE RID: 6366
		public Vector3 position;
	}
}

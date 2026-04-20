using System;
using System.Collections.Generic;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000176 RID: 374
public class RopeClimbingAPI : MonoBehaviour
{
	// Token: 0x06000C29 RID: 3113 RVA: 0x000416C3 File Offset: 0x0003F8C3
	private void Awake()
	{
		this.rope = base.GetComponent<Rope>();
		this.photonView = base.GetComponentInParent<PhotonView>();
	}

	// Token: 0x06000C2A RID: 3114 RVA: 0x000416DD File Offset: 0x0003F8DD
	public float GetMove()
	{
		return -1f * (1f / this.rope.GetTotalLength());
	}

	// Token: 0x06000C2B RID: 3115 RVA: 0x000416F6 File Offset: 0x0003F8F6
	public float GetPercentFromSegmentIndex(int segmentIndex)
	{
		if (this.rope.SegmentCount <= 1)
		{
			return 0f;
		}
		return (float)segmentIndex / ((float)this.rope.SegmentCount - 1f);
	}

	// Token: 0x06000C2C RID: 3116 RVA: 0x00041724 File Offset: 0x0003F924
	public float GetAngleAtPercent(float percent)
	{
		Transform segmentFromPercent = this.GetSegmentFromPercent(percent);
		Debug.DrawLine(segmentFromPercent.transform.position, segmentFromPercent.transform.position + segmentFromPercent.up, Color.red);
		return segmentFromPercent.GetComponent<RopeSegment>().GetAngle();
	}

	// Token: 0x06000C2D RID: 3117 RVA: 0x00041770 File Offset: 0x0003F970
	public Matrix4x4 GetSegmentMatrixFromPercent(float percent)
	{
		int index = Mathf.RoundToInt(Mathf.Lerp(0f, (float)(this.rope.SegmentCount - 1), percent));
		Transform transform = this.rope.GetRopeSegments()[index];
		return Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
	}

	// Token: 0x06000C2E RID: 3118 RVA: 0x000417C4 File Offset: 0x0003F9C4
	public Vector3 GetUp(float ropePercent)
	{
		Transform segmentFromPercent = this.GetSegmentFromPercent(ropePercent);
		Vector3 vector = segmentFromPercent.up;
		if (Vector3.Angle(Vector3.up, segmentFromPercent.up) > 90f)
		{
			vector *= -1f;
		}
		return vector;
	}

	// Token: 0x06000C2F RID: 3119 RVA: 0x00041804 File Offset: 0x0003FA04
	public float UpMult(float percent)
	{
		return (float)((Vector3.Angle(Vector3.up, this.GetSegmentFromPercent(percent).up) < 90f) ? -1 : 1);
	}

	// Token: 0x06000C30 RID: 3120 RVA: 0x00041828 File Offset: 0x0003FA28
	public Vector3 GetPosition(float percent)
	{
		percent = Mathf.Clamp01(percent);
		float num = percent * (float)(this.rope.SegmentCount - 1);
		int num2 = Mathf.FloorToInt(num);
		int num3 = num2;
		if (num2 == 0)
		{
			num2 = 1;
		}
		if (percent < 1f)
		{
			num3 = num2 + 1;
		}
		float t = num - (float)num2;
		List<Transform> ropeSegments = this.rope.GetRopeSegments();
		num2 = math.clamp(num2, 0, ropeSegments.Count - 1);
		num3 = math.clamp(num3, num2, ropeSegments.Count - 1);
		return Vector3.Lerp(ropeSegments[num2].position, ropeSegments[num3].position, t);
	}

	// Token: 0x06000C31 RID: 3121 RVA: 0x000418B8 File Offset: 0x0003FAB8
	public Transform GetSegmentFromPercent(float percent)
	{
		percent = Mathf.Clamp01(percent);
		float num = percent * (float)(this.rope.SegmentCount - 1);
		int num2 = Mathf.FloorToInt(num);
		int num3 = num2;
		if (num2 == 0)
		{
			num2 = 1;
		}
		if (percent < 1f)
		{
			num3 = num2 + 1;
		}
		float num4 = num - (float)num2;
		List<Transform> ropeSegments = this.rope.GetRopeSegments();
		num2 = math.clamp(num2, 0, ropeSegments.Count - 1);
		num3 = math.clamp(num3, num2, ropeSegments.Count - 1);
		return ropeSegments[(num4 > 0.5f) ? num3 : num2];
	}

	// Token: 0x04000B25 RID: 2853
	private Rope rope;

	// Token: 0x04000B26 RID: 2854
	private PhotonView photonView;
}

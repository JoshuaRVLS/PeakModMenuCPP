using System;
using UnityEngine;

// Token: 0x0200010C RID: 268
public class CompassPointer : MonoBehaviour
{
	// Token: 0x060008F1 RID: 2289 RVA: 0x00030DF8 File Offset: 0x0002EFF8
	private void Awake()
	{
		this.item = base.GetComponentInParent<Item>();
	}

	// Token: 0x060008F2 RID: 2290 RVA: 0x00030E06 File Offset: 0x0002F006
	private void Update()
	{
		this.UpdateHeading();
	}

	// Token: 0x060008F3 RID: 2291 RVA: 0x00030E10 File Offset: 0x0002F010
	protected void UpdateHeading()
	{
		bool flag = true;
		switch (this.compassType)
		{
		case CompassPointer.CompassType.Normal:
			this.heading = Vector3.forward;
			break;
		case CompassPointer.CompassType.Warp:
			flag = false;
			this.needle.RotateAround(this.needle.transform.position, this.needle.right, this.warpSpeed * Time.deltaTime * this.speedMultiplier);
			break;
		case CompassPointer.CompassType.Pirate:
			this.UpdateHeadingPirate();
			break;
		}
		if (flag)
		{
			this.heading = Vector3.ProjectOnPlane(this.heading, base.transform.forward);
			this.needle.transform.rotation = Quaternion.LookRotation(Vector3.Slerp(this.needle.transform.forward, this.heading, 10f * Time.deltaTime), base.transform.up);
		}
	}

	// Token: 0x060008F4 RID: 2292 RVA: 0x00030EF0 File Offset: 0x0002F0F0
	protected void UpdateHeadingPirate()
	{
		if (Luggage.ALL_LUGGAGE.Count == 0)
		{
			this.heading = Quaternion.Euler(0f, Time.time * this.warpSpeed, 0f) * Vector3.forward;
		}
		if (this.item.inActiveList)
		{
			float num = float.MaxValue;
			foreach (Luggage luggage in Luggage.ALL_LUGGAGE)
			{
				if (Vector3.Distance(luggage.Center(), base.transform.position) < num)
				{
					num = Vector3.Distance(luggage.Center(), base.transform.position);
					this.currentLuggageVector = luggage.Center() - base.transform.position;
				}
			}
			this.heading = this.currentLuggageVector;
		}
	}

	// Token: 0x04000883 RID: 2179
	public CompassPointer.CompassType compassType;

	// Token: 0x04000884 RID: 2180
	public Transform needle;

	// Token: 0x04000885 RID: 2181
	public float warpSpeed = 2f;

	// Token: 0x04000886 RID: 2182
	public float speedMultiplier = 1f;

	// Token: 0x04000887 RID: 2183
	private Item item;

	// Token: 0x04000888 RID: 2184
	protected Vector3 heading;

	// Token: 0x04000889 RID: 2185
	private Vector3 currentLuggageVector = Vector3.zero;

	// Token: 0x0200046D RID: 1133
	public enum CompassType
	{
		// Token: 0x04001972 RID: 6514
		Normal,
		// Token: 0x04001973 RID: 6515
		Warp,
		// Token: 0x04001974 RID: 6516
		Pirate
	}
}

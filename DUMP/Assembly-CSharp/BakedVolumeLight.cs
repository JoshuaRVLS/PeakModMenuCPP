using System;
using UnityEngine;

// Token: 0x02000135 RID: 309
public class BakedVolumeLight : MonoBehaviour
{
	// Token: 0x060009EA RID: 2538 RVA: 0x00034C70 File Offset: 0x00032E70
	public float GetRadius()
	{
		if (this.scaleWithLossyScale > 0f)
		{
			float num = Mathf.Max(new float[]
			{
				base.transform.lossyScale.x,
				base.transform.lossyScale.y,
				base.transform.lossyScale.z
			});
			return Mathf.Lerp(this.radius, this.radius * num, this.scaleWithLossyScale);
		}
		return this.radius;
	}

	// Token: 0x060009EB RID: 2539 RVA: 0x00034CF0 File Offset: 0x00032EF0
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = this.color;
		float d = this.GetRadius();
		BakedVolumeLight.LightModes lightModes = this.mode;
		if (lightModes == BakedVolumeLight.LightModes.Point)
		{
			Gizmos.DrawWireSphere(base.transform.position, d);
			return;
		}
		if (lightModes != BakedVolumeLight.LightModes.Spot)
		{
			return;
		}
		Vector3 vector = base.transform.position + base.transform.forward * d;
		Gizmos.DrawLine(base.transform.position, vector);
		float d2 = this.coneSize * 0.034906585f;
		Vector3[] array = new Vector3[]
		{
			vector + base.transform.up * d2 * d,
			vector + base.transform.right * d2 * d,
			vector + -base.transform.up * d2 * d,
			vector + -base.transform.right * d2 * d
		};
		foreach (Vector3 to in array)
		{
			Gizmos.DrawLine(base.transform.position, to);
		}
		Gizmos.DrawLineStrip(array, true);
	}

	// Token: 0x060009EC RID: 2540 RVA: 0x00034E53 File Offset: 0x00033053
	public void Rebake()
	{
		Object.FindAnyObjectByType<LightVolume>().Bake(null);
	}

	// Token: 0x04000946 RID: 2374
	public BakedVolumeLight.LightModes mode;

	// Token: 0x04000947 RID: 2375
	public Color color = Color.white;

	// Token: 0x04000948 RID: 2376
	public float intensity = 1f;

	// Token: 0x04000949 RID: 2377
	public float radius = 10f;

	// Token: 0x0400094A RID: 2378
	[Range(0f, 1f)]
	public float falloff = 0.5f;

	// Token: 0x0400094B RID: 2379
	[Range(0f, 1f)]
	[Tooltip("Percentage width at which the light should be full brightness. 1.0 means the entire cone is full bright, 0.0 means that the fade lerp starts immediately in the center")]
	public float coneFalloff = 0.9f;

	// Token: 0x0400094C RID: 2380
	[Range(0f, 90f)]
	public float coneSize = 30f;

	// Token: 0x0400094D RID: 2381
	[Range(0f, 1f)]
	public float scaleWithLossyScale;

	// Token: 0x02000479 RID: 1145
	public enum LightModes
	{
		// Token: 0x04001999 RID: 6553
		Point,
		// Token: 0x0400199A RID: 6554
		Spot
	}
}

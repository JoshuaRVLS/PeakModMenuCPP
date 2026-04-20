using System;
using UnityEngine;

// Token: 0x02000382 RID: 898
public class WindHeightEffect : MonoBehaviour
{
	// Token: 0x06001775 RID: 6005 RVA: 0x0007904C File Offset: 0x0007724C
	private void Start()
	{
		this.zone = base.GetComponent<WindChillZone>();
	}

	// Token: 0x06001776 RID: 6006 RVA: 0x0007905C File Offset: 0x0007725C
	private void Update()
	{
		if (Character.observedCharacter == null)
		{
			return;
		}
		this.zone.lightVolumeSampleThreshold_lower = Mathf.Lerp(this.from, this.to, Mathf.InverseLerp(this.fromHeight, this.toHeight, Character.observedCharacter.Center.y));
	}

	// Token: 0x040015EE RID: 5614
	public float from;

	// Token: 0x040015EF RID: 5615
	public float to;

	// Token: 0x040015F0 RID: 5616
	public float fromHeight;

	// Token: 0x040015F1 RID: 5617
	public float toHeight;

	// Token: 0x040015F2 RID: 5618
	private WindChillZone zone;
}

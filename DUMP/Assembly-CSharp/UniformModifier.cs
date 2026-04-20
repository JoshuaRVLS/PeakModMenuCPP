using System;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

// Token: 0x020002DF RID: 735
[ModifierID("Uniform")]
public class UniformModifier : ProceduralImageModifier
{
	// Token: 0x17000152 RID: 338
	// (get) Token: 0x06001476 RID: 5238 RVA: 0x000676E4 File Offset: 0x000658E4
	// (set) Token: 0x06001477 RID: 5239 RVA: 0x000676EC File Offset: 0x000658EC
	public float Radius
	{
		get
		{
			return this.radius;
		}
		set
		{
			this.radius = value;
			base._Graphic.SetVerticesDirty();
		}
	}

	// Token: 0x06001478 RID: 5240 RVA: 0x00067700 File Offset: 0x00065900
	public override Vector4 CalculateRadius(Rect imageRect)
	{
		float num = this.radius;
		return new Vector4(num, num, num, num);
	}

	// Token: 0x040012A6 RID: 4774
	[SerializeField]
	private float radius;
}

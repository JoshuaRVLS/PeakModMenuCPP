using System;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

// Token: 0x020002DD RID: 733
[ModifierID("Only One Edge")]
public class OnlyOneEdgeModifier : ProceduralImageModifier
{
	// Token: 0x17000150 RID: 336
	// (get) Token: 0x0600146E RID: 5230 RVA: 0x000675CD File Offset: 0x000657CD
	// (set) Token: 0x0600146F RID: 5231 RVA: 0x000675D5 File Offset: 0x000657D5
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

	// Token: 0x17000151 RID: 337
	// (get) Token: 0x06001470 RID: 5232 RVA: 0x000675E9 File Offset: 0x000657E9
	// (set) Token: 0x06001471 RID: 5233 RVA: 0x000675F1 File Offset: 0x000657F1
	public OnlyOneEdgeModifier.ProceduralImageEdge Side
	{
		get
		{
			return this.side;
		}
		set
		{
			this.side = value;
		}
	}

	// Token: 0x06001472 RID: 5234 RVA: 0x000675FC File Offset: 0x000657FC
	public override Vector4 CalculateRadius(Rect imageRect)
	{
		switch (this.side)
		{
		case OnlyOneEdgeModifier.ProceduralImageEdge.Top:
			return new Vector4(this.radius, this.radius, 0f, 0f);
		case OnlyOneEdgeModifier.ProceduralImageEdge.Bottom:
			return new Vector4(0f, 0f, this.radius, this.radius);
		case OnlyOneEdgeModifier.ProceduralImageEdge.Left:
			return new Vector4(this.radius, 0f, 0f, this.radius);
		case OnlyOneEdgeModifier.ProceduralImageEdge.Right:
			return new Vector4(0f, this.radius, this.radius, 0f);
		default:
			return new Vector4(0f, 0f, 0f, 0f);
		}
	}

	// Token: 0x040012A4 RID: 4772
	[SerializeField]
	private float radius;

	// Token: 0x040012A5 RID: 4773
	[SerializeField]
	private OnlyOneEdgeModifier.ProceduralImageEdge side;

	// Token: 0x02000524 RID: 1316
	public enum ProceduralImageEdge
	{
		// Token: 0x04001C69 RID: 7273
		Top,
		// Token: 0x04001C6A RID: 7274
		Bottom,
		// Token: 0x04001C6B RID: 7275
		Left,
		// Token: 0x04001C6C RID: 7276
		Right
	}
}

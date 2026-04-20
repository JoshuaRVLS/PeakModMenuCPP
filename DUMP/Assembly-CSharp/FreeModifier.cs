using System;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

// Token: 0x020002DC RID: 732
[ModifierID("Free")]
public class FreeModifier : ProceduralImageModifier
{
	// Token: 0x1700014F RID: 335
	// (get) Token: 0x06001469 RID: 5225 RVA: 0x00067512 File Offset: 0x00065712
	// (set) Token: 0x0600146A RID: 5226 RVA: 0x0006751A File Offset: 0x0006571A
	public Vector4 Radius
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

	// Token: 0x0600146B RID: 5227 RVA: 0x0006752E File Offset: 0x0006572E
	public override Vector4 CalculateRadius(Rect imageRect)
	{
		return this.radius;
	}

	// Token: 0x0600146C RID: 5228 RVA: 0x00067538 File Offset: 0x00065738
	protected void OnValidate()
	{
		this.radius.x = Mathf.Max(0f, this.radius.x);
		this.radius.y = Mathf.Max(0f, this.radius.y);
		this.radius.z = Mathf.Max(0f, this.radius.z);
		this.radius.w = Mathf.Max(0f, this.radius.w);
	}

	// Token: 0x040012A3 RID: 4771
	[SerializeField]
	private Vector4 radius;
}

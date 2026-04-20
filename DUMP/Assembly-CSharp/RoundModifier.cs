using System;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

// Token: 0x020002DE RID: 734
[ModifierID("Round")]
public class RoundModifier : ProceduralImageModifier
{
	// Token: 0x06001474 RID: 5236 RVA: 0x000676B9 File Offset: 0x000658B9
	public override Vector4 CalculateRadius(Rect imageRect)
	{
		float num = Mathf.Min(imageRect.width, imageRect.height) * 0.5f;
		return new Vector4(num, num, num, num);
	}
}

using System;
using UnityEngine;

// Token: 0x020002DB RID: 731
public static class EmptySprite
{
	// Token: 0x06001467 RID: 5223 RVA: 0x000674DD File Offset: 0x000656DD
	public static Sprite Get()
	{
		if (EmptySprite.instance == null)
		{
			EmptySprite.instance = Resources.Load<Sprite>("procedural_ui_image_default_sprite");
		}
		return EmptySprite.instance;
	}

	// Token: 0x06001468 RID: 5224 RVA: 0x00067500 File Offset: 0x00065700
	public static bool IsEmptySprite(Sprite s)
	{
		return EmptySprite.Get() == s;
	}

	// Token: 0x040012A2 RID: 4770
	private static Sprite instance;
}

using System;
using TMPro;
using UnityEngine;

// Token: 0x0200011B RID: 283
public class GuidebookSpread : MonoBehaviour
{
	// Token: 0x06000947 RID: 2375 RVA: 0x00032200 File Offset: 0x00030400
	internal void SetPageLeft(RectTransform prefab)
	{
		if (this.pageLeftTransform != null)
		{
			Object.DestroyImmediate(this.pageLeftTransform.gameObject);
		}
		this.pageLeftTransform = Object.Instantiate<RectTransform>(prefab, base.transform);
		this.pageLeftTransform.offsetMax = new Vector2(-this.page1AlignmentRight, -this.page1AlignmentTop);
		this.pageLeftTransform.offsetMin = new Vector2(this.page1AlignmentLeft, this.page1AlignmentBottom);
	}

	// Token: 0x06000948 RID: 2376 RVA: 0x00032278 File Offset: 0x00030478
	internal void SetPageRight(RectTransform prefab)
	{
		if (this.pageRightTransform != null)
		{
			Object.DestroyImmediate(this.pageRightTransform.gameObject);
		}
		this.pageRightTransform = Object.Instantiate<RectTransform>(prefab, base.transform);
		this.pageRightTransform.offsetMax = new Vector2(-this.page1AlignmentLeft, -this.page1AlignmentTop);
		this.pageRightTransform.offsetMin = new Vector2(this.page1AlignmentRight, this.page1AlignmentTop);
	}

	// Token: 0x06000949 RID: 2377 RVA: 0x000322F0 File Offset: 0x000304F0
	internal void ClearContents()
	{
		for (int i = base.transform.childCount - 1; i >= 0; i--)
		{
			Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
		}
	}

	// Token: 0x040008B3 RID: 2227
	public TextMeshProUGUI pageNumberLeft;

	// Token: 0x040008B4 RID: 2228
	public TextMeshProUGUI pageNumberRight;

	// Token: 0x040008B5 RID: 2229
	public RectTransform pageLeftTransform;

	// Token: 0x040008B6 RID: 2230
	public RectTransform pageRightTransform;

	// Token: 0x040008B7 RID: 2231
	public float page1AlignmentLeft;

	// Token: 0x040008B8 RID: 2232
	public float page1AlignmentRight;

	// Token: 0x040008B9 RID: 2233
	public float page1AlignmentTop;

	// Token: 0x040008BA RID: 2234
	public float page1AlignmentBottom;
}

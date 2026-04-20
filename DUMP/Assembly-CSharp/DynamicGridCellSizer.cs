using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200025C RID: 604
[ExecuteAlways]
[RequireComponent(typeof(GridLayoutGroup))]
public class DynamicGridCellSizer : MonoBehaviour
{
	// Token: 0x0600120C RID: 4620 RVA: 0x0005A943 File Offset: 0x00058B43
	private void Awake()
	{
		this.grid = base.GetComponent<GridLayoutGroup>();
	}

	// Token: 0x0600120D RID: 4621 RVA: 0x0005A951 File Offset: 0x00058B51
	private void Update()
	{
		if (base.transform.childCount != this.childCount)
		{
			this.childCount = base.transform.childCount;
			this.ResizeCells();
		}
	}

	// Token: 0x0600120E RID: 4622 RVA: 0x0005A980 File Offset: 0x00058B80
	public void ResizeCells()
	{
		this.iconCount = this.grid.transform.childCount;
		float width = this.gridRectTransform.rect.width;
		float height = this.gridRectTransform.rect.height;
		int num = Mathf.Max(1, Mathf.CeilToInt((float)this.iconCount / (float)this.maxIconsPerRow));
		int num2 = Mathf.CeilToInt((float)this.iconCount / (float)num);
		float a = (width - (float)this.grid.padding.left - (float)this.grid.padding.right - this.grid.spacing.x * (float)(num2 - 1)) / (float)num2;
		float b = (height - (float)this.grid.padding.top - (float)this.grid.padding.bottom - this.grid.spacing.y * (float)(num - 1)) / (float)num;
		float num3 = Mathf.Min(a, b);
		this.grid.cellSize = new Vector2(num3, num3);
	}

	// Token: 0x0400100E RID: 4110
	public RectTransform gridRectTransform;

	// Token: 0x0400100F RID: 4111
	public int iconCount;

	// Token: 0x04001010 RID: 4112
	public int maxIconsPerRow = 8;

	// Token: 0x04001011 RID: 4113
	private GridLayoutGroup grid;

	// Token: 0x04001012 RID: 4114
	private int childCount = -1;
}

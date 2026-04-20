using System;
using UnityEngine;

// Token: 0x02000242 RID: 578
public class CursorAnimation : MonoBehaviour
{
	// Token: 0x060011A4 RID: 4516 RVA: 0x00058F64 File Offset: 0x00057164
	private void Start()
	{
		Cursor.SetCursor(this.cursorOpen, this.cursorHotspot, CursorMode.Auto);
	}

	// Token: 0x060011A5 RID: 4517 RVA: 0x00058F78 File Offset: 0x00057178
	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Cursor.SetCursor(this.curserClosed, this.cursorHotspot, CursorMode.Auto);
			return;
		}
		if (Input.GetMouseButtonUp(0))
		{
			Cursor.SetCursor(this.cursorOpen, this.cursorHotspot, CursorMode.Auto);
		}
	}

	// Token: 0x04000F90 RID: 3984
	public Texture2D cursorOpen;

	// Token: 0x04000F91 RID: 3985
	public Texture2D curserClosed;

	// Token: 0x04000F92 RID: 3986
	private Vector2 cursorHotspot = new Vector2(32f, 32f);
}

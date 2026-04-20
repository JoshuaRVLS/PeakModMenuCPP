using System;
using UnityEngine;
using Zorro.ControllerSupport;
using Zorro.Core;
using Zorro.Core.CLI;
using Zorro.UI.Modal;

// Token: 0x02000243 RID: 579
public class CursorHandler : Singleton<CursorHandler>
{
	// Token: 0x060011A7 RID: 4519 RVA: 0x00058FCC File Offset: 0x000571CC
	private void Update()
	{
		if (InputHandler.GetCurrentUsedInputScheme() != InputScheme.KeyboardMouse || (!this.isMenuScene && !DebugUIHandler.IsOpen && (!(GUIManager.instance != null) || (!GUIManager.instance.windowShowingCursor && !GUIManager.instance.wheelActive)) && !Modal.IsOpen))
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			return;
		}
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	// Token: 0x04000F93 RID: 3987
	public bool isMenuScene;
}

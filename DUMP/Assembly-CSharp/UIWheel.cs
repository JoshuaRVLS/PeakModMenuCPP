using System;
using UnityEngine;
using Zorro.ControllerSupport;
using Zorro.Core;

// Token: 0x020001F2 RID: 498
public class UIWheel : MonoBehaviour
{
	// Token: 0x06000FBD RID: 4029 RVA: 0x0004D593 File Offset: 0x0004B793
	protected virtual Vector2 GetCursorOrigin()
	{
		return new Vector2(base.transform.position.x, base.transform.position.y);
	}

	// Token: 0x06000FBE RID: 4030 RVA: 0x0004D5BA File Offset: 0x0004B7BA
	protected virtual void Update()
	{
		if (InputHandler.GetCurrentUsedInputScheme() == InputScheme.Gamepad)
		{
			this.TestGamepadInput();
		}
	}

	// Token: 0x06000FBF RID: 4031 RVA: 0x0004D5CC File Offset: 0x0004B7CC
	protected void TestGamepadInput()
	{
		Vector2 wheelNavigationVector = Singleton<UIInputHandler>.Instance.wheelNavigationVector;
		this.TestSelectSliceGamepad(wheelNavigationVector);
	}

	// Token: 0x06000FC0 RID: 4032 RVA: 0x0004D5EB File Offset: 0x0004B7EB
	protected virtual void TestSelectSliceGamepad(Vector2 gamepadVector)
	{
	}

	// Token: 0x04000D5E RID: 3422
	public float maxCursorDistance;
}

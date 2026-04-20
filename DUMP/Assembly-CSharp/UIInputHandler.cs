using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Zorro.ControllerSupport;
using Zorro.Core;

// Token: 0x020001F5 RID: 501
[DefaultExecutionOrder(-1000)]
public class UIInputHandler : Singleton<UIInputHandler>
{
	// Token: 0x17000123 RID: 291
	// (get) Token: 0x06000FC8 RID: 4040 RVA: 0x0004D700 File Offset: 0x0004B900
	// (set) Token: 0x06000FC9 RID: 4041 RVA: 0x0004D708 File Offset: 0x0004B908
	public Vector2 wheelNavigationVector { get; private set; }

	// Token: 0x06000FCA RID: 4042 RVA: 0x0004D714 File Offset: 0x0004B914
	public void Initialize()
	{
		UIInputHandler.action_confirm = InputSystem.actions.FindAction("UIConfirm", false);
		UIInputHandler.action_cancel = InputSystem.actions.FindAction("UICancel", false);
		UIInputHandler.action_tabLeft = InputSystem.actions.FindAction("UITabLeft", false);
		UIInputHandler.action_tabRight = InputSystem.actions.FindAction("UITabRight", false);
		UIInputHandler.action_navigateWheel = InputSystem.actions.FindAction("NavigateWheel", false);
		InputHandler instance = RetrievableResourceSingleton<InputHandler>.Instance;
		instance.InputSchemeChanged = (Action<InputScheme>)Delegate.Combine(instance.InputSchemeChanged, new Action<InputScheme>(this.OnInputSchemeChanged));
	}

	// Token: 0x06000FCB RID: 4043 RVA: 0x0004D7B0 File Offset: 0x0004B9B0
	public override void OnDestroy()
	{
		base.OnDestroy();
		InputHandler instance = RetrievableResourceSingleton<InputHandler>.Instance;
		instance.InputSchemeChanged = (Action<InputScheme>)Delegate.Remove(instance.InputSchemeChanged, new Action<InputScheme>(this.OnInputSchemeChanged));
	}

	// Token: 0x06000FCC RID: 4044 RVA: 0x0004D7DE File Offset: 0x0004B9DE
	private void Update()
	{
		this.Sample();
	}

	// Token: 0x06000FCD RID: 4045 RVA: 0x0004D7E8 File Offset: 0x0004B9E8
	private void Sample()
	{
		this.confirmWasPressed = UIInputHandler.action_confirm.WasPressedThisFrame();
		this.cancelWasPressed = UIInputHandler.action_cancel.WasPressedThisFrame();
		this.tabLeftWasPressed = UIInputHandler.action_tabLeft.WasPressedThisFrame();
		this.tabRightWasPressed = UIInputHandler.action_tabRight.WasPressedThisFrame();
		this.wheelNavigationVector = UIInputHandler.action_navigateWheel.ReadValue<Vector2>();
	}

	// Token: 0x06000FCE RID: 4046 RVA: 0x0004D845 File Offset: 0x0004BA45
	private void OnInputSchemeChanged(InputScheme scheme)
	{
	}

	// Token: 0x06000FCF RID: 4047 RVA: 0x0004D847 File Offset: 0x0004BA47
	public static void SetSelectedObject(GameObject obj)
	{
		if (InputHandler.GetCurrentUsedInputScheme() == InputScheme.Gamepad)
		{
			EventSystem.current.SetSelectedGameObject(obj);
		}
	}

	// Token: 0x06000FD0 RID: 4048 RVA: 0x0004D85C File Offset: 0x0004BA5C
	private void Deselect()
	{
		EventSystem.current.SetSelectedGameObject(null);
	}

	// Token: 0x06000FD1 RID: 4049 RVA: 0x0004D869 File Offset: 0x0004BA69
	private void SelectPrevious()
	{
		EventSystem.current.SetSelectedGameObject(UIInputHandler.previouslySelectedControllerElement);
	}

	// Token: 0x04000D68 RID: 3432
	public static InputAction action_confirm;

	// Token: 0x04000D69 RID: 3433
	public static InputAction action_cancel;

	// Token: 0x04000D6A RID: 3434
	public static InputAction action_tabLeft;

	// Token: 0x04000D6B RID: 3435
	public static InputAction action_tabRight;

	// Token: 0x04000D6C RID: 3436
	public static InputAction action_navigateWheel;

	// Token: 0x04000D6D RID: 3437
	public bool confirmWasPressed;

	// Token: 0x04000D6E RID: 3438
	public bool cancelWasPressed;

	// Token: 0x04000D6F RID: 3439
	public bool tabLeftWasPressed;

	// Token: 0x04000D70 RID: 3440
	public bool tabRightWasPressed;

	// Token: 0x04000D72 RID: 3442
	internal static GameObject previouslySelectedControllerElement;
}

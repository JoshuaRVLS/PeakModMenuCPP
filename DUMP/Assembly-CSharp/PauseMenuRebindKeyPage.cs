using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Zorro.ControllerSupport;
using Zorro.Core;
using Zorro.UI;

// Token: 0x020001E5 RID: 485
public class PauseMenuRebindKeyPage : UIPage, INavigationPage
{
	// Token: 0x06000F6F RID: 3951 RVA: 0x0004BB71 File Offset: 0x00049D71
	private void Awake()
	{
		this.action_pause = InputSystem.actions.FindAction("Pause", false);
	}

	// Token: 0x06000F70 RID: 3952 RVA: 0x0004BB8C File Offset: 0x00049D8C
	private void Update()
	{
		if (this.action_pause.WasPressedThisFrame() && this.rebindOperation != null && this.rebindOperation.started && !this.rebindOperation.completed)
		{
			Debug.Log("CANCEL REBINDING " + PauseMenuRebindKeyPage.inputAction.name);
			this.rebindOperation.Cancel();
			this.rebindOperation.Dispose();
		}
	}

	// Token: 0x06000F71 RID: 3953 RVA: 0x0004BBF7 File Offset: 0x00049DF7
	public GameObject GetFirstSelectedGameObject()
	{
		return this.dummyButton;
	}

	// Token: 0x06000F72 RID: 3954 RVA: 0x0004BC00 File Offset: 0x00049E00
	public override void OnPageEnter()
	{
		base.OnPageEnter();
		if (PauseMenuRebindKeyPage.inputAction == null)
		{
			return;
		}
		if (PauseMenuRebindKeyPage.inputAction.name == "Pause")
		{
			return;
		}
		int bindingIndex = -1;
		for (int i = 0; i < PauseMenuRebindKeyPage.inputAction.bindings.Count; i++)
		{
			if (PauseMenuRebindKeyPage.inputAction.bindings[i].groups.Contains("Keyboard&Mouse") && PauseMenuRebindKeyPage.forcedInputScheme == InputScheme.KeyboardMouse)
			{
				bindingIndex = i;
				break;
			}
			if (PauseMenuRebindKeyPage.inputAction.bindings[i].groups.Contains("Gamepad") && PauseMenuRebindKeyPage.forcedInputScheme == InputScheme.Gamepad)
			{
				bindingIndex = i;
				break;
			}
		}
		PauseMenuRebindKeyPage.inputAction.Disable();
		this.rebindOperation = PauseMenuRebindKeyPage.inputAction.PerformInteractiveRebinding(bindingIndex).WithControlsExcluding("<Mouse>/position").WithControlsExcluding("<Mouse>/delta").WithControlsExcluding("<Gamepad>/Start").WithControlsExcluding("<Gamepad>/leftStick/left").WithControlsExcluding("<Gamepad>/leftStick/right").WithControlsExcluding("<Gamepad>/leftStick/up").WithControlsExcluding("<Gamepad>/leftStick/down").WithControlsExcluding("<Gamepad>/rightStick/left").WithControlsExcluding("<Gamepad>/rightStick/right").WithControlsExcluding("<Gamepad>/rightStick/up").WithControlsExcluding("<Gamepad>/rightStick/down").WithControlsExcluding("<Keyboard>/leftMeta").WithControlsExcluding("<Keyboard>/rightMeta").WithControlsExcluding("<Keyboard>/contextMenu").WithControlsExcluding("<Keyboard>/anyKey").WithCancelingThrough("<Keyboard>/escape").WithCancelingThrough("<Gamepad>/Start").WithControlsExcluding("<Keyboard>/escape").OnComplete(delegate(InputActionRebindingExtensions.RebindingOperation operation)
		{
			this.Completed();
		}).OnCancel(delegate(InputActionRebindingExtensions.RebindingOperation operation)
		{
			this.Cancelled();
		});
		if (PauseMenuRebindKeyPage.forcedInputScheme == InputScheme.Gamepad)
		{
			this.rebindOperation = this.rebindOperation.WithControlsExcluding("<Keyboard>").WithControlsExcluding("<Mouse>");
		}
		else if (PauseMenuRebindKeyPage.forcedInputScheme == InputScheme.KeyboardMouse)
		{
			this.rebindOperation = this.rebindOperation.WithControlsExcluding("<Gamepad>");
		}
		this.rebindOperation.Start();
		this.promptText.text = LocalizedText.GetText("PROMPT_REBIND", true).Replace("@", LocalizedText.GetText(PauseMenuRebindKeyPage.inputLocIndex, true));
	}

	// Token: 0x06000F73 RID: 3955 RVA: 0x0004BE28 File Offset: 0x0004A028
	private void Completed()
	{
		Debug.Log(string.Format("FINISHED REBINDING {0} to {1}", PauseMenuRebindKeyPage.inputAction.name, this.rebindOperation.selectedControl));
		foreach (InputBinding inputBinding in PauseMenuRebindKeyPage.inputAction.bindings)
		{
			Debug.Log("Checking against " + inputBinding.path);
			if (InputSpriteData.GetPathEnd(inputBinding.path) == InputSpriteData.GetPathEnd(this.rebindOperation.selectedControl.path))
			{
				this.rebindOperation.action.RemoveAllBindingOverrides();
			}
		}
		this.rebindOperation.Dispose();
		PauseMenuRebindKeyPage.inputAction.Enable();
		Rebinding.SaveRebindingsToFile(null);
		Action<InputScheme> inputSchemeChanged = RetrievableResourceSingleton<InputHandler>.Instance.InputSchemeChanged;
		if (inputSchemeChanged != null)
		{
			inputSchemeChanged(InputHandler.GetCurrentUsedInputScheme());
		}
		base.StartCoroutine(this.ReturnRoutine());
	}

	// Token: 0x06000F74 RID: 3956 RVA: 0x0004BF30 File Offset: 0x0004A130
	private void Cancelled()
	{
		this.rebindOperation.Dispose();
		PauseMenuRebindKeyPage.inputAction.Enable();
		base.StartCoroutine(this.ReturnRoutine());
	}

	// Token: 0x06000F75 RID: 3957 RVA: 0x0004BF54 File Offset: 0x0004A154
	private IEnumerator ReturnRoutine()
	{
		yield return null;
		this.pageHandler.TransistionToPage<PauseMenuControlsPage>();
		yield break;
	}

	// Token: 0x06000F76 RID: 3958 RVA: 0x0004BF64 File Offset: 0x0004A164
	public override void OnPageExit()
	{
		if (this.rebindOperation != null && this.rebindOperation.started && !this.rebindOperation.completed)
		{
			Debug.Log("CANCEL REBINDING " + PauseMenuRebindKeyPage.inputAction.name);
			this.rebindOperation.Cancel();
			this.rebindOperation.Dispose();
		}
	}

	// Token: 0x04000CF6 RID: 3318
	private InputActionRebindingExtensions.RebindingOperation rebindOperation;

	// Token: 0x04000CF7 RID: 3319
	public GameObject dummyButton;

	// Token: 0x04000CF8 RID: 3320
	internal static InputAction inputAction;

	// Token: 0x04000CF9 RID: 3321
	internal static string inputLocIndex;

	// Token: 0x04000CFA RID: 3322
	public TextMeshProUGUI promptText;

	// Token: 0x04000CFB RID: 3323
	internal static InputScheme forcedInputScheme;

	// Token: 0x04000CFC RID: 3324
	private InputAction action_pause;
}

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zorro.ControllerSupport;
using Zorro.Core;
using Zorro.UI;

// Token: 0x020001E4 RID: 484
public class PauseMenuRebindButton : MonoBehaviour
{
	// Token: 0x06000F68 RID: 3944 RVA: 0x0004B9D4 File Offset: 0x00049BD4
	private void Awake()
	{
		this.inputAction = InputSystem.actions.FindAction(this.inputActionName, false);
		this.rebindButton.onClick.AddListener(new UnityAction(this.OnRebindClicked));
		this.resetButton.onClick.AddListener(new UnityAction(this.OnResetClicked));
	}

	// Token: 0x06000F69 RID: 3945 RVA: 0x0004BA30 File Offset: 0x00049C30
	internal void UpdateBindingVisuals(PauseMenuRebindButton[] allButtons, InputScheme scheme)
	{
		bool flag;
		this.currentBindingPath = InputSpriteData.GetBindingPath(this.inputActionName, scheme, out flag);
		bool active = false;
		foreach (PauseMenuRebindButton pauseMenuRebindButton in allButtons)
		{
			bool flag2;
			if (!(pauseMenuRebindButton == this) && pauseMenuRebindButton.gameObject.activeInHierarchy && InputSpriteData.GetBindingPath(pauseMenuRebindButton.inputActionName, scheme, out flag2) == this.currentBindingPath)
			{
				active = true;
			}
		}
		this.warning.SetActive(active);
		if (flag)
		{
			this.inputDescriptionText.tmp.color = this.overriddenTextColor;
			return;
		}
		this.inputDescriptionText.tmp.color = this.defaultTextColor;
	}

	// Token: 0x06000F6A RID: 3946 RVA: 0x0004BADA File Offset: 0x00049CDA
	public void Init(UIPageHandler pageHandler)
	{
		this.pageHandler = pageHandler;
	}

	// Token: 0x06000F6B RID: 3947 RVA: 0x0004BAE3 File Offset: 0x00049CE3
	private void OnRebindClicked()
	{
		PauseMenuRebindKeyPage.inputAction = this.inputAction;
		PauseMenuRebindKeyPage.inputLocIndex = this.inputDescriptionText.index;
		PauseMenuRebindKeyPage.forcedInputScheme = InputHandler.GetCurrentUsedInputScheme();
		this.pageHandler.TransistionToPage<PauseMenuRebindKeyPage>();
	}

	// Token: 0x06000F6C RID: 3948 RVA: 0x0004BB16 File Offset: 0x00049D16
	private void OnResetClicked()
	{
		this.inputAction.RemoveAllBindingOverrides();
		Rebinding.SaveRebindingsToFile(null);
		Action<InputScheme> inputSchemeChanged = RetrievableResourceSingleton<InputHandler>.Instance.InputSchemeChanged;
		if (inputSchemeChanged == null)
		{
			return;
		}
		inputSchemeChanged(InputHandler.GetCurrentUsedInputScheme());
	}

	// Token: 0x06000F6D RID: 3949 RVA: 0x0004BB42 File Offset: 0x00049D42
	public InputAction GetInputAction()
	{
		if (this.inputAction == null)
		{
			this.inputAction = InputSystem.actions.FindAction(this.inputActionName, false);
		}
		return this.inputAction;
	}

	// Token: 0x04000CEA RID: 3306
	private InputAction inputAction;

	// Token: 0x04000CEB RID: 3307
	public string inputActionName;

	// Token: 0x04000CEC RID: 3308
	public LocalizedText inputDescriptionText;

	// Token: 0x04000CED RID: 3309
	public string currentBindingPath;

	// Token: 0x04000CEE RID: 3310
	public Button rebindButton;

	// Token: 0x04000CEF RID: 3311
	public Button resetButton;

	// Token: 0x04000CF0 RID: 3312
	[SerializeField]
	private UIPageHandler pageHandler;

	// Token: 0x04000CF1 RID: 3313
	public Color defaultTextColor;

	// Token: 0x04000CF2 RID: 3314
	public Color overriddenTextColor;

	// Token: 0x04000CF3 RID: 3315
	public GameObject warning;

	// Token: 0x04000CF4 RID: 3316
	public bool allowAxisBinding;

	// Token: 0x04000CF5 RID: 3317
	private bool initialized;
}

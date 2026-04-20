using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zorro.ControllerSupport;
using Zorro.Core;
using Zorro.UI;

// Token: 0x020001E1 RID: 481
public class PauseMenuControlsPage : UIPage, IHaveParentPage, INavigationPage
{
	// Token: 0x06000F48 RID: 3912 RVA: 0x0004B377 File Offset: 0x00049577
	private void Awake()
	{
		Rebinding.LoadRebindingsFromFile(null);
		this.restoreAllButton.onClick.AddListener(new UnityAction(this.OnResetClicked));
	}

	// Token: 0x06000F49 RID: 3913 RVA: 0x0004B39B File Offset: 0x0004959B
	private void OnResetClicked()
	{
		InputSystem.actions.RemoveAllBindingOverrides();
		Action<InputScheme> inputSchemeChanged = RetrievableResourceSingleton<InputHandler>.Instance.InputSchemeChanged;
		if (inputSchemeChanged != null)
		{
			inputSchemeChanged(InputHandler.GetCurrentUsedInputScheme());
		}
		Rebinding.SaveRebindingsToFile(null);
	}

	// Token: 0x06000F4A RID: 3914 RVA: 0x0004B3C7 File Offset: 0x000495C7
	private void OnEnable()
	{
		this.InitButtons();
		InputHandler instance = RetrievableResourceSingleton<InputHandler>.Instance;
		instance.InputSchemeChanged = (Action<InputScheme>)Delegate.Combine(instance.InputSchemeChanged, new Action<InputScheme>(this.OnDeviceChange));
		this.OnDeviceChange(InputHandler.GetCurrentUsedInputScheme());
	}

	// Token: 0x06000F4B RID: 3915 RVA: 0x0004B400 File Offset: 0x00049600
	private void OnDisable()
	{
		InputHandler instance = RetrievableResourceSingleton<InputHandler>.Instance;
		instance.InputSchemeChanged = (Action<InputScheme>)Delegate.Remove(instance.InputSchemeChanged, new Action<InputScheme>(this.OnDeviceChange));
	}

	// Token: 0x06000F4C RID: 3916 RVA: 0x0004B428 File Offset: 0x00049628
	private void OnDeviceChange(InputScheme scheme)
	{
		if (scheme == InputScheme.KeyboardMouse)
		{
			GameObject[] array = this.keyboardOnlyObjects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(true);
			}
			array = this.controllerOnlyObjects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
		}
		else if (scheme == InputScheme.Gamepad)
		{
			GameObject[] array = this.keyboardOnlyObjects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
			array = this.controllerOnlyObjects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(true);
			}
		}
		this.InitButtonBindingVisuals(scheme);
		LayoutRebuilder.ForceRebuildLayoutImmediate(base.transform as RectTransform);
	}

	// Token: 0x06000F4D RID: 3917 RVA: 0x0004B4CD File Offset: 0x000496CD
	private void InitButtons()
	{
		if (this.pageHandler == null)
		{
			this.pageHandler = base.GetComponentInParent<UIPageHandler>();
		}
		if (!this.initializedButtons)
		{
			this.controlsMenuButtons = this.controlsMenuButtonsParent.GetComponentsInChildren<PauseMenuRebindButton>(true);
			this.initializedButtons = true;
		}
	}

	// Token: 0x06000F4E RID: 3918 RVA: 0x0004B50C File Offset: 0x0004970C
	private void InitButtonBindingVisuals(InputScheme scheme)
	{
		for (int i = 0; i < this.controlsMenuButtons.Length; i++)
		{
			this.controlsMenuButtons[i].Init(this.pageHandler);
			this.controlsMenuButtons[i].UpdateBindingVisuals(this.controlsMenuButtons, scheme);
		}
	}

	// Token: 0x06000F4F RID: 3919 RVA: 0x0004B553 File Offset: 0x00049753
	private void Start()
	{
		this.backButton.onClick.AddListener(new UnityAction(this.BackClicked));
	}

	// Token: 0x06000F50 RID: 3920 RVA: 0x0004B571 File Offset: 0x00049771
	private void BackClicked()
	{
		this.pageHandler.TransistionToPage<PauseMenuMainPage>();
	}

	// Token: 0x06000F51 RID: 3921 RVA: 0x0004B57F File Offset: 0x0004977F
	public ValueTuple<UIPage, PageTransistion> GetParentPage()
	{
		return new ValueTuple<UIPage, PageTransistion>(this.pageHandler.GetPage<PauseMenuMainPage>(), new SetActivePageTransistion());
	}

	// Token: 0x06000F52 RID: 3922 RVA: 0x0004B598 File Offset: 0x00049798
	public GameObject GetFirstSelectedGameObject()
	{
		if (this.controlsMenuButtons.Length != 0)
		{
			for (int i = 0; i < this.controlsMenuButtons.Length; i++)
			{
				if (this.controlsMenuButtons[i].gameObject.activeInHierarchy)
				{
					return this.controlsMenuButtons[i].gameObject;
				}
			}
		}
		return this.backButton.gameObject;
	}

	// Token: 0x04000CD8 RID: 3288
	public Button backButton;

	// Token: 0x04000CD9 RID: 3289
	public Button restoreAllButton;

	// Token: 0x04000CDA RID: 3290
	private PauseMenuRebindButton[] controlsMenuButtons;

	// Token: 0x04000CDB RID: 3291
	public Transform controlsMenuButtonsParent;

	// Token: 0x04000CDC RID: 3292
	private bool initializedButtons;

	// Token: 0x04000CDD RID: 3293
	public GameObject[] keyboardOnlyObjects;

	// Token: 0x04000CDE RID: 3294
	public GameObject[] controllerOnlyObjects;
}

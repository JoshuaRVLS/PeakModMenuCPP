using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zorro.ControllerSupport;
using Zorro.Core;

// Token: 0x020001DA RID: 474
public class MenuWindow : MonoBehaviour, INavigationContainer
{
	// Token: 0x17000113 RID: 275
	// (get) Token: 0x06000F0E RID: 3854 RVA: 0x0004AC99 File Offset: 0x00048E99
	public virtual bool openOnStart
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000114 RID: 276
	// (get) Token: 0x06000F0F RID: 3855 RVA: 0x0004AC9C File Offset: 0x00048E9C
	public virtual bool selectOnOpen
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000115 RID: 277
	// (get) Token: 0x06000F10 RID: 3856 RVA: 0x0004AC9F File Offset: 0x00048E9F
	public virtual Selectable objectToSelectOnOpen
	{
		get
		{
			return null;
		}
	}

	// Token: 0x17000116 RID: 278
	// (get) Token: 0x06000F11 RID: 3857 RVA: 0x0004ACA2 File Offset: 0x00048EA2
	public virtual bool closeOnPause
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000117 RID: 279
	// (get) Token: 0x06000F12 RID: 3858 RVA: 0x0004ACA5 File Offset: 0x00048EA5
	public virtual bool closeOnUICancel
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000118 RID: 280
	// (get) Token: 0x06000F13 RID: 3859 RVA: 0x0004ACA8 File Offset: 0x00048EA8
	public virtual bool blocksPlayerInput
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000119 RID: 281
	// (get) Token: 0x06000F14 RID: 3860 RVA: 0x0004ACAB File Offset: 0x00048EAB
	public virtual bool showCursorWhileOpen
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700011A RID: 282
	// (get) Token: 0x06000F15 RID: 3861 RVA: 0x0004ACAE File Offset: 0x00048EAE
	public virtual bool autoHideOnClose
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700011B RID: 283
	// (get) Token: 0x06000F16 RID: 3862 RVA: 0x0004ACB1 File Offset: 0x00048EB1
	// (set) Token: 0x06000F17 RID: 3863 RVA: 0x0004ACB9 File Offset: 0x00048EB9
	public bool isOpen { get; private set; }

	// Token: 0x1700011C RID: 284
	// (get) Token: 0x06000F18 RID: 3864 RVA: 0x0004ACC2 File Offset: 0x00048EC2
	// (set) Token: 0x06000F19 RID: 3865 RVA: 0x0004ACCA File Offset: 0x00048ECA
	public bool inputActive { get; private set; }

	// Token: 0x1700011D RID: 285
	// (get) Token: 0x06000F1A RID: 3866 RVA: 0x0004ACD3 File Offset: 0x00048ED3
	// (set) Token: 0x06000F1B RID: 3867 RVA: 0x0004ACDB File Offset: 0x00048EDB
	public bool initialized { get; private set; }

	// Token: 0x1700011E RID: 286
	// (get) Token: 0x06000F1C RID: 3868 RVA: 0x0004ACE4 File Offset: 0x00048EE4
	public virtual GameObject panel
	{
		get
		{
			return base.gameObject;
		}
	}

	// Token: 0x06000F1D RID: 3869 RVA: 0x0004ACEC File Offset: 0x00048EEC
	protected virtual void Start()
	{
		if (!this.isOpen)
		{
			if (this.openOnStart)
			{
				this.Open();
				return;
			}
			this.StartClosed();
		}
	}

	// Token: 0x06000F1E RID: 3870 RVA: 0x0004AD0B File Offset: 0x00048F0B
	protected virtual void Update()
	{
		if (this.isOpen)
		{
			INavigationContainer.PushActive(this);
		}
		this.TestCloseViaInput();
	}

	// Token: 0x06000F1F RID: 3871 RVA: 0x0004AD24 File Offset: 0x00048F24
	private void TestCloseViaInput()
	{
		if (this.inputActive)
		{
			if (this.closeOnPause && Character.localCharacter && Character.localCharacter.input.pauseWasPressed)
			{
				this.Close();
				Character.localCharacter.input.pauseWasPressed = false;
				return;
			}
			if (this.closeOnUICancel && Singleton<UIInputHandler>.Instance.cancelWasPressed)
			{
				this.Close();
				Singleton<UIInputHandler>.Instance.cancelWasPressed = false;
				return;
			}
		}
	}

	// Token: 0x06000F20 RID: 3872 RVA: 0x0004AD9B File Offset: 0x00048F9B
	protected virtual void Initialize()
	{
	}

	// Token: 0x06000F21 RID: 3873 RVA: 0x0004ADA0 File Offset: 0x00048FA0
	internal virtual void Open()
	{
		Debug.Log("opening window", base.gameObject);
		this.isOpen = true;
		if (!MenuWindow.AllActiveWindows.Contains(this))
		{
			MenuWindow.AllActiveWindows.Add(this);
		}
		this.Show();
		if (!this.initialized)
		{
			this.Initialize();
			this.initialized = true;
		}
		this.OnOpen();
		if (this.selectOnOpen)
		{
			this.SelectStartingElement();
		}
		this.SetInputActive(true);
	}

	// Token: 0x06000F22 RID: 3874 RVA: 0x0004AE12 File Offset: 0x00049012
	protected virtual void OnOpen()
	{
	}

	// Token: 0x06000F23 RID: 3875 RVA: 0x0004AE14 File Offset: 0x00049014
	private void OnDestroy()
	{
		if (MenuWindow.AllActiveWindows.Contains(this))
		{
			MenuWindow.AllActiveWindows.Remove(this);
		}
	}

	// Token: 0x06000F24 RID: 3876 RVA: 0x0004AE30 File Offset: 0x00049030
	public static void CloseAllWindows()
	{
		for (int i = MenuWindow.AllActiveWindows.Count - 1; i >= 0; i--)
		{
			if (MenuWindow.AllActiveWindows[i] != null)
			{
				MenuWindow.AllActiveWindows[i].ForceClose();
			}
		}
	}

	// Token: 0x06000F25 RID: 3877 RVA: 0x0004AE77 File Offset: 0x00049077
	internal void StartClosed()
	{
		this.isOpen = false;
		this.SetInputActive(false);
		this.panel.SetActive(false);
	}

	// Token: 0x06000F26 RID: 3878 RVA: 0x0004AE94 File Offset: 0x00049094
	internal void Close()
	{
		Debug.Log(base.gameObject.name + " closing.");
		this.isOpen = false;
		if (MenuWindow.AllActiveWindows.Contains(this))
		{
			MenuWindow.AllActiveWindows.Remove(this);
		}
		this.OnClose();
		this.SetInputActive(false);
		if (this.autoHideOnClose)
		{
			this.Hide();
		}
	}

	// Token: 0x06000F27 RID: 3879 RVA: 0x0004AEF6 File Offset: 0x000490F6
	internal void ForceClose()
	{
		this.Close();
	}

	// Token: 0x06000F28 RID: 3880 RVA: 0x0004AEFE File Offset: 0x000490FE
	protected virtual void OnClose()
	{
	}

	// Token: 0x06000F29 RID: 3881 RVA: 0x0004AF00 File Offset: 0x00049100
	public void Show()
	{
		this.panel.SetActive(true);
	}

	// Token: 0x06000F2A RID: 3882 RVA: 0x0004AF0E File Offset: 0x0004910E
	public void Hide()
	{
		Debug.Log("Hiding " + base.gameObject.name);
		this.panel.SetActive(false);
	}

	// Token: 0x06000F2B RID: 3883 RVA: 0x0004AF36 File Offset: 0x00049136
	public void SetInputActive(bool active)
	{
		this.inputActive = active;
	}

	// Token: 0x06000F2C RID: 3884 RVA: 0x0004AF3F File Offset: 0x0004913F
	private void SelectStartingElement()
	{
		UIInputHandler.SetSelectedObject((this.objectToSelectOnOpen == null) ? null : this.objectToSelectOnOpen.gameObject);
	}

	// Token: 0x06000F2D RID: 3885 RVA: 0x0004AF62 File Offset: 0x00049162
	public int GetContainerPriority()
	{
		return 1;
	}

	// Token: 0x06000F2E RID: 3886 RVA: 0x0004AF65 File Offset: 0x00049165
	public GameObject GetDefaultSelection()
	{
		if (this.objectToSelectOnOpen == null)
		{
			return null;
		}
		return this.objectToSelectOnOpen.gameObject;
	}

	// Token: 0x06000F2F RID: 3887 RVA: 0x0004AF82 File Offset: 0x00049182
	public bool IsValidSelection(GameObject selection)
	{
		return selection.activeInHierarchy;
	}

	// Token: 0x04000CC2 RID: 3266
	public static List<MenuWindow> AllActiveWindows = new List<MenuWindow>();
}

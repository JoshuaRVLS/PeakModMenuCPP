using System;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020002C6 RID: 710
public class PauseMainMenu : MenuWindow
{
	// Token: 0x17000146 RID: 326
	// (get) Token: 0x06001405 RID: 5125 RVA: 0x000653BB File Offset: 0x000635BB
	public override bool openOnStart
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000147 RID: 327
	// (get) Token: 0x06001406 RID: 5126 RVA: 0x000653BE File Offset: 0x000635BE
	public override bool selectOnOpen
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000148 RID: 328
	// (get) Token: 0x06001407 RID: 5127 RVA: 0x000653C1 File Offset: 0x000635C1
	public override bool closeOnPause
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000149 RID: 329
	// (get) Token: 0x06001408 RID: 5128 RVA: 0x000653C4 File Offset: 0x000635C4
	public override bool closeOnUICancel
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001409 RID: 5129 RVA: 0x000653C7 File Offset: 0x000635C7
	protected override void Initialize()
	{
		this.backButton.onClick.AddListener(new UnityAction(base.Close));
	}

	// Token: 0x0600140A RID: 5130 RVA: 0x000653E5 File Offset: 0x000635E5
	protected override void OnClose()
	{
		this.mainMenu.Open();
	}

	// Token: 0x0400123B RID: 4667
	public MenuWindow mainMenu;

	// Token: 0x0400123C RID: 4668
	public Button backButton;
}

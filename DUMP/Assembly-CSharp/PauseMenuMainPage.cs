using System;
using Peak;
using Photon.Pun;
using Steamworks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.ControllerSupport;
using Zorro.UI;

// Token: 0x020001E3 RID: 483
public class PauseMenuMainPage : UIPage, INavigationPage
{
	// Token: 0x17000120 RID: 288
	// (get) Token: 0x06000F58 RID: 3928 RVA: 0x0004B6F0 File Offset: 0x000498F0
	public Button resumeButton
	{
		get
		{
			return this.m_resumeButton;
		}
	}

	// Token: 0x06000F59 RID: 3929 RVA: 0x0004B6F8 File Offset: 0x000498F8
	private void Start()
	{
		this.m_quitButton.onClick.AddListener(new UnityAction(this.OnQuitClicked));
		this.m_settingsButton.onClick.AddListener(new UnityAction(this.OnSettingsClicked));
		this.m_resumeButton.onClick.AddListener(new UnityAction(this.OnResumeClicked));
		this.m_accoladesButton.onClick.AddListener(new UnityAction(this.OnAccoladesClicked));
		this.m_controllsButton.onClick.AddListener(new UnityAction(this.OnControlsClicked));
		this.m_inviteButton.onClick.AddListener(new UnityAction(this.InviteFriendsClicked));
		this.m_confirmCancelButton.onClick.AddListener(new UnityAction(this.ConfirmCancel));
		if (PhotonNetwork.OfflineMode)
		{
			Navigation navigation = this.resumeButton.navigation;
			navigation.selectOnDown = this.m_accoladesButton;
			this.resumeButton.navigation = navigation;
		}
	}

	// Token: 0x06000F5A RID: 3930 RVA: 0x0004B7F5 File Offset: 0x000499F5
	private void OnEnable()
	{
		this.m_inviteButton.gameObject.SetActive(!PhotonNetwork.OfflineMode);
	}

	// Token: 0x06000F5B RID: 3931 RVA: 0x0004B80F File Offset: 0x00049A0F
	private void OnDisable()
	{
		if (this.confirmWindow.isOpen)
		{
			this.confirmWindow.Close();
		}
	}

	// Token: 0x06000F5C RID: 3932 RVA: 0x0004B82C File Offset: 0x00049A2C
	private void InviteFriendsClicked()
	{
		CSteamID steamIDLobby;
		if (GameHandler.GetService<SteamLobbyHandler>().InSteamLobby(out steamIDLobby))
		{
			SteamFriends.ActivateGameOverlayInviteDialog(steamIDLobby);
		}
	}

	// Token: 0x06000F5D RID: 3933 RVA: 0x0004B84D File Offset: 0x00049A4D
	private void OnControlsClicked()
	{
		this.pageHandler.TransistionToPage<PauseMenuControlsPage>();
	}

	// Token: 0x06000F5E RID: 3934 RVA: 0x0004B85B File Offset: 0x00049A5B
	private void OnAccoladesClicked()
	{
		this.pageHandler.TransistionToPage<PauseMenuAccoladesPage>();
	}

	// Token: 0x06000F5F RID: 3935 RVA: 0x0004B869 File Offset: 0x00049A69
	private void OnResumeClicked()
	{
		this.pageHandler.gameObject.SetActive(false);
	}

	// Token: 0x06000F60 RID: 3936 RVA: 0x0004B87C File Offset: 0x00049A7C
	private void OnSettingsClicked()
	{
		this.pageHandler.TransistionToPage<PauseMenuSettingsMenuPage>();
	}

	// Token: 0x06000F61 RID: 3937 RVA: 0x0004B88A File Offset: 0x00049A8A
	private void OnQuitClicked()
	{
		this.OpenQuitConfirmWindow();
	}

	// Token: 0x06000F62 RID: 3938 RVA: 0x0004B894 File Offset: 0x00049A94
	private void OpenQuitConfirmWindow()
	{
		this.confirmWindow.Open();
		if (Quicksave.Exists)
		{
			this.confirmText.SetTextLocalized("LEAVE_GAME_CONFIRM_QUICKSAVE");
		}
		else
		{
			this.confirmText.SetTextLocalized("LEAVE_GAME_CONFIRM");
		}
		this.m_confirmOkButton.onClick.RemoveAllListeners();
		this.m_confirmOkButton.onClick.AddListener(new UnityAction(this.Quit));
		this.m_confirmCancelButton.Select();
	}

	// Token: 0x06000F63 RID: 3939 RVA: 0x0004B90C File Offset: 0x00049B0C
	public Button OpenKickConfirmWindow(string playerName)
	{
		this.confirmWindow.Open();
		this.confirmText.SetText(LocalizedText.GetText("MODAL_KICK_PROMPT", true).Replace("#", playerName));
		this.confirmWindow.SetInputActive(true);
		this.m_confirmOkButton.onClick.RemoveAllListeners();
		this.m_confirmOkButton.onClick.AddListener(new UnityAction(this.ConfirmCancel));
		this.m_confirmCancelButton.Select();
		return this.m_confirmOkButton;
	}

	// Token: 0x06000F64 RID: 3940 RVA: 0x0004B98E File Offset: 0x00049B8E
	private void ConfirmCancel()
	{
		this.confirmWindow.Close();
	}

	// Token: 0x06000F65 RID: 3941 RVA: 0x0004B99B File Offset: 0x00049B9B
	private void Quit()
	{
		this.confirmWindow.Close();
		this.pageHandler.gameObject.SetActive(false);
		Player.LeaveCurrentGame();
	}

	// Token: 0x06000F66 RID: 3942 RVA: 0x0004B9BE File Offset: 0x00049BBE
	public GameObject GetFirstSelectedGameObject()
	{
		return this.m_resumeButton.gameObject;
	}

	// Token: 0x04000CE0 RID: 3296
	[SerializeField]
	private Button m_quitButton;

	// Token: 0x04000CE1 RID: 3297
	[SerializeField]
	private Button m_settingsButton;

	// Token: 0x04000CE2 RID: 3298
	[SerializeField]
	private Button m_resumeButton;

	// Token: 0x04000CE3 RID: 3299
	[SerializeField]
	private Button m_accoladesButton;

	// Token: 0x04000CE4 RID: 3300
	[SerializeField]
	private Button m_controllsButton;

	// Token: 0x04000CE5 RID: 3301
	[SerializeField]
	private Button m_inviteButton;

	// Token: 0x04000CE6 RID: 3302
	public MenuWindow confirmWindow;

	// Token: 0x04000CE7 RID: 3303
	[SerializeField]
	private Button m_confirmOkButton;

	// Token: 0x04000CE8 RID: 3304
	[SerializeField]
	private Button m_confirmCancelButton;

	// Token: 0x04000CE9 RID: 3305
	public LocalizedText confirmText;
}

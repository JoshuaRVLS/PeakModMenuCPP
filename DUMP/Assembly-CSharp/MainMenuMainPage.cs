using System;
using Peak;
using Peak.Network;
using Peak.UI;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.ControllerSupport;
using Zorro.UI;

// Token: 0x020001D2 RID: 466
public class MainMenuMainPage : UIPage, INavigationPage
{
	// Token: 0x1700010B RID: 267
	// (get) Token: 0x06000EDB RID: 3803 RVA: 0x0004A407 File Offset: 0x00048607
	public Button PlayButton
	{
		get
		{
			return this.m_playButton;
		}
	}

	// Token: 0x06000EDC RID: 3804 RVA: 0x0004A410 File Offset: 0x00048610
	private void Start()
	{
		this.m_playButton.onClick.AddListener(new UnityAction(this.PlayClicked));
		this.m_settingsButton.onClick.AddListener(new UnityAction(this.SettingsClicked));
		this.SetUpContinueButton();
		NetworkingUtilities.ConnectToNetwork();
		GameHandler.GetService<RichPresenceService>().SetState(RichPresenceState.Status_MainMenu);
	}

	// Token: 0x06000EDD RID: 3805 RVA: 0x0004A46C File Offset: 0x0004866C
	private void SetUpContinueButton()
	{
		bool flag = Quicksave.Exists && Quicksave.TryLoadSave();
		this.m_continueButton.gameObject.SetActive(flag);
		Navigation navigation = this.m_playButton.navigation;
		navigation.selectOnUp = (flag ? this.m_continueButton : this.m_quitButton);
		this.m_playButton.navigation = navigation;
		if (flag)
		{
			this.m_continueButton.onClick.AddListener(new UnityAction(this.ContinueFromQuicksaveClicked));
		}
	}

	// Token: 0x06000EDE RID: 3806 RVA: 0x0004A4E9 File Offset: 0x000486E9
	private void SettingsClicked()
	{
		this.pageHandler.TransistionToPage<MainMenuSettingsPage>(new SetActivePageTransistion());
	}

	// Token: 0x06000EDF RID: 3807 RVA: 0x0004A4FC File Offset: 0x000486FC
	private void OnDestroy()
	{
	}

	// Token: 0x06000EE0 RID: 3808 RVA: 0x0004A4FE File Offset: 0x000486FE
	private void PlayClicked()
	{
		if (Quicksave.Exists)
		{
			ConfirmPage.Open("SAVE_DESTROY_ON_HOST", new Action(this.DestroySaveAndStartLobby), null);
			return;
		}
		this.StartMultiplayerLobby(true);
	}

	// Token: 0x06000EE1 RID: 3809 RVA: 0x0004A528 File Offset: 0x00048728
	private void ContinueFromQuicksaveClicked()
	{
		Quicksave.ShouldUseSaveData = true;
		if (Quicksave.SavedRun.LobbyType != Quicksave.LobbyMode.Offline)
		{
			this.StartMultiplayerLobby(false);
			return;
		}
		Quicksave.LoadSavedGameScene();
	}

	// Token: 0x06000EE2 RID: 3810 RVA: 0x0004A558 File Offset: 0x00048758
	private void DestroySaveAndStartLobby()
	{
		this.StartMultiplayerLobby(true);
	}

	// Token: 0x06000EE3 RID: 3811 RVA: 0x0004A561 File Offset: 0x00048761
	private void StartMultiplayerLobby(bool destroyQuicksave = true)
	{
		if (destroyQuicksave && Quicksave.Exists)
		{
			Quicksave.DestroySaveData();
		}
		NetCode.Matchmaking.CreateLobby(GameHandler.Instance.SettingsHandler.GetSetting<LobbyTypeSetting>().Value);
	}

	// Token: 0x06000EE4 RID: 3812 RVA: 0x0004A590 File Offset: 0x00048790
	private void Update()
	{
		this.m_playButton.gameObject.SetActive(!PhotonNetwork.OfflineMode);
	}

	// Token: 0x06000EE5 RID: 3813 RVA: 0x0004A5AA File Offset: 0x000487AA
	public GameObject GetFirstSelectedGameObject()
	{
		return this.m_playButton.gameObject;
	}

	// Token: 0x04000CA4 RID: 3236
	[SerializeField]
	private Button m_playButton;

	// Token: 0x04000CA5 RID: 3237
	[SerializeField]
	private Button m_continueButton;

	// Token: 0x04000CA6 RID: 3238
	[SerializeField]
	private Button m_settingsButton;

	// Token: 0x04000CA7 RID: 3239
	[SerializeField]
	private Button m_quitButton;
}

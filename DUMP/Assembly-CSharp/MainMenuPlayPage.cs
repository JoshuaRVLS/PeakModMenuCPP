using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.Core;
using Zorro.UI;

// Token: 0x020001D5 RID: 469
public class MainMenuPlayPage : UIPage, IHaveParentPage
{
	// Token: 0x06000EF2 RID: 3826 RVA: 0x0004A854 File Offset: 0x00048A54
	private void Start()
	{
		this.m_playButton.onClick.AddListener(new UnityAction(this.PlayClicked));
	}

	// Token: 0x06000EF3 RID: 3827 RVA: 0x0004A872 File Offset: 0x00048A72
	public ValueTuple<UIPage, PageTransistion> GetParentPage()
	{
		return new ValueTuple<UIPage, PageTransistion>(this.pageHandler.GetPage<MainMenuMainPage>(), new SetActivePageTransistion());
	}

	// Token: 0x06000EF4 RID: 3828 RVA: 0x0004A88C File Offset: 0x00048A8C
	public void PlayClicked()
	{
		if (string.IsNullOrEmpty(this.m_usernameField.text))
		{
			Debug.LogError("Failed to get username field...");
			return;
		}
		if (string.IsNullOrEmpty(this.m_roomField.text))
		{
			Debug.LogError("Failed to get room name field...");
			return;
		}
		JoinSpecificRoomState joinSpecificRoomState = GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<JoinSpecificRoomState>(false);
		joinSpecificRoomState.RoomName = this.m_roomField.text.ToLower();
		joinSpecificRoomState.RegionToJoin = "eu";
		RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Basic, null, new IEnumerator[]
		{
			RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess("Airport", false, true, 3f)
		});
	}

	// Token: 0x04000CAF RID: 3247
	[SerializeField]
	private Button m_playButton;

	// Token: 0x04000CB0 RID: 3248
	[SerializeField]
	private TMP_InputField m_usernameField;

	// Token: 0x04000CB1 RID: 3249
	[SerializeField]
	private TMP_InputField m_roomField;
}

using System;
using System.Linq;
using TMPro;
using Unity.Multiplayer.Playmode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Token: 0x02000099 RID: 153
public class DebugMainMenu : MonoBehaviour
{
	// Token: 0x0600060C RID: 1548 RVA: 0x00022B28 File Offset: 0x00020D28
	private void Start()
	{
		this.m_matchmakeButton.onClick.AddListener(new UnityAction(this.MatchmakeClicked));
		this.m_debugJoinButton.onClick.AddListener(new UnityAction(this.DebugJoinClicked));
		this.m_debugCreateButton.onClick.AddListener(new UnityAction(this.DebugCreateClicked));
		this.m_debugRejoinButton.onClick.AddListener(new UnityAction(this.DebugRejoinClicked));
		if (this.debugJoinOnAwake)
		{
			this.DebugHaxxClicked();
		}
	}

	// Token: 0x0600060D RID: 1549 RVA: 0x00022BB3 File Offset: 0x00020DB3
	private void DebugRejoinClicked()
	{
		Debug.Log("Rejoining...");
		GameHandler.GetService<ConnectionService>();
		SceneManager.LoadScene("WilIsland");
	}

	// Token: 0x0600060E RID: 1550 RVA: 0x00022BCF File Offset: 0x00020DCF
	private void DebugCreateClicked()
	{
		GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<HostState>(false).RoomName = "THEPETHEN";
		SceneManager.LoadScene("WilIsland");
	}

	// Token: 0x0600060F RID: 1551 RVA: 0x00022BF5 File Offset: 0x00020DF5
	private void DebugJoinClicked()
	{
		GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<JoinSpecificRoomState>(false).RoomName = "THEPETHEN";
		SceneManager.LoadScene("WilIsland");
	}

	// Token: 0x06000610 RID: 1552 RVA: 0x00022C1C File Offset: 0x00020E1C
	private void DebugHaxxClicked()
	{
		ConnectionService service = GameHandler.GetService<ConnectionService>();
		if (CurrentPlayer.ReadOnlyTags().Contains("Client") || !DebugMainMenu.first)
		{
			service.StateMachine.SwitchState<JoinSpecificRoomState>(false).RoomName = "THEPETHEN";
		}
		else
		{
			service.StateMachine.SwitchState<HostState>(false).RoomName = "THEPETHEN";
		}
		DebugMainMenu.first = false;
		SceneManager.LoadScene("WilIsland");
	}

	// Token: 0x06000611 RID: 1553 RVA: 0x00022C88 File Offset: 0x00020E88
	private void MatchmakeClicked()
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
		SceneManager.LoadScene("WilIsland");
	}

	// Token: 0x04000625 RID: 1573
	[SerializeField]
	private Button m_matchmakeButton;

	// Token: 0x04000626 RID: 1574
	[SerializeField]
	private Button m_debugJoinButton;

	// Token: 0x04000627 RID: 1575
	[SerializeField]
	private Button m_debugCreateButton;

	// Token: 0x04000628 RID: 1576
	[SerializeField]
	private Button m_debugRejoinButton;

	// Token: 0x04000629 RID: 1577
	[SerializeField]
	private TMP_InputField m_usernameField;

	// Token: 0x0400062A RID: 1578
	[SerializeField]
	private TMP_InputField m_roomField;

	// Token: 0x0400062B RID: 1579
	public bool debugJoinOnAwake = true;

	// Token: 0x0400062C RID: 1580
	private static bool first = true;
}

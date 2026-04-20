using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zorro.UI.Modal;

// Token: 0x0200007C RID: 124
public class KickedState : ConnectionState
{
	// Token: 0x06000577 RID: 1399 RVA: 0x0002040C File Offset: 0x0001E60C
	public static void DisplayModal()
	{
		LoadingScreenHandler.KillCurrentLoadingScreen();
		HeaderModalOption headerContent = new DefaultHeaderModalOption(LocalizedText.GetText("MODAL_KICKED_TITLE", true), LocalizedText.GetText("MODAL_KICKED_BODY", true));
		ModalButtonsOption.Option[] array = new ModalButtonsOption.Option[1];
		array[0] = new ModalButtonsOption.Option(LocalizedText.GetText("OK", true), delegate()
		{
		});
		Modal.OpenModal(headerContent, new ModalButtonsOption(array), null);
	}

	// Token: 0x06000578 RID: 1400 RVA: 0x0002047C File Offset: 0x0001E67C
	public override void Enter()
	{
		base.Enter();
		Debug.Log("Getting booted!!");
		Player.LeaveCurrentGame();
		SceneManager.LoadScene("Title");
	}

	// Token: 0x040005AC RID: 1452
	private const string ModalTitleKey = "MODAL_KICKED_TITLE";

	// Token: 0x040005AD RID: 1453
	private const string ModalBodyKey = "MODAL_KICKED_BODY";
}

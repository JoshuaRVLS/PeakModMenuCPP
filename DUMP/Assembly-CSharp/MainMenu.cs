using System;
using System.Collections;
using System.Collections.Generic;
using Peak;
using Peak.UI;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.Core;

// Token: 0x020002AF RID: 687
public class MainMenu : MonoBehaviour
{
	// Token: 0x0600136D RID: 4973 RVA: 0x00062A68 File Offset: 0x00060C68
	private void Start()
	{
		AudioLevels.ResetSliders();
		this.playSoloButton.onClick.AddListener(new UnityAction(this.PlaySoloClicked));
		this.creditsButton.onClick.AddListener(new UnityAction(this.ToggleCredits));
		this.quitButton.onClick.AddListener(new UnityAction(this.Quit));
		this.discordButton.onClick.AddListener(new UnityAction(this.OpenDiscord));
		this.landfallButton.onClick.AddListener(new UnityAction(this.OpenLandfallWebsite));
		this.aggrocrabButton.onClick.AddListener(new UnityAction(this.OpenAggrocrabWebsite));
		Time.timeScale = 1f;
		if (NetworkConnector.CurrentConnectionState is KickedState)
		{
			KickedState.DisplayModal();
		}
	}

	// Token: 0x0600136E RID: 4974 RVA: 0x00062B3D File Offset: 0x00060D3D
	public void ToggleCredits()
	{
		this.credits.SetActive(!this.credits.activeSelf);
		if (this.credits.activeSelf)
		{
			this.RandomizeMainGuys();
		}
	}

	// Token: 0x0600136F RID: 4975 RVA: 0x00062B6B File Offset: 0x00060D6B
	public void OpenDiscord()
	{
		Application.OpenURL("https://discord.gg/peakgame");
	}

	// Token: 0x06001370 RID: 4976 RVA: 0x00062B77 File Offset: 0x00060D77
	public void OpenLandfallWebsite()
	{
		Application.OpenURL("https://landfall.se/");
	}

	// Token: 0x06001371 RID: 4977 RVA: 0x00062B83 File Offset: 0x00060D83
	public void OpenAggrocrabWebsite()
	{
		Application.OpenURL("https://aggrocrab.com/");
	}

	// Token: 0x06001372 RID: 4978 RVA: 0x00062B90 File Offset: 0x00060D90
	public void RandomizeMainGuys()
	{
		Transform transform = this.mainGuysHolder;
		List<Transform> list = new List<Transform>();
		for (int i = 0; i < transform.childCount; i++)
		{
			list.Add(transform.GetChild(i));
		}
		for (int j = list.Count - 1; j > 0; j--)
		{
			int num = Random.Range(0, j + 1);
			List<Transform> list2 = list;
			int index = j;
			List<Transform> list3 = list;
			int index2 = num;
			Transform value = list[num];
			Transform value2 = list[j];
			list2[index] = value;
			list3[index2] = value2;
		}
		for (int k = 0; k < list.Count; k++)
		{
			list[k].SetSiblingIndex(k);
		}
	}

	// Token: 0x06001373 RID: 4979 RVA: 0x00062C42 File Offset: 0x00060E42
	public void Quit()
	{
		Application.Quit();
	}

	// Token: 0x06001374 RID: 4980 RVA: 0x00062C49 File Offset: 0x00060E49
	private void PlaySoloClicked()
	{
		if (Quicksave.Exists)
		{
			ConfirmPage.Open("SAVE_DESTROY_ON_HOST", new Action(this.StartSoloSession), null);
			return;
		}
		this.StartSoloSession();
	}

	// Token: 0x06001375 RID: 4981 RVA: 0x00062C70 File Offset: 0x00060E70
	private void StartSoloSession()
	{
		if (Quicksave.Exists)
		{
			Quicksave.DestroySaveData();
		}
		RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Basic, null, new IEnumerator[]
		{
			this.StartOfflineModeRoutine()
		});
	}

	// Token: 0x06001376 RID: 4982 RVA: 0x00062C99 File Offset: 0x00060E99
	public static IEnumerator DisconnectForOfflineMode()
	{
		PhotonNetwork.IsMessageQueueRunning = true;
		GameHandler.AddStatus<IsDisconnectingForOfflineMode>(new IsDisconnectingForOfflineMode());
		PhotonNetwork.Disconnect();
		while (PhotonNetwork.IsConnected)
		{
			Debug.Log("We are still connected.. waiting for disconnect");
			yield return null;
		}
		PhotonNetwork.OfflineMode = true;
		GameHandler.ClearStatus<IsDisconnectingForOfflineMode>();
		yield break;
	}

	// Token: 0x06001377 RID: 4983 RVA: 0x00062CA1 File Offset: 0x00060EA1
	private IEnumerator StartOfflineModeRoutine()
	{
		yield return MainMenu.DisconnectForOfflineMode();
		yield return RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess("Airport", false, true, 3f);
		yield break;
	}

	// Token: 0x040011B5 RID: 4533
	public const string WILL_DESTROY_QUICKSAVE_KEY = "SAVE_DESTROY_ON_HOST";

	// Token: 0x040011B6 RID: 4534
	public GameObject credits;

	// Token: 0x040011B7 RID: 4535
	public Transform mainGuysHolder;

	// Token: 0x040011B8 RID: 4536
	public const string SceneName = "Title";

	// Token: 0x040011B9 RID: 4537
	public Button playSoloButton;

	// Token: 0x040011BA RID: 4538
	public Button creditsButton;

	// Token: 0x040011BB RID: 4539
	public Button quitButton;

	// Token: 0x040011BC RID: 4540
	public Button discordButton;

	// Token: 0x040011BD RID: 4541
	public Button landfallButton;

	// Token: 0x040011BE RID: 4542
	public Button aggrocrabButton;
}

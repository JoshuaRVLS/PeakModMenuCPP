using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000181 RID: 385
public class RunStarter : MonoBehaviour
{
	// Token: 0x06000CB9 RID: 3257 RVA: 0x0004409C File Offset: 0x0004229C
	private IEnumerator Start()
	{
		while (!PhotonNetwork.InRoom || !Character.localCharacter || LoadingScreenHandler.loading)
		{
			yield return null;
		}
		Debug.Log("Starting the RunManager back up!");
		this.StartRun();
		yield break;
	}

	// Token: 0x06000CBA RID: 3258 RVA: 0x000440AB File Offset: 0x000422AB
	private void StartRun()
	{
		RunManager.Instance.StartRun();
	}
}

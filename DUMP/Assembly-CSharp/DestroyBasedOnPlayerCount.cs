using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000254 RID: 596
public class DestroyBasedOnPlayerCount : MonoBehaviourPun
{
	// Token: 0x060011F9 RID: 4601 RVA: 0x0005A6DF File Offset: 0x000588DF
	private IEnumerator Start()
	{
		while (!PhotonNetwork.InRoom)
		{
			yield return null;
		}
		if (!PhotonNetwork.IsMasterClient)
		{
			yield break;
		}
		if (PhotonNetwork.PlayerList.Length < this.destroyIfPlayerCountIsLessThan)
		{
			Debug.Log(string.Format("Item was told to destroy if player count <{0} and it is {1}", this.destroyIfPlayerCountIsLessThan, PhotonNetwork.PlayerList.Length));
			PhotonNetwork.Destroy(base.photonView);
		}
		yield break;
	}

	// Token: 0x04001000 RID: 4096
	public int destroyIfPlayerCountIsLessThan;
}

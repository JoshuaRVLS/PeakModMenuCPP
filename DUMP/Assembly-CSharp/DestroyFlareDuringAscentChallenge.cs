using System;
using System.Collections;
using Photon.Pun;

// Token: 0x0200009B RID: 155
public class DestroyFlareDuringAscentChallenge : MonoBehaviourPun
{
	// Token: 0x06000619 RID: 1561 RVA: 0x00022EF6 File Offset: 0x000210F6
	private IEnumerator Start()
	{
		while (!PhotonNetwork.InRoom)
		{
			yield return null;
		}
		if (!Ascents.shouldSpawnFlare)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
		yield break;
	}
}

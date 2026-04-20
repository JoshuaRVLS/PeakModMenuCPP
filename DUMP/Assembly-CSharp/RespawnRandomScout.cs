using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200012B RID: 299
public class RespawnRandomScout : MonoBehaviour
{
	// Token: 0x060009BF RID: 2495 RVA: 0x00033DE4 File Offset: 0x00031FE4
	private void Start()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			List<Character> list = new List<Character>();
			foreach (Character character in Character.AllCharacters)
			{
				if (character.data.dead || character.data.fullyPassedOut)
				{
					list.Add(character);
				}
			}
			list.RandomSelection((Character c) => 1).photonView.RPC("RPCA_ReviveAtPosition", RpcTarget.All, new object[]
			{
				base.transform.position,
				false,
				-1
			});
		}
		Object.Destroy(base.gameObject);
	}
}

using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200035B RID: 859
public class TickTrigger : MonoBehaviour
{
	// Token: 0x060016C4 RID: 5828 RVA: 0x00075080 File Offset: 0x00073280
	private void Start()
	{
		if (Random.value > this.tickChance)
		{
			Object.Destroy(base.gameObject);
			return;
		}
	}

	// Token: 0x060016C5 RID: 5829 RVA: 0x0007509C File Offset: 0x0007329C
	private void OnTriggerEnter(Collider other)
	{
		Character character;
		if (!CharacterRagdoll.TryGetCharacterFromCollider(other, out character))
		{
			return;
		}
		if (character.IsLocal)
		{
			foreach (KeyValuePair<Bugfix, Character> keyValuePair in Bugfix.AllAttachedBugs)
			{
				if (keyValuePair.Value == character)
				{
					return;
				}
			}
			PhotonNetwork.Instantiate("BugfixOnYou", Vector3.zero, Quaternion.identity, 0, null).GetComponent<PhotonView>().RPC("AttachBug", RpcTarget.All, new object[]
			{
				character.photonView.ViewID
			});
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x0400152E RID: 5422
	public float tickChance = 0.01f;
}

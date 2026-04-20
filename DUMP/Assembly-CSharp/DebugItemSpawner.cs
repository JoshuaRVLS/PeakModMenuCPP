using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000117 RID: 279
public class DebugItemSpawner : MonoBehaviour
{
	// Token: 0x06000930 RID: 2352 RVA: 0x00031BDF File Offset: 0x0002FDDF
	private IEnumerator Start()
	{
		ISpawner spawner = base.GetComponent<ISpawner>();
		if (spawner == null || !PhotonNetwork.IsMasterClient)
		{
			Object.Destroy(this);
			yield break;
		}
		while (!PhotonNetwork.InRoom || !Character.localCharacter)
		{
			yield return null;
		}
		spawner.TrySpawnItems();
		yield break;
	}
}

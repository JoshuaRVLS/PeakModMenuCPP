using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000261 RID: 609
public class EruptionSpawner : MonoBehaviour
{
	// Token: 0x0600121B RID: 4635 RVA: 0x0005AC5D File Offset: 0x00058E5D
	private void Start()
	{
		this.min = base.transform.GetChild(0);
		this.max = base.transform.GetChild(1);
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x0600121C RID: 4636 RVA: 0x0005AC90 File Offset: 0x00058E90
	private void Update()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (!HelperFunctions.AnyPlayerInZRange(this.min.position.z, this.max.position.z))
		{
			return;
		}
		this.counter -= Time.deltaTime;
		if (this.counter < 0f)
		{
			this.counter = Random.Range(-5f, 15f);
			Vector3 position = base.transform.position;
			position.x += Random.Range(-155f, 155f);
			position.z += Random.Range(-140f, 140f);
			this.photonView.RPC("RPCA_SpawnEruption", RpcTarget.All, new object[]
			{
				position
			});
		}
	}

	// Token: 0x0600121D RID: 4637 RVA: 0x0005AD61 File Offset: 0x00058F61
	[PunRPC]
	public void RPCA_SpawnEruption(Vector3 position)
	{
		Object.Instantiate<GameObject>(this.eruption, position, Quaternion.LookRotation(Vector3.up));
	}

	// Token: 0x0400101B RID: 4123
	private float counter = 10f;

	// Token: 0x0400101C RID: 4124
	public GameObject eruption;

	// Token: 0x0400101D RID: 4125
	private PhotonView photonView;

	// Token: 0x0400101E RID: 4126
	private Transform min;

	// Token: 0x0400101F RID: 4127
	private Transform max;
}

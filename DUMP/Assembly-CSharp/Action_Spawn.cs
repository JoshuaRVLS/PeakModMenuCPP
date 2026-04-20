using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000200 RID: 512
public class Action_Spawn : ItemAction
{
	// Token: 0x06001004 RID: 4100 RVA: 0x0004E750 File Offset: 0x0004C950
	public override void RunAction()
	{
		if (this.spawnPoint == null)
		{
			this.spawnPoint = base.transform;
		}
		this.item.photonView.RPC("RPCSpawn", RpcTarget.All, new object[]
		{
			this.spawnPoint.transform.position,
			this.spawnPoint.transform.rotation
		});
	}

	// Token: 0x06001005 RID: 4101 RVA: 0x0004E7C3 File Offset: 0x0004C9C3
	[PunRPC]
	public void RPCSpawn(Vector3 position, Quaternion rotation)
	{
		Object.Instantiate<GameObject>(this.objectToSpawn, position, rotation);
	}

	// Token: 0x04000DA3 RID: 3491
	public GameObject objectToSpawn;

	// Token: 0x04000DA4 RID: 3492
	public Transform spawnPoint;
}

using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002BC RID: 700
public class NetworkedObjectSpawner : MonoBehaviour
{
	// Token: 0x060013BF RID: 5055 RVA: 0x0006468F File Offset: 0x0006288F
	private void Start()
	{
		this.SetCounter();
	}

	// Token: 0x060013C0 RID: 5056 RVA: 0x00064697 File Offset: 0x00062897
	private void SetCounter()
	{
		this.untilNext = Mathf.Lerp(this.minRate, this.maxRate, Mathf.Pow(Random.value, this.randomPow));
	}

	// Token: 0x060013C1 RID: 5057 RVA: 0x000646C0 File Offset: 0x000628C0
	private void Update()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		this.untilNext -= Time.deltaTime;
		if (this.untilNext < 0f)
		{
			this.SetCounter();
			this.SpawnObject();
		}
	}

	// Token: 0x060013C2 RID: 5058 RVA: 0x000646F5 File Offset: 0x000628F5
	private void SpawnObject()
	{
		PhotonNetwork.Instantiate(this.objToSpawn.name, base.transform.position + this.spawnOffset, base.transform.rotation, 0, null);
	}

	// Token: 0x04001207 RID: 4615
	public GameObject objToSpawn;

	// Token: 0x04001208 RID: 4616
	public float minRate = 3f;

	// Token: 0x04001209 RID: 4617
	public float maxRate = 6f;

	// Token: 0x0400120A RID: 4618
	public float randomPow = 2f;

	// Token: 0x0400120B RID: 4619
	public Vector3 spawnOffset;

	// Token: 0x0400120C RID: 4620
	private float untilNext;
}

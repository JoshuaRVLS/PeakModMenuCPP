using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000363 RID: 867
public class TornadoSpawner : MonoBehaviour
{
	// Token: 0x060016F3 RID: 5875 RVA: 0x000761D5 File Offset: 0x000743D5
	private void Start()
	{
		this.untilNext = Random.Range(this.minSpawnTimeFirst, this.maxSpawnTimeFirst);
		this.view = base.GetComponent<PhotonView>();
		if (RunSettings.GetValue(RunSettings.SETTINGTYPE.Hazard_Tornado, false) == 0)
		{
			base.enabled = false;
			return;
		}
	}

	// Token: 0x060016F4 RID: 5876 RVA: 0x00076210 File Offset: 0x00074410
	private void Update()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		this.untilNext -= Time.deltaTime;
		if (this.untilNext <= 0f)
		{
			this.SpawnTornado();
			this.untilNext = Random.Range(this.minSpawnTime, this.maxSpawnTime);
		}
	}

	// Token: 0x060016F5 RID: 5877 RVA: 0x00076264 File Offset: 0x00074464
	private void SpawnTornado()
	{
		PhotonNetwork.Instantiate("Tornado", this.GetSpawnPos(), Quaternion.identity, 0, null).GetComponent<PhotonView>().RPC("RPCA_InitTornado", RpcTarget.All, new object[]
		{
			this.view.ViewID
		});
	}

	// Token: 0x060016F6 RID: 5878 RVA: 0x000762B4 File Offset: 0x000744B4
	private Vector3 GetSpawnPos()
	{
		Transform transform = base.transform.Find("TornadoPoints");
		return transform.GetChild(Random.Range(0, transform.childCount)).position;
	}

	// Token: 0x0400155E RID: 5470
	public float minSpawnTimeFirst = 30f;

	// Token: 0x0400155F RID: 5471
	public float maxSpawnTimeFirst = 300f;

	// Token: 0x04001560 RID: 5472
	public float minSpawnTime = 30f;

	// Token: 0x04001561 RID: 5473
	public float maxSpawnTime = 300f;

	// Token: 0x04001562 RID: 5474
	private float untilNext;

	// Token: 0x04001563 RID: 5475
	private bool firstTime = true;

	// Token: 0x04001564 RID: 5476
	private PhotonView view;
}

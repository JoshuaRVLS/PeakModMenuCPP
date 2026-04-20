using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001B9 RID: 441
public class TempleConfig : MonoBehaviourPunCallbacks
{
	// Token: 0x06000E22 RID: 3618 RVA: 0x000471E2 File Offset: 0x000453E2
	private void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000E23 RID: 3619 RVA: 0x000471F0 File Offset: 0x000453F0
	private void Start()
	{
		for (int i = 0; i < this.columns.Count; i++)
		{
			this.positions.Add(this.columns[i].transform.position);
		}
	}

	// Token: 0x06000E24 RID: 3620 RVA: 0x00047234 File Offset: 0x00045434
	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		if (this.view.IsMine)
		{
			this.view.RPC("CreateTemple_RPC", RpcTarget.AllBuffered, new object[]
			{
				(int)DateTime.Now.Ticks
			});
		}
	}

	// Token: 0x06000E25 RID: 3621 RVA: 0x00047284 File Offset: 0x00045484
	[PunRPC]
	public void CreateTemple_RPC(int seed)
	{
		Debug.Log("Set Seed");
		Random.InitState(seed);
		List<GameObject> list = this.columns;
		list = (from x in list
		orderby Random.value
		select x).ToList<GameObject>();
		for (int i = 0; i < list.Count; i++)
		{
			list[i].transform.position = this.positions[i];
			this.columns[i].transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, (float)((int)(Random.value * 4f) * 90)));
		}
		for (int j = 0; j < this.arrowShooters.Length; j++)
		{
			if (Random.value < this.arrowShooterChance)
			{
				this.arrowShooters[j].SetActive(true);
			}
			else
			{
				this.arrowShooters[j].SetActive(false);
			}
		}
	}

	// Token: 0x06000E26 RID: 3622 RVA: 0x00047379 File Offset: 0x00045579
	private void Update()
	{
	}

	// Token: 0x04000BEB RID: 3051
	private PhotonView view;

	// Token: 0x04000BEC RID: 3052
	[Range(0f, 1f)]
	public float arrowShooterChance;

	// Token: 0x04000BED RID: 3053
	public List<GameObject> columns;

	// Token: 0x04000BEE RID: 3054
	private List<Vector3> positions = new List<Vector3>();

	// Token: 0x04000BEF RID: 3055
	public GameObject[] arrowShooters;
}

using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000A7 RID: 167
public class BerryVine : Spawner
{
	// Token: 0x06000666 RID: 1638 RVA: 0x00024E90 File Offset: 0x00023090
	protected override List<Transform> GetSpawnSpots()
	{
		Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
		List<Transform> list = new List<Transform>();
		for (int i = 1; i < componentsInChildren.Length - 1; i++)
		{
			list.Add(componentsInChildren[i].transform);
		}
		return list;
	}

	// Token: 0x06000667 RID: 1639 RVA: 0x00024ECC File Offset: 0x000230CC
	public override List<PhotonView> SpawnItems(List<Transform> spawnSpots)
	{
		List<PhotonView> list = new List<PhotonView>();
		if (!PhotonNetwork.IsMasterClient)
		{
			return list;
		}
		List<Transform> list2 = new List<Transform>(spawnSpots);
		GameObject gameObject = this.spawns.GetSpawns(1, true)[0];
		float num = Random.value;
		num = Mathf.Pow(num, this.randomPow);
		int num2 = Mathf.RoundToInt(Mathf.Lerp(this.possibleBerries.x, this.possibleBerries.y, num));
		int num3 = 0;
		while (num3 < spawnSpots.Count && num3 < num2)
		{
			int index = Random.Range(0, list2.Count);
			Item component = PhotonNetwork.InstantiateItemRoom(gameObject.name, list2[index].position + this.spawnOffsetWorldSpace, Quaternion.identity).GetComponent<Item>();
			list.Add(component.GetComponent<PhotonView>());
			if (this.spawnUpTowardsTarget)
			{
				component.transform.up = (this.spawnUpTowardsTarget.position - component.transform.position).normalized;
			}
			component.transform.rotation = Quaternion.Euler(0f, (float)Random.Range(0, 360), 0f);
			if (component != null)
			{
				component.GetComponent<PhotonView>().RPC("SetKinematicRPC", RpcTarget.AllBuffered, new object[]
				{
					true,
					component.transform.position,
					component.transform.rotation
				});
			}
			list2.RemoveAt(index);
			num3++;
		}
		return list;
	}

	// Token: 0x04000662 RID: 1634
	public Vector2 possibleBerries;

	// Token: 0x04000663 RID: 1635
	public Vector3 spawnOffsetWorldSpace;

	// Token: 0x04000664 RID: 1636
	public float randomPow = 1f;
}

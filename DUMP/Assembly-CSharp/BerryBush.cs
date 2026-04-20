using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000A6 RID: 166
public class BerryBush : Spawner
{
	// Token: 0x06000663 RID: 1635 RVA: 0x00024C50 File Offset: 0x00022E50
	public override List<PhotonView> SpawnItems(List<Transform> spawnSpots)
	{
		List<PhotonView> list = new List<PhotonView>();
		if (!PhotonNetwork.IsMasterClient)
		{
			return list;
		}
		List<Transform> list2 = new List<Transform>(spawnSpots);
		GameObject gameObject;
		if (this.spawnMode == Spawner.SpawnMode.SingleItem)
		{
			gameObject = this.spawnedObjectPrefab;
		}
		else
		{
			gameObject = LootData.GetRandomItem(this.spawnPool, false);
		}
		float num = Random.value;
		num = Mathf.Pow(num, this.randomPow);
		int num2 = Mathf.RoundToInt(Mathf.Lerp(this.possibleBerries.x, this.possibleBerries.y, num));
		int num3 = 0;
		while (num3 < spawnSpots.Count && num3 < num2)
		{
			int index = Random.Range(0, list2.Count);
			if (!(gameObject == null))
			{
				Item component = PhotonNetwork.InstantiateItemRoom(gameObject.name, list2[index].position, Quaternion.identity).GetComponent<Item>();
				list.Add(component.GetComponent<PhotonView>());
				if (this.spawnUpTowardsTarget)
				{
					component.transform.up = (this.spawnUpTowardsTarget.position - component.transform.position).normalized;
					component.transform.Rotate(Vector3.up, Random.Range(0f, 360f), Space.Self);
				}
				if (this.spawnTransformIsSpawnerTransform)
				{
					component.transform.rotation = list2[index].rotation;
					component.transform.Rotate(Vector3.up, Random.Range(0f, 360f), Space.Self);
				}
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
			}
			num3++;
		}
		return list;
	}

	// Token: 0x06000664 RID: 1636 RVA: 0x00024E34 File Offset: 0x00023034
	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta;
		for (int i = 0; i < this.spawnSpots.Count; i++)
		{
			Gizmos.DrawSphere(this.spawnSpots[i].position, 0.25f);
		}
	}

	// Token: 0x0400065F RID: 1631
	public bool debug;

	// Token: 0x04000660 RID: 1632
	public Vector2 possibleBerries;

	// Token: 0x04000661 RID: 1633
	public float randomPow = 1f;
}

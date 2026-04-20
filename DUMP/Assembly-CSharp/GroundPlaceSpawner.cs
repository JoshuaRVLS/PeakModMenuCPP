using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020000AD RID: 173
public class GroundPlaceSpawner : Spawner
{
	// Token: 0x060006A2 RID: 1698 RVA: 0x00025F58 File Offset: 0x00024158
	public override List<PhotonView> SpawnItems(List<Transform> spawnSpots)
	{
		List<PhotonView> list = new List<PhotonView>();
		if (!PhotonNetwork.IsMasterClient)
		{
			return list;
		}
		List<Transform> list2 = new List<Transform>(spawnSpots);
		GameObject gameObject = this.spawns.GetSpawns(1, true)[0];
		int num = Random.Range(Mathf.FloorToInt(this.possibleItems.x), Mathf.FloorToInt(this.possibleItems.y + 1f));
		int num2 = 0;
		while (num2 < spawnSpots.Count && num2 < num)
		{
			int index = Random.Range(0, list2.Count);
			RaycastHit raycastHit;
			if (Physics.Raycast(list2[index].position, -base.transform.up, out raycastHit, 100f, HelperFunctions.GetMask(HelperFunctions.LayerType.TerrainMap)))
			{
				Item component = PhotonNetwork.InstantiateItemRoom(gameObject.name, raycastHit.point, HelperFunctions.GetRandomRotationWithUp(raycastHit.normal)).GetComponent<Item>();
				list.Add(component.GetComponent<PhotonView>());
				component.SetKinematicNetworked(true, component.transform.position, component.transform.rotation);
			}
			list2.RemoveAt(index);
			num2++;
		}
		return list;
	}

	// Token: 0x04000693 RID: 1683
	[FormerlySerializedAs("possibleBerries")]
	public Vector2 possibleItems;
}

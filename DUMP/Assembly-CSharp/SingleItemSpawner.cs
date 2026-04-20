using System;
using System.Collections.Generic;
using Peak;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001B1 RID: 433
public class SingleItemSpawner : MonoBehaviour, ISpawner
{
	// Token: 0x06000DFE RID: 3582 RVA: 0x0004667C File Offset: 0x0004487C
	public virtual List<PhotonView> TrySpawnItems()
	{
		if (this.belowAscentRequirement != -1 && Ascents.currentAscent >= this.belowAscentRequirement)
		{
			Debug.LogError(string.Format("Not spawning: {0} because ascent is too high: {1}", this.prefab, Ascents.currentAscent));
			return new List<PhotonView>();
		}
		SpawnedItemTracker spawnedItemTracker;
		bool flag = this.HasSpawnTracking(out spawnedItemTracker);
		if (flag && spawnedItemTracker.HasSpawnHistory)
		{
			List<PhotonView> list = spawnedItemTracker.SpawnAndTrackFromItemHistory();
			if (list.Count > 1)
			{
				Debug.LogWarning(string.Format("Uhhh, that's weird. Our single item spawner had {0} items in its spawn ", list.Count) + "history...");
			}
			foreach (PhotonView newItem in list)
			{
				this.InitializePhysics(newItem);
			}
			return list;
		}
		if (this.playersInRoomRequirement > PhotonNetwork.PlayerList.Length)
		{
			Debug.LogError(string.Format("Not spawning: {0} because of players in room req: {1}", this.prefab, this.playersInRoomRequirement));
			return new List<PhotonView>();
		}
		PhotonView component = PhotonNetwork.InstantiateItemRoom(this.prefab.name, base.transform.position + Vector3.up * 0.1f, base.transform.rotation).GetComponent<PhotonView>();
		this.InitializePhysics(component);
		List<PhotonView> list2 = new List<PhotonView>
		{
			component
		};
		if (flag)
		{
			spawnedItemTracker.TrackSpawnedItems(list2);
		}
		return list2;
	}

	// Token: 0x06000DFF RID: 3583 RVA: 0x000467EC File Offset: 0x000449EC
	private void InitializePhysics(PhotonView newItem)
	{
		if (this.isKinematic)
		{
			newItem.RPC("SetKinematicRPC", RpcTarget.AllBuffered, new object[]
			{
				true,
				newItem.transform.position,
				newItem.transform.rotation
			});
		}
	}

	// Token: 0x06000E00 RID: 3584 RVA: 0x00046842 File Offset: 0x00044A42
	private void OnDrawGizmos()
	{
		if (this.prefab != null)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(base.transform.position, 0.25f);
		}
	}

	// Token: 0x04000BD2 RID: 3026
	public bool isKinematic;

	// Token: 0x04000BD3 RID: 3027
	public GameObject prefab;

	// Token: 0x04000BD4 RID: 3028
	public int playersInRoomRequirement;

	// Token: 0x04000BD5 RID: 3029
	public int belowAscentRequirement = -1;
}

using System;
using System.Collections.Generic;
using System.Linq;
using Peak.Network;
using Photon.Pun;
using UnityEngine;

namespace Peak
{
	// Token: 0x020003CA RID: 970
	public class SpawnedItemTracker : MonoBehaviour
	{
		// Token: 0x17000189 RID: 393
		// (get) Token: 0x060019A1 RID: 6561 RVA: 0x00081A0C File Offset: 0x0007FC0C
		public IEnumerable<Item> SpawnedItems
		{
			get
			{
				return from view in this._spawnedItems
				where view != null
				select view.GetComponent<Item>() into item
				where item != null
				select item;
			}
		}

		// Token: 0x1700018A RID: 394
		// (get) Token: 0x060019A2 RID: 6562 RVA: 0x00081A8B File Offset: 0x0007FC8B
		// (set) Token: 0x060019A3 RID: 6563 RVA: 0x00081A93 File Offset: 0x0007FC93
		public Hash128 SpawnerId { get; private set; }

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x060019A4 RID: 6564 RVA: 0x00081A9C File Offset: 0x0007FC9C
		// (set) Token: 0x060019A5 RID: 6565 RVA: 0x00081AA4 File Offset: 0x0007FCA4
		public bool HasSpawnHistory { get; private set; }

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x060019A6 RID: 6566 RVA: 0x00081AAD File Offset: 0x0007FCAD
		private string PathInSceneHierarchy
		{
			get
			{
				return string.Join("/", from t in base.gameObject.GetComponentsInParent<Transform>().Reverse<Transform>()
				select t.name);
			}
		}

		// Token: 0x060019A7 RID: 6567 RVA: 0x00081AF0 File Offset: 0x0007FCF0
		private Hash128 ComputeId()
		{
			Hash128 result = Hash128.Compute(this.PathInSceneHierarchy);
			Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
			for (int i = 0; i < 16; i++)
			{
				result.Append(localToWorldMatrix[i]);
			}
			return result;
		}

		// Token: 0x060019A8 RID: 6568 RVA: 0x00081B32 File Offset: 0x0007FD32
		public void Init()
		{
			this.SpawnerId = this.ComputeId();
			this._spawnedItems = new List<PhotonView>();
		}

		// Token: 0x060019A9 RID: 6569 RVA: 0x00081B4B File Offset: 0x0007FD4B
		public void PopulateHistory(List<SpawnedItemTracker.SpawnRecord> history)
		{
			this._historyFromSave = history;
			this.HasSpawnHistory = true;
		}

		// Token: 0x060019AA RID: 6570 RVA: 0x00081B5C File Offset: 0x0007FD5C
		public void TrackSpawnedItems(List<PhotonView> spawnedItems)
		{
			this.HasSpawnHistory = true;
			int count = this._spawnedItems.Count;
			foreach (PhotonView photonView in spawnedItems)
			{
				Item item;
				if (!photonView.TryGetComponent<Item>(out item))
				{
					Debug.LogWarning("Won't be able to respawn " + photonView.name + " later because it doesn't have an Item component", photonView);
				}
				else
				{
					this._spawnedItems.Add(photonView);
				}
			}
			if (this._spawnedItems.Count - count > 0)
			{
				Debug.Log(string.Format("{0} tracking {1} items.", base.name, this._spawnedItems.Count - count), this);
			}
		}

		// Token: 0x060019AB RID: 6571 RVA: 0x00081C24 File Offset: 0x0007FE24
		public List<PhotonView> SpawnAndTrackFromItemHistory()
		{
			if (!NetCode.Session.IsHost)
			{
				Debug.LogError("Only the host can spawn items!");
				return null;
			}
			if (!this.HasSpawnHistory)
			{
				Debug.LogError("No spawn history available!");
				return null;
			}
			Debug.Log(string.Format("{0}: Spawning {1} items from spawn history", base.name, this._historyFromSave.Count), this);
			List<PhotonView> list = new List<PhotonView>();
			foreach (SpawnedItemTracker.SpawnRecord spawnRecord in this._historyFromSave)
			{
				Item item;
				if (!ItemDatabase.TryGetItem(spawnRecord.itemId, out item))
				{
					Debug.LogWarning(string.Format("Unable to find item prefab for {0}. Will have to skip this spawn.", spawnRecord.itemId));
				}
				else
				{
					GameObject go = PhotonNetwork.InstantiateItemRoom(item.name, spawnRecord.position, spawnRecord.rotation);
					list.Add(go.GetPhotonView());
				}
			}
			this.TrackSpawnedItems(list);
			return list;
		}

		// Token: 0x04001744 RID: 5956
		private List<SpawnedItemTracker.SpawnRecord> _historyFromSave;

		// Token: 0x04001745 RID: 5957
		private List<PhotonView> _spawnedItems;

		// Token: 0x0200056D RID: 1389
		[Serializable]
		public struct SpawnRecord
		{
			// Token: 0x06001FDC RID: 8156 RVA: 0x000905A4 File Offset: 0x0008E7A4
			internal SpawnRecord(Item spawnedItem)
			{
				this.itemId = spawnedItem.itemID;
				this.position = spawnedItem.transform.position;
				this.rotation = spawnedItem.transform.rotation;
			}

			// Token: 0x04001D5F RID: 7519
			[SerializeField]
			public ushort itemId;

			// Token: 0x04001D60 RID: 7520
			[SerializeField]
			public Vector3 position;

			// Token: 0x04001D61 RID: 7521
			[SerializeField]
			public Quaternion rotation;
		}
	}
}

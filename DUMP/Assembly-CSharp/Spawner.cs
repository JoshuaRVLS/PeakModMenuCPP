using System;
using System.Collections.Generic;
using Peak;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;
using Zorro.Core;

// Token: 0x020000B4 RID: 180
public class Spawner : MonoBehaviourPunCallbacks, ISpawner
{
	// Token: 0x17000086 RID: 134
	// (get) Token: 0x060006C2 RID: 1730 RVA: 0x00026C59 File Offset: 0x00024E59
	protected bool isWeightedSpawnPoints
	{
		get
		{
			return this.spawnPointMode == Spawner.SpawnPointMode.WeightedLists;
		}
	}

	// Token: 0x17000087 RID: 135
	// (get) Token: 0x060006C3 RID: 1731 RVA: 0x00026C64 File Offset: 0x00024E64
	private bool isSpawnPool
	{
		get
		{
			return this.spawnMode == Spawner.SpawnMode.SpawnPool;
		}
	}

	// Token: 0x17000088 RID: 136
	// (get) Token: 0x060006C4 RID: 1732 RVA: 0x00026C6F File Offset: 0x00024E6F
	private bool isSingleItem
	{
		get
		{
			return this.spawnMode == Spawner.SpawnMode.SingleItem;
		}
	}

	// Token: 0x17000089 RID: 137
	// (get) Token: 0x060006C5 RID: 1733 RVA: 0x00026C7A File Offset: 0x00024E7A
	private bool isHeightBasedSpawnPool
	{
		get
		{
			return this.spawnMode == Spawner.SpawnMode.HeightBasedSpawnPools;
		}
	}

	// Token: 0x1700008A RID: 138
	// (get) Token: 0x060006C6 RID: 1734 RVA: 0x00026C85 File Offset: 0x00024E85
	public bool hasSpawnList
	{
		get
		{
			return this.isSpawnPool && this.spawns != null && this.spawnPool == SpawnPool.None;
		}
	}

	// Token: 0x060006C7 RID: 1735 RVA: 0x00026CA8 File Offset: 0x00024EA8
	public void ForceSpawn()
	{
		this.TrySpawnItems();
	}

	// Token: 0x060006C8 RID: 1736 RVA: 0x00026CB4 File Offset: 0x00024EB4
	public List<PhotonView> TrySpawnItems()
	{
		this._hasTriedToSpawn = true;
		List<PhotonView> list = new List<PhotonView>();
		if (!PhotonNetwork.IsMasterClient)
		{
			return list;
		}
		if (this.belowAscentRequirement != -1 && Ascents.currentAscent >= this.belowAscentRequirement)
		{
			Debug.Log(string.Format("Not spawning: {0} because ascent is too high: {1}", base.gameObject.name, Ascents.currentAscent));
			return list;
		}
		SpawnedItemTracker spawnedItemTracker;
		bool flag = this.HasSpawnTracking(out spawnedItemTracker);
		if (flag && spawnedItemTracker.HasSpawnHistory)
		{
			this._hasSuccessfullySpawned = true;
			Debug.Log("Tracked history found for " + base.name + "! Attempting to spawn from history", this);
			List<PhotonView> list2 = spawnedItemTracker.SpawnAndTrackFromItemHistory();
			foreach (PhotonView photonView in list2)
			{
				this.InitializePhysics(photonView.GetComponent<Item>());
			}
			return list2;
		}
		if (!this.spawnOnStart)
		{
			return list;
		}
		if (this.playersInRoomRequirement > PhotonNetwork.PlayerList.Length)
		{
			Debug.Log(string.Format("Not spawning: {0} because of players in room req: {1}", base.gameObject.name, this.playersInRoomRequirement));
			return list;
		}
		if (Random.Range(0f, 1f) <= this.baseSpawnChance)
		{
			list.AddRange(this.SpawnItems(this.GetSpawnSpots()));
		}
		if (flag)
		{
			spawnedItemTracker.TrackSpawnedItems(list);
		}
		this._hasSuccessfullySpawned = true;
		return list;
	}

	// Token: 0x060006C9 RID: 1737 RVA: 0x00026E18 File Offset: 0x00025018
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		if (this._hasTriedToSpawn && !this._hasSuccessfullySpawned && this.playersInRoomRequirement <= PhotonNetwork.PlayerList.Length)
		{
			this.TrySpawnItems();
		}
	}

	// Token: 0x060006CA RID: 1738 RVA: 0x00026E40 File Offset: 0x00025040
	protected virtual List<Transform> GetSpawnSpots()
	{
		Spawner.SpawnPointMode spawnPointMode = this.spawnPointMode;
		if (spawnPointMode == Spawner.SpawnPointMode.SingleList)
		{
			return this.spawnSpots;
		}
		if (spawnPointMode != Spawner.SpawnPointMode.WeightedLists)
		{
			return new List<Transform>();
		}
		return this.weightedSpawnSpots.RandomSelection((Spawner.WeightedSpawnPointEntry w) => w.weight).spawnSpots;
	}

	// Token: 0x060006CB RID: 1739 RVA: 0x00026E9C File Offset: 0x0002509C
	public virtual List<PhotonView> SpawnItems(List<Transform> spawnSpots)
	{
		List<PhotonView> list = new List<PhotonView>();
		Debug.Log(string.Format("Spawning: {0}", base.gameObject));
		if (!PhotonNetwork.IsMasterClient)
		{
			return list;
		}
		if (spawnSpots.Count == 0)
		{
			return list;
		}
		List<GameObject> objectsToSpawn = this.GetObjectsToSpawn(spawnSpots.Count, this.canRepeatSpawns);
		int num = 0;
		while (num < spawnSpots.Count && num < objectsToSpawn.Count)
		{
			if (!(objectsToSpawn[num] == null))
			{
				Item component = PhotonNetwork.InstantiateItemRoom(objectsToSpawn[num].name, spawnSpots[num].position, spawnSpots[num].rotation).GetComponent<Item>();
				list.Add(component.GetComponent<PhotonView>());
				if (this.spawnUpTowardsTarget)
				{
					component.transform.up = (this.spawnUpTowardsTarget.position - component.transform.position).normalized;
				}
				if (this.centerItemsVisually)
				{
					Vector3 b = spawnSpots[num].position - component.Center();
					component.transform.position += b;
				}
				this.OffsetSpawn(component);
				this.InitializePhysics(component);
			}
			num++;
		}
		return list;
	}

	// Token: 0x060006CC RID: 1740 RVA: 0x00026FDC File Offset: 0x000251DC
	protected void InitializePhysics(Item newItem)
	{
		newItem.ForceSyncForFrames(10);
		if (newItem != null && this.isKinematic)
		{
			newItem.GetComponent<PhotonView>().RPC("SetKinematicRPC", RpcTarget.AllBuffered, new object[]
			{
				true,
				newItem.transform.position,
				newItem.transform.rotation
			});
		}
	}

	// Token: 0x060006CD RID: 1741 RVA: 0x00027048 File Offset: 0x00025248
	protected virtual void OffsetSpawn(Item item)
	{
	}

	// Token: 0x060006CE RID: 1742 RVA: 0x0002704C File Offset: 0x0002524C
	protected SpawnPool GetSpawnPool()
	{
		if (this.isHeightBasedSpawnPool)
		{
			for (int i = this.heightBasedSpawnPools.Count - 1; i >= 0; i--)
			{
				Spawner.HeightBasedSpawnListEntry heightBasedSpawnListEntry = this.heightBasedSpawnPools[i];
				if (i == 0 || (base.transform.position.z > heightBasedSpawnListEntry.minimumZ && (!heightBasedSpawnListEntry.hasBiomeRequirement || Singleton<MapHandler>.Instance.BiomeIsPresent(heightBasedSpawnListEntry.biomeRequirement))))
				{
					return heightBasedSpawnListEntry.spawnPool;
				}
			}
		}
		return this.spawnPool;
	}

	// Token: 0x060006CF RID: 1743 RVA: 0x000270C8 File Offset: 0x000252C8
	private List<GameObject> GetObjectsToSpawn(int spawnCount, bool canRepeat = false)
	{
		if (this.isSingleItem)
		{
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < spawnCount; i++)
			{
				list.Add(this.spawnedObjectPrefab);
			}
			return list;
		}
		if (this.isSpawnPool)
		{
			return LootData.GetRandomItems(this.spawnPool, spawnCount, canRepeat, this.fallbackSpawn);
		}
		if (this.isHeightBasedSpawnPool)
		{
			for (int j = this.heightBasedSpawnPools.Count - 1; j >= 0; j--)
			{
				Spawner.HeightBasedSpawnListEntry heightBasedSpawnListEntry = this.heightBasedSpawnPools[j];
				if (j == 0 || (base.transform.position.z > heightBasedSpawnListEntry.minimumZ && (!heightBasedSpawnListEntry.hasBiomeRequirement || Singleton<MapHandler>.Instance.BiomeIsPresent(heightBasedSpawnListEntry.biomeRequirement))))
				{
					return LootData.GetRandomItems(heightBasedSpawnListEntry.spawnPool, spawnCount, canRepeat, this.fallbackSpawn);
				}
			}
		}
		List<GameObject> list2 = new List<GameObject>();
		for (int k = 0; k < spawnCount; k++)
		{
			list2.Add(null);
		}
		return list2;
	}

	// Token: 0x060006D0 RID: 1744 RVA: 0x000271B4 File Offset: 0x000253B4
	private void FindOutdatedSpawners()
	{
		bool flag = false;
		Spawner[] array = Object.FindObjectsOfType<Spawner>();
		string text = "";
		foreach (Spawner spawner in array)
		{
			if (spawner.hasSpawnList)
			{
				text = text + "Found outdated spawner: " + spawner.gameObject.name + "\n";
				flag = true;
			}
		}
		if (!flag)
		{
			Debug.Log("NO OUTDATED SPAWNERS! YIPPEEEE");
			return;
		}
		Debug.Log(text);
	}

	// Token: 0x060006D1 RID: 1745 RVA: 0x00027220 File Offset: 0x00025420
	[ContextMenu("Test Weighted Spawn Points")]
	private void TestWeightedSpawnPoints()
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		int num = 1000;
		for (int i = 0; i < num; i++)
		{
			Spawner.WeightedSpawnPointEntry item = this.weightedSpawnSpots.RandomSelection((Spawner.WeightedSpawnPointEntry w) => w.weight);
			int num2 = this.weightedSpawnSpots.IndexOf(item);
			if (dictionary.ContainsKey(num2))
			{
				Dictionary<int, int> dictionary2 = dictionary;
				int key = num2;
				int num3 = dictionary2[key];
				dictionary2[key] = num3 + 1;
			}
			else
			{
				dictionary.Add(num2, 1);
			}
		}
		string text = string.Format("Test spawned {0} times.\n", num);
		foreach (int num4 in dictionary.Keys)
		{
			text += string.Format("Chose #{0} {1} times. ({2}%)\n", num4, dictionary[num4], (float)dictionary[num4] / (float)num * 100f);
		}
		Debug.Log(text);
	}

	// Token: 0x060006D2 RID: 1746 RVA: 0x00027344 File Offset: 0x00025544
	public void DebugSpawnRates()
	{
		SpawnPool spawnPool = this.GetSpawnPool();
		if (spawnPool != SpawnPool.None)
		{
			LootData.PrintOdds(spawnPool);
		}
	}

	// Token: 0x1700008B RID: 139
	// (get) Token: 0x060006D3 RID: 1747 RVA: 0x00027364 File Offset: 0x00025564
	private bool hasMultipleFlagsSet
	{
		get
		{
			int num = 0;
			foreach (object obj in Enum.GetValues(typeof(SpawnPool)))
			{
				SpawnPool spawnPool = (SpawnPool)obj;
				if (spawnPool != SpawnPool.None && this.spawnPool.HasFlag(spawnPool))
				{
					if (num >= 1)
					{
						return true;
					}
					num++;
				}
			}
			return false;
		}
	}

	// Token: 0x040006B7 RID: 1719
	public int playersInRoomRequirement;

	// Token: 0x040006B8 RID: 1720
	public int belowAscentRequirement = -1;

	// Token: 0x040006B9 RID: 1721
	public Spawner.SpawnMode spawnMode = Spawner.SpawnMode.SpawnPool;

	// Token: 0x040006BA RID: 1722
	[FormerlySerializedAs("spawnCountMode")]
	public Spawner.SpawnPointMode spawnPointMode;

	// Token: 0x040006BB RID: 1723
	private bool _hasTriedToSpawn;

	// Token: 0x040006BC RID: 1724
	private bool _hasSuccessfullySpawned;

	// Token: 0x040006BD RID: 1725
	private bool _tryingAgainIfPlayerCountChanges;

	// Token: 0x040006BE RID: 1726
	[Range(0f, 1f)]
	public float baseSpawnChance;

	// Token: 0x040006BF RID: 1727
	public GameObject spawnedObjectPrefab;

	// Token: 0x040006C0 RID: 1728
	public SpawnList spawns;

	// Token: 0x040006C1 RID: 1729
	public SpawnPool spawnPool;

	// Token: 0x040006C2 RID: 1730
	public bool canRepeatSpawns;

	// Token: 0x040006C3 RID: 1731
	public List<Transform> spawnSpots;

	// Token: 0x040006C4 RID: 1732
	public List<Spawner.WeightedSpawnPointEntry> weightedSpawnSpots = new List<Spawner.WeightedSpawnPointEntry>();

	// Token: 0x040006C5 RID: 1733
	public Transform spawnUpTowardsTarget;

	// Token: 0x040006C6 RID: 1734
	public bool spawnTransformIsSpawnerTransform;

	// Token: 0x040006C7 RID: 1735
	public bool spawnAwayFromUpTarget;

	// Token: 0x040006C8 RID: 1736
	public bool centerItemsVisually;

	// Token: 0x040006C9 RID: 1737
	public bool spawnOnStart;

	// Token: 0x040006CA RID: 1738
	public bool isKinematic = true;

	// Token: 0x040006CB RID: 1739
	public List<Spawner.HeightBasedSpawnListEntry> heightBasedSpawnPools;

	// Token: 0x040006CC RID: 1740
	public GameObject fallbackSpawn;

	// Token: 0x0200044E RID: 1102
	public enum SpawnMode
	{
		// Token: 0x040018EA RID: 6378
		SingleItem,
		// Token: 0x040018EB RID: 6379
		SpawnPool,
		// Token: 0x040018EC RID: 6380
		HeightBasedSpawnPools,
		// Token: 0x040018ED RID: 6381
		Guidebook
	}

	// Token: 0x0200044F RID: 1103
	public enum SpawnPointMode
	{
		// Token: 0x040018EF RID: 6383
		SingleList,
		// Token: 0x040018F0 RID: 6384
		WeightedLists
	}

	// Token: 0x02000450 RID: 1104
	[Serializable]
	public class HeightBasedSpawnListEntry
	{
		// Token: 0x040018F1 RID: 6385
		public SpawnPool spawnPool;

		// Token: 0x040018F2 RID: 6386
		public float minimumHeight;

		// Token: 0x040018F3 RID: 6387
		public float minimumZ;

		// Token: 0x040018F4 RID: 6388
		public bool hasBiomeRequirement;

		// Token: 0x040018F5 RID: 6389
		public Biome.BiomeType biomeRequirement;
	}

	// Token: 0x02000451 RID: 1105
	[Serializable]
	public class WeightedSpawnPointEntry
	{
		// Token: 0x040018F6 RID: 6390
		public List<Transform> spawnSpots;

		// Token: 0x040018F7 RID: 6391
		public int weight;

		// Token: 0x040018F8 RID: 6392
		[SerializeField]
		internal float percentageOdds;
	}
}

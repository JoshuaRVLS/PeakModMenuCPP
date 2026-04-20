using System;
using System.Collections;
using System.Collections.Generic;
using Peak.Network;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.Serizalization;

// Token: 0x020000BE RID: 190
public class GameUtils : MonoBehaviourPunCallbacks
{
	// Token: 0x06000722 RID: 1826 RVA: 0x00028C98 File Offset: 0x00026E98
	public override void OnEnable()
	{
		base.OnEnable();
		this.m_inAirport = (base.gameObject.scene.name == "Airport");
	}

	// Token: 0x06000723 RID: 1827 RVA: 0x00028CD0 File Offset: 0x00026ED0
	private void Awake()
	{
		GameUtils.instance = this;
		this.photonView = base.GetComponent<PhotonView>();
		this.m_inAirport = (base.gameObject.scene.name == "Airport");
	}

	// Token: 0x06000724 RID: 1828 RVA: 0x00028D12 File Offset: 0x00026F12
	public static float GetTimeScale()
	{
		if (!GameUtils.instance)
		{
			return 1f;
		}
		if (GameUtils.instance.m_inAirport)
		{
			return 1f;
		}
		if (RunSettings.GetValue(RunSettings.SETTINGTYPE.TimeScale, false) == 1)
		{
			return 2f;
		}
		return 1f;
	}

	// Token: 0x06000725 RID: 1829 RVA: 0x00028D51 File Offset: 0x00026F51
	private IEnumerator Start()
	{
		while (!RunSettings.initialized)
		{
			yield return null;
		}
		Time.timeScale = GameUtils.GetTimeScale();
		yield break;
	}

	// Token: 0x06000726 RID: 1830 RVA: 0x00028D59 File Offset: 0x00026F59
	public void StartFeed(int giverID, int receiverID, ushort itemID, float totalItemTime)
	{
		this.feedData.Add(new FeedData
		{
			giverID = giverID,
			receiverID = receiverID,
			itemID = itemID,
			totalItemTime = totalItemTime
		});
		Action onUpdatedFeedData = this.OnUpdatedFeedData;
		if (onUpdatedFeedData == null)
		{
			return;
		}
		onUpdatedFeedData();
	}

	// Token: 0x06000727 RID: 1831 RVA: 0x00028D98 File Offset: 0x00026F98
	public List<FeedData> GetFeedDataForReceiver(int receiverID)
	{
		return this.feedData.FindAll((FeedData x) => x.receiverID == receiverID);
	}

	// Token: 0x06000728 RID: 1832 RVA: 0x00028DCC File Offset: 0x00026FCC
	public void EndFeed(int giverID)
	{
		for (int i = this.feedData.Count - 1; i >= 0; i--)
		{
			if (this.feedData[i].giverID == giverID)
			{
				this.feedData.RemoveAt(i);
			}
		}
		Action onUpdatedFeedData = this.OnUpdatedFeedData;
		if (onUpdatedFeedData == null)
		{
			return;
		}
		onUpdatedFeedData();
	}

	// Token: 0x06000729 RID: 1833 RVA: 0x00028E21 File Offset: 0x00027021
	private void FixedUpdate()
	{
		this.UpdateCollisionIgnores();
	}

	// Token: 0x0600072A RID: 1834 RVA: 0x00028E2C File Offset: 0x0002702C
	private void UpdateCollisionIgnores()
	{
		for (int i = this.ignoredCollidersCache.Count - 1; i >= 0; i--)
		{
			this.ignoredCollidersCache[i].time -= Time.fixedDeltaTime;
			if (this.ignoredCollidersCache[i].time <= 0f)
			{
				if (this.ignoredCollidersCache[i].colliderA != null && this.ignoredCollidersCache[i].colliderB != null)
				{
					Physics.IgnoreCollision(this.ignoredCollidersCache[i].colliderA, this.ignoredCollidersCache[i].colliderB, false);
				}
				this.ignoredCollidersCache.RemoveAt(i);
			}
		}
	}

	// Token: 0x0600072B RID: 1835 RVA: 0x00028EF4 File Offset: 0x000270F4
	public void IgnoreCollisions(GameObject object1, GameObject object2, float time)
	{
		Collider[] componentsInChildren = object1.GetComponentsInChildren<Collider>();
		Collider[] componentsInChildren2 = object2.GetComponentsInChildren<Collider>();
		this.IgnoreCollisions(componentsInChildren, componentsInChildren2, time);
	}

	// Token: 0x0600072C RID: 1836 RVA: 0x00028F18 File Offset: 0x00027118
	public void IgnoreCollisions(Character c, Item item, float time)
	{
		foreach (Collider collider in item.GetComponentsInChildren<Collider>())
		{
			foreach (Collider collider2 in c.refs.ragdoll.colliderList)
			{
				Physics.IgnoreCollision(collider2, collider);
				this.ignoredCollidersCache.Add(new GameUtils.IgnoredCollidersEntry(collider2, collider, time));
			}
		}
	}

	// Token: 0x0600072D RID: 1837 RVA: 0x00028FA4 File Offset: 0x000271A4
	public void IgnoreCollisions(Collider[] collidersA, Collider[] collidersB, float time)
	{
		foreach (Collider collider in collidersA)
		{
			foreach (Collider collider2 in collidersB)
			{
				Physics.IgnoreCollision(collider, collider2);
				this.ignoredCollidersCache.Add(new GameUtils.IgnoredCollidersEntry(collider, collider2, time));
			}
		}
	}

	// Token: 0x0600072E RID: 1838 RVA: 0x00028FFC File Offset: 0x000271FC
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (NetCode.Session.IsHost)
		{
			if (!NetCode.Matchmaking.PlayerIsInLobby(newPlayer.UserId))
			{
				Debug.LogError(newPlayer.NickName + " (" + newPlayer.UserId + ") is not in our Steam lobby. That's too sussy to allow. Kicking them.");
				NetCode.Session.Kick(newPlayer.UserId);
				return;
			}
			GameHandler.GetService<PersistentPlayerDataService>().SyncToPlayer(newPlayer);
			this.photonView.RPC("RPC_SyncAscent", newPlayer, new object[]
			{
				Ascents.currentAscent
			});
			RunSettings.PushRunSettings(newPlayer);
		}
		GlobalEvents.TriggerPlayerConnected(newPlayer);
	}

	// Token: 0x0600072F RID: 1839 RVA: 0x0002909A File Offset: 0x0002729A
	public override void OnPlayerLeftRoom(Photon.Realtime.Player newPlayer)
	{
		base.OnPlayerLeftRoom(newPlayer);
		GlobalEvents.TriggerPlayerDisconnected(newPlayer);
	}

	// Token: 0x06000730 RID: 1840 RVA: 0x000290A9 File Offset: 0x000272A9
	internal void SyncAscentAll(int ascent)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		this.photonView.RPC("RPC_SyncAscent", RpcTarget.All, new object[]
		{
			ascent
		});
	}

	// Token: 0x06000731 RID: 1841 RVA: 0x000290D3 File Offset: 0x000272D3
	[PunRPC]
	internal void RPC_SyncAscent(int ascent)
	{
		Ascents.currentAscent = ascent;
	}

	// Token: 0x06000732 RID: 1842 RVA: 0x000290DB File Offset: 0x000272DB
	[PunRPC]
	internal void RPC_WarpToSegment(int segment)
	{
		Debug.LogWarning("A naughty player attempted to use debug commands to warp everyone somewhere. That's not allowed!");
	}

	// Token: 0x06000733 RID: 1843 RVA: 0x000290E7 File Offset: 0x000272E7
	internal void ThrowBingBongAchievement()
	{
		this.photonView.RPC("ThrowBingBongAchievementRpc", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06000734 RID: 1844 RVA: 0x000290FF File Offset: 0x000272FF
	[PunRPC]
	private void ThrowBingBongAchievementRpc()
	{
		Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.BingBongBadge);
	}

	// Token: 0x06000735 RID: 1845 RVA: 0x0002910D File Offset: 0x0002730D
	internal void ThrowSacrificeAchievement()
	{
		this.photonView.RPC("ThrowSacrificeAchievementRpc", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06000736 RID: 1846 RVA: 0x00029125 File Offset: 0x00027325
	[PunRPC]
	private void ThrowSacrificeAchievementRpc()
	{
		Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.TwentyFourKaratBadge);
	}

	// Token: 0x06000737 RID: 1847 RVA: 0x00029133 File Offset: 0x00027333
	internal void IncrementPermanentItemsPlaced()
	{
		this.photonView.RPC("IncrementPermanentItemsPlacedRpc", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06000738 RID: 1848 RVA: 0x0002914B File Offset: 0x0002734B
	[PunRPC]
	private void IncrementPermanentItemsPlacedRpc()
	{
		Singleton<AchievementManager>.Instance.AddToRunBasedInt(RUNBASEDVALUETYPE.PermanentItemsPlaced, 1);
	}

	// Token: 0x06000739 RID: 1849 RVA: 0x00029159 File Offset: 0x00027359
	internal void IncrementFriendHealing(int amt, Photon.Realtime.Player target)
	{
		this.photonView.RPC("IncrementFriendHealingRpc", target, new object[]
		{
			amt
		});
	}

	// Token: 0x0600073A RID: 1850 RVA: 0x0002917B File Offset: 0x0002737B
	[PunRPC]
	private void IncrementFriendHealingRpc(int amt)
	{
		Singleton<AchievementManager>.Instance.AddToRunBasedInt(RUNBASEDVALUETYPE.FriendsHealedAmount, amt);
	}

	// Token: 0x0600073B RID: 1851 RVA: 0x00029189 File Offset: 0x00027389
	internal void IncrementFriendPoisonHealing(int amt, Photon.Realtime.Player target)
	{
		this.photonView.RPC("IncrementPoisonHealedStat", target, new object[]
		{
			amt
		});
	}

	// Token: 0x0600073C RID: 1852 RVA: 0x000291AB File Offset: 0x000273AB
	[PunRPC]
	protected void IncrementPoisonHealedStat(int amt)
	{
		Singleton<AchievementManager>.Instance.IncrementSteamStat(STEAMSTATTYPE.PoisonHealed, amt);
	}

	// Token: 0x0600073D RID: 1853 RVA: 0x000291BA File Offset: 0x000273BA
	internal void ThrowEmergencyPreparednessAchievement(Photon.Realtime.Player target)
	{
		this.photonView.RPC("ThrowEmergencyPreparednessAchievementRpc", target, Array.Empty<object>());
	}

	// Token: 0x0600073E RID: 1854 RVA: 0x000291D2 File Offset: 0x000273D2
	[PunRPC]
	private void ThrowEmergencyPreparednessAchievementRpc()
	{
		Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.EmergencyPreparednessBadge);
	}

	// Token: 0x0600073F RID: 1855 RVA: 0x000291E0 File Offset: 0x000273E0
	[PunRPC]
	private void InstantiateAndGrabRPC(string itemPrefabName, PhotonView characterView, int cookedAmount)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		Character component = characterView.GetComponent<Character>();
		component.refs.items.lastEquippedSlotTime = 0f;
		Bodypart bodypart = component.GetBodypart(BodypartType.Hip);
		PhotonNetwork.InstantiateItemRoom(itemPrefabName, bodypart.transform.position + bodypart.transform.forward * 0.5f, Quaternion.identity).GetComponent<Item>().Interact(component);
	}

	// Token: 0x06000740 RID: 1856 RVA: 0x00029254 File Offset: 0x00027454
	public void InstantiateAndGrab(Item item, Character character, int cookedAmount = 0)
	{
		this.photonView.RPC("InstantiateAndGrabRPC", RpcTarget.MasterClient, new object[]
		{
			item.gameObject.name,
			character.photonView,
			cookedAmount
		});
	}

	// Token: 0x06000741 RID: 1857 RVA: 0x0002928D File Offset: 0x0002748D
	public void SyncLava(bool started, bool ended, float time, float timeWaited)
	{
		this.photonView.RPC("RPC_SyncLava", RpcTarget.Others, new object[]
		{
			started,
			ended,
			time,
			timeWaited
		});
	}

	// Token: 0x06000742 RID: 1858 RVA: 0x000292CB File Offset: 0x000274CB
	[PunRPC]
	public void RPC_SyncLava(bool started, bool ended, float time, float timeWaited)
	{
		Singleton<LavaRising>.Instance.RecieveLavaData(started, ended, time, timeWaited);
	}

	// Token: 0x06000743 RID: 1859 RVA: 0x000292DC File Offset: 0x000274DC
	[ContextMenu("Debug All Items")]
	private void DebugAllItems()
	{
		string text = "";
		foreach (KeyValuePair<ushort, Item> keyValuePair in SingletonAsset<ItemDatabase>.Instance.itemLookup)
		{
			text = text + keyValuePair.Value.UIData.itemName + "\n";
		}
		Debug.Log(text);
		text = "";
		foreach (KeyValuePair<ushort, Item> keyValuePair2 in SingletonAsset<ItemDatabase>.Instance.itemLookup)
		{
			text = text + keyValuePair2.Value.gameObject.name + "\n";
		}
		Debug.Log(text);
	}

	// Token: 0x06000744 RID: 1860 RVA: 0x000293C0 File Offset: 0x000275C0
	public void SpawnResourceAtPositionNetworked(string resourcePath, Vector3 position, RpcTarget target)
	{
		this.photonView.RPC("RPC_SpawnResourceAtPosition", target, new object[]
		{
			resourcePath,
			position
		});
	}

	// Token: 0x06000745 RID: 1861 RVA: 0x000293E6 File Offset: 0x000275E6
	[PunRPC]
	public void RPC_SpawnResourceAtPosition(string resourcePath, Vector3 pos)
	{
		GameObject gameObject = Resources.Load(resourcePath) as GameObject;
		if (gameObject == null)
		{
			Debug.Log("OBJECT WAS NULL");
		}
		Object.Instantiate<GameObject>(gameObject, pos, Quaternion.identity);
	}

	// Token: 0x06000746 RID: 1862 RVA: 0x00029412 File Offset: 0x00027612
	private static RunSettingSyncData DeserializeStatusArray(in byte[] data)
	{
		return IBinarySerializable.GetFromManagedArray<RunSettingSyncData>(data);
	}

	// Token: 0x06000747 RID: 1863 RVA: 0x0002941B File Offset: 0x0002761B
	[PunRPC]
	public void RPC_SyncRunSettings(byte[] stream)
	{
		GameUtils.ApplySerializedRunSettings(stream);
	}

	// Token: 0x06000748 RID: 1864 RVA: 0x00029423 File Offset: 0x00027623
	public static void ApplySerializedRunSettings(byte[] stream)
	{
		RunSettings.SetDataFromSyncData(GameUtils.DeserializeStatusArray(stream));
	}

	// Token: 0x04000718 RID: 1816
	public static GameUtils instance;

	// Token: 0x04000719 RID: 1817
	[SerializeField]
	public List<FeedData> feedData = new List<FeedData>();

	// Token: 0x0400071A RID: 1818
	public Action OnUpdatedFeedData;

	// Token: 0x0400071B RID: 1819
	internal new PhotonView photonView;

	// Token: 0x0400071C RID: 1820
	internal bool m_inAirport;

	// Token: 0x0400071D RID: 1821
	public const string AIRPORT_SCENE_NAME = "Airport";

	// Token: 0x0400071E RID: 1822
	private List<GameUtils.IgnoredCollidersEntry> ignoredCollidersCache = new List<GameUtils.IgnoredCollidersEntry>();

	// Token: 0x02000457 RID: 1111
	private class IgnoredCollidersEntry
	{
		// Token: 0x06001C17 RID: 7191 RVA: 0x000870FF File Offset: 0x000852FF
		public IgnoredCollidersEntry(Collider A, Collider B, float time)
		{
			this.colliderA = A;
			this.colliderB = B;
			this.time = time;
		}

		// Token: 0x04001908 RID: 6408
		public Collider colliderA;

		// Token: 0x04001909 RID: 6409
		public Collider colliderB;

		// Token: 0x0400190A RID: 6410
		public float time;
	}
}

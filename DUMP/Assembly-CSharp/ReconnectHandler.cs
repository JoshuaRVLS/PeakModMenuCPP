using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Peak.Dev;
using Peak.Network;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using WebSocketSharp;
using Zorro.Core;

// Token: 0x02000167 RID: 359
public class ReconnectHandler : Singleton<ReconnectHandler>, IInRoomCallbacks
{
	// Token: 0x170000DE RID: 222
	// (get) Token: 0x06000BB9 RID: 3001 RVA: 0x0003ED4C File Offset: 0x0003CF4C
	internal IReadOnlyDictionary<string, ReconnectHandler.ReconnectDataRecord> Records
	{
		get
		{
			return this._reconnectRecords;
		}
	}

	// Token: 0x06000BBA RID: 3002 RVA: 0x0003ED54 File Offset: 0x0003CF54
	protected override void Awake()
	{
		base.Awake();
		this.photonView = base.GetComponent<PhotonView>();
		this._cachedPlayers = default(CachedPlayerList);
		this._reconnectRecords = new Dictionary<string, ReconnectHandler.ReconnectDataRecord>();
		this._achievementProgress = new Dictionary<string, SerializableRunBasedValues>();
		this._observedActors = new HashSet<int>();
		PhotonNetwork.AddCallbackTarget(this);
	}

	// Token: 0x06000BBB RID: 3003 RVA: 0x0003EDA6 File Offset: 0x0003CFA6
	private void Start()
	{
		this.Clear();
	}

	// Token: 0x06000BBC RID: 3004 RVA: 0x0003EDAE File Offset: 0x0003CFAE
	public void Clear()
	{
		this._observedActors.Clear();
		this._reconnectRecords.Clear();
		this._achievementProgress.Clear();
	}

	// Token: 0x06000BBD RID: 3005 RVA: 0x0003EDD1 File Offset: 0x0003CFD1
	public override void OnDestroy()
	{
		base.OnDestroy();
		PhotonNetwork.RemoveCallbackTarget(this);
		this._cachedPlayers.Dispose();
	}

	// Token: 0x06000BBE RID: 3006 RVA: 0x0003EDEC File Offset: 0x0003CFEC
	private void Update()
	{
		Character character;
		if (!PhotonNetwork.InRoom || !PlayerHandler.TryGetCharacter(PhotonNetwork.LocalPlayer.ActorNumber, out character))
		{
			return;
		}
		foreach (global::Player player in PlayerHandler.GetAllPlayers())
		{
			bool flag = this._observedActors.Contains(player.GetActorNumber());
			global::Player player2;
			bool flag2 = Singleton<MapHandler>.Instance != null && PlayerHandler.TryGetPlayer(player.GetActorNumber(), out player2);
			if (!flag && flag2)
			{
				this.RegisterPlayer(player);
			}
			else if (flag)
			{
				ReconnectHandler.ReconnectDataRecord reconnectDataRecord = this._reconnectRecords[player.GetUserId()];
				if (PhotonNetwork.ServerTimestamp - reconnectDataRecord.Timestamp < 1000)
				{
					this.UpdateReconnectData(player.character);
				}
			}
		}
	}

	// Token: 0x06000BBF RID: 3007 RVA: 0x0003EEC4 File Offset: 0x0003D0C4
	private void RegisterPlayer(global::Player player)
	{
		string userId = player.GetUserId();
		ReconnectHandler.ReconnectDataRecord reconnectDataRecord;
		if (this._reconnectRecords.TryGetValue(userId, out reconnectDataRecord))
		{
			Debug.Log("Already have reconnect record for " + player.name + " so no need to make a new one now:\n" + Pretty.Print(reconnectDataRecord));
		}
		else
		{
			ReconnectHandler.ReconnectDataRecord reconnectDataRecord2 = new ReconnectHandler.ReconnectDataRecord(userId, ReconnectData.CreateFromCharacter(player.character));
			if (!reconnectDataRecord2.Data.isValid)
			{
				Debug.LogWarning("Uh oh! Initial reconnectdata was not valid! This will cause problems if not overwritten.\n" + Pretty.Print(reconnectDataRecord2.Data));
			}
			this._reconnectRecords.Add(userId, reconnectDataRecord2);
			this._achievementProgress.Add(userId, SerializableRunBasedValues.ConstructNew());
		}
		player.character.data.CharacterStateUpdated += delegate()
		{
			this.UpdateReconnectData(player.character);
		};
		player.character.refs.afflictions.StatusesUpdated += this.UpdateReconnectData;
		global::Player player2 = player;
		player2.itemsChangedAction = (Action<ItemSlot[]>)Delegate.Combine(player2.itemsChangedAction, new Action<ItemSlot[]>(delegate(ItemSlot[] _)
		{
			this.UpdateReconnectData(player.character);
		}));
		Debug.Log(string.Concat(new string[]
		{
			"Registered ",
			player.name,
			". Most recent data: ",
			this._reconnectRecords[userId].ToPrettyString(),
			"\nKey: ",
			userId
		}));
		this._observedActors.Add(player.GetActorNumber());
	}

	// Token: 0x170000DF RID: 223
	// (get) Token: 0x06000BC0 RID: 3008 RVA: 0x0003F066 File Offset: 0x0003D266
	public IEnumerable<string> PlayersIdsInRecords
	{
		get
		{
			return this._reconnectRecords.Keys;
		}
	}

	// Token: 0x06000BC1 RID: 3009 RVA: 0x0003F074 File Offset: 0x0003D274
	public void UpdateAchievementProgress(global::Player player, SerializableRunBasedValues newValues)
	{
		string userId = player.GetUserId();
		if (!this._achievementProgress.ContainsKey(userId))
		{
			Debug.LogWarning(string.Format("We're missing {0}'s ({1}|#{2}) achievement record. ", player.name, userId, player.GetActorNumber()) + "How did that happen???");
		}
		this._achievementProgress[userId] = newValues;
	}

	// Token: 0x06000BC2 RID: 3010 RVA: 0x0003F0CE File Offset: 0x0003D2CE
	public void PopulateReconnectRecord(string playerId, ReconnectData reconnectData, SerializableRunBasedValues achievementData)
	{
		this._reconnectRecords.TryAdd(playerId, new ReconnectHandler.ReconnectDataRecord(playerId, reconnectData));
		this._achievementProgress.TryAdd(playerId, achievementData);
	}

	// Token: 0x06000BC3 RID: 3011 RVA: 0x0003F0F4 File Offset: 0x0003D2F4
	public void UpdateReconnectData(Character character)
	{
		ReconnectHandler.ReconnectDataRecord reconnectDataRecord = new ReconnectHandler.ReconnectDataRecord(character.player.GetUserId(), ReconnectData.CreateFromCharacter(character));
		if (!reconnectDataRecord.Data.isValid)
		{
			return;
		}
		this._reconnectRecords[character.player.GetUserId()] = reconnectDataRecord;
	}

	// Token: 0x06000BC4 RID: 3012 RVA: 0x0003F140 File Offset: 0x0003D340
	public void UpdateForAllCharacters()
	{
		foreach (global::Player player in PlayerHandler.GetAllPlayers())
		{
			if (!this._observedActors.Contains(player.GetActorNumber()))
			{
				global::Player player2;
				if (Singleton<MapHandler>.Instance != null && PlayerHandler.TryGetPlayer(player.GetActorNumber(), out player2))
				{
					Debug.LogWarning("What da heck! how is " + player.name + " not in the records by now? Registering them");
					this.RegisterPlayer(player);
					break;
				}
				Debug.LogError("Noooo!! Player " + player.name + " is in the game but we can't add them to the registry for some reason.");
				break;
			}
			else
			{
				this.UpdateReconnectData(player.character);
			}
		}
	}

	// Token: 0x06000BC5 RID: 3013 RVA: 0x0003F200 File Offset: 0x0003D400
	public void UpdateFromRevive(Character character, Vector3 revivePosition)
	{
		ReconnectData reconnectData = ReconnectData.CreateFromCharacter(character);
		if (reconnectData.dead || reconnectData.isValid || reconnectData.fullyPassedOut)
		{
			Debug.LogError("Post-revive reconnectdata came out malformed!:\n " + Pretty.Print(reconnectData));
		}
		reconnectData.position = revivePosition;
		reconnectData.isValid = true;
		string userId = character.player.GetUserId();
		this._reconnectRecords[userId] = new ReconnectHandler.ReconnectDataRecord(userId, reconnectData);
	}

	// Token: 0x06000BC6 RID: 3014 RVA: 0x0003F278 File Offset: 0x0003D478
	[PunRPC]
	public void RefreshReconnectDataTable(string key, ReconnectData data)
	{
		if (key.IsNullOrEmpty())
		{
			Debug.LogError("Received ReconnectData with no key. Can't do anything!");
			return;
		}
		if (this._reconnectRecords.ContainsKey(key))
		{
			Debug.LogWarning(string.Concat(new string[]
			{
				"Already have reconnect data for ",
				key,
				": ",
				Pretty.Print(this._reconnectRecords[key].Data),
				"\n Discarding new data: ",
				Pretty.Print(this._reconnectRecords[key].Data)
			}));
			return;
		}
		if (key == PhotonNetwork.LocalPlayer.UserId)
		{
			Debug.Log("Ignoring reconnect data for " + key + " because that's me (and I'll get it when my character spawns).\n" + Pretty.Print(data));
			return;
		}
		global::Player player = PlayerHandler.GetPlayer(key);
		Character character = (player != null) ? player.character : null;
		string text = (character != null) ? character.characterName : null;
		text = (text.IsNullOrEmpty() ? "UNKNOWN" : text);
		Debug.Log(string.Concat(new string[]
		{
			"ReconnectData for ",
			text,
			": ",
			Pretty.Print(data),
			"\nSetting for ID ",
			key,
			"."
		}));
		this._reconnectRecords.Add(key, new ReconnectHandler.ReconnectDataRecord(key, data));
		if (!character)
		{
			Debug.LogError("Player with id " + key + " doesn't have a character yet! Their scout report will break!");
		}
		if (character != null)
		{
			character.refs.stats.GetCaughtUp(data.scoutReport, false);
		}
	}

	// Token: 0x06000BC7 RID: 3015 RVA: 0x0003F404 File Offset: 0x0003D604
	private void PrintReconnectDataIfExists(string msgPrefix, string id)
	{
		ReconnectHandler.ReconnectDataRecord reconnectDataRecord;
		if (this._reconnectRecords.TryGetValue(id, out reconnectDataRecord))
		{
			Debug.Log(msgPrefix + Pretty.Print(reconnectDataRecord));
			return;
		}
		Debug.Log(msgPrefix + "No reconnect record found.");
	}

	// Token: 0x06000BC8 RID: 3016 RVA: 0x0003F448 File Offset: 0x0003D648
	public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		if (Character.localCharacter && Character.localCharacter.inAirport)
		{
			return;
		}
		if (PlayerHandler.IsBanned(newPlayer.UserId))
		{
			Debug.LogWarning("Banned player just joined room!!");
		}
		this.PrintReconnectDataIfExists(string.Format("Player {0} ({1}) just entered.\n", newPlayer.UserId, newPlayer.ActorNumber), newPlayer.UserId);
		this.BroadcastMissingPlayerDataIfHost(newPlayer);
	}

	// Token: 0x06000BC9 RID: 3017 RVA: 0x0003F4B3 File Offset: 0x0003D6B3
	public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
	{
		this.PrintReconnectDataIfExists(string.Format("Player {0} ({1}) just left.\n", otherPlayer.UserId, otherPlayer.ActorNumber), otherPlayer.UserId);
	}

	// Token: 0x06000BCA RID: 3018 RVA: 0x0003F4DC File Offset: 0x0003D6DC
	private void BroadcastMissingPlayerDataIfHost(Photon.Realtime.Player newPlayer)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		string[] currentPlayerIds = (from p in PlayerHandler.GetAllPlayers()
		select p.GetUserId()).ToArray<string>();
		string userId = PlayerHandler.GetUserId(NetCode.Session.HostId);
		ReconnectData reconnectData = ReconnectData.CreateFromCharacter(Character.localCharacter);
		this.photonView.RPC("RefreshReconnectDataTable", newPlayer, new object[]
		{
			userId,
			reconnectData
		});
		IEnumerable<KeyValuePair<string, ReconnectHandler.ReconnectDataRecord>> reconnectRecords = this._reconnectRecords;
		Func<KeyValuePair<string, ReconnectHandler.ReconnectDataRecord>, bool> <>9__1;
		Func<KeyValuePair<string, ReconnectHandler.ReconnectDataRecord>, bool> predicate;
		if ((predicate = <>9__1) == null)
		{
			predicate = (<>9__1 = ((KeyValuePair<string, ReconnectHandler.ReconnectDataRecord> record) => !currentPlayerIds.Contains(record.Key) && newPlayer.UserId != record.Key));
		}
		foreach (KeyValuePair<string, ReconnectHandler.ReconnectDataRecord> keyValuePair in reconnectRecords.Where(predicate))
		{
			string text;
			ReconnectHandler.ReconnectDataRecord reconnectDataRecord;
			keyValuePair.Deconstruct(out text, out reconnectDataRecord);
			string text2 = text;
			ReconnectHandler.ReconnectDataRecord reconnectDataRecord2 = reconnectDataRecord;
			Debug.Log("Sending " + text2 + ": " + Pretty.Print(reconnectDataRecord2.Data));
			this.photonView.RPC("RefreshReconnectDataTable", newPlayer, new object[]
			{
				text2,
				reconnectDataRecord2.Data
			});
		}
	}

	// Token: 0x06000BCB RID: 3019 RVA: 0x0003F640 File Offset: 0x0003D840
	public static bool TryGetReconnectData(Photon.Realtime.Player player, out ReconnectData data, out SerializableRunBasedValues achievementData)
	{
		return ReconnectHandler.TryGetReconnectData(player.UserId, out data, out achievementData);
	}

	// Token: 0x06000BCC RID: 3020 RVA: 0x0003F650 File Offset: 0x0003D850
	public static bool TryGetReconnectData(string playerId, out ReconnectData data, out SerializableRunBasedValues achievementData)
	{
		bool flag = Singleton<ReconnectHandler>.Instance != null && Singleton<ReconnectHandler>.Instance._achievementProgress.ContainsKey(playerId);
		data = (flag ? Singleton<ReconnectHandler>.Instance.GetReconnectData(playerId) : ReconnectData.Invalid);
		achievementData = (flag ? Singleton<ReconnectHandler>.Instance._achievementProgress[playerId] : SerializableRunBasedValues.ConstructNew());
		return flag && data.isValid;
	}

	// Token: 0x06000BCD RID: 3021 RVA: 0x0003F6C4 File Offset: 0x0003D8C4
	public ReconnectData GetReconnectData(Photon.Realtime.Player player)
	{
		return this.GetReconnectData(player.UserId);
	}

	// Token: 0x06000BCE RID: 3022 RVA: 0x0003F6D4 File Offset: 0x0003D8D4
	public ReconnectData GetReconnectData(string playerId)
	{
		ReconnectHandler.ReconnectDataRecord reconnectDataRecord;
		if (!this._reconnectRecords.TryGetValue(playerId, out reconnectDataRecord))
		{
			return ReconnectData.Invalid;
		}
		return reconnectDataRecord.Data;
	}

	// Token: 0x06000BCF RID: 3023 RVA: 0x0003F6FD File Offset: 0x0003D8FD
	public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x06000BD0 RID: 3024 RVA: 0x0003F6FF File Offset: 0x0003D8FF
	public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x06000BD1 RID: 3025 RVA: 0x0003F701 File Offset: 0x0003D901
	public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
	{
	}

	// Token: 0x04000AC4 RID: 2756
	private const int k_UpdatePeriodInMilliseconds = 1000;

	// Token: 0x04000AC5 RID: 2757
	private CachedPlayerList _cachedPlayers;

	// Token: 0x04000AC6 RID: 2758
	private Dictionary<string, ReconnectHandler.ReconnectDataRecord> _reconnectRecords;

	// Token: 0x04000AC7 RID: 2759
	private Dictionary<string, SerializableRunBasedValues> _achievementProgress;

	// Token: 0x04000AC8 RID: 2760
	private HashSet<int> _observedActors;

	// Token: 0x04000AC9 RID: 2761
	private PhotonView photonView;

	// Token: 0x0200049F RID: 1183
	internal struct ReconnectDataRecord : IPrettyPrintable
	{
		// Token: 0x06001D15 RID: 7445 RVA: 0x00089540 File Offset: 0x00087740
		public string ToPrettyString()
		{
			return Pretty.Print(this.Data) + string.Format("\n(Time: {0} | ID: {1})", this.Timestamp, this.UserID);
		}

		// Token: 0x06001D16 RID: 7446 RVA: 0x00089572 File Offset: 0x00087772
		internal ReconnectDataRecord(string userId, ReconnectData data)
		{
			this.Timestamp = PhotonNetwork.ServerTimestamp;
			this.UserID = userId;
			this.Data = data;
		}

		// Token: 0x04001A40 RID: 6720
		internal readonly int Timestamp;

		// Token: 0x04001A41 RID: 6721
		internal readonly string UserID;

		// Token: 0x04001A42 RID: 6722
		internal readonly ReconnectData Data;
	}
}

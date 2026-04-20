using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Peak.Network;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200015F RID: 351
public class PlayerHandler : GameService, IDisposable
{
	// Token: 0x170000DC RID: 220
	// (get) Token: 0x06000B8E RID: 2958 RVA: 0x0003E206 File Offset: 0x0003C406
	[Obsolete("Moved to static CharacterRegistered event. This will be deleted.")]
	public Action<Character> OnCharacterRegistered
	{
		get
		{
			return PlayerHandler.CharacterRegistered;
		}
	}

	// Token: 0x06000B8F RID: 2959 RVA: 0x0003E210 File Offset: 0x0003C410
	public PlayerHandler()
	{
		NetCode.Session.NetworkEventReceived += this.OnNetworkEvent;
	}

	// Token: 0x170000DD RID: 221
	// (get) Token: 0x06000B90 RID: 2960 RVA: 0x0003E265 File Offset: 0x0003C465
	protected static PlayerHandler Instance
	{
		get
		{
			return GameHandler.GetService<PlayerHandler>();
		}
	}

	// Token: 0x06000B91 RID: 2961 RVA: 0x0003E26C File Offset: 0x0003C46C
	public static List<Character> GetAllPlayerCharacters()
	{
		if (PlayerHandler.s_FrameCharactersLastCached == Time.frameCount)
		{
			return PlayerHandler.s_CharacterCache;
		}
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, Character> keyValuePair in PlayerHandler.Instance.m_playerCharacterLookup)
		{
			Photon.Realtime.Player player;
			if (!PhotonNetwork.TryGetPlayer(keyValuePair.Key, out player))
			{
				list.Add(keyValuePair.Key);
			}
			else if (player.IsInactive)
			{
				list.Add(keyValuePair.Key);
			}
			else if (keyValuePair.Value == null)
			{
				Debug.LogError(string.Format("{0}({1}) doesn't have a Character any more! ", player.NickName, player.ActorNumber) + "How'd they do that???");
				list.Add(keyValuePair.Key);
			}
		}
		foreach (int num in list)
		{
			PlayerHandler.Instance.m_playerCharacterLookup.Remove(num);
			Debug.Log(string.Format("Removing character for id {0} from list..", num));
		}
		PlayerHandler.s_CharacterCache = PlayerHandler.Instance.m_playerCharacterLookup.Values.ToList<Character>();
		PlayerHandler.s_FrameCharactersLastCached = Time.frameCount;
		return PlayerHandler.s_CharacterCache;
	}

	// Token: 0x06000B92 RID: 2962 RVA: 0x0003E3E0 File Offset: 0x0003C5E0
	public override void ClearSessionData()
	{
		this.ClearSceneData();
		this.m_playerSessionData.Clear();
	}

	// Token: 0x06000B93 RID: 2963 RVA: 0x0003E3F3 File Offset: 0x0003C5F3
	public override void ClearSceneData()
	{
		Debug.Log("Clearing player lookups.");
		this.m_playerLookup.Clear();
		this.m_playerCharacterLookup.Clear();
		this.m_assignedVoiceGroups.Clear();
	}

	// Token: 0x06000B94 RID: 2964 RVA: 0x0003E420 File Offset: 0x0003C620
	public static void RegisterPlayer(global::Player player)
	{
		PlayerHandler.Instance.RegisterPlayerImpl(player);
	}

	// Token: 0x06000B95 RID: 2965 RVA: 0x0003E430 File Offset: 0x0003C630
	private void RegisterPlayerImpl(global::Player player)
	{
		PhotonView component = player.GetComponent<PhotonView>();
		int actorNumber = component.Owner.ActorNumber;
		Debug.Log(string.Format("Registering Player object for {0} : {1}", component.Owner.NickName, actorNumber));
		if (this.m_playerLookup.ContainsKey(actorNumber))
		{
			global::Player player2 = this.m_playerLookup[actorNumber];
			if (player2 == null)
			{
				Debug.LogWarning(string.Format("Found a null entry already in player lookup for actor #{0}. ", actorNumber) + "Cleaning it up.");
			}
			else
			{
				Debug.LogError("Already found player " + player2.name + " already in the lookup table. This shouldn't be possible! Replacing with " + player.name);
			}
			this.m_playerLookup.Remove(actorNumber);
		}
		this.m_playerLookup.Add(actorNumber, player);
		string userId = component.Owner.UserId;
		if (this.m_playerSessionData.ContainsKey(userId))
		{
			int actorNumber2 = this.m_playerSessionData[userId].ActorNumber;
			Debug.LogWarning(string.Format("Stale entry detected. Updating actor {0} to {1}", actorNumber2, actorNumber) + "\n (key: " + component.Owner.UserId + ")");
			this.m_playerSessionData[component.Owner.UserId].ActorNumber = actorNumber;
			return;
		}
		this.m_playerSessionData.Add(userId, new PlayerSessionData(userId, actorNumber));
	}

	// Token: 0x06000B96 RID: 2966 RVA: 0x0003E582 File Offset: 0x0003C782
	private void OnNetworkEvent(INetworkEventData eventData)
	{
		if (eventData.EventCode == 18)
		{
			Debug.LogWarning("Kick event received!");
		}
	}

	// Token: 0x06000B97 RID: 2967 RVA: 0x0003E598 File Offset: 0x0003C798
	public static void RegisterCharacter(Character character)
	{
		PhotonView component = character.GetComponent<PhotonView>();
		if (PlayerHandler.Instance.m_playerCharacterLookup.ContainsKey(component.Owner.ActorNumber))
		{
			Debug.Log(string.Format("Overwriting character for {0}", component.Owner.ActorNumber));
			Character character2 = PlayerHandler.Instance.m_playerCharacterLookup[component.Owner.ActorNumber];
			if (character2 != null)
			{
				character2.gameObject.SetActive(false);
				Debug.LogError("Disabled Old Player....");
			}
			PlayerHandler.Instance.m_playerCharacterLookup.Remove(component.Owner.ActorNumber);
		}
		PlayerHandler.Instance.m_playerCharacterLookup.Add(component.Owner.ActorNumber, character);
		Debug.Log(string.Format("Registering Character object for {0} : {1}", component.Owner.NickName, component.Owner.ActorNumber));
		Action<Character> characterRegistered = PlayerHandler.CharacterRegistered;
		if (characterRegistered == null)
		{
			return;
		}
		characterRegistered(character);
	}

	// Token: 0x06000B98 RID: 2968 RVA: 0x0003E691 File Offset: 0x0003C891
	public static void Kick(int actorNumber)
	{
		if (!NetCode.Session.IsHost)
		{
			PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
			Debug.LogError("Kick button should not show up for a non-host!");
			return;
		}
		global::Player.localPlayer.StartCoroutine(PlayerHandler.KickRoutine(actorNumber));
	}

	// Token: 0x06000B99 RID: 2969 RVA: 0x0003E6C6 File Offset: 0x0003C8C6
	private static IEnumerator KickRoutine(int actorNumber)
	{
		if (!NetCode.Session.IsHost)
		{
			yield break;
		}
		global::Player player = PlayerHandler.GetPlayer(actorNumber);
		string playerId = player.GetUserId();
		PlayerHandler.Instance.m_playerSessionData[playerId].IsBanned = true;
		player.photonView.RPC("RPC_GetKicked", player.photonView.Owner, Array.Empty<object>());
		yield return new WaitForSeconds(0.25f);
		NetCode.Session.Kick(playerId);
		yield break;
	}

	// Token: 0x06000B9A RID: 2970 RVA: 0x0003E6D5 File Offset: 0x0003C8D5
	public static global::Player GetPlayer(Photon.Realtime.Player photonPlayer)
	{
		return PlayerHandler.Instance.m_playerLookup.GetValueOrDefault(photonPlayer.ActorNumber);
	}

	// Token: 0x06000B9B RID: 2971 RVA: 0x0003E6EC File Offset: 0x0003C8EC
	public static global::Player GetPlayer(int actorNumber)
	{
		PlayerHandler instance = PlayerHandler.Instance;
		if (instance == null)
		{
			return null;
		}
		return instance.m_playerLookup.GetValueOrDefault(actorNumber);
	}

	// Token: 0x06000B9C RID: 2972 RVA: 0x0003E704 File Offset: 0x0003C904
	public static string GetUserId(int actorNumber)
	{
		Photon.Realtime.Player player = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber, false);
		if (player == null)
		{
			return null;
		}
		return player.UserId;
	}

	// Token: 0x06000B9D RID: 2973 RVA: 0x0003E720 File Offset: 0x0003C920
	public static global::Player GetPlayer(string userID)
	{
		PlayerSessionData playerSessionData = null;
		PlayerHandler instance = PlayerHandler.Instance;
		if (instance == null || !instance.m_playerSessionData.TryGetValue(userID, out playerSessionData))
		{
			return null;
		}
		return PlayerHandler.Instance.m_playerLookup.GetValueOrDefault(playerSessionData.ActorNumber);
	}

	// Token: 0x06000B9E RID: 2974 RVA: 0x0003E764 File Offset: 0x0003C964
	public static bool IsBanned(string id)
	{
		PlayerSessionData playerSessionData;
		return PlayerHandler.Instance.m_playerSessionData.TryGetValue(id, out playerSessionData) && playerSessionData.IsBanned;
	}

	// Token: 0x06000B9F RID: 2975 RVA: 0x0003E78D File Offset: 0x0003C98D
	public static bool TryGetPlayer(int actorNumber, out global::Player player)
	{
		player = PlayerHandler.GetPlayer(actorNumber);
		return player != null;
	}

	// Token: 0x06000BA0 RID: 2976 RVA: 0x0003E79F File Offset: 0x0003C99F
	public static Character GetPlayerCharacter(Photon.Realtime.Player photonPlayer)
	{
		return PlayerHandler.Instance.m_playerCharacterLookup.GetValueOrDefault(photonPlayer.ActorNumber);
	}

	// Token: 0x06000BA1 RID: 2977 RVA: 0x0003E7B6 File Offset: 0x0003C9B6
	public static bool HasHadPlayerCharacter(Photon.Realtime.Player photonPlayer)
	{
		return PlayerHandler.Instance.m_playerCharacterLookup.ContainsKey(photonPlayer.ActorNumber);
	}

	// Token: 0x06000BA2 RID: 2978 RVA: 0x0003E7D0 File Offset: 0x0003C9D0
	public static byte AssignMixerGroup(Character character)
	{
		for (byte b = 0; b < 4; b += 1)
		{
			if (!PlayerHandler.Instance.m_assignedVoiceGroups.ContainsKey(b) || !PlayerHandler.Instance.m_assignedVoiceGroups[b].UnityObjectExists<Character>())
			{
				PlayerHandler.Instance.m_assignedVoiceGroups[b] = character;
				return b;
			}
		}
		return byte.MaxValue;
	}

	// Token: 0x06000BA3 RID: 2979 RVA: 0x0003E82B File Offset: 0x0003CA2B
	public void Dispose()
	{
		Debug.Log("Disposing PlayerHandler. WARNING: This force-unsubscribes everything listening to CharacterRegistered");
		this._playerList.Dispose();
		PlayerHandler.CharacterRegistered = null;
		NetCode.Session.NetworkEventReceived -= this.OnNetworkEvent;
	}

	// Token: 0x06000BA4 RID: 2980 RVA: 0x0003E85E File Offset: 0x0003CA5E
	public static IEnumerable<global::Player> GetAllPlayers()
	{
		if (!PhotonNetwork.InRoom)
		{
			yield break;
		}
		Photon.Realtime.Player[] array = (PlayerHandler.Instance == null) ? PhotonNetwork.PlayerList : PlayerHandler.Instance._playerList.Get();
		foreach (Photon.Realtime.Player player in array)
		{
			global::Player player2;
			if (!player.IsInactive && PlayerHandler.TryGetPlayer(player.ActorNumber, out player2))
			{
				yield return player2;
			}
		}
		Photon.Realtime.Player[] array2 = null;
		yield break;
	}

	// Token: 0x06000BA5 RID: 2981 RVA: 0x0003E868 File Offset: 0x0003CA68
	public static bool TryGetCharacter(int actorID, out Character character)
	{
		global::Player player;
		if (!PlayerHandler.TryGetPlayer(actorID, out player))
		{
			character = null;
			return false;
		}
		character = player.character;
		if (character == null)
		{
			Debug.LogWarning(string.Format("Null character entry found for {0} ({1})! ", player.name, actorID) + "Won't be cleaned up until the player registry is updated.");
			return false;
		}
		return true;
	}

	// Token: 0x04000AAD RID: 2733
	[Obsolete("This data is stored in m_playerSessionData now")]
	private Dictionary<string, int> m_userIdToActorNumber;

	// Token: 0x04000AAE RID: 2734
	private CachedPlayerList _playerList;

	// Token: 0x04000AAF RID: 2735
	private Dictionary<int, global::Player> m_playerLookup = new Dictionary<int, global::Player>();

	// Token: 0x04000AB0 RID: 2736
	private Dictionary<string, PlayerSessionData> m_playerSessionData = new Dictionary<string, PlayerSessionData>();

	// Token: 0x04000AB1 RID: 2737
	private Dictionary<int, Character> m_playerCharacterLookup = new Dictionary<int, Character>();

	// Token: 0x04000AB2 RID: 2738
	private Dictionary<byte, Character> m_assignedVoiceGroups = new Dictionary<byte, Character>();

	// Token: 0x04000AB3 RID: 2739
	public static Action<Character> CharacterRegistered;

	// Token: 0x04000AB4 RID: 2740
	private static List<Character> s_CharacterCache;

	// Token: 0x04000AB5 RID: 2741
	private static int s_FrameCharactersLastCached;
}

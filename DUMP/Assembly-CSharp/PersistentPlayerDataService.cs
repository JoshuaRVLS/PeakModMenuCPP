using System;
using System.Collections.Generic;
using Peak.Network;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Zorro.PhotonUtility;

// Token: 0x0200008C RID: 140
public class PersistentPlayerDataService : GameService, IDisposable
{
	// Token: 0x060005BF RID: 1471 RVA: 0x00020DE8 File Offset: 0x0001EFE8
	public PersistentPlayerDataService()
	{
		this.syncPersistentPlayerDataHandle = CustomCommands<CustomCommandType>.RegisterListener<SyncPersistentPlayerDataPackage>(new Action<SyncPersistentPlayerDataPackage>(this.OnSyncReceived));
		PlayerHandler.CharacterRegistered = (Action<Character>)Delegate.Combine(PlayerHandler.CharacterRegistered, new Action<Character>(this.OnCharacterRegistered));
	}

	// Token: 0x060005C0 RID: 1472 RVA: 0x00020E48 File Offset: 0x0001F048
	public void Dispose()
	{
		CustomCommands<CustomCommandType>.UnregisterListener(this.syncPersistentPlayerDataHandle);
		PlayerHandler.CharacterRegistered = (Action<Character>)Delegate.Remove(PlayerHandler.CharacterRegistered, new Action<Character>(this.OnCharacterRegistered));
	}

	// Token: 0x060005C1 RID: 1473 RVA: 0x00020E75 File Offset: 0x0001F075
	public override void ClearSessionData()
	{
		if (this.PersistentPlayerDatas.Count > 0)
		{
			Debug.Log("Clearing persistent player data from previous session and OnChangeActions");
		}
		this.PersistentPlayerDatas.Clear();
		this.OnChangeActions.Clear();
	}

	// Token: 0x060005C2 RID: 1474 RVA: 0x00020EA8 File Offset: 0x0001F0A8
	private void OnSyncReceived(SyncPersistentPlayerDataPackage package)
	{
		global::Player player;
		if (PlayerHandler.TryGetPlayer(package.ActorNumber, out player))
		{
			Debug.Log(string.Format("Received persistent player data for player #{0} ", package.ActorNumber) + string.Format("({0}|{1}): {2}", player.name, player.photonView.Owner.UserId, package.Data.customizationData));
		}
		else
		{
			Debug.LogWarning(string.Format("Received customization data ({0}) for player #{1} ", package.Data.customizationData, package.ActorNumber) + "but the PlayerHandler doesn't have them registered yet! That seems out of order, and something might be broken.");
		}
		this.PersistentPlayerDatas[package.ActorNumber] = package.Data;
		if (this.OnChangeActions.ContainsKey(package.ActorNumber))
		{
			Action<PersistentPlayerData> action = this.OnChangeActions[package.ActorNumber];
			if (action == null)
			{
				return;
			}
			action(package.Data);
		}
	}

	// Token: 0x060005C3 RID: 1475 RVA: 0x00020F8A File Offset: 0x0001F18A
	public PersistentPlayerData GetPlayerData(Photon.Realtime.Player player)
	{
		return this.GetPlayerData(player.ActorNumber);
	}

	// Token: 0x060005C4 RID: 1476 RVA: 0x00020F98 File Offset: 0x0001F198
	public PersistentPlayerData GetPlayerData(int actorNumber)
	{
		if (!this.PersistentPlayerDatas.ContainsKey(actorNumber))
		{
			this.PersistentPlayerDatas[actorNumber] = new PersistentPlayerData();
			Debug.Log(string.Format("Initializing player data for player: {0}", actorNumber));
		}
		return this.PersistentPlayerDatas[actorNumber];
	}

	// Token: 0x060005C5 RID: 1477 RVA: 0x00020FE8 File Offset: 0x0001F1E8
	public void SetPlayerData(Photon.Realtime.Player player, PersistentPlayerData playerData)
	{
		this.PersistentPlayerDatas[player.ActorNumber] = playerData;
		Debug.Log("Setting Player Data for: " + player.NickName);
		if (this.OnChangeActions.ContainsKey(player.ActorNumber))
		{
			Action<PersistentPlayerData> action = this.OnChangeActions[player.ActorNumber];
			if (action != null)
			{
				action(playerData);
			}
		}
		CustomCommands<CustomCommandType>.SendPackage(new SyncPersistentPlayerDataPackage
		{
			Data = playerData,
			ActorNumber = player.ActorNumber
		}, ReceiverGroup.Others);
	}

	// Token: 0x060005C6 RID: 1478 RVA: 0x0002106B File Offset: 0x0001F26B
	private void OnCharacterRegistered(Character newCharacter)
	{
		if (!NetCode.Session.IsHost)
		{
			return;
		}
		this.SyncToPlayer(newCharacter.photonView.Owner);
	}

	// Token: 0x060005C7 RID: 1479 RVA: 0x0002108C File Offset: 0x0001F28C
	public void SubscribeToPlayerDataChange(Photon.Realtime.Player player, Action<PersistentPlayerData> onChange)
	{
		if (!this.OnChangeActions.ContainsKey(player.ActorNumber))
		{
			this.OnChangeActions[player.ActorNumber] = onChange;
			return;
		}
		Dictionary<int, Action<PersistentPlayerData>> onChangeActions = this.OnChangeActions;
		int actorNumber = player.ActorNumber;
		onChangeActions[actorNumber] = (Action<PersistentPlayerData>)Delegate.Combine(onChangeActions[actorNumber], onChange);
	}

	// Token: 0x060005C8 RID: 1480 RVA: 0x000210E8 File Offset: 0x0001F2E8
	public void UnsubscribeToPlayerDataChange(Photon.Realtime.Player player, Action<PersistentPlayerData> onChange)
	{
		if (this.OnChangeActions.ContainsKey(player.ActorNumber))
		{
			Dictionary<int, Action<PersistentPlayerData>> onChangeActions = this.OnChangeActions;
			int actorNumber = player.ActorNumber;
			onChangeActions[actorNumber] = (Action<PersistentPlayerData>)Delegate.Remove(onChangeActions[actorNumber], onChange);
		}
	}

	// Token: 0x060005C9 RID: 1481 RVA: 0x00021130 File Offset: 0x0001F330
	public void SyncToPlayer(Photon.Realtime.Player newPlayer)
	{
		foreach (KeyValuePair<int, PersistentPlayerData> keyValuePair in this.PersistentPlayerDatas)
		{
			int num;
			PersistentPlayerData persistentPlayerData;
			keyValuePair.Deconstruct(out num, out persistentPlayerData);
			int actorNumber = num;
			PersistentPlayerData data = persistentPlayerData;
			Photon.Realtime.Player player;
			if (PhotonNetwork.TryGetPlayer(actorNumber, out player) && !player.IsInactive)
			{
				RaiseEventOptions eventOptions = new RaiseEventOptions
				{
					TargetActors = new int[]
					{
						newPlayer.ActorNumber
					}
				};
				CustomCommands<CustomCommandType>.SendPackage(new SyncPersistentPlayerDataPackage
				{
					Data = data,
					ActorNumber = actorNumber
				}, eventOptions);
			}
		}
	}

	// Token: 0x040005C9 RID: 1481
	private Dictionary<int, PersistentPlayerData> PersistentPlayerDatas = new Dictionary<int, PersistentPlayerData>();

	// Token: 0x040005CA RID: 1482
	private Dictionary<int, Action<PersistentPlayerData>> OnChangeActions = new Dictionary<int, Action<PersistentPlayerData>>();

	// Token: 0x040005CB RID: 1483
	private ListenerHandle syncPersistentPlayerDataHandle;
}

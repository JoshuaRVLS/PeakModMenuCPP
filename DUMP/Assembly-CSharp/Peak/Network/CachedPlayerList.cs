using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Peak.Network
{
	// Token: 0x020003D3 RID: 979
	public struct CachedPlayerList : IInRoomCallbacks, IDisposable
	{
		// Token: 0x17000192 RID: 402
		// (get) Token: 0x060019D9 RID: 6617 RVA: 0x00082201 File Offset: 0x00080401
		private bool ShouldRefreshCache
		{
			get
			{
				return this._frameCacheLastDirtied - this._frameCacheLastUpdated > 1 || this._playerList.Length != PhotonNetwork.CurrentRoom.PlayerCount;
			}
		}

		// Token: 0x060019DA RID: 6618 RVA: 0x0008222C File Offset: 0x0008042C
		public Photon.Realtime.Player[] Get()
		{
			if (!this._isInitialized)
			{
				PhotonNetwork.AddCallbackTarget(this);
				this._isInitialized = true;
				this.RefreshCache();
			}
			if (!PhotonNetwork.InRoom)
			{
				Debug.LogWarning("Attempting to fetch player list when not in a room...");
				return CachedPlayerList.s_EmptyList;
			}
			if (this.ShouldRefreshCache || this._playerList.Length != PhotonNetwork.CurrentRoom.PlayerCount)
			{
				this.RefreshCache();
			}
			return this._playerList;
		}

		// Token: 0x060019DB RID: 6619 RVA: 0x000822A0 File Offset: 0x000804A0
		public Photon.Realtime.Player GetNetworkPlayerByActorId(int actorId)
		{
			foreach (Photon.Realtime.Player player in this.Get())
			{
				if (player.ActorNumber == actorId)
				{
					return player;
				}
			}
			return null;
		}

		// Token: 0x060019DC RID: 6620 RVA: 0x000822D2 File Offset: 0x000804D2
		public string GetUserID(int actorId)
		{
			Photon.Realtime.Player networkPlayerByActorId = this.GetNetworkPlayerByActorId(actorId);
			return ((networkPlayerByActorId != null) ? networkPlayerByActorId.UserId : null) ?? string.Empty;
		}

		// Token: 0x060019DD RID: 6621 RVA: 0x000822F0 File Offset: 0x000804F0
		private void RefreshCache()
		{
			this._playerList = PhotonNetwork.PlayerList;
			this._frameCacheLastUpdated = Time.frameCount;
		}

		// Token: 0x060019DE RID: 6622 RVA: 0x00082308 File Offset: 0x00080508
		private void MarkDirty()
		{
			this._frameCacheLastDirtied = Time.frameCount;
		}

		// Token: 0x060019DF RID: 6623 RVA: 0x00082315 File Offset: 0x00080515
		public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
		{
			this.MarkDirty();
		}

		// Token: 0x060019E0 RID: 6624 RVA: 0x0008231D File Offset: 0x0008051D
		public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
		{
			this.MarkDirty();
		}

		// Token: 0x060019E1 RID: 6625 RVA: 0x00082325 File Offset: 0x00080525
		public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
			this.MarkDirty();
		}

		// Token: 0x060019E2 RID: 6626 RVA: 0x0008232D File Offset: 0x0008052D
		public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
		{
			this.MarkDirty();
		}

		// Token: 0x060019E3 RID: 6627 RVA: 0x00082335 File Offset: 0x00080535
		public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
		{
			this.MarkDirty();
		}

		// Token: 0x060019E4 RID: 6628 RVA: 0x0008233D File Offset: 0x0008053D
		public void Dispose()
		{
			if (this._isInitialized)
			{
				PhotonNetwork.RemoveCallbackTarget(this);
			}
		}

		// Token: 0x0400175B RID: 5979
		private bool _isInitialized;

		// Token: 0x0400175C RID: 5980
		private int _frameCacheLastDirtied;

		// Token: 0x0400175D RID: 5981
		private int _frameCacheLastUpdated;

		// Token: 0x0400175E RID: 5982
		private static Photon.Realtime.Player[] s_EmptyList = new Photon.Realtime.Player[0];

		// Token: 0x0400175F RID: 5983
		private Photon.Realtime.Player[] _playerList;
	}
}

using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Realtime.Demo
{
	// Token: 0x020003A5 RID: 933
	public class ConnectAndJoinRandomLb : MonoBehaviour, IConnectionCallbacks, IMatchmakingCallbacks, ILobbyCallbacks
	{
		// Token: 0x060018E9 RID: 6377 RVA: 0x0007E110 File Offset: 0x0007C310
		public void Start()
		{
			this.lbc = new LoadBalancingClient(ConnectionProtocol.Udp);
			this.lbc.AddCallbackTarget(this);
			if (!this.lbc.ConnectUsingSettings(this.appSettings))
			{
				Debug.LogError("Error while connecting");
			}
			this.ch = base.gameObject.GetComponent<ConnectionHandler>();
			if (this.ch != null)
			{
				this.ch.Client = this.lbc;
				this.ch.StartFallbackSendAckThread();
			}
		}

		// Token: 0x060018EA RID: 6378 RVA: 0x0007E190 File Offset: 0x0007C390
		public void Update()
		{
			LoadBalancingClient loadBalancingClient = this.lbc;
			if (loadBalancingClient != null)
			{
				loadBalancingClient.Service();
				Text stateUiText = this.StateUiText;
				string text = loadBalancingClient.State.ToString();
				if (stateUiText != null && !stateUiText.text.Equals(text))
				{
					stateUiText.text = "State: " + text;
				}
			}
		}

		// Token: 0x060018EB RID: 6379 RVA: 0x0007E1F1 File Offset: 0x0007C3F1
		public void OnConnected()
		{
		}

		// Token: 0x060018EC RID: 6380 RVA: 0x0007E1F3 File Offset: 0x0007C3F3
		public void OnConnectedToMaster()
		{
			Debug.Log("OnConnectedToMaster");
			this.lbc.OpJoinRandomRoom(null);
		}

		// Token: 0x060018ED RID: 6381 RVA: 0x0007E20C File Offset: 0x0007C40C
		public void OnDisconnected(DisconnectCause cause)
		{
			Debug.Log("OnDisconnected(" + cause.ToString() + ")");
		}

		// Token: 0x060018EE RID: 6382 RVA: 0x0007E22F File Offset: 0x0007C42F
		public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
		{
		}

		// Token: 0x060018EF RID: 6383 RVA: 0x0007E231 File Offset: 0x0007C431
		public void OnCustomAuthenticationFailed(string debugMessage)
		{
		}

		// Token: 0x060018F0 RID: 6384 RVA: 0x0007E233 File Offset: 0x0007C433
		public void OnRegionListReceived(RegionHandler regionHandler)
		{
			Debug.Log("OnRegionListReceived");
			regionHandler.PingMinimumOfRegions(new Action<RegionHandler>(this.OnRegionPingCompleted), null);
		}

		// Token: 0x060018F1 RID: 6385 RVA: 0x0007E253 File Offset: 0x0007C453
		public void OnRoomListUpdate(List<RoomInfo> roomList)
		{
		}

		// Token: 0x060018F2 RID: 6386 RVA: 0x0007E255 File Offset: 0x0007C455
		public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
		{
		}

		// Token: 0x060018F3 RID: 6387 RVA: 0x0007E257 File Offset: 0x0007C457
		public void OnJoinedLobby()
		{
		}

		// Token: 0x060018F4 RID: 6388 RVA: 0x0007E259 File Offset: 0x0007C459
		public void OnLeftLobby()
		{
		}

		// Token: 0x060018F5 RID: 6389 RVA: 0x0007E25B File Offset: 0x0007C45B
		public void OnFriendListUpdate(List<FriendInfo> friendList)
		{
		}

		// Token: 0x060018F6 RID: 6390 RVA: 0x0007E25D File Offset: 0x0007C45D
		public void OnCreatedRoom()
		{
		}

		// Token: 0x060018F7 RID: 6391 RVA: 0x0007E25F File Offset: 0x0007C45F
		public void OnCreateRoomFailed(short returnCode, string message)
		{
		}

		// Token: 0x060018F8 RID: 6392 RVA: 0x0007E261 File Offset: 0x0007C461
		public void OnJoinedRoom()
		{
			Debug.Log("OnJoinedRoom");
		}

		// Token: 0x060018F9 RID: 6393 RVA: 0x0007E26D File Offset: 0x0007C46D
		public void OnJoinRoomFailed(short returnCode, string message)
		{
		}

		// Token: 0x060018FA RID: 6394 RVA: 0x0007E26F File Offset: 0x0007C46F
		public void OnJoinRandomFailed(short returnCode, string message)
		{
			Debug.Log("OnJoinRandomFailed");
			this.lbc.OpCreateRoom(new EnterRoomParams());
		}

		// Token: 0x060018FB RID: 6395 RVA: 0x0007E28C File Offset: 0x0007C48C
		public void OnLeftRoom()
		{
		}

		// Token: 0x060018FC RID: 6396 RVA: 0x0007E290 File Offset: 0x0007C490
		private void OnRegionPingCompleted(RegionHandler regionHandler)
		{
			string str = "OnRegionPingCompleted ";
			Region bestRegion = regionHandler.BestRegion;
			Debug.Log(str + ((bestRegion != null) ? bestRegion.ToString() : null));
			Debug.Log("RegionPingSummary: " + regionHandler.SummaryToCache);
			this.lbc.ConnectToRegionMaster(regionHandler.BestRegion.Code);
		}

		// Token: 0x040016D6 RID: 5846
		[SerializeField]
		private AppSettings appSettings = new AppSettings();

		// Token: 0x040016D7 RID: 5847
		private LoadBalancingClient lbc;

		// Token: 0x040016D8 RID: 5848
		private ConnectionHandler ch;

		// Token: 0x040016D9 RID: 5849
		public Text StateUiText;
	}
}

using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Zorro.Core;

namespace Peak.Network
{
	// Token: 0x020003DD RID: 989
	public class PhotonShim : PhotonCallbackTarget, ISessionAPI, ISessionEvents, IConnectionCallbacks
	{
		// Token: 0x170001AC RID: 428
		// (get) Token: 0x06001A36 RID: 6710 RVA: 0x0008282D File Offset: 0x00080A2D
		public bool IsHost
		{
			get
			{
				return PhotonNetwork.IsMasterClient;
			}
		}

		// Token: 0x170001AD RID: 429
		// (get) Token: 0x06001A37 RID: 6711 RVA: 0x00082834 File Offset: 0x00080A34
		public bool IsConnectedAndReady
		{
			get
			{
				return PhotonNetwork.IsConnectedAndReady;
			}
		}

		// Token: 0x170001AE RID: 430
		// (get) Token: 0x06001A38 RID: 6712 RVA: 0x0008283B File Offset: 0x00080A3B
		public int HostId
		{
			get
			{
				return PhotonNetwork.MasterClient.ActorNumber;
			}
		}

		// Token: 0x170001AF RID: 431
		// (get) Token: 0x06001A39 RID: 6713 RVA: 0x00082847 File Offset: 0x00080A47
		public bool IsOffline
		{
			get
			{
				return PhotonNetwork.OfflineMode;
			}
		}

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x06001A3A RID: 6714 RVA: 0x0008284E File Offset: 0x00080A4E
		public int SeatNumber
		{
			get
			{
				return PhotonNetwork.LocalPlayer.ActorNumber;
			}
		}

		// Token: 0x14000018 RID: 24
		// (add) Token: 0x06001A3B RID: 6715 RVA: 0x0008285C File Offset: 0x00080A5C
		// (remove) Token: 0x06001A3C RID: 6716 RVA: 0x00082894 File Offset: 0x00080A94
		public event Action ConnectedAndReady;

		// Token: 0x14000019 RID: 25
		// (add) Token: 0x06001A3D RID: 6717 RVA: 0x000828CC File Offset: 0x00080ACC
		// (remove) Token: 0x06001A3E RID: 6718 RVA: 0x00082904 File Offset: 0x00080B04
		public event Action<INetworkEventData> NetworkEventReceived;

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x06001A3F RID: 6719 RVA: 0x00082939 File Offset: 0x00080B39
		// (set) Token: 0x06001A40 RID: 6720 RVA: 0x00082945 File Offset: 0x00080B45
		public string NickName
		{
			get
			{
				return PhotonNetwork.NetworkingClient.NickName;
			}
			set
			{
				PhotonNetwork.NetworkingClient.NickName = value;
			}
		}

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x06001A41 RID: 6721 RVA: 0x00082952 File Offset: 0x00080B52
		// (set) Token: 0x06001A42 RID: 6722 RVA: 0x00082967 File Offset: 0x00080B67
		public AuthenticationValues AuthValues
		{
			get
			{
				if (PhotonNetwork.NetworkingClient == null)
				{
					return null;
				}
				return PhotonNetwork.NetworkingClient.AuthValues;
			}
			set
			{
				if (PhotonNetwork.NetworkingClient != null)
				{
					PhotonNetwork.NetworkingClient.AuthValues = value;
				}
			}
		}

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x06001A43 RID: 6723 RVA: 0x0008297B File Offset: 0x00080B7B
		public bool InRoom
		{
			get
			{
				return PhotonNetwork.NetworkClientState == ClientState.Joined;
			}
		}

		// Token: 0x06001A44 RID: 6724 RVA: 0x00082986 File Offset: 0x00080B86
		public PhotonShim()
		{
			PhotonNetwork.NetworkingClient.EventReceived += this.RaiseGenericEvent;
		}

		// Token: 0x06001A45 RID: 6725 RVA: 0x000829A4 File Offset: 0x00080BA4
		public void InitializeNetcodeBackend()
		{
			BuildVersion buildVersion = new BuildVersion(Application.version, "???");
			PhotonNetwork.AutomaticallySyncScene = true;
			PhotonNetwork.GameVersion = buildVersion.ToString();
			PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = buildVersion.ToMatchmaking();
			PhotonNetwork.ConnectUsingSettings();
			Debug.Log("Photon Start" + PhotonNetwork.NetworkClientState.ToString() + " using app version: " + buildVersion.ToMatchmaking());
		}

		// Token: 0x06001A46 RID: 6726 RVA: 0x00082A24 File Offset: 0x00080C24
		public override void Dispose()
		{
			base.Dispose();
			this.ConnectedAndReady = null;
			PhotonNetwork.NetworkingClient.EventReceived -= this.RaiseGenericEvent;
		}

		// Token: 0x06001A47 RID: 6727 RVA: 0x00082A49 File Offset: 0x00080C49
		public void OnConnectedToMaster()
		{
			Action connectedAndReady = this.ConnectedAndReady;
			if (connectedAndReady == null)
			{
				return;
			}
			connectedAndReady();
		}

		// Token: 0x06001A48 RID: 6728 RVA: 0x00082A5B File Offset: 0x00080C5B
		public void OnConnected()
		{
		}

		// Token: 0x06001A49 RID: 6729 RVA: 0x00082A5D File Offset: 0x00080C5D
		public void OnDisconnected(DisconnectCause cause)
		{
		}

		// Token: 0x06001A4A RID: 6730 RVA: 0x00082A5F File Offset: 0x00080C5F
		public void OnRegionListReceived(RegionHandler regionHandler)
		{
		}

		// Token: 0x06001A4B RID: 6731 RVA: 0x00082A61 File Offset: 0x00080C61
		public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
		{
		}

		// Token: 0x06001A4C RID: 6732 RVA: 0x00082A63 File Offset: 0x00080C63
		public void OnCustomAuthenticationFailed(string debugMessage)
		{
		}

		// Token: 0x06001A4D RID: 6733 RVA: 0x00082A65 File Offset: 0x00080C65
		public void Kick(string userId)
		{
			if (!NetCode.Session.IsHost)
			{
				Debug.LogWarning("Someone who is not MasterClient is clicking the Kick button (why do they have a kick button?)");
				return;
			}
			PhotonNetwork.RaiseEvent(18, userId, RaiseEventOptions.Default, SendOptions.SendReliable);
		}

		// Token: 0x06001A4E RID: 6734 RVA: 0x00082A91 File Offset: 0x00080C91
		private void RaiseGenericEvent(EventData eventData)
		{
			Action<INetworkEventData> networkEventReceived = this.NetworkEventReceived;
			if (networkEventReceived == null)
			{
				return;
			}
			networkEventReceived(new PhotonShim.NetworkEventData(eventData));
		}

		// Token: 0x02000570 RID: 1392
		public class NetworkEventData : INetworkEventData
		{
			// Token: 0x170002D0 RID: 720
			// (get) Token: 0x06001FE9 RID: 8169 RVA: 0x00090689 File Offset: 0x0008E889
			public int EventCode
			{
				get
				{
					return (int)this._eventData.Code;
				}
			}

			// Token: 0x06001FEA RID: 8170 RVA: 0x00090696 File Offset: 0x0008E896
			internal NetworkEventData(EventData eventData)
			{
				this._eventData = eventData;
			}

			// Token: 0x04001D69 RID: 7529
			private EventData _eventData;
		}
	}
}

using System;
using ExitGames.Client.Photon;
using Photon.Realtime;

namespace Peak.Network
{
	// Token: 0x020003DF RID: 991
	public class PhotonRoomEvents : PhotonCallbackTarget, IInRoomCallbacks, INetworkRoomEvents
	{
		// Token: 0x1400001A RID: 26
		// (add) Token: 0x06001A51 RID: 6737 RVA: 0x00082AE4 File Offset: 0x00080CE4
		// (remove) Token: 0x06001A52 RID: 6738 RVA: 0x00082B1C File Offset: 0x00080D1C
		public event Action PlayerEntered;

		// Token: 0x1400001B RID: 27
		// (add) Token: 0x06001A53 RID: 6739 RVA: 0x00082B54 File Offset: 0x00080D54
		// (remove) Token: 0x06001A54 RID: 6740 RVA: 0x00082B8C File Offset: 0x00080D8C
		public event Action PlayerLeft;

		// Token: 0x1400001C RID: 28
		// (add) Token: 0x06001A55 RID: 6741 RVA: 0x00082BC4 File Offset: 0x00080DC4
		// (remove) Token: 0x06001A56 RID: 6742 RVA: 0x00082BFC File Offset: 0x00080DFC
		public event Action RoomOwnerChanged;

		// Token: 0x06001A57 RID: 6743 RVA: 0x00082C31 File Offset: 0x00080E31
		public override void Dispose()
		{
			base.Dispose();
			this.PlayerLeft = null;
			this.RoomOwnerChanged = null;
		}

		// Token: 0x06001A58 RID: 6744 RVA: 0x00082C47 File Offset: 0x00080E47
		public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
		{
			Action playerEntered = this.PlayerEntered;
			if (playerEntered == null)
			{
				return;
			}
			playerEntered();
		}

		// Token: 0x06001A59 RID: 6745 RVA: 0x00082C59 File Offset: 0x00080E59
		public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
		}

		// Token: 0x06001A5A RID: 6746 RVA: 0x00082C5B File Offset: 0x00080E5B
		public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
		{
		}

		// Token: 0x06001A5B RID: 6747 RVA: 0x00082C5D File Offset: 0x00080E5D
		public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
		{
			Action roomOwnerChanged = this.RoomOwnerChanged;
			if (roomOwnerChanged == null)
			{
				return;
			}
			roomOwnerChanged();
		}

		// Token: 0x06001A5C RID: 6748 RVA: 0x00082C6F File Offset: 0x00080E6F
		public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
		{
			Action playerLeft = this.PlayerLeft;
			if (playerLeft == null)
			{
				return;
			}
			playerLeft();
		}
	}
}

using System;
using Photon.Pun;

// Token: 0x02000155 RID: 341
public abstract class OnNetworkStart : MonoBehaviourPunCallbacks
{
	// Token: 0x06000B43 RID: 2883 RVA: 0x0003CA6A File Offset: 0x0003AC6A
	private void Start()
	{
		this.TryRunningNetworkStart();
	}

	// Token: 0x06000B44 RID: 2884 RVA: 0x0003CA72 File Offset: 0x0003AC72
	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		this.TryRunningNetworkStart();
	}

	// Token: 0x06000B45 RID: 2885 RVA: 0x0003CA80 File Offset: 0x0003AC80
	private void TryRunningNetworkStart()
	{
		if (this.hasRunNetworkStart)
		{
			return;
		}
		if (PhotonNetwork.InRoom)
		{
			this.NetworkStart();
			this.hasRunNetworkStart = true;
		}
	}

	// Token: 0x06000B46 RID: 2886
	public abstract void NetworkStart();

	// Token: 0x04000A5B RID: 2651
	private bool hasRunNetworkStart;
}

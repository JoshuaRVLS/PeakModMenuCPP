using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000173 RID: 371
public class RopeAnchorProjectile : MonoBehaviourPunCallbacks
{
	// Token: 0x06000C13 RID: 3091 RVA: 0x00040C48 File Offset: 0x0003EE48
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (PhotonNetwork.IsMasterClient && this.shot)
		{
			this.photonView.RPC("GetShot", newPlayer, new object[]
			{
				this.lastShotTo,
				this.lastShotTravelTime,
				this.lastShotRopeLength,
				this.lastShotFlyingRotation
			});
		}
	}

	// Token: 0x06000C14 RID: 3092 RVA: 0x00040CBA File Offset: 0x0003EEBA
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000C15 RID: 3093 RVA: 0x00040CC8 File Offset: 0x0003EEC8
	[PunRPC]
	public void GetShot(Vector3 to, float travelTime, float ropeLength, Vector3 flyingRotation)
	{
		RopeAnchorProjectile.<>c__DisplayClass10_0 CS$<>8__locals1 = new RopeAnchorProjectile.<>c__DisplayClass10_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.flyingRotation = flyingRotation;
		CS$<>8__locals1.to = to;
		CS$<>8__locals1.travelTime = travelTime;
		CS$<>8__locals1.ropeLength = ropeLength;
		this.lastShotTo = CS$<>8__locals1.to;
		this.lastShotTravelTime = CS$<>8__locals1.travelTime;
		this.lastShotRopeLength = CS$<>8__locals1.ropeLength;
		this.lastShotFlyingRotation = CS$<>8__locals1.flyingRotation;
		this.shot = true;
		this.startRotation = base.transform.rotation;
		this.startPosition = base.transform.position;
		base.StartCoroutine(CS$<>8__locals1.<GetShot>g__SpawnRopeRoutine|0());
	}

	// Token: 0x04000B09 RID: 2825
	public new PhotonView photonView;

	// Token: 0x04000B0A RID: 2826
	public bool shot;

	// Token: 0x04000B0B RID: 2827
	private Vector3 startPosition;

	// Token: 0x04000B0C RID: 2828
	private Quaternion startRotation;

	// Token: 0x04000B0D RID: 2829
	private Vector3 lastShotTo;

	// Token: 0x04000B0E RID: 2830
	private float lastShotTravelTime;

	// Token: 0x04000B0F RID: 2831
	private float lastShotRopeLength;

	// Token: 0x04000B10 RID: 2832
	private Vector3 lastShotFlyingRotation;
}

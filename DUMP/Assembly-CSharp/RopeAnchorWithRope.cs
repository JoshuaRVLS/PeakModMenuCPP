using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x02000174 RID: 372
public class RopeAnchorWithRope : MonoBehaviourPunCallbacks
{
	// Token: 0x06000C17 RID: 3095 RVA: 0x00040D6D File Offset: 0x0003EF6D
	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		this.SpawnRope();
	}

	// Token: 0x06000C18 RID: 3096 RVA: 0x00040D7C File Offset: 0x0003EF7C
	public Rope SpawnRope()
	{
		if (!base.photonView.IsMine)
		{
			return null;
		}
		this.ropeInstance = PhotonNetwork.Instantiate(this.ropePrefab.name, this.anchor.anchorPoint.position, this.anchor.anchorPoint.rotation, 0, null);
		this.rope = this.ropeInstance.GetComponent<Rope>();
		this.rope.Segments = this.ropeSegmentLength;
		this.rope.photonView.RPC("AttachToAnchor_Rpc", RpcTarget.AllBuffered, new object[]
		{
			this.anchor.photonView,
			this.ropeSegmentLength
		});
		base.StartCoroutine(this.<SpawnRope>g__SpoolOut|7_0());
		return this.rope;
	}

	// Token: 0x06000C19 RID: 3097 RVA: 0x00040E3D File Offset: 0x0003F03D
	public virtual void Awake()
	{
		this.anchor = base.GetComponent<RopeAnchor>();
	}

	// Token: 0x06000C1B RID: 3099 RVA: 0x00040E69 File Offset: 0x0003F069
	[CompilerGenerated]
	private IEnumerator <SpawnRope>g__SpoolOut|7_0()
	{
		float elapsed = 0f;
		while (elapsed < this.spoolOutTime)
		{
			elapsed += Time.deltaTime;
			this.rope.Segments = Mathf.Lerp(0f, this.ropeSegmentLength, (elapsed / this.spoolOutTime).Clamp01());
			yield return null;
		}
		yield break;
	}

	// Token: 0x04000B11 RID: 2833
	public float ropeSegmentLength = 20f;

	// Token: 0x04000B12 RID: 2834
	public float spoolOutTime = 5f;

	// Token: 0x04000B13 RID: 2835
	public GameObject ropePrefab;

	// Token: 0x04000B14 RID: 2836
	public GameObject ropeInstance;

	// Token: 0x04000B15 RID: 2837
	public RopeAnchor anchor;

	// Token: 0x04000B16 RID: 2838
	public Rope rope;
}

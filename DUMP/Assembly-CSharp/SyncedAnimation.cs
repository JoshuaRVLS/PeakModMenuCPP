using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000357 RID: 855
public class SyncedAnimation : MonoBehaviour
{
	// Token: 0x060016A1 RID: 5793 RVA: 0x0007424B File Offset: 0x0007244B
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
		this.anim = base.GetComponent<Animator>();
	}

	// Token: 0x060016A2 RID: 5794 RVA: 0x00074268 File Offset: 0x00072468
	private void Update()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.syncCounter += Time.deltaTime;
			if (this.syncCounter > 5f)
			{
				this.view.RPC("RPCA_SyncAnim", RpcTarget.All, new object[]
				{
					this.anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f
				});
				this.syncCounter = 0f;
			}
		}
	}

	// Token: 0x060016A3 RID: 5795 RVA: 0x000742E0 File Offset: 0x000724E0
	[PunRPC]
	public void RPCA_SyncAnim(float syncTime)
	{
		this.anim.Play(this.anim.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, syncTime);
	}

	// Token: 0x04001509 RID: 5385
	private PhotonView view;

	// Token: 0x0400150A RID: 5386
	private Animator anim;

	// Token: 0x0400150B RID: 5387
	private float syncCounter;
}

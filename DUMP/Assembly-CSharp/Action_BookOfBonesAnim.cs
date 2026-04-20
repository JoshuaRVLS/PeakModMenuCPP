using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001FE RID: 510
public class Action_BookOfBonesAnim : MonoBehaviour
{
	// Token: 0x06000FF9 RID: 4089 RVA: 0x0004E354 File Offset: 0x0004C554
	private void Update()
	{
		if (this.item.holderCharacter && !this.item.holderCharacter.IsLocal)
		{
			return;
		}
		if (this.usingOnSkeleton)
		{
			return;
		}
		if ((this.item.isUsingPrimary || this.item.isUsingSecondary) && !this.usingLocal)
		{
			this.Open(false);
			return;
		}
		if (!this.item.isUsingPrimary && !this.item.isUsingSecondary && this.usingLocal)
		{
			this.Close();
		}
	}

	// Token: 0x06000FFA RID: 4090 RVA: 0x0004E3E1 File Offset: 0x0004C5E1
	public void Open(bool usingOnSkeleton)
	{
		this.usingOnSkeleton = usingOnSkeleton;
		this.item.photonView.RPC("BookOfBonesAnimRPC", RpcTarget.All, new object[]
		{
			true
		});
	}

	// Token: 0x06000FFB RID: 4091 RVA: 0x0004E40F File Offset: 0x0004C60F
	public void Close()
	{
		this.usingOnSkeleton = false;
		this.item.photonView.RPC("BookOfBonesAnimRPC", RpcTarget.All, new object[]
		{
			false
		});
	}

	// Token: 0x06000FFC RID: 4092 RVA: 0x0004E43D File Offset: 0x0004C63D
	[PunRPC]
	private void BookOfBonesAnimRPC(bool isUsing)
	{
		this.anim.SetBool("Using", isUsing);
		this.usingLocal = isUsing;
	}

	// Token: 0x04000D96 RID: 3478
	public Animator anim;

	// Token: 0x04000D97 RID: 3479
	public Item item;

	// Token: 0x04000D98 RID: 3480
	private bool usingLocal;

	// Token: 0x04000D99 RID: 3481
	private bool usingOnSkeleton;
}

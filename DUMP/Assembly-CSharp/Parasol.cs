using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002C1 RID: 705
[RequireComponent(typeof(PhotonView))]
public class Parasol : MonoBehaviour
{
	// Token: 0x060013EF RID: 5103 RVA: 0x00064F78 File Offset: 0x00063178
	internal void ToggleOpen()
	{
		if (this.item.photonView.IsMine)
		{
			this.item.photonView.RPC("ToggleOpenRPC", RpcTarget.All, new object[]
			{
				!this.isOpen
			});
		}
	}

	// Token: 0x060013F0 RID: 5104 RVA: 0x00064FC4 File Offset: 0x000631C4
	private void FixedUpdate()
	{
		if (this.item.holderCharacter && !this.item.holderCharacter.data.isGrounded && this.isOpen)
		{
			this.item.holderCharacter.refs.movement.ApplyParasolDrag(this.extraYDrag, this.extraXZDrag, false);
		}
	}

	// Token: 0x060013F1 RID: 5105 RVA: 0x00065029 File Offset: 0x00063229
	private void OnDisable()
	{
		if (this.isOpen)
		{
			this.OnClose();
		}
	}

	// Token: 0x060013F2 RID: 5106 RVA: 0x00065039 File Offset: 0x00063239
	private void OnClose()
	{
		if (this.item.holderCharacter)
		{
			this.item.holderCharacter.data.sinceGrounded = this.sinceGroundedOnClose;
		}
	}

	// Token: 0x060013F3 RID: 5107 RVA: 0x00065068 File Offset: 0x00063268
	[PunRPC]
	private void ToggleOpenRPC(bool open)
	{
		this.isOpen = open;
		this.anim.SetBool("Open", open);
		if (!this.isOpen)
		{
			this.OnClose();
		}
	}

	// Token: 0x04001227 RID: 4647
	public Item item;

	// Token: 0x04001228 RID: 4648
	public float extraYDrag = 0.8f;

	// Token: 0x04001229 RID: 4649
	public float extraXZDrag = 0.8f;

	// Token: 0x0400122A RID: 4650
	public float sinceGroundedOnClose = 2f;

	// Token: 0x0400122B RID: 4651
	public GameObject openParasol;

	// Token: 0x0400122C RID: 4652
	public GameObject closedParasol;

	// Token: 0x0400122D RID: 4653
	public Animator anim;

	// Token: 0x0400122E RID: 4654
	public bool isOpen;
}

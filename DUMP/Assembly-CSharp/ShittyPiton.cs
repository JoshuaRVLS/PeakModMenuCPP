using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200033F RID: 831
public class ShittyPiton : MonoBehaviourPunCallbacks
{
	// Token: 0x0600161E RID: 5662 RVA: 0x000706F4 File Offset: 0x0006E8F4
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
		this.handle = base.GetComponent<ClimbHandle>();
		ClimbHandle climbHandle = this.handle;
		climbHandle.onHangStart = (Action<Character>)Delegate.Combine(climbHandle.onHangStart, new Action<Character>(this.OnHang));
		ClimbHandle climbHandle2 = this.handle;
		climbHandle2.onHangStop = (Action)Delegate.Combine(climbHandle2.onHangStop, new Action(this.OnHangStop));
		this.totalSecondsOfHang = Random.Range(1f, 5f);
	}

	// Token: 0x0600161F RID: 5663 RVA: 0x0007077C File Offset: 0x0006E97C
	private void OnHangStop()
	{
		this.isHung = false;
	}

	// Token: 0x06001620 RID: 5664 RVA: 0x00070785 File Offset: 0x0006E985
	private void OnHang(Character character)
	{
		this.isHung = true;
	}

	// Token: 0x06001621 RID: 5665 RVA: 0x00070790 File Offset: 0x0006E990
	private void Update()
	{
		if (this.isBreaking)
		{
			if (this.isHung)
			{
				this.sinceCrack += Time.deltaTime;
			}
			if (this.sinceCrack > 1.5f)
			{
				this.Crack();
				this.sinceCrack = 0f;
			}
			this.crack.transform.localScale = Vector3.Lerp(this.crack.transform.localScale, Vector3.one * this.crackScale, Time.deltaTime * 15f);
			return;
		}
		if (!this.view.IsMine)
		{
			return;
		}
		if (this.isHung)
		{
			this.totalSecondsOfHang -= Time.deltaTime;
			if (this.totalSecondsOfHang < 0f)
			{
				this.view.RPC("RPCA_StartBreaking", RpcTarget.All, Array.Empty<object>());
			}
		}
	}

	// Token: 0x06001622 RID: 5666 RVA: 0x0007086C File Offset: 0x0006EA6C
	private void Crack()
	{
		this.crackScale += 0.75f;
		this.cracksToBreak--;
		GamefeelHandler.instance.AddPerlinShakeProximity(base.transform.position, 2f + this.crackScale, 0.2f, 15f, 10f);
		for (int i = 0; i < this.cracKSFX.Length; i++)
		{
			this.cracKSFX[i].Play(base.transform.position);
		}
		if (this.cracksToBreak <= 0 && this.view.IsMine)
		{
			this.view.RPC("RPCA_Break", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06001623 RID: 5667 RVA: 0x00070920 File Offset: 0x0006EB20
	[PunRPC]
	private void RPCA_Break()
	{
		this.vfx.transform.SetParent(null);
		this.vfx.SetActive(true);
		for (int i = 0; i < this.breakSFX.Length; i++)
		{
			this.breakSFX[i].Play(base.transform.position);
		}
		this.disabled = true;
		this.crack.transform.SetParent(null);
		this.handle.Break();
	}

	// Token: 0x06001624 RID: 5668 RVA: 0x00070998 File Offset: 0x0006EB98
	[PunRPC]
	public void RPCA_StartBreaking()
	{
		this.isBreaking = true;
		this.crack.SetActive(true);
		this.crack.transform.localScale *= 0f;
	}

	// Token: 0x06001625 RID: 5669 RVA: 0x000709CD File Offset: 0x0006EBCD
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (this.disabled && !newPlayer.IsLocal && PhotonNetwork.IsMasterClient)
		{
			this.view.RPC("RPCA_Break", newPlayer, Array.Empty<object>());
		}
	}

	// Token: 0x04001436 RID: 5174
	private ClimbHandle handle;

	// Token: 0x04001437 RID: 5175
	private PhotonView view;

	// Token: 0x04001438 RID: 5176
	private float totalSecondsOfHang;

	// Token: 0x04001439 RID: 5177
	public GameObject crack;

	// Token: 0x0400143A RID: 5178
	public GameObject vfx;

	// Token: 0x0400143B RID: 5179
	private float crackScale;

	// Token: 0x0400143C RID: 5180
	private int cracksToBreak = 4;

	// Token: 0x0400143D RID: 5181
	private float sinceCrack = 10f;

	// Token: 0x0400143E RID: 5182
	private bool disabled;

	// Token: 0x0400143F RID: 5183
	public SFX_Instance[] cracKSFX;

	// Token: 0x04001440 RID: 5184
	public SFX_Instance[] breakSFX;

	// Token: 0x04001441 RID: 5185
	private bool isHung;

	// Token: 0x04001442 RID: 5186
	private bool isBreaking;
}

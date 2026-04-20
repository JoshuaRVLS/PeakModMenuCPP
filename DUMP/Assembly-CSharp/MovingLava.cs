using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002B5 RID: 693
public class MovingLava : MonoBehaviour
{
	// Token: 0x060013A1 RID: 5025 RVA: 0x00063AA1 File Offset: 0x00061CA1
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x060013A2 RID: 5026 RVA: 0x00063AB0 File Offset: 0x00061CB0
	private void Update()
	{
		if (base.transform.position.y > 1150f)
		{
			return;
		}
		if (!this.timeToMove)
		{
			if (this.PlayersHaveMovedOn())
			{
				this.view.RPC("RPCA_StartLavaRise", RpcTarget.All, Array.Empty<object>());
			}
			return;
		}
		base.transform.position += Vector3.up * this.speed * Time.deltaTime;
		this.sinceSync += Time.deltaTime;
		if (this.sinceSync > 1f)
		{
			this.sinceSync = 0f;
			this.view.RPC("RPCA_SyncLavaHeight", RpcTarget.All, new object[]
			{
				base.transform.position.y
			});
		}
	}

	// Token: 0x060013A3 RID: 5027 RVA: 0x00063B85 File Offset: 0x00061D85
	[PunRPC]
	public void RPCA_SyncLavaHeight(float height)
	{
		base.transform.position = new Vector3(base.transform.position.x, height, base.transform.position.z);
	}

	// Token: 0x060013A4 RID: 5028 RVA: 0x00063BB8 File Offset: 0x00061DB8
	[PunRPC]
	public void RPCA_StartLavaRise()
	{
		this.rockAnim.Play("RockDoor", 0, 0f);
		this.timeToMove = true;
		GamefeelHandler.instance.AddPerlinShake(3f, 2f, 10f);
		GamefeelHandler.instance.AddPerlinShake(15f, 0.3f, 15f);
	}

	// Token: 0x060013A5 RID: 5029 RVA: 0x00063C14 File Offset: 0x00061E14
	private bool PlayersHaveMovedOn()
	{
		if (Character.AllCharacters.Count == 0)
		{
			return false;
		}
		float num = 879f;
		for (int i = 0; i < Character.AllCharacters.Count; i++)
		{
			if (Character.AllCharacters[i].Center.y > num)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x040011F1 RID: 4593
	public float speed = 0.25f;

	// Token: 0x040011F2 RID: 4594
	public Animator rockAnim;

	// Token: 0x040011F3 RID: 4595
	private PhotonView view;

	// Token: 0x040011F4 RID: 4596
	private bool timeToMove;

	// Token: 0x040011F5 RID: 4597
	private float sinceSync;
}

using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002CB RID: 715
[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(PhotonView))]
public class ChangeColor : MonoBehaviour
{
	// Token: 0x06001422 RID: 5154 RVA: 0x00065D44 File Offset: 0x00063F44
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		if (this.photonView.IsMine)
		{
			Color color = Random.ColorHSV();
			this.photonView.RPC("ChangeColour", RpcTarget.AllBuffered, new object[]
			{
				new Vector3(color.r, color.g, color.b)
			});
		}
	}

	// Token: 0x06001423 RID: 5155 RVA: 0x00065DA6 File Offset: 0x00063FA6
	[PunRPC]
	private void ChangeColour(Vector3 randomColor)
	{
		base.GetComponent<Renderer>().material.SetColor("_Color", new Color(randomColor.x, randomColor.y, randomColor.z));
	}

	// Token: 0x0400125B RID: 4699
	private PhotonView photonView;
}

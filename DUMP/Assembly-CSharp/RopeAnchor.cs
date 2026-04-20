using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000172 RID: 370
public class RopeAnchor : MonoBehaviour
{
	// Token: 0x06000C0E RID: 3086 RVA: 0x00040BDD File Offset: 0x0003EDDD
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x170000E7 RID: 231
	// (get) Token: 0x06000C0F RID: 3087 RVA: 0x00040BEB File Offset: 0x0003EDEB
	// (set) Token: 0x06000C10 RID: 3088 RVA: 0x00040BF3 File Offset: 0x0003EDF3
	public bool Ghost
	{
		get
		{
			return this.isGhost;
		}
		set
		{
			this.isGhost = value;
			this.HideAll();
			if (this.isGhost)
			{
				this.ghostPart.SetActive(true);
				return;
			}
			this.normalPart.SetActive(true);
		}
	}

	// Token: 0x06000C11 RID: 3089 RVA: 0x00040C23 File Offset: 0x0003EE23
	private void HideAll()
	{
		this.ghostPart.SetActive(false);
		this.normalPart.SetActive(false);
	}

	// Token: 0x04000B04 RID: 2820
	public GameObject ghostPart;

	// Token: 0x04000B05 RID: 2821
	public GameObject normalPart;

	// Token: 0x04000B06 RID: 2822
	public Transform anchorPoint;

	// Token: 0x04000B07 RID: 2823
	private bool isGhost;

	// Token: 0x04000B08 RID: 2824
	public PhotonView photonView;
}

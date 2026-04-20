using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200023B RID: 571
[RequireComponent(typeof(PhotonView))]
public class CloudFungus : MonoBehaviour
{
	// Token: 0x06001190 RID: 4496 RVA: 0x00058761 File Offset: 0x00056961
	private void Awake()
	{
		this.item = base.GetComponent<Item>();
	}

	// Token: 0x06001191 RID: 4497 RVA: 0x00058770 File Offset: 0x00056970
	private void Update()
	{
		if (this.item.itemState != ItemState.Ground || !this.item.photonView.IsMine)
		{
			base.enabled = false;
			return;
		}
		if (this.item.itemState == ItemState.Ground && this.item.rig.linearVelocity.y < this.yVelocityNeeded && this.timeAlive > this.timeAliveNeeded)
		{
			this.Break();
		}
		this.timeAlive += Time.deltaTime;
	}

	// Token: 0x06001192 RID: 4498 RVA: 0x000587F4 File Offset: 0x000569F4
	public void Break()
	{
		if (this.alreadyBroke)
		{
			return;
		}
		this.alreadyBroke = true;
		string prefabName = "0_Items/" + this.instantiateOnBreak.name;
		Quaternion rotation = Quaternion.Euler(0f, (float)Random.Range(0, 360), 0f);
		PhotonNetwork.Instantiate(prefabName, base.transform.position, rotation, 0, null);
		PhotonNetwork.Destroy(base.gameObject);
	}

	// Token: 0x04000F64 RID: 3940
	private Item item;

	// Token: 0x04000F65 RID: 3941
	public GameObject instantiateOnBreak;

	// Token: 0x04000F66 RID: 3942
	public float timeAliveNeeded = 0.5f;

	// Token: 0x04000F67 RID: 3943
	public float yVelocityNeeded = -1f;

	// Token: 0x04000F68 RID: 3944
	private float timeAlive;

	// Token: 0x04000F69 RID: 3945
	private bool alreadyBroke;
}

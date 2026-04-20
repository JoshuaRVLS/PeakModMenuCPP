using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200033E RID: 830
[RequireComponent(typeof(PhotonView))]
public class ShelfShroom : MonoBehaviour
{
	// Token: 0x0600161A RID: 5658 RVA: 0x000705E0 File Offset: 0x0006E7E0
	private void Awake()
	{
		this.item = base.GetComponent<Item>();
	}

	// Token: 0x0600161B RID: 5659 RVA: 0x000705F0 File Offset: 0x0006E7F0
	private void OnCollisionEnter(Collision collision)
	{
		if (!this.item.photonView.IsMine)
		{
			return;
		}
		if (this.item.itemState == ItemState.Ground && this.breakOnCollision && this.item.rig && collision.relativeVelocity.magnitude > this.minBreakVelocity)
		{
			this.Break(collision);
		}
	}

	// Token: 0x0600161C RID: 5660 RVA: 0x00070654 File Offset: 0x0006E854
	public void Break(Collision coll)
	{
		if (this.alreadyBroke)
		{
			return;
		}
		this.alreadyBroke = true;
		string prefabName = "0_Items/" + this.instantiateOnBreak.name;
		Quaternion rotation = Quaternion.Euler(0f, (float)Random.Range(0, 360), 0f);
		if (this.stickToNormal)
		{
			rotation = Quaternion.LookRotation(Vector3.forward, coll.contacts[0].normal);
		}
		PhotonNetwork.Instantiate(prefabName, coll.contacts[0].point, rotation, 0, null);
		PhotonNetwork.Destroy(base.gameObject);
	}

	// Token: 0x0400142F RID: 5167
	private Item item;

	// Token: 0x04001430 RID: 5168
	public bool breakOnCollision;

	// Token: 0x04001431 RID: 5169
	public float minBreakVelocity;

	// Token: 0x04001432 RID: 5170
	public GameObject instantiateOnBreak;

	// Token: 0x04001433 RID: 5171
	public Transform instantiatePoint;

	// Token: 0x04001434 RID: 5172
	public bool stickToNormal;

	// Token: 0x04001435 RID: 5173
	private bool alreadyBroke;
}

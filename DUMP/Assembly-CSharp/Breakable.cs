using System;
using System.Collections.Generic;
using Peak;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000108 RID: 264
[RequireComponent(typeof(PhotonView))]
public class Breakable : MonoBehaviour
{
	// Token: 0x060008E3 RID: 2275 RVA: 0x00030961 File Offset: 0x0002EB61
	private void Awake()
	{
		this.item = base.GetComponent<Item>();
		this.rig = base.GetComponent<Rigidbody>();
	}

	// Token: 0x060008E4 RID: 2276 RVA: 0x0003097B File Offset: 0x0002EB7B
	private void Start()
	{
		if (!this.rig.isKinematic)
		{
			base.gameObject.AddComponent<BreakableHelper>().breakable = this;
		}
	}

	// Token: 0x060008E5 RID: 2277 RVA: 0x0003099C File Offset: 0x0002EB9C
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

	// Token: 0x060008E6 RID: 2278 RVA: 0x00030A00 File Offset: 0x0002EC00
	public virtual void Break(Collision coll)
	{
		if (this.alreadyBroke)
		{
			return;
		}
		this.alreadyBroke = true;
		for (int i = 0; i < this.breakSFX.Count; i++)
		{
			this.breakSFX[i].Play(base.transform.position);
		}
		if (this.ragdollCharacterOnBreak)
		{
			Character componentInParent = coll.transform.GetComponentInParent<Character>();
			if (componentInParent)
			{
				Rigidbody componentInParent2 = coll.transform.GetComponentInParent<Rigidbody>();
				Vector3 a = this.lastVelocity.normalized * this.pushForce;
				componentInParent.AddForceToBodyPart(componentInParent2, a * this.pushForce, a * this.wholeBodyPushForce);
				componentInParent.Fall(2f, 15f);
			}
		}
		if (this.alternateChance > 0f && Random.value < this.alternateChance)
		{
			this.instantiateOnBreak = this.alternateInstantiateOnBreak;
			this.spawnsItemsKinematic = false;
		}
		for (int j = 0; j < this.instantiateOnBreak.Count; j++)
		{
			Item component = PhotonNetwork.Instantiate("0_Items/" + this.instantiateOnBreak[j].name, this.instantiatePoints[j].position, this.instantiatePoints[j].rotation, 0, null).GetComponent<Item>();
			if (component)
			{
				IntItemData intItemData;
				if (this.item.data.TryGetDataEntry<IntItemData>(DataEntryKey.CookedAmount, out intItemData))
				{
					component.photonView.RPC("SetCookedAmountRPC", RpcTarget.All, new object[]
					{
						intItemData.Value
					});
				}
				if (this.spawnsItemsKinematic)
				{
					component.SetKinematic(true);
					component.transform.position = coll.contacts[0].point;
					component.transform.up = coll.contacts[0].normal;
				}
				else
				{
					component.rig.linearVelocity = this.item.rig.linearVelocity;
					component.rig.angularVelocity = this.item.rig.angularVelocity;
				}
				if (this.playAnimationOnInstantiatedObject)
				{
					Animator componentInChildren = component.GetComponentInChildren<Animator>();
					if (componentInChildren)
					{
						componentInChildren.Play(this.animString, 0, 0f);
					}
				}
			}
		}
		if (this.instantiateNonItemOnBreak.Count > 0)
		{
			this.item.photonView.RPC("RPC_NonItemBreak", RpcTarget.All, Array.Empty<object>());
		}
		PhotonNetwork.Destroy(base.gameObject);
	}

	// Token: 0x060008E7 RID: 2279 RVA: 0x00030C84 File Offset: 0x0002EE84
	[PunRPC]
	private void RPC_NonItemBreak()
	{
		for (int i = 0; i < this.instantiateNonItemOnBreak.Count; i++)
		{
			Rigidbody component = Object.Instantiate<GameObject>(this.instantiateNonItemOnBreak[i], base.transform.position, base.transform.rotation).GetComponent<Rigidbody>();
			if (component)
			{
				component.linearVelocity = this.item.rig.linearVelocity;
				component.angularVelocity = this.item.rig.angularVelocity;
			}
		}
	}

	// Token: 0x04000870 RID: 2160
	private Item item;

	// Token: 0x04000871 RID: 2161
	public bool breakOnCollision;

	// Token: 0x04000872 RID: 2162
	public float minBreakVelocity;

	// Token: 0x04000873 RID: 2163
	public List<GameObject> instantiateOnBreak;

	// Token: 0x04000874 RID: 2164
	public float alternateChance;

	// Token: 0x04000875 RID: 2165
	public List<GameObject> alternateInstantiateOnBreak;

	// Token: 0x04000876 RID: 2166
	public List<SFX_Instance> breakSFX;

	// Token: 0x04000877 RID: 2167
	public List<GameObject> instantiateNonItemOnBreak;

	// Token: 0x04000878 RID: 2168
	public List<Transform> instantiatePoints;

	// Token: 0x04000879 RID: 2169
	public bool spawnsItemsKinematic;

	// Token: 0x0400087A RID: 2170
	public bool playAnimationOnInstantiatedObject;

	// Token: 0x0400087B RID: 2171
	public string animString;

	// Token: 0x0400087C RID: 2172
	public bool ragdollCharacterOnBreak;

	// Token: 0x0400087D RID: 2173
	[HideInInspector]
	public Rigidbody rig;

	// Token: 0x0400087E RID: 2174
	private bool alreadyBroke;

	// Token: 0x0400087F RID: 2175
	[HideInInspector]
	public Vector3 lastVelocity;

	// Token: 0x04000880 RID: 2176
	public float pushForce = 2f;

	// Token: 0x04000881 RID: 2177
	public float wholeBodyPushForce = 1f;
}

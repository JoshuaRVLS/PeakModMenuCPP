using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000107 RID: 263
[RequireComponent(typeof(PhotonView))]
public class Bonkable : MonoBehaviour
{
	// Token: 0x060008DB RID: 2267 RVA: 0x000306C8 File Offset: 0x0002E8C8
	private void Awake()
	{
		this.item = base.GetComponent<Item>();
	}

	// Token: 0x060008DC RID: 2268 RVA: 0x000306D6 File Offset: 0x0002E8D6
	private void OnEnable()
	{
		GlobalEvents.OnItemThrown = (Action<Item>)Delegate.Combine(GlobalEvents.OnItemThrown, new Action<Item>(this.TestItemThrown));
	}

	// Token: 0x060008DD RID: 2269 RVA: 0x000306F8 File Offset: 0x0002E8F8
	private void OnDisable()
	{
		GlobalEvents.OnItemThrown = (Action<Item>)Delegate.Remove(GlobalEvents.OnItemThrown, new Action<Item>(this.TestItemThrown));
	}

	// Token: 0x060008DE RID: 2270 RVA: 0x0003071A File Offset: 0x0002E91A
	private void TestItemThrown(Item thrownItem)
	{
		if (thrownItem == this.item)
		{
			this.thrown = true;
		}
	}

	// Token: 0x060008DF RID: 2271 RVA: 0x00030734 File Offset: 0x0002E934
	public bool VelocityAboveThreshold(Collision coll)
	{
		float magnitude = coll.relativeVelocity.magnitude;
		if (this.thrown && this.decreaseMinVelocityIfRecentlyThrown)
		{
			return magnitude >= this.minBonkVelocityThrown;
		}
		return magnitude >= this.minBonkVelocity;
	}

	// Token: 0x060008E0 RID: 2272 RVA: 0x0003077C File Offset: 0x0002E97C
	private void OnCollisionEnter(Collision coll)
	{
		if (!this.item.photonView.IsMine)
		{
			return;
		}
		if ((HelperFunctions.terrainMapMask & 1 << coll.transform.gameObject.layer) != 0)
		{
			this.thrown = false;
		}
		if (this.item.itemState == ItemState.Ground && this.item.rig && this.VelocityAboveThreshold(coll))
		{
			this.Bonk(coll);
		}
	}

	// Token: 0x060008E1 RID: 2273 RVA: 0x000307F4 File Offset: 0x0002E9F4
	private void Bonk(Collision coll)
	{
		Character componentInParent = coll.gameObject.GetComponentInParent<Character>();
		if (componentInParent && Time.time > this.lastBonkedTime + this.bonkCooldown)
		{
			componentInParent.Fall(this.ragdollTime, 0f);
			for (int i = 0; i < this.bonk.Length; i++)
			{
				this.bonk[i].Play(base.transform.position);
			}
			this.lastBonkedTime = Time.time;
			if (this.bonkMassMultiplier)
			{
				componentInParent.AddForceAtPosition(-coll.relativeVelocity.normalized * this.bonkForce * this.item.GetComponent<ItemScaleSyncer>().currentScale, coll.contacts[0].point, this.bonkRange);
				return;
			}
			componentInParent.AddForceAtPosition(-coll.relativeVelocity.normalized * this.bonkForce, coll.contacts[0].point, this.bonkRange);
		}
	}

	// Token: 0x04000864 RID: 2148
	private Item item;

	// Token: 0x04000865 RID: 2149
	public float minBonkVelocity = 5f;

	// Token: 0x04000866 RID: 2150
	public bool decreaseMinVelocityIfRecentlyThrown;

	// Token: 0x04000867 RID: 2151
	public float minBonkVelocityThrown = 5f;

	// Token: 0x04000868 RID: 2152
	public float ragdollTime = 1f;

	// Token: 0x04000869 RID: 2153
	public float bonkForce = 1000f;

	// Token: 0x0400086A RID: 2154
	public float bonkRange = 3f;

	// Token: 0x0400086B RID: 2155
	public SFX_Instance[] bonk;

	// Token: 0x0400086C RID: 2156
	public float lastBonkedTime;

	// Token: 0x0400086D RID: 2157
	private float bonkCooldown = 1f;

	// Token: 0x0400086E RID: 2158
	private bool thrown;

	// Token: 0x0400086F RID: 2159
	public bool bonkMassMultiplier;
}

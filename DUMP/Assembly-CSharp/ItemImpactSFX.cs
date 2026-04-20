using System;
using UnityEngine;

// Token: 0x02000285 RID: 645
public class ItemImpactSFX : MonoBehaviour
{
	// Token: 0x060012A2 RID: 4770 RVA: 0x0005DAB1 File Offset: 0x0005BCB1
	private void Start()
	{
		this.rig = base.GetComponent<Rigidbody>();
		this.item = base.GetComponent<Item>();
	}

	// Token: 0x060012A3 RID: 4771 RVA: 0x0005DACC File Offset: 0x0005BCCC
	private void Update()
	{
		if (this.rig)
		{
			if (this.rig.isKinematic)
			{
				return;
			}
			this.vel = Mathf.Lerp(this.vel, Vector3.SqrMagnitude(this.rig.linearVelocity) * this.velMult, 10f * Time.deltaTime);
		}
	}

	// Token: 0x060012A4 RID: 4772 RVA: 0x0005DB28 File Offset: 0x0005BD28
	private void OnCollisionEnter(Collision collision)
	{
		if (this.rig)
		{
			if (this.item)
			{
				if (!this.item.holderCharacter)
				{
					if (this.vel > 4f)
					{
						for (int i = 0; i < this.impact.Length; i++)
						{
							this.impact[i].Play(base.transform.position);
						}
					}
				}
				else if (this.vel > 36f && !this.disallowInHands)
				{
					for (int j = 0; j < this.impact.Length; j++)
					{
						this.impact[j].Play(base.transform.position);
					}
				}
			}
			if (!this.item && !collision.rigidbody && this.vel > 64f)
			{
				for (int k = 0; k < this.impact.Length; k++)
				{
					this.impact[k].Play(base.transform.position);
				}
			}
			this.vel = 0f;
		}
	}

	// Token: 0x040010B8 RID: 4280
	public float vel;

	// Token: 0x040010B9 RID: 4281
	private Rigidbody rig;

	// Token: 0x040010BA RID: 4282
	private Item item;

	// Token: 0x040010BB RID: 4283
	public float velMult = 1f;

	// Token: 0x040010BC RID: 4284
	public SFX_Instance[] impact;

	// Token: 0x040010BD RID: 4285
	public bool disallowInHands;
}

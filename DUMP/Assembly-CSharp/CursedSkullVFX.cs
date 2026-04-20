using System;
using UnityEngine;

// Token: 0x02000241 RID: 577
public class CursedSkullVFX : ItemVFX
{
	// Token: 0x060011A2 RID: 4514 RVA: 0x00058F23 File Offset: 0x00057123
	protected override void Start()
	{
		base.Start();
		this.curseParticles.Play();
		this.animator.enabled = true;
		if (this.item.itemState == ItemState.InBackpack)
		{
			this.disableInBackpack.SetActive(false);
		}
	}

	// Token: 0x04000F8D RID: 3981
	public ParticleSystem curseParticles;

	// Token: 0x04000F8E RID: 3982
	public Animator animator;

	// Token: 0x04000F8F RID: 3983
	public GameObject disableInBackpack;
}

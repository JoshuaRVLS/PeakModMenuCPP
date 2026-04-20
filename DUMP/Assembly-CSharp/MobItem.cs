using System;
using UnityEngine;

// Token: 0x0200002A RID: 42
public class MobItem : Item
{
	// Token: 0x06000307 RID: 775 RVA: 0x000153E4 File Offset: 0x000135E4
	protected override void Awake()
	{
		base.Awake();
		this.mob = base.GetComponent<Mob>();
		this.syncer = base.GetComponent<MobItemPhysicsSyncer>();
	}

	// Token: 0x06000308 RID: 776 RVA: 0x00015404 File Offset: 0x00013604
	protected override void Start()
	{
		base.Start();
		this.mob.forceNoMovement = (base.itemState != ItemState.Ground || !base.photonView.IsMine);
		if (base.cooking.timesCookedLocal > 0)
		{
			this.mob.anim.Play("ScorpionCooked", 0, 1f);
		}
		if (base.itemState == ItemState.Held && Character.localCharacter.data.currentItem == this)
		{
			this.mob.SetForcedTarget(Character.localCharacter);
		}
	}

	// Token: 0x06000309 RID: 777 RVA: 0x00015494 File Offset: 0x00013694
	protected override void Update()
	{
		this.syncer.shouldSync = !this.mob.sleeping;
		this.UIData.hasMainInteract = (base.cooking.timesCookedLocal > 0);
		this.canUseOnFriend = (base.cooking.timesCookedLocal > 0);
		if (base.cooking.timesCookedLocal > 0)
		{
			this.mob.mobState = Mob.MobState.Dead;
			return;
		}
		if (this.mob.sleeping)
		{
			return;
		}
		this.mob.forceNoMovement = (base.itemState != ItemState.Ground || !base.photonView.IsMine);
		if (base.photonView.IsMine && base.itemState == ItemState.Ground)
		{
			base.ForceSyncForFrames(10);
		}
	}

	// Token: 0x0600030A RID: 778 RVA: 0x00015551 File Offset: 0x00013751
	public override bool CanUsePrimary()
	{
		return base.cooking.timesCookedLocal > 0;
	}

	// Token: 0x040002CA RID: 714
	protected Mob mob;

	// Token: 0x040002CB RID: 715
	private MobItemPhysicsSyncer syncer;

	// Token: 0x040002CC RID: 716
	public float sleepDistance = 50f;

	// Token: 0x040002CD RID: 717
	public Animator anim;
}

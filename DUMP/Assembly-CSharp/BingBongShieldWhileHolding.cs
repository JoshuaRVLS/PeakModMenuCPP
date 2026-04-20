using System;
using Peak.Afflictions;
using UnityEngine;

// Token: 0x02000106 RID: 262
public class BingBongShieldWhileHolding : ItemComponent
{
	// Token: 0x060008D5 RID: 2261 RVA: 0x000305D1 File Offset: 0x0002E7D1
	private void Start()
	{
		this.TryApplyInvincibility();
	}

	// Token: 0x060008D6 RID: 2262 RVA: 0x000305DC File Offset: 0x0002E7DC
	private void Update()
	{
		this.tick += Time.deltaTime;
		if (this.tick >= 1.5f)
		{
			this.TryApplyInvincibility();
			this.tick = 0f;
		}
		if (!this.wasHeldByLocal && Character.localCharacter.data.currentItem == this.item)
		{
			this.wasHeldByLocal = true;
		}
	}

	// Token: 0x060008D7 RID: 2263 RVA: 0x00030644 File Offset: 0x0002E844
	private void TryApplyInvincibility()
	{
		if (Character.localCharacter && Character.localCharacter.data.currentItem == this.item)
		{
			Character.localCharacter.refs.afflictions.AddAffliction(new Affliction_BingBongShield
			{
				totalTime = 2f
			}, false);
		}
	}

	// Token: 0x060008D8 RID: 2264 RVA: 0x0003069E File Offset: 0x0002E89E
	private void OnDestroy()
	{
		if (this.wasHeldByLocal)
		{
			Character.localCharacter.refs.afflictions.RemoveAffliction(Affliction.AfflictionType.BingBongShield);
		}
	}

	// Token: 0x060008D9 RID: 2265 RVA: 0x000306BE File Offset: 0x0002E8BE
	public override void OnInstanceDataSet()
	{
	}

	// Token: 0x04000862 RID: 2146
	private bool wasHeldByLocal;

	// Token: 0x04000863 RID: 2147
	private float tick;
}

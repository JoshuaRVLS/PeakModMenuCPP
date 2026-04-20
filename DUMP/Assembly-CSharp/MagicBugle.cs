using System;
using Peak.Network;
using UnityEngine;
using UnityEngine.UI.Extensions;

// Token: 0x02000126 RID: 294
public class MagicBugle : ItemComponent
{
	// Token: 0x1700009F RID: 159
	// (get) Token: 0x060009A1 RID: 2465 RVA: 0x00033579 File Offset: 0x00031779
	public float currentFuel
	{
		get
		{
			return this.fuel;
		}
	}

	// Token: 0x060009A2 RID: 2466 RVA: 0x00033584 File Offset: 0x00031784
	public override void Awake()
	{
		base.Awake();
		Item item = this.item;
		item.OnPrimaryStarted = (Action)Delegate.Combine(item.OnPrimaryStarted, new Action(this.StartToot));
		Item item2 = this.item;
		item2.OnPrimaryCancelled = (Action)Delegate.Combine(item2.OnPrimaryCancelled, new Action(this.CancelToot));
	}

	// Token: 0x060009A3 RID: 2467 RVA: 0x000335E8 File Offset: 0x000317E8
	public void OnDestroy()
	{
		Item item = this.item;
		item.OnPrimaryHeld = (Action)Delegate.Remove(item.OnPrimaryHeld, new Action(this.StartToot));
		Item item2 = this.item;
		item2.OnPrimaryCancelled = (Action)Delegate.Remove(item2.OnPrimaryCancelled, new Action(this.CancelToot));
	}

	// Token: 0x060009A4 RID: 2468 RVA: 0x00033644 File Offset: 0x00031844
	public override void OnInstanceDataSet()
	{
		if (base.HasData(DataEntryKey.Fuel))
		{
			this.fuel = base.GetData<FloatItemData>(DataEntryKey.Fuel).Value;
			this.item.SetUseRemainingPercentage(this.fuel / this.totalTootTime);
			return;
		}
		if (this.photonView.IsMine)
		{
			this.fuel = this.totalTootTime;
			this.item.SetUseRemainingPercentage(1f);
		}
	}

	// Token: 0x060009A5 RID: 2469 RVA: 0x000336B0 File Offset: 0x000318B0
	private void Update()
	{
		this.UpdateToot();
	}

	// Token: 0x060009A6 RID: 2470 RVA: 0x000336B8 File Offset: 0x000318B8
	private void UpdateToot()
	{
		if (this.tooting && this.HasAuthority())
		{
			this.fuel -= Time.deltaTime;
			if (this.fuel <= 0f)
			{
				this.fuel = 0f;
				if (this.photonView.IsMine)
				{
					this.CancelToot();
				}
			}
			else if (this.photonView.IsMine)
			{
				this.tootTick -= Time.deltaTime;
				if (this.tootTick <= 0f)
				{
					this.massAffliction.RunAction();
					this.tootTick = 0.1f;
				}
			}
			base.GetData<FloatItemData>(DataEntryKey.Fuel).Value = this.fuel;
			this.item.SetUseRemainingPercentage(this.fuel / this.totalTootTime);
		}
	}

	// Token: 0x060009A7 RID: 2471 RVA: 0x00033788 File Offset: 0x00031988
	private void StartToot()
	{
		Debug.Log("Started toot");
		if (this.fuel >= this.initialTootCost)
		{
			this.fuel -= this.initialTootCost;
			this.tooting = true;
			this.item.SetUseRemainingPercentage(this.fuel / this.totalTootTime);
		}
	}

	// Token: 0x060009A8 RID: 2472 RVA: 0x000337DF File Offset: 0x000319DF
	private void CancelToot()
	{
		Debug.Log("Cancelled toot");
		this.tooting = false;
	}

	// Token: 0x040008E7 RID: 2279
	public float initialTootCost;

	// Token: 0x040008E8 RID: 2280
	public float totalTootTime;

	// Token: 0x040008E9 RID: 2281
	private bool tooting;

	// Token: 0x040008EA RID: 2282
	[SerializeField]
	[ReadOnly]
	private float fuel;

	// Token: 0x040008EB RID: 2283
	public Action_ApplyMassAffliction massAffliction;

	// Token: 0x040008EC RID: 2284
	private float tootTick;
}

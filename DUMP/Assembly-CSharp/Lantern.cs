using System;
using DG.Tweening;
using Peak.Network;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000028 RID: 40
public class Lantern : ItemComponent
{
	// Token: 0x060002E6 RID: 742 RVA: 0x0001463A File Offset: 0x0001283A
	public override void Awake()
	{
		base.Awake();
		this.item = base.GetComponent<Item>();
	}

	// Token: 0x060002E7 RID: 743 RVA: 0x0001464E File Offset: 0x0001284E
	public override void OnEnable()
	{
		Item item = this.item;
		item.onStashAction = (Action)Delegate.Combine(item.onStashAction, new Action(this.SnuffLantern));
	}

	// Token: 0x060002E8 RID: 744 RVA: 0x00014677 File Offset: 0x00012877
	public override void OnDisable()
	{
		Item item = this.item;
		item.onStashAction = (Action)Delegate.Remove(item.onStashAction, new Action(this.SnuffLantern));
	}

	// Token: 0x060002E9 RID: 745 RVA: 0x000146A0 File Offset: 0x000128A0
	private void Start()
	{
		if (base.HasData(DataEntryKey.FlareActive) && base.GetData<BoolItemData>(DataEntryKey.FlareActive).Value)
		{
			this.fireParticle.main.prewarm = true;
			this.fireParticle.Play();
		}
		if (this.activeByDefault && this.item.itemState == ItemState.Held)
		{
			this.lanternLight.gameObject.SetActive(false);
			this.fireParticle.Stop();
		}
	}

	// Token: 0x060002EA RID: 746 RVA: 0x00014718 File Offset: 0x00012918
	public override void OnInstanceDataSet()
	{
		if (base.HasData(DataEntryKey.FlareActive))
		{
			this.lit = base.GetData<BoolItemData>(DataEntryKey.FlareActive).Value;
		}
		this.fuel = base.GetData<FloatItemData>(DataEntryKey.Fuel, new Func<FloatItemData>(this.SetupDefaultFuel)).Value;
		this.item.SetUseRemainingPercentage(this.fuel / this.startingFuel);
	}

	// Token: 0x060002EB RID: 747 RVA: 0x00014778 File Offset: 0x00012978
	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if (this.activeByDefault && this.item.rig.isKinematic)
		{
			return;
		}
		if (this.lanternLight.gameObject.activeSelf != this.lit)
		{
			this.lanternLight.gameObject.SetActive(this.lit);
			if (this.lit)
			{
				this.fireParticle.Play();
				this.currentLightIntensity = 0f;
				DOTween.To(() => this.currentLightIntensity, delegate(float x)
				{
					this.currentLightIntensity = x;
				}, this.lightIntensity, 1f);
			}
			else
			{
				this.fireParticle.Clear();
				this.fireParticle.Stop();
			}
		}
		this.lanternLight.intensity = this.lightCurve.Evaluate(Time.time * this.lightSpeed) * this.currentLightIntensity;
		this.item.UIData.mainInteractPrompt = (this.lit ? this.actionPromptWhenLit : this.actionPromptWhenUnlit);
		this.item.usingTimePrimary = (this.lit ? this.useTimeWhenLit : this.useTimeWhenUnlit);
		base.GetData<OptionableIntItemData>(DataEntryKey.ItemUses).Value = ((this.fuel > 0f) ? -1 : 0);
		this.UpdateFuel();
	}

	// Token: 0x060002EC RID: 748 RVA: 0x000148C8 File Offset: 0x00012AC8
	private void UpdateFuel()
	{
		if (this.lit && this.HasAuthority())
		{
			this.fuel -= Time.deltaTime;
			if (this.fuel <= 0f)
			{
				this.fuel = 0f;
				if (this.photonView.IsMine)
				{
					this.SnuffLantern();
				}
			}
			base.GetData<FloatItemData>(DataEntryKey.Fuel, new Func<FloatItemData>(this.SetupDefaultFuel)).Value = this.fuel;
			this.item.SetUseRemainingPercentage(this.fuel / this.startingFuel);
		}
	}

	// Token: 0x060002ED RID: 749 RVA: 0x00014959 File Offset: 0x00012B59
	private FloatItemData SetupDefaultFuel()
	{
		return new FloatItemData
		{
			Value = this.startingFuel
		};
	}

	// Token: 0x060002EE RID: 750 RVA: 0x0001496C File Offset: 0x00012B6C
	public void ToggleLantern()
	{
		this.photonView.RPC("LightLanternRPC", RpcTarget.All, new object[]
		{
			!this.lit
		});
	}

	// Token: 0x060002EF RID: 751 RVA: 0x00014996 File Offset: 0x00012B96
	public void SnuffLantern()
	{
		this.photonView.RPC("LightLanternRPC", RpcTarget.All, new object[]
		{
			false
		});
	}

	// Token: 0x060002F0 RID: 752 RVA: 0x000149B8 File Offset: 0x00012BB8
	[PunRPC]
	public void LightLanternRPC(bool litValue)
	{
		this.fireParticle.main.prewarm = false;
		this.lit = litValue;
		base.GetData<BoolItemData>(DataEntryKey.FlareActive).Value = this.lit;
	}

	// Token: 0x040002A8 RID: 680
	[SerializeField]
	private bool lit;

	// Token: 0x040002A9 RID: 681
	public string actionPromptWhenUnlit;

	// Token: 0x040002AA RID: 682
	public string actionPromptWhenLit;

	// Token: 0x040002AB RID: 683
	public float useTimeWhenUnlit;

	// Token: 0x040002AC RID: 684
	public float useTimeWhenLit;

	// Token: 0x040002AD RID: 685
	public Light lanternLight;

	// Token: 0x040002AE RID: 686
	public AnimationCurve lightCurve;

	// Token: 0x040002AF RID: 687
	public float lightSpeed;

	// Token: 0x040002B0 RID: 688
	public float lightIntensity = 10f;

	// Token: 0x040002B1 RID: 689
	public float startingFuel;

	// Token: 0x040002B2 RID: 690
	[SerializeField]
	private float fuel;

	// Token: 0x040002B3 RID: 691
	public ParticleSystem fireParticle;

	// Token: 0x040002B4 RID: 692
	private new Item item;

	// Token: 0x040002B5 RID: 693
	private float currentLightIntensity;

	// Token: 0x040002B6 RID: 694
	public bool activeByDefault;
}

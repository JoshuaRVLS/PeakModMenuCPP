using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002B0 RID: 688
public class Mandrake : ItemComponent
{
	// Token: 0x06001379 RID: 4985 RVA: 0x00062CB1 File Offset: 0x00060EB1
	public override void Awake()
	{
		base.Awake();
	}

	// Token: 0x0600137A RID: 4986 RVA: 0x00062CB9 File Offset: 0x00060EB9
	private IEnumerator Start()
	{
		while (!PhotonNetwork.InRoom)
		{
			yield return null;
		}
		this.CheckNearby();
		if (base.HasData(DataEntryKey.Used))
		{
			this.waitBeforeScreamTime = Random.Range(this.screamWaitMin, this.screamWaitMax);
		}
		else
		{
			this.waitBeforeScreamTime = this.screamWaitMax;
			base.GetData<BoolItemData>(DataEntryKey.Used).Value = true;
		}
		this.sfxVolumeSetting = GameHandler.Instance.SettingsHandler.GetSetting<SFXVolumeSetting>();
		this.masterVolumeSetting = GameHandler.Instance.SettingsHandler.GetSetting<MasterVolumeSetting>();
		yield break;
	}

	// Token: 0x0600137B RID: 4987 RVA: 0x00062CC8 File Offset: 0x00060EC8
	private void CheckNearby()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		this.checkTime += Time.deltaTime;
		if (this.checkTime > 1f)
		{
			bool closeToSomebody = false;
			this.checkTime = 0f;
			if (this.item.cooking.timesCookedLocal > 0)
			{
				base.enabled = false;
				closeToSomebody = false;
				return;
			}
			foreach (Character character in Character.AllCharacters)
			{
				if (Vector3.Distance(base.transform.position, character.Center) < this.nearPlayersDistance && character.data.fullyConscious)
				{
					closeToSomebody = true;
					break;
				}
			}
			this._closeToSomebody = closeToSomebody;
		}
	}

	// Token: 0x0600137C RID: 4988 RVA: 0x00062DA0 File Offset: 0x00060FA0
	public void Update()
	{
		this.CheckNearby();
		this.CheckScream();
	}

	// Token: 0x0600137D RID: 4989 RVA: 0x00062DB0 File Offset: 0x00060FB0
	private void CheckScream()
	{
		if (PhotonNetwork.IsMasterClient && this._closeToSomebody && !this.screaming && (this.item.itemState != ItemState.Ground || (this.item.itemState == ItemState.Ground && !this.item.rig.isKinematic)))
		{
			this._time += Time.deltaTime;
			if (this._time > this.waitBeforeScreamTime)
			{
				this.view.RPC("RPC_Scream", RpcTarget.All, Array.Empty<object>());
				this._time = 0f;
				this.waitBeforeScreamTime = Random.Range(this.screamWaitMin, this.screamWaitMax);
			}
		}
		this.aoe.enabled = (this.sfxVolumeSetting.Value > 0.01f && this.masterVolumeSetting.Value > 0.01f);
		this.timeEvent.enabled = this.aoe.enabled;
	}

	// Token: 0x0600137E RID: 4990 RVA: 0x00062EA9 File Offset: 0x000610A9
	[PunRPC]
	public void RPC_Scream()
	{
		base.StartCoroutine(this.ScreamRoutine());
	}

	// Token: 0x0600137F RID: 4991 RVA: 0x00062EB8 File Offset: 0x000610B8
	private IEnumerator ScreamRoutine()
	{
		if (this.item.cooking.timesCookedLocal > 0)
		{
			yield break;
		}
		this.screaming = true;
		this.anim.SetBool("Scream", true);
		while (this.currentScreamTime < this.screamWaitTime)
		{
			this.currentScreamTime += Time.deltaTime;
			base.GetData<FloatItemData>(DataEntryKey.ScreamTime).Value = this.currentScreamTime;
			yield return null;
			if (this.item.cooking.timesCookedLocal > 0)
			{
				break;
			}
		}
		this.screaming = false;
		this.currentScreamTime = 0f;
		this.anim.SetBool("Scream", false);
		yield break;
	}

	// Token: 0x06001380 RID: 4992 RVA: 0x00062EC7 File Offset: 0x000610C7
	private void OnDestroy()
	{
		SFX_Player.StopPlaying(this.handle, 0f);
	}

	// Token: 0x06001381 RID: 4993 RVA: 0x00062ED9 File Offset: 0x000610D9
	public override void OnInstanceDataSet()
	{
		this.currentScreamTime = base.GetData<FloatItemData>(DataEntryKey.ScreamTime).Value;
		if (this.currentScreamTime > 0f && !this.screaming)
		{
			base.StartCoroutine(this.ScreamRoutine());
		}
	}

	// Token: 0x040011BF RID: 4543
	public new Item item;

	// Token: 0x040011C0 RID: 4544
	public Animator anim;

	// Token: 0x040011C1 RID: 4545
	public SFX_Instance sfxScream;

	// Token: 0x040011C2 RID: 4546
	public PhotonView view;

	// Token: 0x040011C3 RID: 4547
	public float screamWaitMin;

	// Token: 0x040011C4 RID: 4548
	public float screamWaitMax;

	// Token: 0x040011C5 RID: 4549
	private float _time;

	// Token: 0x040011C6 RID: 4550
	private float screamWaitTime = 3f;

	// Token: 0x040011C7 RID: 4551
	private float currentScreamTime;

	// Token: 0x040011C8 RID: 4552
	private MandrakeScreamFX mandrakeScreamFX;

	// Token: 0x040011C9 RID: 4553
	private SFX_Player.SoundEffectHandle handle;

	// Token: 0x040011CA RID: 4554
	public ParticleSystem vfxScream;

	// Token: 0x040011CB RID: 4555
	private bool _closeToSomebody = true;

	// Token: 0x040011CC RID: 4556
	public bool screaming;

	// Token: 0x040011CD RID: 4557
	public float nearPlayersDistance = 20f;

	// Token: 0x040011CE RID: 4558
	private SFXVolumeSetting sfxVolumeSetting;

	// Token: 0x040011CF RID: 4559
	private MasterVolumeSetting masterVolumeSetting;

	// Token: 0x040011D0 RID: 4560
	public AOE aoe;

	// Token: 0x040011D1 RID: 4561
	public TimeEvent timeEvent;

	// Token: 0x040011D2 RID: 4562
	private float checkTime;

	// Token: 0x040011D3 RID: 4563
	[SerializeField]
	private float waitBeforeScreamTime;
}

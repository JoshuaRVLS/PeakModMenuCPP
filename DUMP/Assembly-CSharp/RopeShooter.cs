using System;
using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000179 RID: 377
public class RopeShooter : ItemComponent
{
	// Token: 0x170000EA RID: 234
	// (get) Token: 0x06000C45 RID: 3141 RVA: 0x00041E4D File Offset: 0x0004004D
	// (set) Token: 0x06000C46 RID: 3142 RVA: 0x00041E68 File Offset: 0x00040068
	private int Ammo
	{
		get
		{
			return base.GetData<IntItemData>(DataEntryKey.PetterItemUses, new Func<IntItemData>(this.GetNew)).Value;
		}
		set
		{
			base.GetData<IntItemData>(DataEntryKey.PetterItemUses, new Func<IntItemData>(this.GetNew)).Value = value;
			this.item.SetUseRemainingPercentage((float)value / (float)this.startAmmo);
		}
	}

	// Token: 0x06000C47 RID: 3143 RVA: 0x00041E99 File Offset: 0x00040099
	private IntItemData GetNew()
	{
		Debug.Log(string.Format("GetNew startAmmo: {0}", this.startAmmo));
		return new IntItemData
		{
			Value = this.startAmmo
		};
	}

	// Token: 0x06000C48 RID: 3144 RVA: 0x00041EC6 File Offset: 0x000400C6
	public override void Awake()
	{
		base.Awake();
		Item item = this.item;
		item.OnPrimaryFinishedCast = (Action)Delegate.Combine(item.OnPrimaryFinishedCast, new Action(this.OnPrimaryFinishedCast));
	}

	// Token: 0x06000C49 RID: 3145 RVA: 0x00041EF5 File Offset: 0x000400F5
	private void OnDestroy()
	{
		Item item = this.item;
		item.OnPrimaryFinishedCast = (Action)Delegate.Remove(item.OnPrimaryFinishedCast, new Action(this.OnPrimaryFinishedCast));
	}

	// Token: 0x06000C4A RID: 3146 RVA: 0x00041F20 File Offset: 0x00040120
	public void Update()
	{
		RaycastHit raycastHit;
		this.item.overrideUsability = Optionable<bool>.Some(this.WillAttach(out raycastHit));
	}

	// Token: 0x170000EB RID: 235
	// (get) Token: 0x06000C4B RID: 3147 RVA: 0x00041F45 File Offset: 0x00040145
	public bool HasAmmo
	{
		get
		{
			return this.Ammo >= 1;
		}
	}

	// Token: 0x06000C4C RID: 3148 RVA: 0x00041F54 File Offset: 0x00040154
	private void OnPrimaryFinishedCast()
	{
		RaycastHit raycastHit;
		if (!this.WillAttach(out raycastHit))
		{
			return;
		}
		Debug.Log("OnPrimaryFinishedCast");
		if (!this.HasAmmo)
		{
			this.fumesVFX.Play();
			Debug.Log(string.Format("totalUses < 1,  {0}", this.item.totalUses));
			for (int i = 0; i < this.emptySound.Length; i++)
			{
				this.emptySound[i].Play(base.transform.position);
			}
			return;
		}
		Transform transform = MainCamera.instance.transform;
		RaycastHit raycastHit2;
		if (!transform.ForwardRay<Transform>().Raycast(out raycastHit2, HelperFunctions.LayerType.TerrainMap.ToLayerMask(), 0f))
		{
			return;
		}
		Quaternion identity = Quaternion.identity;
		if (Vector3.Angle(raycastHit2.normal, Vector3.up) < 45f)
		{
			Debug.Log("Angle is less than 45");
			ExtQuaternion.FromUpAndRightPrioUp(base.transform.forward, raycastHit2.normal);
		}
		else
		{
			Debug.Log("Angle is more than 45");
			ExtQuaternion.FromUpAndRightPrioUp(Vector3.down, -transform.forward);
		}
		GameObject gameObject = PhotonNetwork.Instantiate(this.ropeAnchorWithRopePref.name, this.spawnPoint.position, ExtQuaternion.FromUpAndRightPrioUp(base.transform.forward, raycastHit2.normal), 0, null);
		float num = Vector3.Distance(this.spawnPoint.position, raycastHit2.point) * 0.01f;
		this.gunshotVFX.Play();
		for (int j = 0; j < this.shotSound.Length; j++)
		{
			this.shotSound[j].Play(base.transform.position);
		}
		GamefeelHandler.instance.AddPerlinShakeProximity(this.gunshotVFX.transform.position, this.screenshakeIntensity, 0.3f, 15f, 10f);
		this.hideOnFire.SetActive(this.HasAmmo);
		int ammo = this.Ammo;
		this.Ammo = ammo - 1;
		this.photonView.RPC("Sync_Rpc", RpcTarget.AllBuffered, new object[]
		{
			this.HasAmmo
		});
		gameObject.GetComponent<RopeAnchorProjectile>().photonView.RPC("GetShot", RpcTarget.AllBuffered, new object[]
		{
			raycastHit2.point,
			num,
			this.length,
			-Camera.main.transform.forward
		});
		if (this.photonView.IsMine)
		{
			Singleton<AchievementManager>.Instance.AddToRunBasedFloat(RUNBASEDVALUETYPE.RopePlaced, Rope.GetLengthInMeters(this.length));
			GameUtils.instance.IncrementPermanentItemsPlaced();
		}
	}

	// Token: 0x06000C4D RID: 3149 RVA: 0x000421F3 File Offset: 0x000403F3
	[PunRPC]
	private void Sync_Rpc(bool show)
	{
		Debug.Log(string.Format("Sync_Rpc: {0}", show));
		this.hideOnFire.SetActive(show);
	}

	// Token: 0x06000C4E RID: 3150 RVA: 0x00042218 File Offset: 0x00040418
	public bool WillAttach(out RaycastHit hit)
	{
		hit = default(RaycastHit);
		return Character.localCharacter.data.isGrounded && this.HasAmmo && Physics.Raycast(MainCamera.instance.transform.position, MainCamera.instance.transform.forward, out hit, this.maxLength, HelperFunctions.LayerType.TerrainMap.ToLayerMask(), QueryTriggerInteraction.UseGlobal);
	}

	// Token: 0x06000C4F RID: 3151 RVA: 0x00042284 File Offset: 0x00040484
	public override void OnInstanceDataSet()
	{
		this.hideOnFire.SetActive(this.HasAmmo);
		Debug.Log(string.Format(" OnInstanceDataSet item.totalUses: {0}", this.Ammo));
		this.item.SetUseRemainingPercentage((float)this.Ammo / (float)this.startAmmo);
	}

	// Token: 0x04000B32 RID: 2866
	public ParticleSystem gunshotVFX;

	// Token: 0x04000B33 RID: 2867
	public ParticleSystem fumesVFX;

	// Token: 0x04000B34 RID: 2868
	public bool cantReFire;

	// Token: 0x04000B35 RID: 2869
	public Transform spawnPoint;

	// Token: 0x04000B36 RID: 2870
	public float length;

	// Token: 0x04000B37 RID: 2871
	public GameObject ropeAnchorWithRopePref;

	// Token: 0x04000B38 RID: 2872
	public GameObject hideOnFire;

	// Token: 0x04000B39 RID: 2873
	public float screenshakeIntensity = 30f;

	// Token: 0x04000B3A RID: 2874
	public int startAmmo = 1;

	// Token: 0x04000B3B RID: 2875
	public SFX_Instance[] shotSound;

	// Token: 0x04000B3C RID: 2876
	public SFX_Instance[] emptySound;

	// Token: 0x04000B3D RID: 2877
	public float maxLength = 30f;
}

using System;
using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x0200017A RID: 378
public class RopeSpool : ItemComponent
{
	// Token: 0x170000EC RID: 236
	// (get) Token: 0x06000C51 RID: 3153 RVA: 0x000422FB File Offset: 0x000404FB
	public bool IsOutOfRope
	{
		get
		{
			return this.ropeFuel <= 2f;
		}
	}

	// Token: 0x170000ED RID: 237
	// (get) Token: 0x06000C52 RID: 3154 RVA: 0x0004230D File Offset: 0x0004050D
	// (set) Token: 0x06000C53 RID: 3155 RVA: 0x00042328 File Offset: 0x00040528
	public float RopeFuel
	{
		get
		{
			return base.GetData<FloatItemData>(DataEntryKey.Fuel, new Func<FloatItemData>(this.DefaultFuel)).Value;
		}
		set
		{
			base.GetData<FloatItemData>(DataEntryKey.Fuel, new Func<FloatItemData>(this.DefaultFuel)).Value = value;
			this.ropeFuel = value;
			if (this.ropeFuel <= 2f)
			{
				int num = (this.item.holderCharacter == null) ? -1 : this.item.holderCharacter.photonView.ViewID;
				this.photonView.RPC("Consume", RpcTarget.All, new object[]
				{
					num
				});
			}
			this.item.SetUseRemainingPercentage(this.ropeFuel / this.ropeStartFuel);
		}
	}

	// Token: 0x06000C54 RID: 3156 RVA: 0x000423C7 File Offset: 0x000405C7
	private FloatItemData DefaultFuel()
	{
		return new FloatItemData
		{
			Value = this.ropeStartFuel
		};
	}

	// Token: 0x170000EE RID: 238
	// (get) Token: 0x06000C55 RID: 3157 RVA: 0x000423DA File Offset: 0x000405DA
	// (set) Token: 0x06000C56 RID: 3158 RVA: 0x000423E2 File Offset: 0x000405E2
	public float Segments
	{
		get
		{
			return this.segments;
		}
		set
		{
			this.segments = value;
		}
	}

	// Token: 0x06000C57 RID: 3159 RVA: 0x000423EB File Offset: 0x000405EB
	public override void Awake()
	{
		base.Awake();
		this.ropeTier = base.GetComponent<RopeTier>();
		this.rig = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06000C58 RID: 3160 RVA: 0x0004240C File Offset: 0x0004060C
	private void OnDestroy()
	{
		if (this.item.itemState == ItemState.Held && this.photonView.IsMine)
		{
			this.ClearRope();
		}
		if (!this.photonView.IsMine)
		{
			return;
		}
		this.ropeFuel = this.RopeFuel;
		this.item.SetUseRemainingPercentage(this.ropeFuel / this.ropeStartFuel);
	}

	// Token: 0x06000C59 RID: 3161 RVA: 0x0004246C File Offset: 0x0004066C
	private void Update()
	{
		if (this.item.itemState != ItemState.Held || this.IsOutOfRope)
		{
			return;
		}
		if (!this.photonView.IsMine)
		{
			return;
		}
		if (this.ropeInstance == null && !this.IsOutOfRope)
		{
			this.ropeInstance = PhotonNetwork.Instantiate(this.ropePrefab.name, this.ropeBase.position, this.ropeBase.rotation, 0, null);
			this.rope = this.ropeInstance.GetComponent<Rope>();
			this.rope.photonView.RPC("AttachToSpool_Rpc", RpcTarget.AllBuffered, new object[]
			{
				this.photonView
			});
			this.Segments = 0f;
			this.segsVel = 0f;
			this.scroll = 0f;
			this.rope.Segments = this.Segments;
		}
		this.item.SetUseRemainingPercentage(((this.ropeFuel - this.rope.Segments) / this.ropeStartFuel).Clamp01());
		if (this.item.holderCharacter.input.scrollForwardIsPressed)
		{
			this.scroll = 0.4f;
		}
		else if (this.item.holderCharacter.input.scrollBackwardIsPressed)
		{
			this.scroll = -0.4f;
		}
		else
		{
			this.scroll = this.item.holderCharacter.input.scrollInput;
		}
		if (this.ropeTier.LookingToPlaceAnchor)
		{
			this.scroll = 0f;
			this.segsVel = 0f;
		}
	}

	// Token: 0x06000C5A RID: 3162 RVA: 0x00042600 File Offset: 0x00040800
	private void FixedUpdate()
	{
		this.segsVel = Mathf.Lerp(this.segsVel, this.scroll, Time.fixedDeltaTime * 4f);
		this.segsVel = Mathf.Clamp(this.segsVel, -1f, 5f);
		if (this.photonView.IsMine && this.rope != null)
		{
			this.Segments += this.segsVel * Time.fixedDeltaTime * 25f;
			this.Segments = Mathf.Clamp(this.Segments, this.minSegments, Mathf.Min(this.ropeFuel, (float)Rope.MaxSegments));
			float num = this.Segments - this.rope.Segments;
			this.ropeSpoolTf.transform.localEulerAngles += new Vector3(0f, 0f, num * -50f);
			this.rope.Segments = this.Segments;
		}
	}

	// Token: 0x06000C5B RID: 3163 RVA: 0x00042708 File Offset: 0x00040908
	public void ClearRope()
	{
		Debug.Log(string.Format("ClearRope{0}", this.ropeInstance));
		if (this.ropeInstance != null)
		{
			Debug.Log("Destroy rope");
			PhotonNetwork.Destroy(this.rope.view);
		}
		this.rope = null;
	}

	// Token: 0x06000C5C RID: 3164 RVA: 0x0004275C File Offset: 0x0004095C
	public override void OnInstanceDataSet()
	{
		if (base.HasData(DataEntryKey.Fuel))
		{
			Debug.Log("HasData");
			this.ropeFuel = base.GetData<FloatItemData>(DataEntryKey.Fuel).Value;
			Debug.Log(string.Format("ropeFuel {0}", this.ropeFuel));
		}
	}

	// Token: 0x04000B3E RID: 2878
	public float segments;

	// Token: 0x04000B3F RID: 2879
	public float minSegments = 3.5f;

	// Token: 0x04000B40 RID: 2880
	public float ropeStartFuel = 60f;

	// Token: 0x04000B41 RID: 2881
	private float ropeFuel = 60f;

	// Token: 0x04000B42 RID: 2882
	public GameObject ropePrefab;

	// Token: 0x04000B43 RID: 2883
	public Transform ropeBase;

	// Token: 0x04000B44 RID: 2884
	public Transform ropeStart;

	// Token: 0x04000B45 RID: 2885
	public Transform ropeSpoolTf;

	// Token: 0x04000B46 RID: 2886
	public GameObject ropeInstance;

	// Token: 0x04000B47 RID: 2887
	public Rigidbody rig;

	// Token: 0x04000B48 RID: 2888
	public Rope rope;

	// Token: 0x04000B49 RID: 2889
	private float scroll;

	// Token: 0x04000B4A RID: 2890
	private float segsVel;

	// Token: 0x04000B4B RID: 2891
	private RopeTier ropeTier;

	// Token: 0x04000B4C RID: 2892
	public bool isAntiRope;
}

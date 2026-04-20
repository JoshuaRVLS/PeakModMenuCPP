using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000159 RID: 345
public class ItemScaleSyncer : ItemComponent
{
	// Token: 0x06000B6E RID: 2926 RVA: 0x0003D544 File Offset: 0x0003B744
	public override void Awake()
	{
		base.Awake();
		this.item.forceScale = false;
	}

	// Token: 0x06000B6F RID: 2927 RVA: 0x0003D558 File Offset: 0x0003B758
	public void Start()
	{
		this.InitScale();
	}

	// Token: 0x06000B70 RID: 2928 RVA: 0x0003D560 File Offset: 0x0003B760
	public void InitScale()
	{
		if (this._isInitialized)
		{
			return;
		}
		this._isInitialized = true;
		FloatItemData floatItemData;
		if (this.item.HasData(DataEntryKey.Scale) && this.item.data.TryGetDataEntry<FloatItemData>(DataEntryKey.Scale, out floatItemData))
		{
			this.currentScale = floatItemData.Value;
			this.ApplyScale(this.currentScale);
		}
		else
		{
			this.currentScale = base.transform.localScale.x;
		}
		this.previousScale = this.currentScale;
	}

	// Token: 0x06000B71 RID: 2929 RVA: 0x0003D5E0 File Offset: 0x0003B7E0
	private void ApplyScale(float scale)
	{
		Vector3 localScale = scale * Vector3.one;
		if (this._applyDirectlyToMeshAndCollider)
		{
			this.item.mainRenderer.transform.localScale = localScale;
			this.item.colliders[0].transform.localScale = localScale;
			return;
		}
		this.item.transform.localScale = localScale;
	}

	// Token: 0x06000B72 RID: 2930 RVA: 0x0003D641 File Offset: 0x0003B841
	public override void OnInstanceDataSet()
	{
	}

	// Token: 0x06000B73 RID: 2931 RVA: 0x0003D644 File Offset: 0x0003B844
	public void Update()
	{
		if (this.photonView.IsMine && Mathf.Abs(this.currentScale - this.previousScale) > 0.01f)
		{
			this.OnScaleChanged();
		}
		float scale = (this.item.itemState == ItemState.InBackpack) ? (this.currentScale * 0.5f) : this.currentScale;
		this.ApplyScale(scale);
	}

	// Token: 0x06000B74 RID: 2932 RVA: 0x0003D6A8 File Offset: 0x0003B8A8
	private void OnScaleChanged()
	{
		if (!this.photonView.IsMine)
		{
			return;
		}
		this.photonView.RPC("RPC_SyncScale", RpcTarget.All, new object[]
		{
			this.currentScale
		});
		this.previousScale = this.currentScale;
	}

	// Token: 0x06000B75 RID: 2933 RVA: 0x0003D6F4 File Offset: 0x0003B8F4
	[PunRPC]
	private void RPC_SyncScale(float scale)
	{
		FloatItemData floatItemData;
		if (this.item.HasData(DataEntryKey.Scale) && this.item.data.TryGetDataEntry<FloatItemData>(DataEntryKey.Scale, out floatItemData))
		{
			floatItemData.Value = this.currentScale;
		}
		else
		{
			this.item.data.RegisterEntry<FloatItemData>(DataEntryKey.Scale, new FloatItemData
			{
				Value = this.currentScale
			});
		}
		this.currentScale = scale;
	}

	// Token: 0x04000A81 RID: 2689
	private bool _isInitialized;

	// Token: 0x04000A82 RID: 2690
	[SerializeField]
	private bool _applyDirectlyToMeshAndCollider;

	// Token: 0x04000A83 RID: 2691
	public float currentScale = 1f;

	// Token: 0x04000A84 RID: 2692
	private float previousScale;
}

using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200011E RID: 286
public abstract class ItemComponent : MonoBehaviourPunCallbacks
{
	// Token: 0x06000957 RID: 2391 RVA: 0x00032585 File Offset: 0x00030785
	public virtual void Awake()
	{
		this.item = base.GetComponent<Item>();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000958 RID: 2392 RVA: 0x0003259F File Offset: 0x0003079F
	public T GetData<T>(DataEntryKey key) where T : DataEntryValue, new()
	{
		return this.item.GetData<T>(key);
	}

	// Token: 0x06000959 RID: 2393 RVA: 0x000325AD File Offset: 0x000307AD
	public T GetData<T>(DataEntryKey key, Func<T> getNew) where T : DataEntryValue, new()
	{
		return this.item.GetData<T>(key, getNew);
	}

	// Token: 0x0600095A RID: 2394 RVA: 0x000325BC File Offset: 0x000307BC
	public bool HasData(DataEntryKey key)
	{
		return this.item.data != null && this.item.data.HasData(key);
	}

	// Token: 0x0600095B RID: 2395
	public abstract void OnInstanceDataSet();

	// Token: 0x0600095C RID: 2396 RVA: 0x000325DE File Offset: 0x000307DE
	public void ForceSync()
	{
		if (!this.photonView.IsMine)
		{
			Debug.LogError("Not allowed to force sync an object you don't own..");
			return;
		}
		this.photonView.RPC("SetItemInstanceDataRPC", RpcTarget.Others, new object[]
		{
			this.item.data
		});
	}

	// Token: 0x040008C2 RID: 2242
	[NonSerialized]
	public Item item;

	// Token: 0x040008C3 RID: 2243
	protected new PhotonView photonView;
}

using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000CF RID: 207
public class ItemBackpackVisuals : BackpackVisuals
{
	// Token: 0x0600081E RID: 2078 RVA: 0x0002DED3 File Offset: 0x0002C0D3
	private void Awake()
	{
		this.item = base.GetComponent<Item>();
	}

	// Token: 0x0600081F RID: 2079 RVA: 0x0002DEE1 File Offset: 0x0002C0E1
	public override BackpackData GetBackpackData()
	{
		return base.GetComponent<Item>().GetData<BackpackData>(DataEntryKey.BackpackData);
	}

	// Token: 0x06000820 RID: 2080 RVA: 0x0002DEEF File Offset: 0x0002C0EF
	protected override void PutItemInBackpack(GameObject visual, byte slotID)
	{
		visual.GetComponent<PhotonView>().RPC("PutInBackpackRPC", RpcTarget.All, new object[]
		{
			slotID,
			BackpackReference.GetFromBackpackItem(this.item)
		});
	}

	// Token: 0x040007E1 RID: 2017
	private Item item;
}

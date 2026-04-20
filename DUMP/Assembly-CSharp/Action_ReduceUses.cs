using System;
using Photon.Pun;

// Token: 0x020000F8 RID: 248
public class Action_ReduceUses : ItemAction
{
	// Token: 0x060008A5 RID: 2213 RVA: 0x0002FB09 File Offset: 0x0002DD09
	public override void RunAction()
	{
		this.item.photonView.RPC("ReduceUsesRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x060008A6 RID: 2214 RVA: 0x0002FB28 File Offset: 0x0002DD28
	[PunRPC]
	public void ReduceUsesRPC()
	{
		OptionableIntItemData data = this.item.GetData<OptionableIntItemData>(DataEntryKey.ItemUses);
		if (data.HasData && data.Value > 0)
		{
			data.Value--;
			if (this.item.totalUses > 0)
			{
				this.item.SetUseRemainingPercentage((float)data.Value / (float)this.item.totalUses);
			}
			if (data.Value == 0 && this.consumeOnFullyUsed && base.character && base.character.IsLocal && base.character.data.currentItem == this.item)
			{
				this.item.StartCoroutine(this.item.ConsumeDelayed(false));
			}
		}
	}

	// Token: 0x0400083E RID: 2110
	public bool consumeOnFullyUsed;
}

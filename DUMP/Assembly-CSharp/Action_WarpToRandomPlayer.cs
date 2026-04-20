using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000103 RID: 259
public class Action_WarpToRandomPlayer : ItemAction
{
	// Token: 0x060008C6 RID: 2246 RVA: 0x00030200 File Offset: 0x0002E400
	public override void RunAction()
	{
		for (int i = 0; i < this.warpSFX.Length; i++)
		{
			this.warpSFX[i].Play(default(Vector3));
		}
		List<Character> list = new List<Character>();
		foreach (Character character in Character.AllCharacters)
		{
			if (!(character == base.character) && !character.data.dead && Vector3.Distance(base.character.Center, character.Center) > this.minimumDistance)
			{
				list.Add(character);
			}
		}
		if (list.Count == 0 && this.restoreUsesOnFailure)
		{
			this.item.photonView.RPC("IncreaseUsesRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		Vector3 center = list.RandomSelection((Character c) => 1).Center;
		base.character.photonView.RPC("WarpPlayerRPC", RpcTarget.All, new object[]
		{
			center,
			true
		});
	}

	// Token: 0x060008C7 RID: 2247 RVA: 0x00030348 File Offset: 0x0002E548
	[PunRPC]
	public void IncreaseUsesRPC()
	{
		OptionableIntItemData data = this.item.GetData<OptionableIntItemData>(DataEntryKey.ItemUses);
		if (data.HasData && data.Value != -1)
		{
			data.Value++;
			if (this.item.totalUses > 0)
			{
				this.item.SetUseRemainingPercentage((float)data.Value / (float)this.item.totalUses);
			}
		}
	}

	// Token: 0x0400085A RID: 2138
	public float minimumDistance = 12f;

	// Token: 0x0400085B RID: 2139
	public bool restoreUsesOnFailure = true;

	// Token: 0x0400085C RID: 2140
	public SFX_Instance[] warpSFX;
}

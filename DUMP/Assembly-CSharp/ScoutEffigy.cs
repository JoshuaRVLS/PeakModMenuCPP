using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200012D RID: 301
public class ScoutEffigy : Constructable
{
	// Token: 0x060009C3 RID: 2499 RVA: 0x00033F08 File Offset: 0x00032108
	protected override void Update()
	{
		if (this.item.holderCharacter)
		{
			if (!Character.PlayerIsDeadOrDown())
			{
				this.item.overrideUsability = Optionable<bool>.Some(false);
			}
			else
			{
				this.item.overrideUsability = Optionable<bool>.None;
			}
		}
		base.Update();
	}

	// Token: 0x060009C4 RID: 2500 RVA: 0x00033F58 File Offset: 0x00032158
	public override GameObject FinishConstruction()
	{
		if (!this.constructing)
		{
			return null;
		}
		if (this.currentPreview == null)
		{
			return null;
		}
		List<Character> list = new List<Character>();
		foreach (Character character in Character.AllCharacters)
		{
			if (character.data.dead || character.data.fullyPassedOut)
			{
				list.Add(character);
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		list.RandomSelection((Character c) => 1).photonView.RPC("RPCA_ReviveAtPosition", RpcTarget.All, new object[]
		{
			this.currentConstructHit.point + Vector3.up * 1f,
			false,
			-1
		});
		if (Singleton<AchievementManager>.Instance)
		{
			Singleton<AchievementManager>.Instance.AddToRunBasedInt(RUNBASEDVALUETYPE.ScoutsResurrected, 1);
		}
		return null;
	}
}

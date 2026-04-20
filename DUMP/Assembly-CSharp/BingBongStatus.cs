using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200021C RID: 540
public class BingBongStatus : MonoBehaviour
{
	// Token: 0x06001088 RID: 4232 RVA: 0x000522CB File Offset: 0x000504CB
	private void OnEnable()
	{
		this.bingBongPowers = base.GetComponent<BingBongPowers>();
		this.bingBongPowers.SetTexts("STATUS", this.descr);
	}

	// Token: 0x06001089 RID: 4233 RVA: 0x000522EF File Offset: 0x000504EF
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x0600108A RID: 4234 RVA: 0x00052300 File Offset: 0x00050500
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F))
		{
			this.allStatusSelected = true;
			this.bingBongPowers.SetTip("Status: All", 2);
		}
		string[] names = Enum.GetNames(typeof(CharacterAfflictions.STATUSTYPE));
		int num = names.Length;
		int num2 = (int)this.currentStatusTarget;
		if (Input.GetKeyDown(KeyCode.V))
		{
			this.allStatusSelected = false;
			num2--;
		}
		if (Input.GetKeyDown(KeyCode.C))
		{
			this.allStatusSelected = false;
			num2++;
		}
		if (num2 < 0)
		{
			num2 = num - 1;
		}
		if (num2 >= num)
		{
			num2 = 0;
		}
		if (this.currentStatusTarget != (CharacterAfflictions.STATUSTYPE)num2)
		{
			this.currentStatusTarget = (CharacterAfflictions.STATUSTYPE)num2;
			this.bingBongPowers.SetTip(names[num2] ?? "", 2);
		}
		Character target = this.GetTarget();
		if (target)
		{
			if (Input.GetKeyDown(KeyCode.Mouse0))
			{
				this.view.RPC("RPCA_AddStatusBingBing", RpcTarget.All, new object[]
				{
					target.photonView.ViewID,
					(int)(this.allStatusSelected ? ((CharacterAfflictions.STATUSTYPE)(-1)) : this.currentStatusTarget),
					1
				});
			}
			if (Input.GetKeyDown(KeyCode.Mouse1))
			{
				this.view.RPC("RPCA_AddStatusBingBing", RpcTarget.All, new object[]
				{
					target.photonView.ViewID,
					(int)(this.allStatusSelected ? ((CharacterAfflictions.STATUSTYPE)(-1)) : this.currentStatusTarget),
					-1
				});
			}
		}
	}

	// Token: 0x0600108B RID: 4235 RVA: 0x0005246C File Offset: 0x0005066C
	[PunRPC]
	public void RPCA_AddStatusBingBing(int target, int statusID, int mult)
	{
		Character component = PhotonView.Find(target).GetComponent<Character>();
		if (component.IsLocal)
		{
			if (mult > 0)
			{
				if (statusID == -1)
				{
					component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, 1f, false, true, true);
					component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Hunger, 1f, false, true, true);
					component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Cold, 1f, false, true, true);
					component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Poison, 1f, false, true, true);
					component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Curse, 1f, false, true, true);
					component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Drowsy, 1f, false, true, true);
					component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Weight, 1f, false, true, true);
					component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Hot, 1f, false, true, true);
					return;
				}
				component.refs.afflictions.AddStatus(this.currentStatusTarget, 0.2f, false, true, true);
				return;
			}
			else
			{
				if (statusID == -1)
				{
					component.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Injury, 1f, false, false);
					component.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Hunger, 1f, false, false);
					component.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Cold, 1f, false, false);
					component.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Poison, 1f, false, false);
					component.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Curse, 1f, false, false);
					component.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Drowsy, 1f, false, false);
					component.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Weight, 1f, false, false);
					component.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Hot, 1f, false, false);
					return;
				}
				component.refs.afflictions.SubtractStatus(this.currentStatusTarget, 0.2f, false, false);
			}
		}
	}

	// Token: 0x0600108C RID: 4236 RVA: 0x00052674 File Offset: 0x00050874
	private Character GetTarget()
	{
		Character result = null;
		float num = float.MaxValue;
		foreach (Character character in Character.AllCharacters)
		{
			float num2 = Vector3.Angle(MainCamera.instance.transform.forward, character.Center - MainCamera.instance.transform.position);
			if (num2 < num)
			{
				num = num2;
				result = character;
			}
		}
		return result;
	}

	// Token: 0x04000E79 RID: 3705
	private BingBongPowers bingBongPowers;

	// Token: 0x04000E7A RID: 3706
	private string descr = "Add status: [LMB]\n\nRemove status: [RMB]\n\nSelect all status: [F]\n\nPrev status: [V]\n\nNext status: [C]\n\n";

	// Token: 0x04000E7B RID: 3707
	private PhotonView view;

	// Token: 0x04000E7C RID: 3708
	private bool allStatusSelected;

	// Token: 0x04000E7D RID: 3709
	private CharacterAfflictions.STATUSTYPE currentStatusTarget;
}

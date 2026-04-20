using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000071 RID: 113
public class CharacterBackpackHandler : MonoBehaviour
{
	// Token: 0x06000527 RID: 1319 RVA: 0x0001E5D8 File Offset: 0x0001C7D8
	private void Awake()
	{
		this.character = base.GetComponent<Character>();
		this.characterItems = base.GetComponent<CharacterItems>();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000528 RID: 1320 RVA: 0x0001E600 File Offset: 0x0001C800
	private void LateUpdate()
	{
		bool flag = !this.character.player.backpackSlot.IsEmpty();
		bool flag2 = this.characterItems.currentSelectedSlot.IsSome && this.characterItems.currentSelectedSlot.Value == 3;
		bool flag3 = flag && !flag2;
		bool active = flag3;
		if (this.character.photonView.IsMine && !MainCameraMovement.IsSpectating)
		{
			active = false;
		}
		this.backpack.SetActive(active);
		if (flag3)
		{
			if (!this.t)
			{
				for (int i = 0; i < this.wearSFX.Length; i++)
				{
					this.wearSFX[i].Play(this.character.refs.hip.transform.position);
				}
			}
			this.t = true;
		}
		else
		{
			this.t = false;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			if (!this.lastShow && flag3)
			{
				base.StartCoroutine(this.RefreshVisualsDelayed());
			}
			else if (this.lastShow && !flag3)
			{
				this.backpackVisuals.RemoveVisuals();
			}
		}
		this.lastShow = flag3;
	}

	// Token: 0x06000529 RID: 1321 RVA: 0x0001E716 File Offset: 0x0001C916
	private IEnumerator RefreshVisualsDelayed()
	{
		yield return null;
		this.backpackVisuals.RefreshVisuals();
		yield break;
	}

	// Token: 0x0600052A RID: 1322 RVA: 0x0001E728 File Offset: 0x0001C928
	public void StashInBackpack(Character interactor, byte backpackSlotID)
	{
		CharacterItems items = interactor.refs.items;
		if (items.currentSelectedSlot.IsNone)
		{
			Debug.LogError("Need item slot selected to stash item in backpack!");
			return;
		}
		ItemSlot itemSlot = interactor.player.GetItemSlot(items.currentSelectedSlot.Value);
		if (itemSlot == null)
		{
			Debug.LogError(string.Format("Slot ID {0} is invalid!", items.currentSelectedSlot.Value));
		}
		if (itemSlot.IsEmpty())
		{
			Debug.LogError(string.Format("Item slot {0} is empty!", itemSlot.itemSlotID));
			return;
		}
		this.photonView.RPC("RPCAddItemToCharacterBackpack", RpcTarget.All, new object[]
		{
			interactor.player.GetComponent<PhotonView>(),
			items.currentSelectedSlot.Value,
			backpackSlotID
		});
		interactor.player.EmptySlot(items.currentSelectedSlot);
		items.EquipSlot(Optionable<byte>.None);
	}

	// Token: 0x0600052B RID: 1323 RVA: 0x0001E814 File Offset: 0x0001CA14
	[PunRPC]
	public void RPCAddItemToCharacterBackpack(PhotonView playerView, byte inventorySlotID, byte backpackSlotID)
	{
		BackpackData backpackData;
		if (!this.character.player.backpackSlot.data.TryGetDataEntry<BackpackData>(DataEntryKey.BackpackData, out backpackData))
		{
			backpackData = this.character.player.backpackSlot.data.RegisterNewEntry<BackpackData>(DataEntryKey.BackpackData);
		}
		ItemSlot itemSlot = playerView.GetComponent<Player>().GetItemSlot(inventorySlotID);
		if (itemSlot == null)
		{
			Debug.LogError(string.Format("Slot ID {0} is invalid!", inventorySlotID));
			return;
		}
		backpackData.AddItem(itemSlot.prefab, itemSlot.data, backpackSlotID);
		if (PhotonNetwork.IsMasterClient)
		{
			this.backpackVisuals.RefreshVisuals();
		}
		if (this.character.IsLocal)
		{
			this.character.refs.afflictions.UpdateWeight();
		}
	}

	// Token: 0x04000578 RID: 1400
	private Character character;

	// Token: 0x04000579 RID: 1401
	private CharacterItems characterItems;

	// Token: 0x0400057A RID: 1402
	private PhotonView photonView;

	// Token: 0x0400057B RID: 1403
	public BackpackOnBackVisuals backpackVisuals;

	// Token: 0x0400057C RID: 1404
	public GameObject backpack;

	// Token: 0x0400057D RID: 1405
	private bool lastShow;

	// Token: 0x0400057E RID: 1406
	public SFX_Instance[] wearSFX;

	// Token: 0x0400057F RID: 1407
	private bool t;
}

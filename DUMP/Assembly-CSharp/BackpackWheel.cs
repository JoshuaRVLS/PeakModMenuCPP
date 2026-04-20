using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zorro.Core;

// Token: 0x020001BE RID: 446
public class BackpackWheel : UIWheel
{
	// Token: 0x06000E45 RID: 3653 RVA: 0x000479FC File Offset: 0x00045BFC
	public void InitWheel(BackpackReference bp)
	{
		this.backpack = bp;
		this.chosenSlice = Optionable<BackpackWheelSlice.SliceData>.None;
		this.chosenItemText.text = "";
		ItemSlot[] itemSlots = this.backpack.GetData().itemSlots;
		byte b = 0;
		while ((int)b < itemSlots.Length)
		{
			this.slices[(int)(b + 1)].InitItemSlot(new ValueTuple<BackpackReference, byte>(bp, b), this);
			b += 1;
		}
		base.gameObject.SetActive(true);
		this.slices[0].InitPickupBackpack(bp, this);
		if (Character.localCharacter.data.currentItem)
		{
			this.currentlyHeldItem.texture = Character.localCharacter.data.currentItem.UIData.GetIcon();
			this.UpdateCookedAmount(Character.localCharacter.data.currentItem);
			this.currentlyHeldItem.enabled = true;
			return;
		}
		this.UpdateCookedAmount(null);
		this.currentlyHeldItem.enabled = false;
	}

	// Token: 0x06000E46 RID: 3654 RVA: 0x00047AEC File Offset: 0x00045CEC
	private void UpdateCookedAmount(Item item)
	{
		if (item == null || item.data == null)
		{
			this.currentlyHeldItemCookedAmount = 0;
			this.currentlyHeldItem.color = Color.white;
			return;
		}
		IntItemData intItemData;
		if (item.data.TryGetDataEntry<IntItemData>(DataEntryKey.CookedAmount, out intItemData) && this.currentlyHeldItemCookedAmount != intItemData.Value)
		{
			this.currentlyHeldItem.color = Color.white;
			this.currentlyHeldItem.color = ItemCooking.GetCookColor(intItemData.Value);
			this.currentlyHeldItemCookedAmount = intItemData.Value;
		}
	}

	// Token: 0x06000E47 RID: 3655 RVA: 0x00047B74 File Offset: 0x00045D74
	protected override void Update()
	{
		if (!Character.localCharacter.input.interactIsPressed)
		{
			this.Choose();
			GUIManager.instance.CloseBackpackWheel();
			return;
		}
		if (this.backpack.locationTransform != null && Vector3.Distance(this.backpack.locationTransform.position, Character.localCharacter.Center) > 6f)
		{
			GUIManager.instance.CloseBackpackWheel();
			return;
		}
		if (this.chosenSlice.IsSome && !this.chosenSlice.Value.isBackpackWear && !this.slices[(int)(this.chosenSlice.Value.slotID + 1)].image.enabled)
		{
			this.currentlyHeldItem.transform.position = Vector3.Lerp(this.currentlyHeldItem.transform.position, this.slices[(int)(this.chosenSlice.Value.slotID + 1)].transform.GetChild(0).GetChild(0).position, Time.deltaTime * 20f);
		}
		else
		{
			this.currentlyHeldItem.transform.localPosition = Vector3.Lerp(this.currentlyHeldItem.transform.localPosition, Vector3.zero, Time.deltaTime * 20f);
		}
		base.Update();
	}

	// Token: 0x06000E48 RID: 3656 RVA: 0x00047CD0 File Offset: 0x00045ED0
	public void Choose()
	{
		if (this.chosenSlice.IsSome)
		{
			Debug.Log(string.Format("Chose slice {0}", this.chosenSlice.Value.slotID));
			if (this.chosenSlice.Value.isBackpackWear)
			{
				BackpackWheelSlice.SliceData value = this.chosenSlice.Value;
				Backpack backpack;
				if (value.backpackReference.TryGetBackpackItem(out backpack))
				{
					backpack.Wear(Character.localCharacter);
					return;
				}
			}
			else
			{
				if (this.chosenSlice.Value.isStashSlice)
				{
					this.TryStash(this.chosenSlice.Value.slotID);
					return;
				}
				BackpackWheelSlice.SliceData value = this.chosenSlice.Value;
				Item item;
				if (value.backpackReference.GetVisuals().TryGetSpawnedItem(this.chosenSlice.Value.slotID, out item))
				{
					item.Interact(Character.localCharacter);
					return;
				}
				if (Character.localCharacter.data.currentItem)
				{
					this.TryStash(this.chosenSlice.Value.slotID);
				}
			}
		}
	}

	// Token: 0x06000E49 RID: 3657 RVA: 0x00047DE0 File Offset: 0x00045FE0
	private void TryStash(byte backpackSlotID)
	{
		Backpack backpack;
		if (this.backpack.TryGetBackpackItem(out backpack))
		{
			backpack.Stash(Character.localCharacter, backpackSlotID);
			return;
		}
		this.backpack.view.GetComponent<CharacterBackpackHandler>().StashInBackpack(Character.localCharacter, backpackSlotID);
	}

	// Token: 0x06000E4A RID: 3658 RVA: 0x00047E24 File Offset: 0x00046024
	public void Hover(BackpackWheelSlice.SliceData sliceData)
	{
		if (sliceData.isBackpackWear)
		{
			if (sliceData.backpackReference.type == BackpackReference.BackpackType.Equipped)
			{
				return;
			}
			this.chosenItemText.text = LocalizedText.GetText("WEARBACKPACK", true);
			this.chosenSlice = Optionable<BackpackWheelSlice.SliceData>.Some(sliceData);
			return;
		}
		else
		{
			if (!sliceData.isStashSlice)
			{
				ItemSlot itemSlot = this.backpack.GetData().itemSlots[(int)sliceData.slotID];
				bool flag = false;
				if (itemSlot.IsEmpty() && Character.localCharacter.data.currentItem)
				{
					if (Character.localCharacter.data.currentItem)
					{
						this.chosenItemText.text = LocalizedText.GetText("STASHITEM", true).Replace("#", Character.localCharacter.data.currentItem.GetItemName(null));
						flag = true;
					}
				}
				else
				{
					Item prefab = itemSlot.prefab;
					if (prefab != null)
					{
						this.chosenItemText.text = LocalizedText.GetText("TAKEITEM", true).Replace("#", prefab.GetItemName(itemSlot.data));
						flag = true;
					}
				}
				if (flag)
				{
					this.chosenSlice = Optionable<BackpackWheelSlice.SliceData>.Some(sliceData);
				}
				return;
			}
			Item currentItem = Character.localCharacter.data.currentItem;
			if (currentItem != null)
			{
				this.chosenItemText.text = LocalizedText.GetText("STASHITEM", true).Replace("#", currentItem.GetItemName(null));
				this.chosenSlice = Optionable<BackpackWheelSlice.SliceData>.Some(sliceData);
				return;
			}
			this.chosenItemText.text = "";
			this.chosenSlice = Optionable<BackpackWheelSlice.SliceData>.None;
			return;
		}
	}

	// Token: 0x06000E4B RID: 3659 RVA: 0x00047FB0 File Offset: 0x000461B0
	public void Dehover(BackpackWheelSlice.SliceData sliceData)
	{
		if (this.chosenSlice.IsSome && this.chosenSlice.Value.Equals(sliceData))
		{
			this.chosenItemText.text = "";
			this.chosenSlice = Optionable<BackpackWheelSlice.SliceData>.None;
		}
	}

	// Token: 0x06000E4C RID: 3660 RVA: 0x00047FFC File Offset: 0x000461FC
	protected override void TestSelectSliceGamepad(Vector2 gamepadVector)
	{
		float num = 0f;
		BackpackWheelSlice backpackWheelSlice = null;
		if (gamepadVector.sqrMagnitude >= 0.5f)
		{
			for (int i = 0; i < this.slices.Length; i++)
			{
				float num2 = Vector3.Angle(gamepadVector, this.slices[i].GetUpVector());
				if (backpackWheelSlice == null || num2 < num)
				{
					backpackWheelSlice = this.slices[i];
					num = num2;
				}
			}
		}
		if (backpackWheelSlice != null)
		{
			EventSystem.current.SetSelectedGameObject(backpackWheelSlice.button.gameObject);
			backpackWheelSlice.Hover();
			return;
		}
		EventSystem.current.SetSelectedGameObject(null);
		this.Dehover(this.chosenSlice.Value);
	}

	// Token: 0x04000C03 RID: 3075
	public BackpackWheelSlice[] slices;

	// Token: 0x04000C04 RID: 3076
	public TextMeshProUGUI chosenItemText;

	// Token: 0x04000C05 RID: 3077
	public Optionable<BackpackWheelSlice.SliceData> chosenSlice;

	// Token: 0x04000C06 RID: 3078
	public BackpackReference backpack;

	// Token: 0x04000C07 RID: 3079
	public RawImage currentlyHeldItem;

	// Token: 0x04000C08 RID: 3080
	private int currentlyHeldItemCookedAmount;
}

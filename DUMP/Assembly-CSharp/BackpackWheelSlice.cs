using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001BF RID: 447
public class BackpackWheelSlice : UIWheelSlice, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x17000102 RID: 258
	// (get) Token: 0x06000E4E RID: 3662 RVA: 0x000480AC File Offset: 0x000462AC
	// (set) Token: 0x06000E4F RID: 3663 RVA: 0x000480B4 File Offset: 0x000462B4
	public byte backpackSlot { get; private set; }

	// Token: 0x06000E50 RID: 3664 RVA: 0x000480BD File Offset: 0x000462BD
	private void UpdateInteractable()
	{
		this.button.interactable = this.canInteract;
	}

	// Token: 0x17000103 RID: 259
	// (get) Token: 0x06000E51 RID: 3665 RVA: 0x000480D0 File Offset: 0x000462D0
	private bool canInteract
	{
		get
		{
			return this.isBackpackWear || this.stashSlice || this.hasItem || (Character.localCharacter.data.currentItem != null && Character.localCharacter.data.currentItem.UIData.canBackpack);
		}
	}

	// Token: 0x06000E52 RID: 3666 RVA: 0x00048130 File Offset: 0x00046330
	public void InitItemSlot([TupleElementNames(new string[]
	{
		null,
		"slotID"
	})] ValueTuple<BackpackReference, byte> slot, BackpackWheel wheel)
	{
		this.SharedInit(slot.Item1, wheel);
		this.backpackSlot = slot.Item2;
		this.backpackData = this.backpack.GetData();
		this.itemSlot = this.backpackData.itemSlots[(int)this.backpackSlot];
		Item prefab = this.itemSlot.prefab;
		this.SetItemIcon(prefab, this.itemSlot.data);
		this.UpdateInteractable();
	}

	// Token: 0x06000E53 RID: 3667 RVA: 0x000481A5 File Offset: 0x000463A5
	public void InitPickupBackpack(BackpackReference backpack, BackpackWheel wheel)
	{
		this.backpackSlot = byte.MaxValue;
		this.SharedInit(backpack, wheel);
		this.UpdateInteractable();
	}

	// Token: 0x06000E54 RID: 3668 RVA: 0x000481C0 File Offset: 0x000463C0
	public void InitStashSlot(BackpackReference bpRef, BackpackWheel wheel)
	{
		this.backpack = bpRef;
		this.backpackWheel = wheel;
		this.SetItemIcon(Character.localCharacter.data.currentItem, (Character.localCharacter.data.currentItem != null) ? Character.localCharacter.data.currentItem.data : null);
		this.UpdateInteractable();
	}

	// Token: 0x06000E55 RID: 3669 RVA: 0x00048224 File Offset: 0x00046424
	private void SharedInit(BackpackReference bpRef, BackpackWheel wheel)
	{
		this.backpack = bpRef;
		this.backpackWheel = wheel;
		if (bpRef.type == BackpackReference.BackpackType.Item)
		{
			Backpack component = Resources.Load<GameObject>("0_Items/Backpack").GetComponent<Backpack>();
			if (this.backpackSlot == 255)
			{
				base.gameObject.SetActive(true);
			}
			this.SetItemIcon(component, null);
			return;
		}
		this.SetItemIcon(null, null);
		if (this.backpackSlot == 255)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000E56 RID: 3670 RVA: 0x0004829C File Offset: 0x0004649C
	private void SetItemIcon(Item iconHolder, ItemInstanceData itemInstanceData)
	{
		if (iconHolder == null)
		{
			this.image.enabled = false;
			this.hasItem = false;
		}
		else
		{
			this.image.enabled = true;
			this.image.texture = iconHolder.UIData.GetIcon();
			this.hasItem = true;
		}
		this.UpdateCookedAmount(iconHolder, itemInstanceData);
	}

	// Token: 0x06000E57 RID: 3671 RVA: 0x000482F8 File Offset: 0x000464F8
	private void UpdateCookedAmount(Item item, ItemInstanceData itemInstanceData)
	{
		if (item == null || itemInstanceData == null)
		{
			this.cookedAmount = 0;
			this.image.color = Color.white;
			return;
		}
		IntItemData intItemData;
		if (itemInstanceData.TryGetDataEntry<IntItemData>(DataEntryKey.CookedAmount, out intItemData) && this.cookedAmount != intItemData.Value)
		{
			this.image.color = Color.white;
			this.image.color = ItemCooking.GetCookColor(intItemData.Value);
			this.cookedAmount = intItemData.Value;
		}
	}

	// Token: 0x17000104 RID: 260
	// (get) Token: 0x06000E58 RID: 3672 RVA: 0x00048374 File Offset: 0x00046574
	public bool isBackpackWear
	{
		get
		{
			return this.backpackSlot == byte.MaxValue;
		}
	}

	// Token: 0x06000E59 RID: 3673 RVA: 0x00048384 File Offset: 0x00046584
	public void Hover()
	{
		if (!this.canInteract)
		{
			return;
		}
		BackpackWheelSlice.SliceData sliceData = new BackpackWheelSlice.SliceData
		{
			isBackpackWear = this.isBackpackWear,
			isStashSlice = this.stashSlice,
			backpackReference = this.backpack,
			slotID = this.backpackSlot
		};
		this.backpackWheel.Hover(sliceData);
	}

	// Token: 0x06000E5A RID: 3674 RVA: 0x000483E4 File Offset: 0x000465E4
	public void Dehover()
	{
		BackpackWheelSlice.SliceData sliceData = new BackpackWheelSlice.SliceData
		{
			isBackpackWear = (this.backpackSlot == byte.MaxValue),
			isStashSlice = this.stashSlice,
			backpackReference = this.backpack,
			slotID = this.backpackSlot
		};
		this.backpackWheel.Dehover(sliceData);
	}

	// Token: 0x06000E5B RID: 3675 RVA: 0x00048442 File Offset: 0x00046642
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.Hover();
	}

	// Token: 0x06000E5C RID: 3676 RVA: 0x0004844A File Offset: 0x0004664A
	public void OnPointerExit(PointerEventData eventData)
	{
		this.Dehover();
	}

	// Token: 0x04000C09 RID: 3081
	private BackpackWheel backpackWheel;

	// Token: 0x04000C0B RID: 3083
	private BackpackReference backpack;

	// Token: 0x04000C0C RID: 3084
	private BackpackData backpackData;

	// Token: 0x04000C0D RID: 3085
	private ItemSlot itemSlot;

	// Token: 0x04000C0E RID: 3086
	public RawImage image;

	// Token: 0x04000C0F RID: 3087
	public bool stashSlice;

	// Token: 0x04000C10 RID: 3088
	private int cookedAmount;

	// Token: 0x04000C11 RID: 3089
	private bool hasItem;

	// Token: 0x020004D1 RID: 1233
	public struct SliceData : IEquatable<BackpackWheelSlice.SliceData>
	{
		// Token: 0x06001DA1 RID: 7585 RVA: 0x0008A880 File Offset: 0x00088A80
		public bool Equals(BackpackWheelSlice.SliceData other)
		{
			return this.isBackpackWear == other.isBackpackWear && this.slotID == other.slotID;
		}

		// Token: 0x06001DA2 RID: 7586 RVA: 0x0008A8A0 File Offset: 0x00088AA0
		public override bool Equals(object obj)
		{
			if (obj is BackpackWheelSlice.SliceData)
			{
				BackpackWheelSlice.SliceData other = (BackpackWheelSlice.SliceData)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06001DA3 RID: 7587 RVA: 0x0008A8C5 File Offset: 0x00088AC5
		public override int GetHashCode()
		{
			return HashCode.Combine<bool, BackpackReference, byte>(this.isBackpackWear, this.backpackReference, this.slotID);
		}

		// Token: 0x04001B2C RID: 6956
		public bool isBackpackWear;

		// Token: 0x04001B2D RID: 6957
		public bool isStashSlice;

		// Token: 0x04001B2E RID: 6958
		public BackpackReference backpackReference;

		// Token: 0x04001B2F RID: 6959
		public byte slotID;
	}
}

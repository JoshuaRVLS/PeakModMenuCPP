using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zorro.Core;

// Token: 0x020001CF RID: 463
public class InventoryItemUI : MonoBehaviour
{
	// Token: 0x06000EC8 RID: 3784 RVA: 0x00049BD9 File Offset: 0x00047DD9
	public void Start()
	{
		this.startingSizeDelta = this.rectTransform.sizeDelta;
	}

	// Token: 0x06000EC9 RID: 3785 RVA: 0x00049BEC File Offset: 0x00047DEC
	private void UpdateCookedAmount()
	{
		if (this._itemData == null)
		{
			this.cookedAmount = 0;
			this.icon.color = Color.white;
			return;
		}
		IntItemData intItemData;
		if (this._itemData.TryGetDataEntry<IntItemData>(DataEntryKey.CookedAmount, out intItemData) && this.cookedAmount != intItemData.Value)
		{
			this.icon.color = Color.white;
			this.icon.color = ItemCooking.GetCookColor(intItemData.Value);
			this.cookedAmount = intItemData.Value;
		}
	}

	// Token: 0x06000ECA RID: 3786 RVA: 0x00049C6C File Offset: 0x00047E6C
	public void SetItem(ItemSlot slot)
	{
		if (slot == null)
		{
			this.Clear();
		}
		if (this.isBackpack)
		{
			if (Character.observedCharacter.data.carriedPlayer)
			{
				this.icon.color = Character.observedCharacter.data.carriedPlayer.refs.customization.PlayerColor;
				this.icon.texture = this.carryingIcon;
				this.backpackFilledSlotsObject.SetActive(false);
				return;
			}
			this.icon.texture = this.backpackIcon;
			if (slot.IsEmpty())
			{
				this._hasBackpack = false;
				this.icon.color = new Color(0f, 0f, 0f, 0.5f);
				this.backpackFilledSlotsObject.SetActive(false);
				this.fill.enabled = false;
				return;
			}
			this._hasBackpack = true;
			this.icon.color = Color.white;
			BackpackData backpackData;
			if (this.backpackFilledSlotsObject != null && slot.data.TryGetDataEntry<BackpackData>(DataEntryKey.BackpackData, out backpackData))
			{
				int num = backpackData.FilledSlotCount();
				this.backpackFilledSlotsObject.SetActive(num > 0);
				this.backpackFilledSlotsAmountText.text = num.ToString();
			}
			return;
		}
		else
		{
			if (this._itemPrefab == slot.prefab)
			{
				this.TrySetFuel(slot.data);
				this.UpdateNameText();
				this.UpdateCookedAmount();
				return;
			}
			this._itemPrefab = slot.prefab;
			this._itemData = slot.data;
			this.UpdateNameText();
			this.UpdateCookedAmount();
			this.SetSelected();
			if (!slot.IsEmpty())
			{
				if (this._itemPrefab == null)
				{
					this.icon.transform.localScale = Vector3.zero;
					this.icon.transform.DOScale(1f, 0.5f).SetEase(Ease.OutElastic);
				}
				this.icon.texture = this._itemPrefab.UIData.GetIcon();
				this.icon.enabled = true;
				this.TrySetFuel(slot.data);
				return;
			}
			this.Clear();
			return;
		}
	}

	// Token: 0x06000ECB RID: 3787 RVA: 0x00049E84 File Offset: 0x00048084
	public void Clear()
	{
		this.fill.enabled = false;
		this.icon.enabled = false;
		this._itemPrefab = null;
		this._itemData = null;
		this.nameText.enabled = false;
		this.nameText.text = "";
		this.TrySetFuel(null);
	}

	// Token: 0x06000ECC RID: 3788 RVA: 0x00049EDC File Offset: 0x000480DC
	public void TrySetFuel(ItemInstanceData data)
	{
		if (!this.fuelBar)
		{
			return;
		}
		if (Character.observedCharacter != Character.localCharacter)
		{
			this.fuelBar.SetActive(false);
			return;
		}
		if (data == null || this._itemPrefab == null || !data.HasData(DataEntryKey.UseRemainingPercentage) || this._itemPrefab.UIData.hideFuel)
		{
			this.fuelBar.SetActive(false);
			this.fuelBarFill.fillAmount = 1f;
			return;
		}
		this.fuelBar.SetActive(true);
		FloatItemData floatItemData;
		if (data.TryGetDataEntry<FloatItemData>(DataEntryKey.UseRemainingPercentage, out floatItemData))
		{
			this.fuelBarFill.fillAmount = floatItemData.Value;
		}
	}

	// Token: 0x06000ECD RID: 3789 RVA: 0x00049F88 File Offset: 0x00048188
	public void UpdateNameText()
	{
		string text;
		if (this._itemPrefab != null || (this.isBackpack && this._hasBackpack))
		{
			if (this._itemPrefab != null)
			{
				text = this._itemPrefab.GetItemName(this._itemData);
			}
			else
			{
				text = "Backpack";
			}
		}
		else
		{
			text = "";
		}
		if (this.nameText.text != text)
		{
			this.SetSelected();
		}
		this.nameText.text = text;
	}

	// Token: 0x06000ECE RID: 3790 RVA: 0x0004A008 File Offset: 0x00048208
	public void SetSelected()
	{
		Optionable<byte> currentSelectedSlot = Character.observedCharacter.refs.items.currentSelectedSlot;
		bool flag = currentSelectedSlot.IsSome && (int)currentSelectedSlot.Value == base.transform.GetSiblingIndex();
		if (this.isTemporarySlot)
		{
			flag = true;
		}
		if (this.isBackpack)
		{
			flag = (currentSelectedSlot.Value == 3);
		}
		if (this._itemPrefab != null || (this.isBackpack && (this._hasBackpack || Character.observedCharacter.data.carriedPlayer)) || this.isTemporarySlot)
		{
			if (flag)
			{
				this.mySequence.Kill(false);
				this.rectTransform.DOKill(false);
				this.rectTransform.DOSizeDelta(this.startingSizeDelta * 1.2f, 0.5f, false).SetEase(Ease.OutElastic);
				this.fill.enabled = true;
				this.fill.transform.localScale = Vector3.zero;
				this.fill.transform.DOKill(false);
				this.fill.transform.DOScale(1f, 0.25f).SetEase(Ease.OutCubic);
				this.nameText.enabled = true;
				return;
			}
			this.mySequence.Kill(false);
			this.rectTransform.DOKill(false);
			this.rectTransform.DOSizeDelta(this.startingSizeDelta, 0.2f, false).SetEase(Ease.OutCubic);
			this.fill.enabled = false;
			this.nameText.enabled = false;
			return;
		}
		else
		{
			if (flag)
			{
				this.mySequence.Kill(false);
				this.mySequence = DOTween.Sequence();
				this.mySequence.Append(this.rectTransform.DOSizeDelta(this.startingSizeDelta * 1.2f, 0.075f, false).SetEase(Ease.OutCubic));
				this.mySequence.Append(this.rectTransform.DOSizeDelta(this.startingSizeDelta, 0.125f, false).SetEase(Ease.InSine));
				return;
			}
			this.mySequence.Kill(false);
			this.rectTransform.DOKill(false);
			this.rectTransform.sizeDelta = this.startingSizeDelta;
			return;
		}
	}

	// Token: 0x06000ECF RID: 3791 RVA: 0x0004A244 File Offset: 0x00048444
	private void OnDisable()
	{
		this.mySequence.Kill(false);
		this.rectTransform.DOKill(false);
		this.rectTransform.sizeDelta = this.startingSizeDelta;
		this.fill.enabled = false;
		this.nameText.enabled = false;
		this.nameText.text = "";
	}

	// Token: 0x04000C85 RID: 3205
	public RectTransform rectTransform;

	// Token: 0x04000C86 RID: 3206
	public RawImage icon;

	// Token: 0x04000C87 RID: 3207
	public Image fill;

	// Token: 0x04000C88 RID: 3208
	public Image selectedSlotIcon;

	// Token: 0x04000C89 RID: 3209
	public Texture defaultIcon;

	// Token: 0x04000C8A RID: 3210
	public TextMeshProUGUI nameText;

	// Token: 0x04000C8B RID: 3211
	public bool isBackpack;

	// Token: 0x04000C8C RID: 3212
	public GameObject backpackFilledSlotsObject;

	// Token: 0x04000C8D RID: 3213
	public TextMeshProUGUI backpackFilledSlotsAmountText;

	// Token: 0x04000C8E RID: 3214
	private Sequence mySequence;

	// Token: 0x04000C8F RID: 3215
	private Item _itemPrefab;

	// Token: 0x04000C90 RID: 3216
	private bool _hasBackpack;

	// Token: 0x04000C91 RID: 3217
	public GameObject fuelBar;

	// Token: 0x04000C92 RID: 3218
	public Image fuelBarFill;

	// Token: 0x04000C93 RID: 3219
	public Texture backpackIcon;

	// Token: 0x04000C94 RID: 3220
	public Texture carryingIcon;

	// Token: 0x04000C95 RID: 3221
	public ItemInstanceData _itemData;

	// Token: 0x04000C96 RID: 3222
	private int cookedAmount;

	// Token: 0x04000C97 RID: 3223
	public bool isTemporarySlot;

	// Token: 0x04000C98 RID: 3224
	private Vector2 startingSizeDelta;
}

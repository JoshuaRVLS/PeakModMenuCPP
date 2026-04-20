using System;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using Zorro.Core;

// Token: 0x020000D0 RID: 208
public class ItemCooking : ItemComponent
{
	// Token: 0x17000098 RID: 152
	// (get) Token: 0x06000822 RID: 2082 RVA: 0x0002DF2C File Offset: 0x0002C12C
	// (set) Token: 0x06000823 RID: 2083 RVA: 0x0002DF34 File Offset: 0x0002C134
	public int timesCookedLocal { get; protected set; }

	// Token: 0x17000099 RID: 153
	// (get) Token: 0x06000824 RID: 2084 RVA: 0x0002DF3D File Offset: 0x0002C13D
	public bool canBeCooked
	{
		get
		{
			return !this.disableCooking;
		}
	}

	// Token: 0x1700009A RID: 154
	// (get) Token: 0x06000825 RID: 2085 RVA: 0x0002DF48 File Offset: 0x0002C148
	private bool hasExplosion
	{
		get
		{
			AdditionalCookingBehavior[] array = this.additionalCookingBehaviors;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] is CookingBehavior_Explode)
				{
					return true;
				}
			}
			return false;
		}
	}

	// Token: 0x06000826 RID: 2086 RVA: 0x0002DF77 File Offset: 0x0002C177
	public override void OnInstanceDataSet()
	{
		this.UpdateCookedBehavior();
	}

	// Token: 0x06000827 RID: 2087 RVA: 0x0002DF80 File Offset: 0x0002C180
	public virtual void UpdateCookedBehavior()
	{
		IntItemData data = this.item.GetData<IntItemData>(DataEntryKey.CookedAmount);
		if (data.Value == 0)
		{
			data.Value += this.preCooked;
		}
		if (!this.setup)
		{
			this.setup = true;
			Renderer[] componentsInChildren = base.GetComponentsInChildren<MeshRenderer>();
			this.renderers = componentsInChildren;
			componentsInChildren = base.GetComponentsInChildren<SkinnedMeshRenderer>(true);
			Renderer[] array = componentsInChildren;
			if (array.Length != 0)
			{
				this.renderers = this.renderers.Concat(array).ToArray<Renderer>();
			}
			this.defaultTints = new Color[this.renderers.Length];
			for (int i = 0; i < this.renderers.Length; i++)
			{
				this.defaultTints[i] = this.renderers[i].material.GetColor("_Tint");
			}
		}
		int num = data.Value - this.timesCookedLocal;
		this.CookVisually(data.Value);
		if (!this.ignoreDefaultCookBehavior && num > 0)
		{
			for (int j = 1 + this.timesCookedLocal; j <= data.Value; j++)
			{
				this.ChangeStatsCooked(j);
			}
		}
		this.RunAdditionalCookingBehaviors(data.Value);
		this.timesCookedLocal = data.Value;
	}

	// Token: 0x06000828 RID: 2088 RVA: 0x0002E0AC File Offset: 0x0002C2AC
	protected void RunAdditionalCookingBehaviors(int cookedAmount)
	{
		AdditionalCookingBehavior[] array = this.additionalCookingBehaviors;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Cook(this, cookedAmount);
		}
	}

	// Token: 0x06000829 RID: 2089 RVA: 0x0002E0D8 File Offset: 0x0002C2D8
	protected virtual void CookVisually(int cookedAmount)
	{
		if (cookedAmount > 0)
		{
			for (int i = 0; i < this.renderers.Length; i++)
			{
				for (int j = 0; j < this.renderers[i].materials.Length; j++)
				{
					this.renderers[i].materials[j].SetColor("_Tint", this.defaultTints[i] * ItemCooking.GetCookColor(cookedAmount));
				}
			}
		}
	}

	// Token: 0x0600082A RID: 2090 RVA: 0x0002E148 File Offset: 0x0002C348
	public static Color GetCookColor(int cookAmount)
	{
		Color result = Color.white;
		if (cookAmount == 1)
		{
			result = ItemCooking.DefaultCookColorMultiplier;
		}
		else if (cookAmount == 2)
		{
			result = ItemCooking.DefaultCookColorMultiplier * 0.5f;
		}
		else if (cookAmount > 2)
		{
			result = ItemCooking.BurntCookColorMultiplier;
		}
		result.a = 1f;
		return result;
	}

	// Token: 0x0600082B RID: 2091 RVA: 0x0002E194 File Offset: 0x0002C394
	[PunRPC]
	private void FinishCookingRPC()
	{
		this.CancelCookingVisuals();
		IntItemData data = base.GetData<IntItemData>(DataEntryKey.CookedAmount);
		if (this.wreckWhenCooked)
		{
			data.Value = 5;
		}
		else if (data.Value < 12)
		{
			data.Value++;
		}
		this.item.WasActive();
		this.UpdateCookedBehavior();
	}

	// Token: 0x0600082C RID: 2092 RVA: 0x0002E1E9 File Offset: 0x0002C3E9
	public void StartCookingVisuals()
	{
		this.photonView.RPC("EnableCookingSmokeRPC", RpcTarget.All, new object[]
		{
			true
		});
	}

	// Token: 0x0600082D RID: 2093 RVA: 0x0002E20B File Offset: 0x0002C40B
	[PunRPC]
	private void EnableCookingSmokeRPC(bool active)
	{
		this.item.particles.EnableSmoke(active);
	}

	// Token: 0x0600082E RID: 2094 RVA: 0x0002E220 File Offset: 0x0002C420
	public void Wreck()
	{
		ItemComponent[] components = base.GetComponents<ItemComponent>();
		for (int i = components.Length - 1; i >= 0; i--)
		{
			if (components[i] != this)
			{
				Object.Destroy(components[i]);
			}
		}
		ItemAction[] components2 = base.GetComponents<ItemAction>();
		for (int j = components2.Length - 1; j >= 0; j--)
		{
			Object.Destroy(components2[j]);
		}
		this.item.overrideUsability = Optionable<bool>.Some(false);
	}

	// Token: 0x0600082F RID: 2095 RVA: 0x0002E288 File Offset: 0x0002C488
	private void ChangeStatsCooked(int totalCooked)
	{
		if (this.wreckWhenCooked && totalCooked > 0)
		{
			this.Wreck();
			return;
		}
		Action_RestoreHunger component = base.GetComponent<Action_RestoreHunger>();
		if (component)
		{
			if (totalCooked < 2)
			{
				component.restorationAmount *= 2f;
			}
			else if (totalCooked > 2)
			{
				component.restorationAmount = Mathf.Max(component.restorationAmount - 0.05f, 0f);
			}
		}
		Action_GiveExtraStamina action_GiveExtraStamina = base.GetComponent<Action_GiveExtraStamina>();
		if (!action_GiveExtraStamina)
		{
			action_GiveExtraStamina = base.gameObject.AddComponent<Action_GiveExtraStamina>();
			action_GiveExtraStamina.OnConsumed = true;
		}
		if (totalCooked < 2)
		{
			action_GiveExtraStamina.amount = Mathf.Max(0.1f, action_GiveExtraStamina.amount * 1.5f);
		}
		else if (totalCooked > 2)
		{
			action_GiveExtraStamina.amount = 0f;
		}
		Action_ModifyStatus action_ModifyStatus = base.GetComponents<Action_ModifyStatus>().FirstOrDefault((Action_ModifyStatus a) => a.statusType == CharacterAfflictions.STATUSTYPE.Poison);
		if (totalCooked > 3)
		{
			if (!action_ModifyStatus)
			{
				action_ModifyStatus = base.gameObject.AddComponent<Action_ModifyStatus>();
				action_ModifyStatus.OnConsumed = true;
				action_ModifyStatus.statusType = CharacterAfflictions.STATUSTYPE.Poison;
			}
			action_ModifyStatus.changeAmount += 0.1f;
		}
	}

	// Token: 0x06000830 RID: 2096 RVA: 0x0002E3A5 File Offset: 0x0002C5A5
	public void CancelCookingVisuals()
	{
		this.photonView.RPC("EnableCookingSmokeRPC", RpcTarget.All, new object[]
		{
			false
		});
	}

	// Token: 0x06000831 RID: 2097 RVA: 0x0002E3C8 File Offset: 0x0002C5C8
	public void FinishCooking()
	{
		if (!this.photonView.AmController)
		{
			return;
		}
		this.photonView.RPC("FinishCookingRPC", RpcTarget.All, Array.Empty<object>());
		if (this.item.holderCharacter)
		{
			Action<ItemSlot[]> itemsChangedAction = this.item.holderCharacter.player.itemsChangedAction;
			if (itemsChangedAction != null)
			{
				itemsChangedAction(this.item.holderCharacter.player.itemSlots);
			}
			CharacterItems items = this.item.holderCharacter.refs.items;
			if (((items != null) ? items.cookSfx : null) != null)
			{
				this.item.holderCharacter.refs.items.cookSfx.Play(base.transform.position);
			}
		}
		Debug.Log("Cooking Finished");
	}

	// Token: 0x06000832 RID: 2098 RVA: 0x0002E49A File Offset: 0x0002C69A
	public void Explode()
	{
		if (this.photonView.IsMine)
		{
			this.photonView.RPC("RPC_CookingExplode", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06000833 RID: 2099 RVA: 0x0002E4C0 File Offset: 0x0002C6C0
	[PunRPC]
	private void RPC_CookingExplode()
	{
		if (this.explosionPrefab)
		{
			Object.Instantiate<GameObject>(this.explosionPrefab, base.transform.position, base.transform.rotation);
		}
		if (Character.localCharacter.data.currentItem == this.item)
		{
			Player.localPlayer.EmptySlot(Character.localCharacter.refs.items.currentSelectedSlot);
			Character.localCharacter.refs.afflictions.UpdateWeight();
		}
		this.item.ClearDataFromBackpack();
		if (this.photonView.IsMine)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
	}

	// Token: 0x040007E2 RID: 2018
	public int preCooked;

	// Token: 0x040007E4 RID: 2020
	[SerializeField]
	protected bool disableCooking;

	// Token: 0x040007E5 RID: 2021
	[FormerlySerializedAs("burnInstantly")]
	public bool wreckWhenCooked;

	// Token: 0x040007E6 RID: 2022
	public bool ignoreDefaultCookBehavior;

	// Token: 0x040007E7 RID: 2023
	[SerializeReference]
	public AdditionalCookingBehavior[] additionalCookingBehaviors = new AdditionalCookingBehavior[0];

	// Token: 0x040007E8 RID: 2024
	private Renderer[] renderers;

	// Token: 0x040007E9 RID: 2025
	private Color[] defaultTints;

	// Token: 0x040007EA RID: 2026
	private bool setup;

	// Token: 0x040007EB RID: 2027
	public static Color DefaultCookColorMultiplier = new Color(0.66f, 0.47f, 0.25f);

	// Token: 0x040007EC RID: 2028
	public static Color BurntCookColorMultiplier = new Color(0.05f, 0.05f, 0.1f);

	// Token: 0x040007ED RID: 2029
	public const int COOKING_MAX = 12;

	// Token: 0x040007EE RID: 2030
	public GameObject explosionPrefab;
}

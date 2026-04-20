using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000290 RID: 656
public class Lava : MonoBehaviour
{
	// Token: 0x060012E4 RID: 4836 RVA: 0x0005F37D File Offset: 0x0005D57D
	private void Start()
	{
		this.bounds = base.GetComponentInChildren<MeshRenderer>().bounds;
	}

	// Token: 0x060012E5 RID: 4837 RVA: 0x0005F390 File Offset: 0x0005D590
	private void FixedUpdate()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		this.Movement();
		if (Character.localCharacter)
		{
			this.DoEffects();
			this.Heat();
		}
		this.TryCookItems();
	}

	// Token: 0x060012E6 RID: 4838 RVA: 0x0005F3C4 File Offset: 0x0005D5C4
	private void Heat()
	{
		Character localCharacter = Character.localCharacter;
		if (localCharacter == null)
		{
			return;
		}
		this.counter += Time.deltaTime;
		if (this.OutsideBounds(localCharacter.Center))
		{
			return;
		}
		float num = localCharacter.Center.y - base.transform.position.y;
		float num2 = 1f - Mathf.Clamp01(num / this.height);
		if (num2 < 0.01f)
		{
			return;
		}
		if (this.counter < this.heatRate)
		{
			return;
		}
		this.counter = 0f;
		localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Hot, num2 * this.heat * 1.5f, false, true, true);
	}

	// Token: 0x060012E7 RID: 4839 RVA: 0x0005F47C File Offset: 0x0005D67C
	private bool OutsideBounds(Vector3 pos)
	{
		return pos.x > this.bounds.max.x || pos.x < this.bounds.min.x || pos.z > this.bounds.max.z || pos.z < this.bounds.min.z;
	}

	// Token: 0x060012E8 RID: 4840 RVA: 0x0005F4F4 File Offset: 0x0005D6F4
	private void DoEffects()
	{
		Character localCharacter = Character.localCharacter;
		if (this.OutsideBounds(localCharacter.Center))
		{
			return;
		}
		if (localCharacter.Center.y - 0.5f > base.transform.position.y)
		{
			return;
		}
		localCharacter.AddForce(Vector3.up * 80f, 0.5f, 1f);
		localCharacter.data.sinceGrounded = 0f;
		localCharacter.refs.movement.ApplyExtraDrag(0.8f, true);
		if (this.hitPlayers.Contains(localCharacter))
		{
			return;
		}
		if (localCharacter.data.dead)
		{
			return;
		}
		if (localCharacter.refs.afflictions.statusSum > 1.9f)
		{
			return;
		}
		this.HitPlayer(localCharacter);
		base.StartCoroutine(this.IHoldPlayer(localCharacter));
	}

	// Token: 0x060012E9 RID: 4841 RVA: 0x0005F5CC File Offset: 0x0005D7CC
	private void HitPlayer(Character item)
	{
		item.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, 0.25f, false, true, true);
		item.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Hot, 0.25f, false, true, true);
		item.data.sinceGrounded = 0f;
	}

	// Token: 0x060012EA RID: 4842 RVA: 0x0005F61D File Offset: 0x0005D81D
	private IEnumerator IHoldPlayer(Character item)
	{
		this.hitPlayers.Add(item);
		yield return new WaitForSeconds(1f);
		this.hitPlayers.Remove(item);
		yield break;
	}

	// Token: 0x060012EB RID: 4843 RVA: 0x0005F633 File Offset: 0x0005D833
	private void Movement()
	{
	}

	// Token: 0x060012EC RID: 4844 RVA: 0x0005F638 File Offset: 0x0005D838
	private void TryCookItems()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		for (int i = 0; i < Item.ALL_ACTIVE_ITEMS.Count; i++)
		{
			Item item = Item.ALL_ACTIVE_ITEMS[i];
			if (item.UnityObjectExists<Item>() && item.itemState != ItemState.Held && !this.OutsideBounds(item.Center()))
			{
				if (item.itemState == ItemState.InBackpack && item.backpackReference.IsSome && item.backpackReference.Value.Item2.type == BackpackReference.BackpackType.Equipped)
				{
					return;
				}
				if (this.TestSacrificeIdol(item))
				{
					return;
				}
				if (item.cooking.canBeCooked && this.GetItemCookAmount(item) > 0f && this.itemToCookTime.TryAdd(item, 0f))
				{
					Debug.Log("Lava started cooking: " + item.GetItemName(null));
					item.GetComponent<ItemCooking>().StartCookingVisuals();
				}
			}
		}
		this.itemToRemoveList.Clear();
		this.itemToCookList.Clear();
		foreach (Item item2 in this.itemToCookTime.Keys)
		{
			if (item2 == null)
			{
				this.itemToRemoveList.Add(item2);
			}
			else if (this.OutsideBounds(item2.Center()))
			{
				this.itemToRemoveList.Add(item2);
				item2.GetComponent<ItemCooking>().CancelCookingVisuals();
			}
			else
			{
				this.itemToCookList.Add(item2);
			}
		}
		foreach (Item item3 in this.itemToCookList)
		{
			float num = this.GetItemCookAmount(item3) * Time.deltaTime;
			Dictionary<Item, float> dictionary = this.itemToCookTime;
			Item key = item3;
			dictionary[key] += num;
			if (this.itemToCookTime[item3] >= 1f)
			{
				Debug.Log("Lava finished cooking: " + item3.GetItemName(null));
				item3.GetComponent<ItemCooking>().FinishCooking();
				this.itemToCookTime[item3] = 0f;
			}
		}
		foreach (Item key2 in this.itemToRemoveList)
		{
			this.itemToCookTime.Remove(key2);
		}
	}

	// Token: 0x060012ED RID: 4845 RVA: 0x0005F8D0 File Offset: 0x0005DAD0
	private float GetItemCookAmount(Item item)
	{
		float num = item.Center().y - base.transform.position.y;
		float num2 = 1f - Mathf.Clamp01(num / this.height);
		if (num2 < 0.01f)
		{
			return 0f;
		}
		return num2 * 0.7f;
	}

	// Token: 0x060012EE RID: 4846 RVA: 0x0005F924 File Offset: 0x0005DB24
	private bool TestSacrificeIdol(Item item)
	{
		if (!this.isKiln)
		{
			return false;
		}
		if (item.Center().y > base.transform.position.y)
		{
			return false;
		}
		if (item.photonView.IsMine && item.itemTags.HasFlag(Item.ItemTags.GoldenIdol))
		{
			if (Character.localCharacter.data.currentItem == item)
			{
				Player.localPlayer.EmptySlot(Character.localCharacter.refs.items.currentSelectedSlot);
				Character.localCharacter.refs.afflictions.UpdateWeight();
			}
			PhotonNetwork.Destroy(item.gameObject);
			GameUtils.instance.ThrowSacrificeAchievement();
			return true;
		}
		return false;
	}

	// Token: 0x040010FA RID: 4346
	private List<Character> hitPlayers = new List<Character>();

	// Token: 0x040010FB RID: 4347
	public float heatRate = 0.5f;

	// Token: 0x040010FC RID: 4348
	public float heat = 0.02f;

	// Token: 0x040010FD RID: 4349
	public float height = 10f;

	// Token: 0x040010FE RID: 4350
	public bool isKiln;

	// Token: 0x040010FF RID: 4351
	private Bounds bounds;

	// Token: 0x04001100 RID: 4352
	private float counter;

	// Token: 0x04001101 RID: 4353
	public Dictionary<Item, float> itemToCookTime = new Dictionary<Item, float>();

	// Token: 0x04001102 RID: 4354
	private List<Item> itemToRemoveList = new List<Item>();

	// Token: 0x04001103 RID: 4355
	private List<Item> itemToCookList = new List<Item>();
}

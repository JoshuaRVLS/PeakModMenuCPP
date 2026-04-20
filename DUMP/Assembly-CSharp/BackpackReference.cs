using System;
using Photon.Pun;
using UnityEngine;
using Zorro.Core.Serizalization;

// Token: 0x02000105 RID: 261
public struct BackpackReference : IBinarySerializable
{
	// Token: 0x060008CC RID: 2252 RVA: 0x00030420 File Offset: 0x0002E620
	public static BackpackReference GetFromBackpackItem(Item item)
	{
		return new BackpackReference
		{
			type = BackpackReference.BackpackType.Item,
			view = item.GetComponent<PhotonView>(),
			locationTransform = item.transform
		};
	}

	// Token: 0x060008CD RID: 2253 RVA: 0x00030458 File Offset: 0x0002E658
	public static BackpackReference GetFromEquippedBackpack(Character character)
	{
		return new BackpackReference
		{
			type = BackpackReference.BackpackType.Equipped,
			view = character.GetComponent<PhotonView>(),
			locationTransform = character.GetBodypart(BodypartType.Torso).transform
		};
	}

	// Token: 0x060008CE RID: 2254 RVA: 0x00030496 File Offset: 0x0002E696
	public BackpackVisuals GetVisuals()
	{
		if (this.type == BackpackReference.BackpackType.Item)
		{
			return this.view.GetComponent<ItemBackpackVisuals>();
		}
		return this.view.GetComponent<CharacterBackpackHandler>().backpackVisuals;
	}

	// Token: 0x060008CF RID: 2255 RVA: 0x000304BC File Offset: 0x0002E6BC
	public void Serialize(BinarySerializer serializer)
	{
		serializer.WriteByte((byte)this.type);
		serializer.WriteInt(this.view.ViewID);
	}

	// Token: 0x060008D0 RID: 2256 RVA: 0x000304DB File Offset: 0x0002E6DB
	public void Deserialize(BinaryDeserializer deserializer)
	{
		this.type = (BackpackReference.BackpackType)deserializer.ReadByte();
		this.view = PhotonView.Find(deserializer.ReadInt());
	}

	// Token: 0x060008D1 RID: 2257 RVA: 0x000304FA File Offset: 0x0002E6FA
	public ItemInstanceData GetItemInstanceData()
	{
		if (this.type == BackpackReference.BackpackType.Item)
		{
			return this.view.GetComponent<Item>().data;
		}
		return this.view.GetComponent<Character>().player.backpackSlot.data;
	}

	// Token: 0x060008D2 RID: 2258 RVA: 0x00030530 File Offset: 0x0002E730
	public BackpackData GetData()
	{
		if (this.type == BackpackReference.BackpackType.Item)
		{
			return this.view.GetComponent<Item>().GetData<BackpackData>(DataEntryKey.BackpackData);
		}
		BackpackData result;
		if (!this.view.GetComponent<Character>().player.backpackSlot.data.TryGetDataEntry<BackpackData>(DataEntryKey.BackpackData, out result))
		{
			result = this.view.GetComponent<Character>().player.backpackSlot.data.RegisterNewEntry<BackpackData>(DataEntryKey.BackpackData);
		}
		return result;
	}

	// Token: 0x060008D3 RID: 2259 RVA: 0x0003059D File Offset: 0x0002E79D
	public bool IsOnMyBack()
	{
		return this.type != BackpackReference.BackpackType.Item && this.view.IsMine;
	}

	// Token: 0x060008D4 RID: 2260 RVA: 0x000305B4 File Offset: 0x0002E7B4
	public bool TryGetBackpackItem(out Backpack backpack)
	{
		if (this.type == BackpackReference.BackpackType.Item)
		{
			backpack = this.view.GetComponent<Backpack>();
			return true;
		}
		backpack = null;
		return false;
	}

	// Token: 0x0400085F RID: 2143
	public BackpackReference.BackpackType type;

	// Token: 0x04000860 RID: 2144
	public PhotonView view;

	// Token: 0x04000861 RID: 2145
	public Transform locationTransform;

	// Token: 0x0200046C RID: 1132
	public enum BackpackType : byte
	{
		// Token: 0x0400196F RID: 6511
		Item,
		// Token: 0x04001970 RID: 6512
		Equipped
	}
}

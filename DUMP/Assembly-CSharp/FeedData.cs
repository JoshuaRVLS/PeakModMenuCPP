using System;
using UnityEngine;

// Token: 0x0200001C RID: 28
[Serializable]
public class FeedData
{
	// Token: 0x06000246 RID: 582 RVA: 0x00011339 File Offset: 0x0000F539
	public void PrintDescription()
	{
		Debug.Log(this.GetDescription());
	}

	// Token: 0x06000247 RID: 583 RVA: 0x00011348 File Offset: 0x0000F548
	public string GetDescription()
	{
		Character character;
		bool characterWithPhotonID = Character.GetCharacterWithPhotonID(this.giverID, out character);
		Character character2;
		Character.GetCharacterWithPhotonID(this.receiverID, out character2);
		Item item;
		bool flag = ItemDatabase.TryGetItem(this.itemID, out item);
		string text = characterWithPhotonID ? character.characterName : "An unknown scout";
		string text2 = characterWithPhotonID ? character.characterName : "an unknown scout";
		string text3 = flag ? item.GetItemName(null) : "an unknown item";
		return string.Concat(new string[]
		{
			text,
			" is feeding ",
			text2,
			" a ",
			text3,
			"..."
		});
	}

	// Token: 0x04000218 RID: 536
	public int giverID;

	// Token: 0x04000219 RID: 537
	public int receiverID;

	// Token: 0x0400021A RID: 538
	public ushort itemID;

	// Token: 0x0400021B RID: 539
	public float totalItemTime;
}

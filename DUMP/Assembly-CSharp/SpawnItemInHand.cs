using System;
using System.Collections;
using UnityEngine;

// Token: 0x020001B4 RID: 436
public class SpawnItemInHand : MonoBehaviour
{
	// Token: 0x06000E0A RID: 3594 RVA: 0x00046A2B File Offset: 0x00044C2B
	private IEnumerator Start()
	{
		while (!Character.localCharacter)
		{
			yield return null;
		}
		if (this.blockItemSwapUntilSpawned)
		{
			Character.localCharacter.input.itemSwitchBlocked = true;
		}
		yield return null;
		yield return null;
		yield return null;
		yield return new WaitForSeconds(1.5f);
		Character.localCharacter.refs.items.SpawnItemInHand(this.item.gameObject.name);
		while (Character.localCharacter.data.currentItem == null)
		{
			yield return null;
		}
		Character.localCharacter.input.itemSwitchBlocked = false;
		yield break;
	}

	// Token: 0x04000BDE RID: 3038
	public Item item;

	// Token: 0x04000BDF RID: 3039
	public bool blockItemSwapUntilSpawned;
}

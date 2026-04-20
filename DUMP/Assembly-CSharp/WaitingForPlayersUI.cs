using System;
using UnityEngine;
using UnityEngine.UI;
using Zorro.Core;

// Token: 0x020001FA RID: 506
public class WaitingForPlayersUI : MonoBehaviour
{
	// Token: 0x06000FE5 RID: 4069 RVA: 0x0004DD4C File Offset: 0x0004BF4C
	private void Update()
	{
		for (int i = 0; i < this.scoutImages.Length; i++)
		{
			this.scoutImages[i].gameObject.SetActive(false);
		}
		int num = 0;
		foreach (Player player in PlayerHandler.GetAllPlayers())
		{
			bool hasClosedEndScreen = player.hasClosedEndScreen;
			PersistentPlayerData playerData = GameHandler.GetService<PersistentPlayerDataService>().GetPlayerData(player.photonView.Owner);
			Color color = Singleton<Customization>.Instance.skins[playerData.customizationData.currentSkin].color;
			if (num < this.scoutImages.Length)
			{
				this.scoutImages[num].gameObject.SetActive(true);
				this.scoutImages[num].color = (hasClosedEndScreen ? color : this.notReadyColor);
			}
			num++;
		}
	}

	// Token: 0x04000D78 RID: 3448
	public Image[] scoutImages;

	// Token: 0x04000D79 RID: 3449
	public Color notReadyColor;
}

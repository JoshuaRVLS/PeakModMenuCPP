using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zorro.Core;

// Token: 0x020001DC RID: 476
public class PauseBadgesMenu : MonoBehaviour
{
	// Token: 0x06000F38 RID: 3896 RVA: 0x0004B0C0 File Offset: 0x000492C0
	private void OnEnable()
	{
		int num = 0;
		int num2;
		if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.TimesPeaked, out num2))
		{
			num = num2;
		}
		this.peaksSummitedText.text = LocalizedText.GetText("PEAKSSUMMITTTED", true).Replace("#", num.ToString() ?? "");
		this.scoutTitleText.text = this.ascentData.ascents[Singleton<AchievementManager>.Instance.GetMaxAscent() + 1].localizedReward;
		this.badgeSashImage.color = this.ascentData.ascents[Singleton<AchievementManager>.Instance.GetMaxAscent() + 1].color;
	}

	// Token: 0x04000CC5 RID: 3269
	public Image badgeSashImage;

	// Token: 0x04000CC6 RID: 3270
	public TMP_Text scoutTitleText;

	// Token: 0x04000CC7 RID: 3271
	public AscentData ascentData;

	// Token: 0x04000CC8 RID: 3272
	public TMP_Text peaksSummitedText;
}

using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

// Token: 0x020001BD RID: 445
[CreateAssetMenu(fileName = "AscentData", menuName = "Scriptable Objects/AscentData")]
public class AscentData : SingletonAsset<AscentData>
{
	// Token: 0x06000E43 RID: 3651 RVA: 0x000478AC File Offset: 0x00045AAC
	public void AddAllToCSV()
	{
		for (int i = 0; i < this.ascents.Count; i++)
		{
			LocalizedText.AppendCSVLine(this.ascents[i].title.ToUpperInvariant() + "," + this.ascents[i].title.ToUpperInvariant() + ",,,,,,,,,,,,,ENDLINE", "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
		}
		for (int j = 0; j < this.ascents.Count; j++)
		{
			LocalizedText.AppendCSVLine(this.ascents[j].titleReward.ToUpperInvariant() + "," + this.ascents[j].titleReward.ToUpperInvariant() + ",,,,,,,,,,,,,ENDLINE", "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
		}
		for (int k = 0; k < this.ascents.Count; k++)
		{
			LocalizedText.AppendCSVLine(string.Concat(new string[]
			{
				"DESC_",
				this.ascents[k].title.ToUpperInvariant(),
				",",
				this.ascents[k].description.ToUpperInvariant(),
				",,,,,,,,,,,,,ENDLINE"
			}), "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
		}
	}

	// Token: 0x04000C02 RID: 3074
	public List<AscentData.AscentInstanceData> ascents;

	// Token: 0x020004D0 RID: 1232
	[Serializable]
	public class AscentInstanceData
	{
		// Token: 0x1700023E RID: 574
		// (get) Token: 0x06001D9D RID: 7581 RVA: 0x0008A849 File Offset: 0x00088A49
		public string localizedTitle
		{
			get
			{
				return LocalizedText.GetText(this.title, true);
			}
		}

		// Token: 0x1700023F RID: 575
		// (get) Token: 0x06001D9E RID: 7582 RVA: 0x0008A857 File Offset: 0x00088A57
		public string localizedReward
		{
			get
			{
				return LocalizedText.GetText(this.titleReward, true);
			}
		}

		// Token: 0x17000240 RID: 576
		// (get) Token: 0x06001D9F RID: 7583 RVA: 0x0008A865 File Offset: 0x00088A65
		public string localizedDescription
		{
			get
			{
				return LocalizedText.GetText(LocalizedText.GetDescriptionIndex(this.title), true);
			}
		}

		// Token: 0x04001B27 RID: 6951
		public string title;

		// Token: 0x04001B28 RID: 6952
		public string titleReward;

		// Token: 0x04001B29 RID: 6953
		public string description;

		// Token: 0x04001B2A RID: 6954
		public Color color;

		// Token: 0x04001B2B RID: 6955
		public Sprite sashSprite;
	}
}

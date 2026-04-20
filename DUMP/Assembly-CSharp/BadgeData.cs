using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x020001C0 RID: 448
[CreateAssetMenu(fileName = "BadgeData", menuName = "Scriptable Objects/BadgeData")]
public class BadgeData : ScriptableObject
{
	// Token: 0x17000105 RID: 261
	// (get) Token: 0x06000E5E RID: 3678 RVA: 0x0004845A File Offset: 0x0004665A
	public bool IsLocked
	{
		get
		{
			return !Singleton<AchievementManager>.Instance.IsAchievementUnlocked(this.linkedAchievement);
		}
	}

	// Token: 0x06000E5F RID: 3679 RVA: 0x00048470 File Offset: 0x00046670
	public void AddToCSV()
	{
		string line = string.Concat(new string[]
		{
			"NAME_",
			this.displayName.ToUpperInvariant(),
			",",
			this.displayName.ToUpperInvariant(),
			" BADGE,,,,,,,,,,,,,ENDLINE"
		});
		string line2 = string.Concat(new string[]
		{
			"DESC_",
			this.displayName.ToUpperInvariant(),
			",\"",
			this.description,
			"\",,,,,,,,,,,,,,ENDLINE"
		});
		LocalizedText.AppendCSVLine(line, "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
		LocalizedText.AppendCSVLine(line2, "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
	}

	// Token: 0x04000C12 RID: 3090
	public Texture icon;

	// Token: 0x04000C13 RID: 3091
	public string displayName;

	// Token: 0x04000C14 RID: 3092
	public string description;

	// Token: 0x04000C15 RID: 3093
	public ACHIEVEMENTTYPE linkedAchievement;

	// Token: 0x04000C16 RID: 3094
	public bool testLocked;

	// Token: 0x04000C17 RID: 3095
	public int visualID;
}

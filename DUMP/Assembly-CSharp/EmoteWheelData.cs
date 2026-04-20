using System;
using UnityEngine;

// Token: 0x020001C7 RID: 455
[CreateAssetMenu(fileName = "EmoteWheelData", menuName = "Scriptable Objects/EmoteWheelData")]
public class EmoteWheelData : ScriptableObject
{
	// Token: 0x06000E92 RID: 3730 RVA: 0x00048DE6 File Offset: 0x00046FE6
	public void AddNameToCSV()
	{
		LocalizedText.AppendCSVLine(this.emoteName.ToUpperInvariant() + "," + this.emoteName.ToLowerInvariant() + ",,,,,,,,,,,,,ENDLINE", "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
	}

	// Token: 0x04000C3C RID: 3132
	public string emoteName;

	// Token: 0x04000C3D RID: 3133
	public Sprite emoteSprite;

	// Token: 0x04000C3E RID: 3134
	public string anim;

	// Token: 0x04000C3F RID: 3135
	public bool requireGrounded;
}

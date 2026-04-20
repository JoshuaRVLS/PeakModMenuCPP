using System;
using TMPro;
using UnityEngine;

// Token: 0x0200020D RID: 525
public class AutoLocalizer : MonoBehaviour
{
	// Token: 0x0600103E RID: 4158 RVA: 0x00050B88 File Offset: 0x0004ED88
	public void AutoLoc()
	{
		string text = base.GetComponent<TMP_Text>().text;
		if (this.toUpper)
		{
			text = text.ToUpper();
		}
		if (text.Contains(',') || text.Contains('.'))
		{
			text = "\"" + text + "\"";
		}
		LocalizedText.AppendCSVLine(this.index + "," + text + ",,,,,,,,,,,,,ENDLINE", "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
		LocalizedText localizedText = base.gameObject.AddComponent<LocalizedText>();
		localizedText.index = this.index;
		localizedText.DebugReload();
		Object.DestroyImmediate(this);
	}

	// Token: 0x04000E26 RID: 3622
	public string index;

	// Token: 0x04000E27 RID: 3623
	public bool toUpper;
}

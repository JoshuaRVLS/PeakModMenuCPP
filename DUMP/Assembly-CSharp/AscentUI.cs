using System;
using TMPro;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200005C RID: 92
public class AscentUI : MonoBehaviour
{
	// Token: 0x060004A5 RID: 1189 RVA: 0x0001C380 File Offset: 0x0001A580
	private void Start()
	{
		int currentAscent = Ascents._currentAscent;
		if (RunSettings.IsCustomRun)
		{
			this.text.text = SingletonAsset<AscentData>.Instance.ascents[0].localizedTitle;
			return;
		}
		this.text.text = SingletonAsset<AscentData>.Instance.ascents[currentAscent + 2].localizedTitle;
		if (currentAscent == 0)
		{
			this.text.text = "";
		}
	}

	// Token: 0x04000518 RID: 1304
	public TextMeshProUGUI text;
}

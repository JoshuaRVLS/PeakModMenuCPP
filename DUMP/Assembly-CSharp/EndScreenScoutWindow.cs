using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001CA RID: 458
public class EndScreenScoutWindow : MonoBehaviour
{
	// Token: 0x06000EB1 RID: 3761 RVA: 0x00049587 File Offset: 0x00047787
	public void Init(Character character)
	{
		if (character != null)
		{
			this.Init(character.IsLocal, character.characterName, character.refs.customization.PlayerColor);
		}
	}

	// Token: 0x06000EB2 RID: 3762 RVA: 0x000495B4 File Offset: 0x000477B4
	public void Init(bool isLocal, string characterName, Color scoutColor)
	{
		if (isLocal)
		{
			this.scoutName.fontStyle = FontStyles.Underline;
		}
		this.scoutName.text = characterName;
		Color color = scoutColor;
		color.a = this.panelAlpha;
		this.panel.color = color;
		this.altitude.text = "0m";
	}

	// Token: 0x06000EB3 RID: 3763 RVA: 0x00049607 File Offset: 0x00047807
	public void UpdateAltitude(int m)
	{
		this.altitude.text = m.ToString() + "m";
	}

	// Token: 0x04000C72 RID: 3186
	public TMP_Text scoutName;

	// Token: 0x04000C73 RID: 3187
	public TMP_Text altitude;

	// Token: 0x04000C74 RID: 3188
	public float panelAlpha = 0.25f;

	// Token: 0x04000C75 RID: 3189
	public Image panel;
}

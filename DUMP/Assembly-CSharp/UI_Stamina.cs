using System;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

// Token: 0x02000370 RID: 880
public class UI_Stamina : MonoBehaviour
{
	// Token: 0x06001727 RID: 5927 RVA: 0x00077193 File Offset: 0x00075393
	private void Update()
	{
		this.fill.fillAmount = Character.localCharacter.data.currentStamina;
	}

	// Token: 0x040015A4 RID: 5540
	public ProceduralImage fill;
}

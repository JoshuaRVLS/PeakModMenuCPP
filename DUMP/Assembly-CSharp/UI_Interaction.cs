using System;
using TMPro;
using UnityEngine;

// Token: 0x0200036D RID: 877
public class UI_Interaction : MonoBehaviour
{
	// Token: 0x0600171D RID: 5917 RVA: 0x00076E37 File Offset: 0x00075037
	private void Start()
	{
		this.text = base.GetComponentInChildren<TextMeshProUGUI>();
	}

	// Token: 0x0600171E RID: 5918 RVA: 0x00076E45 File Offset: 0x00075045
	private void Update()
	{
		this.OnChange();
	}

	// Token: 0x0600171F RID: 5919 RVA: 0x00076E50 File Offset: 0x00075050
	private void OnChange()
	{
		this.current = Interaction.instance.currentHovered;
		if (this.current != null)
		{
			this.text.text = this.current.GetInteractionText();
			return;
		}
		this.text.text = "";
	}

	// Token: 0x04001591 RID: 5521
	private TextMeshProUGUI text;

	// Token: 0x04001592 RID: 5522
	private IInteractible current;
}

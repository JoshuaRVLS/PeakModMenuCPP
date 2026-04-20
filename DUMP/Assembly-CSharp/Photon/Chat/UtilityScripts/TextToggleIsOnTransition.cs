using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Photon.Chat.UtilityScripts
{
	// Token: 0x020003B0 RID: 944
	[RequireComponent(typeof(Text))]
	public class TextToggleIsOnTransition : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x0600193A RID: 6458 RVA: 0x0007F307 File Offset: 0x0007D507
		public void OnEnable()
		{
			this._text = base.GetComponent<Text>();
			this.OnValueChanged(this.toggle.isOn);
			this.toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnValueChanged));
		}

		// Token: 0x0600193B RID: 6459 RVA: 0x0007F342 File Offset: 0x0007D542
		public void OnDisable()
		{
			this.toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnValueChanged));
		}

		// Token: 0x0600193C RID: 6460 RVA: 0x0007F360 File Offset: 0x0007D560
		public void OnValueChanged(bool isOn)
		{
			this._text.color = (isOn ? (this.isHover ? this.HoverOnColor : this.HoverOnColor) : (this.isHover ? this.NormalOffColor : this.NormalOffColor));
		}

		// Token: 0x0600193D RID: 6461 RVA: 0x0007F39E File Offset: 0x0007D59E
		public void OnPointerEnter(PointerEventData eventData)
		{
			this.isHover = true;
			this._text.color = (this.toggle.isOn ? this.HoverOnColor : this.HoverOffColor);
		}

		// Token: 0x0600193E RID: 6462 RVA: 0x0007F3CD File Offset: 0x0007D5CD
		public void OnPointerExit(PointerEventData eventData)
		{
			this.isHover = false;
			this._text.color = (this.toggle.isOn ? this.NormalOnColor : this.NormalOffColor);
		}

		// Token: 0x040016FF RID: 5887
		public Toggle toggle;

		// Token: 0x04001700 RID: 5888
		private Text _text;

		// Token: 0x04001701 RID: 5889
		public Color NormalOnColor = Color.white;

		// Token: 0x04001702 RID: 5890
		public Color NormalOffColor = Color.black;

		// Token: 0x04001703 RID: 5891
		public Color HoverOnColor = Color.black;

		// Token: 0x04001704 RID: 5892
		public Color HoverOffColor = Color.black;

		// Token: 0x04001705 RID: 5893
		private bool isHover;
	}
}

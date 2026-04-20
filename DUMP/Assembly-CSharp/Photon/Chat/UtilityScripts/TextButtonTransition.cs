using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Photon.Chat.UtilityScripts
{
	// Token: 0x020003AF RID: 943
	[RequireComponent(typeof(Text))]
	public class TextButtonTransition : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x06001934 RID: 6452 RVA: 0x0007F259 File Offset: 0x0007D459
		public void Awake()
		{
			this._text = base.GetComponent<Text>();
		}

		// Token: 0x06001935 RID: 6453 RVA: 0x0007F267 File Offset: 0x0007D467
		public void OnEnable()
		{
			this._text.color = this.NormalColor;
		}

		// Token: 0x06001936 RID: 6454 RVA: 0x0007F27A File Offset: 0x0007D47A
		public void OnDisable()
		{
			this._text.color = this.NormalColor;
		}

		// Token: 0x06001937 RID: 6455 RVA: 0x0007F28D File Offset: 0x0007D48D
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this.Selectable == null || this.Selectable.IsInteractable())
			{
				this._text.color = this.HoverColor;
			}
		}

		// Token: 0x06001938 RID: 6456 RVA: 0x0007F2BB File Offset: 0x0007D4BB
		public void OnPointerExit(PointerEventData eventData)
		{
			if (this.Selectable == null || this.Selectable.IsInteractable())
			{
				this._text.color = this.NormalColor;
			}
		}

		// Token: 0x040016FB RID: 5883
		private Text _text;

		// Token: 0x040016FC RID: 5884
		public Selectable Selectable;

		// Token: 0x040016FD RID: 5885
		public Color NormalColor = Color.white;

		// Token: 0x040016FE RID: 5886
		public Color HoverColor = Color.black;
	}
}

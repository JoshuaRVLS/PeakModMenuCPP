using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Zorro.ControllerSupport;

namespace Peak.UI
{
	// Token: 0x020003D2 RID: 978
	public class SelectableSlider : MonoBehaviour, ISubmitHandler, IEventSystemHandler, ISelectHandler, IDeselectHandler
	{
		// Token: 0x1700018E RID: 398
		// (get) Token: 0x060019CD RID: 6605 RVA: 0x000820F0 File Offset: 0x000802F0
		private bool ChildIsSelected
		{
			get
			{
				return EventSystem.current.currentSelectedGameObject == this._childSlider.gameObject;
			}
		}

		// Token: 0x1700018F RID: 399
		// (get) Token: 0x060019CE RID: 6606 RVA: 0x0008210C File Offset: 0x0008030C
		public Selectable MySelectable
		{
			get
			{
				return base.GetComponent<Selectable>();
			}
		}

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x060019CF RID: 6607 RVA: 0x00082114 File Offset: 0x00080314
		public Slider ChildSlider
		{
			get
			{
				return this._childSlider;
			}
		}

		// Token: 0x17000191 RID: 401
		// (get) Token: 0x060019D0 RID: 6608 RVA: 0x0008211C File Offset: 0x0008031C
		public KickButton KickButton
		{
			get
			{
				return this._kickButton;
			}
		}

		// Token: 0x060019D1 RID: 6609 RVA: 0x00082124 File Offset: 0x00080324
		private void Update()
		{
			this.MySelectable.interactable = (InputHandler.GetCurrentUsedInputScheme() > InputScheme.KeyboardMouse);
		}

		// Token: 0x060019D2 RID: 6610 RVA: 0x0008213C File Offset: 0x0008033C
		public void Init()
		{
			if (this._init)
			{
				return;
			}
			this._init = true;
			this._childSlider = base.GetComponentInChildren<Slider>(true);
			this._childSlider.onValueChanged.AddListener(new UnityAction<float>(this.EnsureReselected));
			this._events = this._childSlider.gameObject.GetOrAddComponent<SelectableEvents>();
			this._events.Submitted += this.SelectSelf;
			this._kickButton = base.GetComponentInChildren<KickButton>(true);
			this._kickButton.Init();
		}

		// Token: 0x060019D3 RID: 6611 RVA: 0x000821C7 File Offset: 0x000803C7
		private void SelectSelf(BaseEventData _)
		{
			this.MySelectable.Select();
		}

		// Token: 0x060019D4 RID: 6612 RVA: 0x000821D4 File Offset: 0x000803D4
		private void EnsureReselected(float _)
		{
			if (InputHandler.GetCurrentUsedInputScheme() != InputScheme.KeyboardMouse)
			{
				this._childSlider.Select();
			}
		}

		// Token: 0x060019D5 RID: 6613 RVA: 0x000821E8 File Offset: 0x000803E8
		public void OnSubmit(BaseEventData eventData)
		{
			this._childSlider.Select();
		}

		// Token: 0x060019D6 RID: 6614 RVA: 0x000821F5 File Offset: 0x000803F5
		public void OnSelect(BaseEventData eventData)
		{
		}

		// Token: 0x060019D7 RID: 6615 RVA: 0x000821F7 File Offset: 0x000803F7
		public void OnDeselect(BaseEventData eventData)
		{
		}

		// Token: 0x04001756 RID: 5974
		private Color _defaultNameColor;

		// Token: 0x04001757 RID: 5975
		private Slider _childSlider;

		// Token: 0x04001758 RID: 5976
		private SelectableEvents _events;

		// Token: 0x04001759 RID: 5977
		private KickButton _kickButton;

		// Token: 0x0400175A RID: 5978
		private bool _init;
	}
}

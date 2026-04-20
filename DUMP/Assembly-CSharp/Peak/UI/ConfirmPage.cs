using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.ControllerSupport;
using Zorro.Core;

namespace Peak.UI
{
	// Token: 0x020003CE RID: 974
	public class ConfirmPage : RetrievableResourceSingleton<ConfirmPage>, INavigationContainer
	{
		// Token: 0x060019B4 RID: 6580 RVA: 0x00081D91 File Offset: 0x0007FF91
		private void Update()
		{
			INavigationContainer.PushActive(this);
		}

		// Token: 0x060019B5 RID: 6581 RVA: 0x00081D99 File Offset: 0x0007FF99
		private void OnDestroy()
		{
			this.ClearTemporaryCallbacks();
			this.ok.onClick.RemoveAllListeners();
			this.cancel.onClick.RemoveAllListeners();
		}

		// Token: 0x060019B6 RID: 6582 RVA: 0x00081DC1 File Offset: 0x0007FFC1
		private void ClearTemporaryCallbacks()
		{
			this._okClicked = null;
			this._cancelClicked = null;
		}

		// Token: 0x060019B7 RID: 6583 RVA: 0x00081DD1 File Offset: 0x0007FFD1
		public static void Open(string localizedPromptKey, Action onOk, Action onCancel = null)
		{
			ConfirmPage instance = RetrievableResourceSingleton<ConfirmPage>.Instance;
			instance.prompt.SetTextLocalized(localizedPromptKey);
			instance._okClicked = onOk;
			instance._cancelClicked = onCancel;
			instance._timeLastOpened = Time.realtimeSinceStartup;
			instance.gameObject.SetActive(true);
		}

		// Token: 0x060019B8 RID: 6584 RVA: 0x00081E08 File Offset: 0x00080008
		protected override void OnCreated()
		{
			this.ok.onClick.AddListener(new UnityAction(this.OnOk));
			this.cancel.onClick.AddListener(new UnityAction(this.OnCancel));
		}

		// Token: 0x060019B9 RID: 6585 RVA: 0x00081E42 File Offset: 0x00080042
		public int GetContainerPriority()
		{
			return 99;
		}

		// Token: 0x060019BA RID: 6586 RVA: 0x00081E46 File Offset: 0x00080046
		public GameObject GetDefaultSelection()
		{
			if (!this.okIsDefault)
			{
				return this.cancel.gameObject;
			}
			return this.ok.gameObject;
		}

		// Token: 0x060019BB RID: 6587 RVA: 0x00081E67 File Offset: 0x00080067
		public bool IsValidSelection(GameObject selection)
		{
			return this.ok.gameObject == selection || this.cancel.gameObject == selection;
		}

		// Token: 0x060019BC RID: 6588 RVA: 0x00081E87 File Offset: 0x00080087
		private void Close()
		{
			base.gameObject.SetActive(false);
			this.ClearTemporaryCallbacks();
		}

		// Token: 0x060019BD RID: 6589 RVA: 0x00081E9B File Offset: 0x0008009B
		private void OnOk()
		{
			if (Time.realtimeSinceStartup - this._timeLastOpened < this.debouncePeriodInSeconds)
			{
				return;
			}
			Action okClicked = this._okClicked;
			if (okClicked != null)
			{
				okClicked();
			}
			this.Close();
		}

		// Token: 0x060019BE RID: 6590 RVA: 0x00081EC9 File Offset: 0x000800C9
		private void OnCancel()
		{
			if (Time.realtimeSinceStartup - this._timeLastOpened < this.debouncePeriodInSeconds)
			{
				return;
			}
			Action cancelClicked = this._cancelClicked;
			if (cancelClicked != null)
			{
				cancelClicked();
			}
			this.Close();
		}

		// Token: 0x04001749 RID: 5961
		private static ConfirmPage _instance;

		// Token: 0x0400174A RID: 5962
		[SerializeField]
		private LocalizedText prompt;

		// Token: 0x0400174B RID: 5963
		[SerializeField]
		private float debouncePeriodInSeconds = 0.2f;

		// Token: 0x0400174C RID: 5964
		[SerializeField]
		private Button ok;

		// Token: 0x0400174D RID: 5965
		[SerializeField]
		private Button cancel;

		// Token: 0x0400174E RID: 5966
		[SerializeField]
		private bool okIsDefault;

		// Token: 0x0400174F RID: 5967
		private Action _okClicked;

		// Token: 0x04001750 RID: 5968
		private Action _cancelClicked;

		// Token: 0x04001751 RID: 5969
		private float _timeLastOpened;
	}
}

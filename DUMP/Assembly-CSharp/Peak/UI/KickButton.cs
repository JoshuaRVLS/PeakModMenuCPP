using System;
using Peak.Network;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Peak.UI
{
	// Token: 0x020003CF RID: 975
	public class KickButton : MonoBehaviour
	{
		// Token: 0x1700018D RID: 397
		// (get) Token: 0x060019C0 RID: 6592 RVA: 0x00081F0A File Offset: 0x0008010A
		public Button MyButton
		{
			get
			{
				return this._button;
			}
		}

		// Token: 0x060019C1 RID: 6593 RVA: 0x00081F14 File Offset: 0x00080114
		public void Init()
		{
			if (this._init)
			{
				return;
			}
			this._button = base.GetComponent<Button>();
			this._button.onClick.AddListener(new UnityAction(this.OpenKickMenu));
			this._slider = base.GetComponentInParent<AudioLevelSlider>();
			this.SetInteractable();
		}

		// Token: 0x060019C2 RID: 6594 RVA: 0x00081F64 File Offset: 0x00080164
		private void OnDestroy()
		{
			if (this._button != null)
			{
				this._button.onClick.RemoveListener(new UnityAction(this.Kick));
			}
		}

		// Token: 0x060019C3 RID: 6595 RVA: 0x00081F90 File Offset: 0x00080190
		private void Update()
		{
			if (!this._init)
			{
				this.Init();
			}
			this.SetInteractable();
		}

		// Token: 0x060019C4 RID: 6596 RVA: 0x00081FA8 File Offset: 0x000801A8
		private void SetInteractable()
		{
			Player player;
			if (!PlayerHandler.TryGetPlayer(this._slider.player.ActorNumber, out player))
			{
				this._slider.gameObject.SetActive(false);
				return;
			}
			bool isHost = NetCode.Session.IsHost;
			this._button.interactable = isHost;
			this._button.gameObject.SetActive(isHost);
		}

		// Token: 0x060019C5 RID: 6597 RVA: 0x00082008 File Offset: 0x00080208
		private void OpenKickMenu()
		{
			GUIManager.instance.pauseMenuMainPage.OpenKickConfirmWindow(this._slider.player.NickName).onClick.AddListener(new UnityAction(this.Kick));
		}

		// Token: 0x060019C6 RID: 6598 RVA: 0x0008203F File Offset: 0x0008023F
		private void Kick()
		{
			PlayerHandler.Kick(this._slider.player.ActorNumber);
		}

		// Token: 0x04001752 RID: 5970
		private bool _init;

		// Token: 0x04001753 RID: 5971
		private Button _button;

		// Token: 0x04001754 RID: 5972
		private AudioLevelSlider _slider;
	}
}

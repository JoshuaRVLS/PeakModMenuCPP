using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.Core;

// Token: 0x02000220 RID: 544
public class BoardingPass : MenuWindow
{
	// Token: 0x17000127 RID: 295
	// (get) Token: 0x06001097 RID: 4247 RVA: 0x00052929 File Offset: 0x00050B29
	public override bool openOnStart
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000128 RID: 296
	// (get) Token: 0x06001098 RID: 4248 RVA: 0x0005292C File Offset: 0x00050B2C
	public override bool selectOnOpen
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000129 RID: 297
	// (get) Token: 0x06001099 RID: 4249 RVA: 0x0005292F File Offset: 0x00050B2F
	public override bool closeOnPause
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700012A RID: 298
	// (get) Token: 0x0600109A RID: 4250 RVA: 0x00052932 File Offset: 0x00050B32
	public override bool closeOnUICancel
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700012B RID: 299
	// (get) Token: 0x0600109B RID: 4251 RVA: 0x00052935 File Offset: 0x00050B35
	public override bool autoHideOnClose
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700012C RID: 300
	// (get) Token: 0x0600109C RID: 4252 RVA: 0x00052938 File Offset: 0x00050B38
	// (set) Token: 0x0600109D RID: 4253 RVA: 0x00052940 File Offset: 0x00050B40
	public int ascentIndex
	{
		get
		{
			return this._ascentIndex;
		}
		set
		{
			this._ascentIndex = value;
		}
	}

	// Token: 0x1700012D RID: 301
	// (get) Token: 0x0600109E RID: 4254 RVA: 0x0005294C File Offset: 0x00050B4C
	public override Selectable objectToSelectOnOpen
	{
		get
		{
			if (this.customRunButton.gameObject.activeInHierarchy && !this.decrementAscentButton.interactable && !this.incrementAscentButton.interactable)
			{
				return this.customRunButton;
			}
			if (this.decrementAscentButton.gameObject.activeInHierarchy)
			{
				return this.decrementAscentButton;
			}
			if (this.incrementAscentButton.gameObject.activeInHierarchy)
			{
				return this.incrementAscentButton;
			}
			return this.startGameButton;
		}
	}

	// Token: 0x0600109F RID: 4255 RVA: 0x000529C4 File Offset: 0x00050BC4
	protected override void Initialize()
	{
		this.incrementAscentButton.onClick.AddListener(new UnityAction(this.IncrementAscent));
		this.decrementAscentButton.onClick.AddListener(new UnityAction(this.DecrementAscent));
		this.customRunButton.onClick.AddListener(new UnityAction(this.ToggleCustom));
		this.customOptionsButton.onClick.AddListener(new UnityAction(this.OpenCustom));
		this.startGameButton.onClick.AddListener(new UnityAction(this.StartGame));
		this.closeButton.onClick.AddListener(new UnityAction(base.Close));
		this.UpdateAscent();
	}

	// Token: 0x060010A0 RID: 4256 RVA: 0x00052A7F File Offset: 0x00050C7F
	private void InitMaxAscent()
	{
		this.maxUnlockedAscent = 0;
		Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.MaxAscent, out this.maxUnlockedAscent);
	}

	// Token: 0x060010A1 RID: 4257 RVA: 0x00052A9C File Offset: 0x00050C9C
	protected override void OnOpen()
	{
		this.playerName.text = Character.localCharacter.characterName;
		List<Character> allCharacters = Character.AllCharacters;
		for (int i = 0; i < this.players.Length; i++)
		{
			if (allCharacters.Count > i)
			{
				this.players[i].gameObject.SetActive(true);
				this.players[i].color = allCharacters[i].refs.customization.PlayerColor;
			}
			else
			{
				this.players[i].gameObject.SetActive(false);
			}
		}
		if (this.openingFromCustomWindow)
		{
			this.openingFromCustomWindow = false;
			this.anim.Play("BoardingPassIn", 0, 1f);
			this.canvasGroup.alpha = 1f;
		}
		else
		{
			this.canvasGroup.alpha = 0f;
			this.canvasGroup.DOFade(1f, 0.5f);
		}
		this.UpdateAscent();
	}

	// Token: 0x060010A2 RID: 4258 RVA: 0x00052B8E File Offset: 0x00050D8E
	protected override void OnClose()
	{
		base.StartCoroutine(this.CloseRoutine());
	}

	// Token: 0x060010A3 RID: 4259 RVA: 0x00052B9D File Offset: 0x00050D9D
	private IEnumerator CloseRoutine()
	{
		TweenerCore<float, float, FloatOptions> tween = this.canvasGroup.DOFade(0f, 0.2f);
		while (tween.IsPlaying())
		{
			if (base.isOpen)
			{
				tween.Kill(false);
				yield break;
			}
			yield return null;
		}
		if (!base.isOpen)
		{
			this.HideIt();
		}
		yield break;
	}

	// Token: 0x060010A4 RID: 4260 RVA: 0x00052BAC File Offset: 0x00050DAC
	private void HideIt()
	{
		base.Hide();
	}

	// Token: 0x060010A5 RID: 4261 RVA: 0x00052BB4 File Offset: 0x00050DB4
	private void UpdateAscent()
	{
		this.customRunButton.GetComponent<Animator>().SetFloat("Custom", (float)(RunSettings.IsCustomRun ? 1 : 0));
		this.maxUnlockedAscent = Singleton<AchievementManager>.Instance.GetMaxAscent();
		int num = Mathf.Min(this.maxAscent, this.maxUnlockedAscent);
		this.incrementAscentButton.interactable = (this.ascentIndex < num && !RunSettings.IsCustomRun);
		this.decrementAscentButton.interactable = (this.ascentIndex > -1 && !RunSettings.IsCustomRun);
		this.ascentTitle.text = this.ascentData.ascents[this.ascentIndex + 2].localizedTitle;
		this.ascentTitle.color = (RunSettings.IsCustomRun ? this.customColor : this.defaultColor);
		this.ascentDesc.text = this.ascentData.ascents[this.ascentIndex + 2].localizedDescription;
		if (this.ascentIndex >= 2)
		{
			TMP_Text tmp_Text = this.ascentDesc;
			tmp_Text.text = tmp_Text.text + "\n\n<alpha=#CC><size=70%>" + LocalizedText.GetText("ANDALLOTHER", true);
		}
		this.customOptionsButton.gameObject.SetActive(false);
		if (RunSettings.IsCustomRun)
		{
			this.customOptionsButton.gameObject.SetActive(true);
			this.customOptionsButton.GetComponent<Animator>().SetFloat("Custom", 1f);
			this.ascentTitle.text = this.ascentData.ascents[0].localizedTitle;
			this.ascentDesc.text = this.ascentData.ascents[0].localizedDescription;
			this.reward.gameObject.SetActive(false);
			return;
		}
		if (this.ascentIndex == this.maxUnlockedAscent && this.ascentIndex > -1 && this.ascentIndex < 8)
		{
			this.reward.gameObject.SetActive(true);
			this.rewardText.text = this.ascentData.ascents[this.ascentIndex + 2].localizedReward;
			this.rewardImage.color = this.ascentData.ascents[this.ascentIndex + 2].color;
			return;
		}
		this.reward.gameObject.SetActive(false);
	}

	// Token: 0x060010A6 RID: 4262 RVA: 0x00052E0C File Offset: 0x0005100C
	public void IncrementAscent()
	{
		int ascentIndex = this.ascentIndex;
		this.ascentIndex = ascentIndex + 1;
		this.UpdateAscent();
	}

	// Token: 0x060010A7 RID: 4263 RVA: 0x00052E30 File Offset: 0x00051030
	public void DecrementAscent()
	{
		int ascentIndex = this.ascentIndex;
		this.ascentIndex = ascentIndex - 1;
		this.UpdateAscent();
	}

	// Token: 0x060010A8 RID: 4264 RVA: 0x00052E53 File Offset: 0x00051053
	public void StartGame()
	{
		if (RunSettings.IsCustomRun)
		{
			this.ascentIndex = 0;
		}
		this.kiosk.StartGame(this.ascentIndex);
	}

	// Token: 0x060010A9 RID: 4265 RVA: 0x00052E74 File Offset: 0x00051074
	private void ToggleCustom()
	{
		if (LoadingScreenHandler.loading)
		{
			return;
		}
		RunSettings.IsCustomRun = !RunSettings.IsCustomRun;
		this.UpdateAscent();
	}

	// Token: 0x060010AA RID: 4266 RVA: 0x00052E91 File Offset: 0x00051091
	public void OpenCustom()
	{
		base.Close();
		this.customOptionsWindow.Open();
	}

	// Token: 0x04000E86 RID: 3718
	public TMP_Text playerName;

	// Token: 0x04000E87 RID: 3719
	public TMP_Text ascentTitle;

	// Token: 0x04000E88 RID: 3720
	public TMP_Text ascentDesc;

	// Token: 0x04000E89 RID: 3721
	public GameObject reward;

	// Token: 0x04000E8A RID: 3722
	public Image rewardImage;

	// Token: 0x04000E8B RID: 3723
	public TextMeshProUGUI rewardText;

	// Token: 0x04000E8C RID: 3724
	public Image[] players;

	// Token: 0x04000E8D RID: 3725
	private int _ascentIndex;

	// Token: 0x04000E8E RID: 3726
	private int maxAscent = 7;

	// Token: 0x04000E8F RID: 3727
	private int maxUnlockedAscent;

	// Token: 0x04000E90 RID: 3728
	public AirportCheckInKiosk kiosk;

	// Token: 0x04000E91 RID: 3729
	public CustomOptionsWindow customOptionsWindow;

	// Token: 0x04000E92 RID: 3730
	public Color defaultColor;

	// Token: 0x04000E93 RID: 3731
	public Color customColor;

	// Token: 0x04000E94 RID: 3732
	public Button incrementAscentButton;

	// Token: 0x04000E95 RID: 3733
	public Button decrementAscentButton;

	// Token: 0x04000E96 RID: 3734
	public Button customRunButton;

	// Token: 0x04000E97 RID: 3735
	public Button customOptionsButton;

	// Token: 0x04000E98 RID: 3736
	public Button startGameButton;

	// Token: 0x04000E99 RID: 3737
	public Button closeButton;

	// Token: 0x04000E9A RID: 3738
	public AscentData ascentData;

	// Token: 0x04000E9B RID: 3739
	public CanvasGroup canvasGroup;

	// Token: 0x04000E9C RID: 3740
	public bool openingFromCustomWindow;

	// Token: 0x04000E9D RID: 3741
	public Animator anim;
}

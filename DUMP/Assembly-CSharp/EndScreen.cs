using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.Core;

// Token: 0x020001C9 RID: 457
public class EndScreen : MenuWindow
{
	// Token: 0x17000108 RID: 264
	// (get) Token: 0x06000E9B RID: 3739 RVA: 0x00048F19 File Offset: 0x00047119
	public override bool openOnStart
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000109 RID: 265
	// (get) Token: 0x06000E9C RID: 3740 RVA: 0x00048F1C File Offset: 0x0004711C
	public override Selectable objectToSelectOnOpen
	{
		get
		{
			return null;
		}
	}

	// Token: 0x1700010A RID: 266
	// (get) Token: 0x06000E9D RID: 3741 RVA: 0x00048F1F File Offset: 0x0004711F
	public override bool selectOnOpen
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000E9E RID: 3742 RVA: 0x00048F22 File Offset: 0x00047122
	private void Awake()
	{
		EndScreen.instance = this;
	}

	// Token: 0x06000E9F RID: 3743 RVA: 0x00048F2A File Offset: 0x0004712A
	protected override void Start()
	{
		base.Start();
		base.StartCoroutine(this.EndSequenceRoutine());
	}

	// Token: 0x06000EA0 RID: 3744 RVA: 0x00048F40 File Offset: 0x00047140
	protected override void Initialize()
	{
		this.nextButton.onClick.AddListener(new UnityAction(this.Next));
		this.cosmeticNextButton.onClick.AddListener(new UnityAction(this.PopupNext));
		this.ascentsNextButton.onClick.AddListener(new UnityAction(this.PopupNext));
		this.promotionNextButton.onClick.AddListener(new UnityAction(this.PopupNext));
	}

	// Token: 0x06000EA1 RID: 3745 RVA: 0x00048FBD File Offset: 0x000471BD
	private void Next()
	{
		this.WaitingForPlayersUI.gameObject.SetActive(true);
		Singleton<GameOverHandler>.Instance.LocalPlayerHasClosedEndScreen();
	}

	// Token: 0x06000EA2 RID: 3746 RVA: 0x00048FDA File Offset: 0x000471DA
	private IEnumerator EndSequenceRoutine()
	{
		UIInputHandler.SetSelectedObject(null);
		this.canvasGroup.alpha = 0f;
		this.canvasGroup.DOFade(1f, 1f);
		List<Character> activeCharacters = Character.AllCharacters.Where(delegate(Character c)
		{
			Player player;
			return c.photonView.IsOwnerActive && PlayerHandler.TryGetPlayer(c.photonView.OwnerActorNr, out player);
		}).ToList<Character>();
		if (activeCharacters.Count != Character.AllCharacters.Count)
		{
			Debug.LogWarning(string.Format("There were {0} broken Characters ", Character.AllCharacters.Count - activeCharacters.Count) + "in our character list! Why didn't those get destroyed??");
		}
		for (int i = 0; i < this.scoutWindows.Length; i++)
		{
			if (i < activeCharacters.Count)
			{
				this.scoutWindows[i].gameObject.SetActive(true);
				this.scoutWindows[i].Init(activeCharacters[i]);
			}
			else
			{
				this.scoutWindows[i].gameObject.SetActive(false);
			}
		}
		this.endTime.gameObject.SetActive(false);
		this.buttons.SetActive(false);
		this.peakBanner.SetActive(Character.localCharacter.refs.stats.won && !RunSettings.isMiniRun);
		this.miniWinBanner.SetActive(Character.localCharacter.refs.stats.won && RunSettings.isMiniRun);
		this.yourFriendsWonBanner.SetActive(!Character.localCharacter.refs.stats.won && Character.localCharacter.refs.stats.somebodyElseWon);
		this.deadBanner.SetActive(!Character.localCharacter.refs.stats.won && !Character.localCharacter.refs.stats.somebodyElseWon);
		this.cosmeticUnlockObject.SetActive(false);
		yield return new WaitForSeconds(2f);
		try
		{
			this.endTime.text = this.GetTimeString(RunManager.Instance.timeSinceRunStarted);
			this.endTime.gameObject.SetActive(true);
		}
		catch (Exception value)
		{
			Console.WriteLine(value);
		}
		if (Character.localCharacter.refs.stats.won)
		{
			Singleton<AchievementManager>.Instance.TestTimeAchievements();
		}
		yield return new WaitForSeconds(1f);
		yield return base.StartCoroutine(this.TimelineRoutine(activeCharacters));
		yield return new WaitForSeconds(0.25f);
		List<int> completedAscentsThisRun = Singleton<AchievementManager>.Instance.runBasedValueData.completedAscentsThisRun;
		yield return base.StartCoroutine(this.AscentRoutine(completedAscentsThisRun));
		yield return new WaitForSeconds(0.25f);
		this.selectedBadge = false;
		yield return base.StartCoroutine(this.BadgeRoutine());
		this.buttons.SetActive(true);
		if (!this.selectedBadge)
		{
			UIInputHandler.SetSelectedObject(this.returnToAirportButton.gameObject);
		}
		yield break;
	}

	// Token: 0x06000EA3 RID: 3747 RVA: 0x00048FEC File Offset: 0x000471EC
	private string GetTimeString(float totalSeconds)
	{
		int num = Mathf.FloorToInt(totalSeconds);
		int num2 = num / 3600;
		int num3 = num % 3600 / 60;
		int num4 = num % 60;
		return string.Format("{0}:{1:00}:{2:00}", num2, num3, num4);
	}

	// Token: 0x06000EA4 RID: 3748 RVA: 0x00049032 File Offset: 0x00047232
	private IEnumerator TimelineRoutine(List<Character> allCharacters)
	{
		for (int j = 0; j < this.scouts.Length; j++)
		{
			this.scouts[j].gameObject.SetActive(false);
			this.scoutsAtPeak[j].gameObject.SetActive(false);
		}
		for (int k = allCharacters.Count - 1; k >= 0; k--)
		{
			if (allCharacters[k] == null)
			{
				Debug.LogWarning(string.Format("Ended up with a null character at index {0} out of {1} characters ", k, allCharacters.Count) + "in the array. This is messed up, because we should have already pruned inactive characters...");
				allCharacters.RemoveAt(k);
			}
			else if (allCharacters[k].refs.stats.timelineInfo.Count == 0)
			{
				Debug.LogWarning("Character " + allCharacters[k].name + " has no info in their scout report! Their timeline must be broken for some reason. Better prune them.");
				allCharacters.RemoveAt(k);
			}
		}
		for (int l = 0; l < allCharacters.Count; l++)
		{
			if (l < this.scouts.Length)
			{
				Color playerColor = allCharacters[l].refs.customization.PlayerColor;
				playerColor.a = 1f;
				this.scouts[l].color = playerColor;
				this.scoutsAtPeak[l].color = this.scouts[l].color;
			}
		}
		yield return new WaitForSeconds(0.1f);
		List<List<EndScreen.TimelineInfo>> timelineInfos = new List<List<EndScreen.TimelineInfo>>();
		for (int m = 0; m < allCharacters.Count; m++)
		{
			timelineInfos.Add(allCharacters[m].refs.stats.timelineInfo);
		}
		for (int n = 0; n < timelineInfos.Count; n++)
		{
			if (n < this.scouts.Length)
			{
				this.scouts[n].gameObject.SetActive(true);
			}
		}
		int num = 1;
		for (int num2 = 0; num2 < timelineInfos.Count; num2++)
		{
			if (timelineInfos[num2].Count > num)
			{
				num = timelineInfos[num2].Count;
			}
		}
		float startTime = 100000f;
		float maxTime = 0f;
		maxTime = Character.localCharacter.refs.stats.GetFinalTimelineInfo().time;
		startTime = Character.localCharacter.refs.stats.GetFirstTimelineInfo().time;
		maxTime -= startTime;
		if (maxTime == 0f)
		{
			maxTime = 1f;
		}
		List<EndScreen.PipData> list = new List<EndScreen.PipData>();
		int num3 = 0;
		while (num3 < this.scouts.Length && num3 < timelineInfos.Count)
		{
			for (int num4 = 0; num4 < timelineInfos[num3].Count; num4++)
			{
				list.Add(new EndScreen.PipData
				{
					playerIdx = num3,
					info = timelineInfos[num3][num4]
				});
			}
			num3++;
		}
		IOrderedEnumerable<EndScreen.PipData> orderedEnumerable = from data in list
		orderby data.info.timestamp
		select data;
		float num5 = 5f * Mathf.Min(7200f, maxTime) / 7200f;
		float realSecondsToReportSeconds = maxTime / num5;
		float nextYield = Time.deltaTime * realSecondsToReportSeconds + startTime;
		foreach (EndScreen.PipData pipData in orderedEnumerable)
		{
			EndScreen.TimelineInfo info = pipData.info;
			if (info.height <= 1300f || (float)pipData.info.timestamp >= startTime + maxTime - 10f)
			{
				while ((float)pipData.info.timestamp > nextYield)
				{
					yield return null;
					nextYield += Time.deltaTime * realSecondsToReportSeconds;
				}
				this.DrawPip(pipData.playerIdx, pipData.info, maxTime, startTime, this.scouts[pipData.playerIdx].color);
				info = pipData.info;
				if (!info.dead)
				{
					info = pipData.info;
					if (!info.died)
					{
						EndScreenScoutWindow endScreenScoutWindow = this.scoutWindows[pipData.playerIdx];
						info = pipData.info;
						endScreenScoutWindow.UpdateAltitude(CharacterStats.UnitsToMeters(info.height));
					}
				}
			}
		}
		IEnumerator<EndScreen.PipData> enumerator = null;
		int num6;
		for (int i = 0; i < timelineInfos.Count; i = num6 + 1)
		{
			Debug.Log(string.Format("Checking timeline info {0}, has infos: {1}", i, timelineInfos[i].Count));
			if (timelineInfos[i].Count > 0)
			{
				this.CheckPeak(i, timelineInfos[i][timelineInfos[i].Count - 1]);
				yield return new WaitForSeconds(0.25f);
			}
			num6 = i;
		}
		yield break;
		yield break;
	}

	// Token: 0x06000EA5 RID: 3749 RVA: 0x00049048 File Offset: 0x00047248
	private List<BadgeData> GetBadgeUnlocks()
	{
		List<BadgeData> list = new List<BadgeData>();
		foreach (ACHIEVEMENTTYPE achievementType in Singleton<AchievementManager>.Instance.runBasedValueData.achievementsEarnedThisRun)
		{
			BadgeData badgeData = GUIManager.instance.mainBadgeManager.GetBadgeData(achievementType);
			if (badgeData != null)
			{
				list.Add(badgeData);
			}
		}
		return list;
	}

	// Token: 0x06000EA6 RID: 3750 RVA: 0x000490C8 File Offset: 0x000472C8
	private IEnumerator AscentRoutine(List<int> completedAscentsThisRun)
	{
		if (completedAscentsThisRun.Count > 0 && completedAscentsThisRun[0] == 0)
		{
			yield return this.AscentsUnlockRoutine();
		}
		int num;
		for (int i = 0; i < completedAscentsThisRun.Count; i = num + 1)
		{
			yield return new WaitForSeconds(0.5f);
			yield return this.PromotionUnlockRoutine(completedAscentsThisRun[i]);
			num = i;
		}
		yield return null;
		yield break;
	}

	// Token: 0x06000EA7 RID: 3751 RVA: 0x000490DE File Offset: 0x000472DE
	private IEnumerator BadgeRoutine()
	{
		BadgeManager bm = base.GetComponent<BadgeManager>();
		bm.InheritData(GUIManager.instance.mainBadgeManager);
		List<BadgeData> badgeUnlocks = this.GetBadgeUnlocks();
		List<ACHIEVEMENTTYPE> achievementsEarnedThisRun = Singleton<AchievementManager>.Instance.runBasedValueData.achievementsEarnedThisRun;
		bool flag = false;
		bool unlockedCrown = false;
		for (int j = 0; j < achievementsEarnedThisRun.Count; j++)
		{
			if (achievementsEarnedThisRun[j] >= ACHIEVEMENTTYPE.TriedYourBestBadge && achievementsEarnedThisRun[j] <= ACHIEVEMENTTYPE.EnduranceBadge)
			{
				flag = true;
			}
		}
		if (flag && Singleton<AchievementManager>.Instance.AllBaseAchievementsUnlocked())
		{
			unlockedCrown = true;
		}
		int num;
		for (int i = 0; i < badgeUnlocks.Count; i = num + 1)
		{
			BadgeUI newBadge = Object.Instantiate<BadgeUI>(this.badge, this.badgeParentTF);
			newBadge.manager = bm;
			newBadge.Init(badgeUnlocks[i]);
			newBadge.canvasGroup.DOFade(1f, 0.2f);
			newBadge.transform.localScale = Vector3.one * 1.5f;
			newBadge.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack);
			List<CustomizationOption> list = Singleton<Customization>.Instance.TryGetUnlockedCosmetics(badgeUnlocks[i]);
			foreach (CustomizationOption cosmetic in list)
			{
				yield return new WaitForSeconds(0.2f);
				yield return this.CosmeticUnlockRoutine(cosmetic);
				cosmetic = null;
			}
			List<CustomizationOption>.Enumerator enumerator = default(List<CustomizationOption>.Enumerator);
			if (i == 0)
			{
				UIInputHandler.SetSelectedObject(newBadge.gameObject);
				this.selectedBadge = true;
			}
			yield return new WaitForSeconds(0.2f);
			newBadge = null;
			num = i;
		}
		if (unlockedCrown)
		{
			yield return this.CosmeticUnlockRoutine(Singleton<Customization>.Instance.crownHat);
			yield return new WaitForSeconds(0.5f);
		}
		yield break;
		yield break;
	}

	// Token: 0x06000EA8 RID: 3752 RVA: 0x000490ED File Offset: 0x000472ED
	public void PopupNext()
	{
		this.inPopupView = false;
	}

	// Token: 0x06000EA9 RID: 3753 RVA: 0x000490F6 File Offset: 0x000472F6
	private IEnumerator CosmeticUnlockRoutine(CustomizationOption cosmetic)
	{
		this.cosmeticUnlockObject.SetActive(true);
		string text = LocalizedText.GetText("NEWHAT", true);
		if (cosmetic.type == Customization.Type.Accessory || cosmetic.type == Customization.Type.Eyes)
		{
			text = LocalizedText.GetText("NEWLOOK", true);
		}
		if (cosmetic.type == Customization.Type.Fit)
		{
			text = LocalizedText.GetText("NEWFIT", true);
		}
		this.cosmeticUnlockTitle.text = text;
		this.cosmeticUnlockIcon.texture = cosmetic.texture;
		Shadow component = this.cosmeticUnlockIcon.GetComponent<Shadow>();
		if (component)
		{
			component.enabled = (cosmetic.type == Customization.Type.Eyes);
		}
		this.cosmeticUnlockIcon.material = ((cosmetic.type == Customization.Type.Eyes) ? this.eyesMaterial : null);
		this.inPopupView = true;
		while (this.inPopupView)
		{
			UIInputHandler.SetSelectedObject(this.cosmeticNextButton.gameObject);
			yield return null;
		}
		this.cosmeticUnlockAnimator.Play("Done", 0, 0f);
		yield return new WaitForSeconds(0.25f);
		this.cosmeticUnlockObject.SetActive(false);
		yield break;
	}

	// Token: 0x06000EAA RID: 3754 RVA: 0x0004910C File Offset: 0x0004730C
	private IEnumerator AscentsUnlockRoutine()
	{
		this.ascentsUnlockObject.SetActive(true);
		this.inPopupView = true;
		while (this.inPopupView)
		{
			UIInputHandler.SetSelectedObject(this.ascentsNextButton.gameObject);
			yield return null;
		}
		this.ascentsUnlockAnimator.Play("Done", 0, 0f);
		yield return new WaitForSeconds(0.25f);
		this.ascentsUnlockObject.SetActive(false);
		yield break;
	}

	// Token: 0x06000EAB RID: 3755 RVA: 0x0004911B File Offset: 0x0004731B
	private IEnumerator PromotionUnlockRoutine(int ascent)
	{
		this.promotionUnlockObject.SetActive(true);
		string localizedReward = this.ascentData.ascents[ascent + 2].localizedReward;
		this.promotionUnlockTitle.text = localizedReward;
		if (ascent < this.ascentData.ascents.Count - 3)
		{
			this.promotionNextAscentUnlockText.text = LocalizedText.GetText("UNLOCKED", true).Replace("#", this.ascentData.ascents[ascent + 3].localizedTitle);
		}
		else
		{
			this.promotionNextAscentUnlockText.text = "";
		}
		this.promotionUnlockIcon.sprite = this.ascentData.ascents[ascent + 2].sashSprite;
		this.inPopupView = true;
		while (this.inPopupView)
		{
			UIInputHandler.SetSelectedObject(this.promotionNextButton.gameObject);
			yield return null;
		}
		this.promotionUnlockAnimator.Play("Done", 0, 0f);
		yield return new WaitForSeconds(0.25f);
		this.promotionUnlockObject.SetActive(false);
		if (ascent == 7)
		{
			yield return this.CosmeticUnlockRoutine(Singleton<Customization>.Instance.goatHat);
		}
		yield break;
	}

	// Token: 0x06000EAC RID: 3756 RVA: 0x00049131 File Offset: 0x00047331
	private float GetRandom(float nudge)
	{
		return Random.Range(-1f + nudge, 0f + nudge);
	}

	// Token: 0x06000EAD RID: 3757 RVA: 0x00049148 File Offset: 0x00047348
	public void DrawPip(int playerIndex, EndScreen.TimelineInfo heightTime, float maxTime, float startTime, Color color)
	{
		if (heightTime.dead)
		{
			return;
		}
		Image image = Object.Instantiate<Image>(heightTime.revived ? this.revivedPip : (heightTime.justPassedOut ? this.passedOutPip : (heightTime.died ? this.deadPip : this.pip)), this.scoutLines[playerIndex]);
		image.color = color;
		image.transform.GetChild(0).GetComponent<Image>().color = image.color;
		float num = 1200f;
		if (this.debug)
		{
			num = 1f;
		}
		image.transform.localPosition = new Vector3(this.timelinePanel.sizeDelta.x * Mathf.Clamp01((heightTime.time - startTime) / maxTime), this.timelinePanel.sizeDelta.y * Mathf.Clamp01(heightTime.height / num), 0f);
		image.transform.localPosition += Vector3.up * (float)playerIndex * 2f;
		this.scouts[playerIndex].transform.localPosition = image.transform.localPosition;
		if (this.oldPip[playerIndex])
		{
			image.transform.right = this.oldPip[playerIndex].transform.position - image.transform.position;
			image.rectTransform.sizeDelta = new Vector2(Vector3.Distance(image.transform.position, this.oldPip[playerIndex].transform.position) / this.timelinePanel.lossyScale.x, 1.5f);
		}
		if (heightTime.died)
		{
			this.scouts[playerIndex].gameObject.SetActive(false);
			image.transform.GetChild(2).GetComponent<Image>().color = image.color;
			image.transform.GetChild(2).transform.rotation = Quaternion.identity;
		}
		if (heightTime.justPassedOut)
		{
			image.transform.GetChild(2).GetComponent<Image>().color = image.color;
			image.transform.GetChild(2).transform.rotation = Quaternion.identity;
		}
		else if (heightTime.passedOut)
		{
			image.transform.GetChild(0).GetComponent<Image>().material = this.passedOutMaterial;
		}
		if (heightTime.revived)
		{
			image.transform.GetChild(2).GetComponent<Image>().color = image.color;
			image.transform.GetChild(2).transform.rotation = Quaternion.identity;
			image.transform.GetChild(0).gameObject.SetActive(false);
			this.scouts[playerIndex].gameObject.SetActive(true);
		}
		this.oldPip[playerIndex] = image;
	}

	// Token: 0x06000EAE RID: 3758 RVA: 0x00049430 File Offset: 0x00047630
	public void CheckPeak(int playerIndex, EndScreen.TimelineInfo timelineInfo)
	{
		if (playerIndex < this.scouts.Length && timelineInfo.time >= 0.99f && timelineInfo.height >= 1f && !this.scoutsAtPeak[playerIndex].gameObject.activeSelf && !timelineInfo.dead && timelineInfo.won)
		{
			this.scouts[playerIndex].gameObject.SetActive(false);
			this.scoutsAtPeak[playerIndex].gameObject.SetActive(true);
			this.scoutsAtPeak[playerIndex].transform.SetSiblingIndex(1);
			this.scoutsAtPeak[playerIndex].rectTransform.sizeDelta = Vector3.zero;
			this.scoutsAtPeak[playerIndex].rectTransform.DOSizeDelta(Vector3.one * 15f, 0.25f, false).SetEase(Ease.OutBack);
		}
	}

	// Token: 0x06000EAF RID: 3759 RVA: 0x00049528 File Offset: 0x00047728
	public void ReturnToAirport()
	{
		RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Basic, null, new IEnumerator[]
		{
			RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess("Airport", true, true, 3f)
		});
	}

	// Token: 0x04000C43 RID: 3139
	public static EndScreen instance;

	// Token: 0x04000C44 RID: 3140
	public CanvasGroup canvasGroup;

	// Token: 0x04000C45 RID: 3141
	public AscentData ascentData;

	// Token: 0x04000C46 RID: 3142
	public bool debug;

	// Token: 0x04000C47 RID: 3143
	public TMP_Text endTime;

	// Token: 0x04000C48 RID: 3144
	public EndScreenScoutWindow[] scoutWindows;

	// Token: 0x04000C49 RID: 3145
	public Color[] debugColors;

	// Token: 0x04000C4A RID: 3146
	public BadgeData[] debugBadgeUnlocks;

	// Token: 0x04000C4B RID: 3147
	public BadgeUI badge;

	// Token: 0x04000C4C RID: 3148
	public Transform badgeParentTF;

	// Token: 0x04000C4D RID: 3149
	public Transform[] scoutLines;

	// Token: 0x04000C4E RID: 3150
	public Image[] scouts;

	// Token: 0x04000C4F RID: 3151
	public Image[] scoutsAtPeak;

	// Token: 0x04000C50 RID: 3152
	public int pipCount = 100;

	// Token: 0x04000C51 RID: 3153
	public float waitTime = 5f;

	// Token: 0x04000C52 RID: 3154
	public RectTransform timelinePanel;

	// Token: 0x04000C53 RID: 3155
	public Image pip;

	// Token: 0x04000C54 RID: 3156
	public Image deadPip;

	// Token: 0x04000C55 RID: 3157
	public Image passedOutPip;

	// Token: 0x04000C56 RID: 3158
	public Image revivedPip;

	// Token: 0x04000C57 RID: 3159
	public Material passedOutMaterial;

	// Token: 0x04000C58 RID: 3160
	public GameObject peakBanner;

	// Token: 0x04000C59 RID: 3161
	public GameObject deadBanner;

	// Token: 0x04000C5A RID: 3162
	public GameObject yourFriendsWonBanner;

	// Token: 0x04000C5B RID: 3163
	public GameObject miniWinBanner;

	// Token: 0x04000C5C RID: 3164
	public GameObject buttons;

	// Token: 0x04000C5D RID: 3165
	public WaitingForPlayersUI WaitingForPlayersUI;

	// Token: 0x04000C5E RID: 3166
	public Button nextButton;

	// Token: 0x04000C5F RID: 3167
	public Button returnToAirportButton;

	// Token: 0x04000C60 RID: 3168
	public Material eyesMaterial;

	// Token: 0x04000C61 RID: 3169
	private bool selectedBadge;

	// Token: 0x04000C62 RID: 3170
	public GameObject cosmeticUnlockObject;

	// Token: 0x04000C63 RID: 3171
	public Animator cosmeticUnlockAnimator;

	// Token: 0x04000C64 RID: 3172
	public TMP_Text cosmeticUnlockTitle;

	// Token: 0x04000C65 RID: 3173
	public Button cosmeticNextButton;

	// Token: 0x04000C66 RID: 3174
	public RawImage cosmeticUnlockIcon;

	// Token: 0x04000C67 RID: 3175
	public GameObject ascentsUnlockObject;

	// Token: 0x04000C68 RID: 3176
	public Animator ascentsUnlockAnimator;

	// Token: 0x04000C69 RID: 3177
	public Button ascentsNextButton;

	// Token: 0x04000C6A RID: 3178
	public GameObject promotionUnlockObject;

	// Token: 0x04000C6B RID: 3179
	public Animator promotionUnlockAnimator;

	// Token: 0x04000C6C RID: 3180
	public TMP_Text promotionUnlockTitle;

	// Token: 0x04000C6D RID: 3181
	public TMP_Text promotionNextAscentUnlockText;

	// Token: 0x04000C6E RID: 3182
	public Button promotionNextButton;

	// Token: 0x04000C6F RID: 3183
	public Image promotionUnlockIcon;

	// Token: 0x04000C70 RID: 3184
	private bool inPopupView;

	// Token: 0x04000C71 RID: 3185
	private Image[] oldPip = new Image[4];

	// Token: 0x020004D2 RID: 1234
	private struct PipData
	{
		// Token: 0x04001B30 RID: 6960
		public int playerIdx;

		// Token: 0x04001B31 RID: 6961
		public EndScreen.TimelineInfo info;
	}

	// Token: 0x020004D3 RID: 1235
	public enum TimelineNote : byte
	{
		// Token: 0x04001B33 RID: 6963
		None,
		// Token: 0x04001B34 RID: 6964
		PassedOut,
		// Token: 0x04001B35 RID: 6965
		Dead,
		// Token: 0x04001B36 RID: 6966
		JustPassedOut,
		// Token: 0x04001B37 RID: 6967
		Died,
		// Token: 0x04001B38 RID: 6968
		Revived,
		// Token: 0x04001B39 RID: 6969
		Won
	}

	// Token: 0x020004D4 RID: 1236
	public struct TimelineInfo
	{
		// Token: 0x17000241 RID: 577
		// (get) Token: 0x06001DA4 RID: 7588 RVA: 0x0008A8DE File Offset: 0x00088ADE
		// (set) Token: 0x06001DA5 RID: 7589 RVA: 0x0008A8E6 File Offset: 0x00088AE6
		public EndScreen.TimelineNote Note { readonly get; private set; }

		// Token: 0x17000242 RID: 578
		// (get) Token: 0x06001DA6 RID: 7590 RVA: 0x0008A8EF File Offset: 0x00088AEF
		public float height
		{
			get
			{
				return (float)this.heightRecord;
			}
		}

		// Token: 0x17000243 RID: 579
		// (get) Token: 0x06001DA7 RID: 7591 RVA: 0x0008A8F8 File Offset: 0x00088AF8
		public float time
		{
			get
			{
				return (float)this.timestamp;
			}
		}

		// Token: 0x06001DA8 RID: 7592 RVA: 0x0008A901 File Offset: 0x00088B01
		public TimelineInfo(float height, float time, EndScreen.TimelineNote note = EndScreen.TimelineNote.None)
		{
			this = new EndScreen.TimelineInfo((ushort)Mathf.FloorToInt(height), (ushort)Mathf.FloorToInt(time), note);
		}

		// Token: 0x06001DA9 RID: 7593 RVA: 0x0008A918 File Offset: 0x00088B18
		public TimelineInfo(ushort height, ushort time, EndScreen.TimelineNote note)
		{
			this.heightRecord = height;
			this.timestamp = time;
			this.Note = note;
		}

		// Token: 0x17000244 RID: 580
		// (get) Token: 0x06001DAA RID: 7594 RVA: 0x0008A92F File Offset: 0x00088B2F
		// (set) Token: 0x06001DAB RID: 7595 RVA: 0x0008A93A File Offset: 0x00088B3A
		public bool died
		{
			get
			{
				return this.Note == EndScreen.TimelineNote.Died;
			}
			set
			{
				this.SetNote(value, EndScreen.TimelineNote.Died);
			}
		}

		// Token: 0x17000245 RID: 581
		// (get) Token: 0x06001DAC RID: 7596 RVA: 0x0008A944 File Offset: 0x00088B44
		// (set) Token: 0x06001DAD RID: 7597 RVA: 0x0008A94F File Offset: 0x00088B4F
		public bool dead
		{
			get
			{
				return this.Note == EndScreen.TimelineNote.Dead;
			}
			set
			{
				this.SetNote(value, EndScreen.TimelineNote.Dead);
			}
		}

		// Token: 0x17000246 RID: 582
		// (get) Token: 0x06001DAE RID: 7598 RVA: 0x0008A959 File Offset: 0x00088B59
		// (set) Token: 0x06001DAF RID: 7599 RVA: 0x0008A964 File Offset: 0x00088B64
		public bool revived
		{
			get
			{
				return this.Note == EndScreen.TimelineNote.Revived;
			}
			set
			{
				this.SetNote(value, EndScreen.TimelineNote.Revived);
			}
		}

		// Token: 0x17000247 RID: 583
		// (get) Token: 0x06001DB0 RID: 7600 RVA: 0x0008A96E File Offset: 0x00088B6E
		// (set) Token: 0x06001DB1 RID: 7601 RVA: 0x0008A979 File Offset: 0x00088B79
		public bool justPassedOut
		{
			get
			{
				return this.Note == EndScreen.TimelineNote.JustPassedOut;
			}
			set
			{
				this.SetNote(value, EndScreen.TimelineNote.JustPassedOut);
			}
		}

		// Token: 0x17000248 RID: 584
		// (get) Token: 0x06001DB2 RID: 7602 RVA: 0x0008A983 File Offset: 0x00088B83
		// (set) Token: 0x06001DB3 RID: 7603 RVA: 0x0008A98E File Offset: 0x00088B8E
		public bool passedOut
		{
			get
			{
				return this.Note == EndScreen.TimelineNote.PassedOut;
			}
			set
			{
				this.SetNote(value, EndScreen.TimelineNote.PassedOut);
			}
		}

		// Token: 0x17000249 RID: 585
		// (get) Token: 0x06001DB4 RID: 7604 RVA: 0x0008A998 File Offset: 0x00088B98
		// (set) Token: 0x06001DB5 RID: 7605 RVA: 0x0008A9A3 File Offset: 0x00088BA3
		public bool won
		{
			get
			{
				return this.Note == EndScreen.TimelineNote.Won;
			}
			set
			{
				this.SetNote(value, EndScreen.TimelineNote.Won);
			}
		}

		// Token: 0x06001DB6 RID: 7606 RVA: 0x0008A9B0 File Offset: 0x00088BB0
		private void SetNote(bool value, EndScreen.TimelineNote noteType)
		{
			if (value)
			{
				if (noteType != EndScreen.TimelineNote.None && this.Note != EndScreen.TimelineNote.None)
				{
					Debug.LogWarning(string.Format("Setting note to {0} which will override previous type {1}", noteType, this.Note));
				}
				this.Note = noteType;
				return;
			}
			Debug.LogWarning(string.Format("WHOA! When do we ever set a timeline event to FALSE? Something funky going on with {0}", noteType));
			if (this.Note == noteType)
			{
				this.Note = EndScreen.TimelineNote.None;
				return;
			}
			Debug.LogError(string.Format("Can't clear note {0} because current note is different {1}", noteType, this.Note));
		}

		// Token: 0x04001B3B RID: 6971
		public readonly ushort timestamp;

		// Token: 0x04001B3C RID: 6972
		public readonly ushort heightRecord;
	}
}

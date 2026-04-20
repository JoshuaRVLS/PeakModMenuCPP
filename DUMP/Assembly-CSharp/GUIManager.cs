using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zorro.Core;
using Zorro.Settings;

// Token: 0x020000C6 RID: 198
public class GUIManager : MonoBehaviour
{
	// Token: 0x17000090 RID: 144
	// (get) Token: 0x06000775 RID: 1909 RVA: 0x0002A2F8 File Offset: 0x000284F8
	public bool wheelActive
	{
		get
		{
			return this.emoteWheel.gameObject.activeSelf || this.backpackWheel.gameObject.activeSelf;
		}
	}

	// Token: 0x17000091 RID: 145
	// (get) Token: 0x06000776 RID: 1910 RVA: 0x0002A31E File Offset: 0x0002851E
	// (set) Token: 0x06000777 RID: 1911 RVA: 0x0002A326 File Offset: 0x00028526
	internal IInteractible currentInteractable { get; private set; }

	// Token: 0x17000092 RID: 146
	// (get) Token: 0x06000778 RID: 1912 RVA: 0x0002A32F File Offset: 0x0002852F
	// (set) Token: 0x06000779 RID: 1913 RVA: 0x0002A337 File Offset: 0x00028537
	public ControllerManager controllerManager { get; private set; }

	// Token: 0x17000093 RID: 147
	// (get) Token: 0x0600077A RID: 1914 RVA: 0x0002A340 File Offset: 0x00028540
	public static bool InPauseMenu
	{
		get
		{
			GUIManager guimanager = GUIManager.instance;
			return guimanager != null && guimanager.pauseMenu.activeSelf;
		}
	}

	// Token: 0x17000094 RID: 148
	// (get) Token: 0x0600077B RID: 1915 RVA: 0x0002A357 File Offset: 0x00028557
	// (set) Token: 0x0600077C RID: 1916 RVA: 0x0002A35E File Offset: 0x0002855E
	public static float TimeLastPaused { get; private set; }

	// Token: 0x0600077D RID: 1917 RVA: 0x0002A366 File Offset: 0x00028566
	private void Awake()
	{
		GUIManager.instance = this;
		this.controllerManager = new ControllerManager();
		this.controllerManager.Init();
		this.InitReticleList();
	}

	// Token: 0x0600077E RID: 1918 RVA: 0x0002A38C File Offset: 0x0002858C
	private void OnDestroy()
	{
		this.controllerManager.Destroy();
		if (this.character != null)
		{
			CharacterItems characterItems = this.character.refs.items;
			characterItems.onSlotEquipped = (Action)Delegate.Remove(characterItems.onSlotEquipped, new Action(this.OnSlotEquipped));
			GameUtils gameUtils = GameUtils.instance;
			gameUtils.OnUpdatedFeedData = (Action)Delegate.Remove(gameUtils.OnUpdatedFeedData, new Action(this.OnUpdatedFeedData));
		}
	}

	// Token: 0x0600077F RID: 1919 RVA: 0x0002A40C File Offset: 0x0002860C
	private void Start()
	{
		this.UpdateItemPrompts();
		this.OnInteractChange();
		this.throwGO.SetActive(false);
		this.spectatingObject.SetActive(false);
		this.heroObject.SetActive(false);
		PhotosensitiveSetting setting = GameHandler.Instance.SettingsHandler.GetSetting<PhotosensitiveSetting>();
		ColorblindSetting setting2 = GameHandler.Instance.SettingsHandler.GetSetting<ColorblindSetting>();
		this.photosensitivity = (setting.Value == OffOnMode.ON);
		this.colorblindness = (setting2.Value == OffOnMode.ON);
	}

	// Token: 0x06000780 RID: 1920 RVA: 0x0002A488 File Offset: 0x00028688
	private void LateUpdate()
	{
		this.UpdateDebug();
		this.UpdateBinocularOverlay();
		this.UpdateWindowStatus();
		if (Character.localCharacter)
		{
			if (Interaction.instance.currentHovered != this.currentInteractable)
			{
				this.OnInteractChange();
			}
			if (this.wasPitonClimbing)
			{
				this.RefreshInteractablePrompt();
			}
			this.interactPromptLunge.SetActive(Character.localCharacter.data.isClimbing && Character.localCharacter.data.currentStamina < 0.05f && Character.localCharacter.data.currentStamina > 0.0001f);
			this.wasPitonClimbing = (Character.localCharacter.data.climbingSpikeCount > 0 && Character.localCharacter.data.isClimbing);
			if (!this.character)
			{
				this.character = Character.localCharacter;
				CharacterItems characterItems = this.character.refs.items;
				characterItems.onSlotEquipped = (Action)Delegate.Combine(characterItems.onSlotEquipped, new Action(this.OnSlotEquipped));
				GameUtils gameUtils = GameUtils.instance;
				gameUtils.OnUpdatedFeedData = (Action)Delegate.Combine(gameUtils.OnUpdatedFeedData, new Action(this.OnUpdatedFeedData));
			}
			this.UpdateReticle();
			this.UpdateThrow();
			this.UpdateRope();
			this.UpdateDyingBar();
			this.UpdateEmoteWheel();
			this.TestUpdateItemPrompts();
			this.UpdateSpectate();
			this.UpdatePaused();
		}
		if (Character.observedCharacter)
		{
			this.UpdateItems();
		}
	}

	// Token: 0x17000095 RID: 149
	// (get) Token: 0x06000781 RID: 1921 RVA: 0x0002A602 File Offset: 0x00028802
	// (set) Token: 0x06000782 RID: 1922 RVA: 0x0002A60A File Offset: 0x0002880A
	public bool windowShowingCursor { get; private set; }

	// Token: 0x17000096 RID: 150
	// (get) Token: 0x06000783 RID: 1923 RVA: 0x0002A613 File Offset: 0x00028813
	// (set) Token: 0x06000784 RID: 1924 RVA: 0x0002A61B File Offset: 0x0002881B
	public bool windowBlockingInput { get; private set; }

	// Token: 0x06000785 RID: 1925 RVA: 0x0002A624 File Offset: 0x00028824
	public void UpdateWindowStatus()
	{
		this.windowShowingCursor = false;
		this.windowBlockingInput = false;
		foreach (MenuWindow menuWindow in MenuWindow.AllActiveWindows)
		{
			if (menuWindow.blocksPlayerInput)
			{
				this.lastBlockedInput = Time.frameCount;
			}
			if (menuWindow.showCursorWhileOpen)
			{
				this.windowShowingCursor = true;
			}
		}
		if (this.pauseMenu.activeSelf)
		{
			this.windowShowingCursor = true;
			this.windowBlockingInput = true;
		}
		if (Time.frameCount < this.lastBlockedInput + 2)
		{
			this.windowBlockingInput = true;
		}
	}

	// Token: 0x06000786 RID: 1926 RVA: 0x0002A6D0 File Offset: 0x000288D0
	public void UpdatePaused()
	{
		if (Character.localCharacter.input.pauseWasPressed && !LoadingScreenHandler.loading && !this.pauseMenu.activeSelf)
		{
			if (this.wheelActive)
			{
				return;
			}
			if (this.endScreen.isOpen)
			{
				return;
			}
			this.pauseMenu.SetActive(true);
			Character.localCharacter.input.pauseWasPressed = false;
		}
		if (GUIManager.InPauseMenu)
		{
			GUIManager.TimeLastPaused = Time.unscaledTime;
		}
	}

	// Token: 0x06000787 RID: 1927 RVA: 0x0002A748 File Offset: 0x00028948
	private void OnSlotEquipped()
	{
		for (int i = 0; i < this.items.Length; i++)
		{
			if (i < Character.localCharacter.player.itemSlots.Length)
			{
				this.items[i].SetSelected();
			}
		}
		this.backpack.SetSelected();
	}

	// Token: 0x06000788 RID: 1928 RVA: 0x0002A794 File Offset: 0x00028994
	private void OnUpdatedFeedData()
	{
		GUIManager.<>c__DisplayClass139_0 CS$<>8__locals1 = new GUIManager.<>c__DisplayClass139_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.feedData = GameUtils.instance.GetFeedDataForReceiver(Character.localCharacter.photonView.ViewID);
		int k;
		int j;
		for (k = 0; k < CS$<>8__locals1.feedData.Count; k = j + 1)
		{
			if (!this.friendUseItemProgressList.Any((UI_UseItemProgressFriend f) => f.giverID == CS$<>8__locals1.feedData[k].giverID))
			{
				UI_UseItemProgressFriend ui_UseItemProgressFriend = Object.Instantiate<UI_UseItemProgressFriend>(this.friendUseItemProgressPrefab, this.friendProgressTF);
				this.friendUseItemProgressList.Add(ui_UseItemProgressFriend);
				ui_UseItemProgressFriend.Init(CS$<>8__locals1.feedData[k]);
			}
			j = k;
		}
		int i;
		for (i = 0; i < this.friendUseItemProgressList.Count; i = j + 1)
		{
			if (!CS$<>8__locals1.feedData.Any((FeedData f) => f.giverID == CS$<>8__locals1.<>4__this.friendUseItemProgressList[i].giverID))
			{
				this.friendUseItemProgressList[i].Kill();
				this.friendUseItemProgressList.RemoveAt(i);
			}
			j = i;
		}
	}

	// Token: 0x06000789 RID: 1929 RVA: 0x0002A8E8 File Offset: 0x00028AE8
	public void SetHeroTitle(string text, AudioClip stinger)
	{
		Debug.Log("Set hero title: " + text);
		if (this._heroRoutine != null)
		{
			base.StopCoroutine(this._heroRoutine);
		}
		if (this.stingerSound && stinger != null)
		{
			this.stingerSound.clip = stinger;
			this.stingerSound.Play();
		}
		this._heroRoutine = base.StartCoroutine(this.<SetHeroTitle>g__HeroRoutine|140_0(text));
	}

	// Token: 0x0600078A RID: 1930 RVA: 0x0002A959 File Offset: 0x00028B59
	public void OpenBackpackWheel(BackpackReference backpackReference)
	{
		if (!this.wheelActive && !this.windowBlockingInput)
		{
			Character.localCharacter.data.usingBackpackWheel = true;
			this.backpackWheel.InitWheel(backpackReference);
		}
	}

	// Token: 0x0600078B RID: 1931 RVA: 0x0002A987 File Offset: 0x00028B87
	public void CloseBackpackWheel()
	{
		Debug.Log("Close Input Wheel");
		Character.localCharacter.data.usingBackpackWheel = false;
		this.backpackWheel.gameObject.SetActive(false);
	}

	// Token: 0x17000097 RID: 151
	// (get) Token: 0x0600078C RID: 1932 RVA: 0x0002A9B4 File Offset: 0x00028BB4
	private bool canEmote
	{
		get
		{
			return !Character.localCharacter.data.dead;
		}
	}

	// Token: 0x0600078D RID: 1933 RVA: 0x0002A9C8 File Offset: 0x00028BC8
	private void UpdateEmoteWheel()
	{
		if (this.canEmote && Character.localCharacter.input.emoteIsPressed && !Character.localCharacter.data.isClimbing)
		{
			if (!this.wheelActive && !this.windowBlockingInput)
			{
				this.emoteWheel.SetActive(true);
				Character.localCharacter.data.usingEmoteWheel = true;
				return;
			}
		}
		else if (Character.localCharacter.data.usingEmoteWheel)
		{
			this.emoteWheel.SetActive(false);
			Character.localCharacter.data.usingEmoteWheel = false;
		}
	}

	// Token: 0x0600078E RID: 1934 RVA: 0x0002AA5C File Offset: 0x00028C5C
	private void UpdateDyingBar()
	{
		this.dyingBarObject.gameObject.SetActive(Character.localCharacter.data.fullyPassedOut || Character.localCharacter.data.dead);
		if (this.dyingBarObject.gameObject.activeSelf)
		{
			this.dyingBarImage.fillAmount = 1f - Character.localCharacter.data.deathTimer;
			this.dyingBarImage.color = this.dyingBarGradient.Evaluate(1f - Character.localCharacter.data.deathTimer);
			this.dyingBarMushrooms.SetActive(Character.localCharacter.refs.afflictions.willZombify);
			if (Character.localCharacter.data.deathTimer >= 1f && !this.dead)
			{
				this.dyingBarAnimator.Play("Dead", 0, 0f);
				this.dead = true;
				return;
			}
		}
		else
		{
			this.dead = false;
		}
	}

	// Token: 0x0600078F RID: 1935 RVA: 0x0002AB60 File Offset: 0x00028D60
	private void UpdateSpectate()
	{
		if (MainCameraMovement.specCharacter != this.currentSpecCharacter)
		{
			this.currentSpecCharacter = MainCameraMovement.specCharacter;
			if (this.currentSpecCharacter)
			{
				this.spectatingObject.SetActive(true);
				this.spectatingInputs.SetActive(Character.localCharacter.data.dead);
				if (this.currentSpecCharacter == Character.localCharacter)
				{
					this.spectatingNameText.text = LocalizedText.GetText("YOURSELF", true);
					this.spectatingNameText.color = this.spectatingYourselfColor;
					return;
				}
				this.spectatingNameText.text = MainCameraMovement.specCharacter.characterName;
				this.spectatingNameText.color = this.spectatingNameColor;
				return;
			}
			else
			{
				this.spectatingObject.SetActive(false);
			}
		}
	}

	// Token: 0x06000790 RID: 1936 RVA: 0x0002AC30 File Offset: 0x00028E30
	private void UpdateRope()
	{
		RopeSpool ropeSpool;
		if (Character.localCharacter.data.currentItem && Character.localCharacter.data.currentItem.TryGetComponent<RopeSpool>(out ropeSpool))
		{
			this.ui_rope.gameObject.SetActive(true);
			if (ropeSpool.rope)
			{
				this.ui_rope.UpdateRope(ropeSpool.rope.GetRopeSegments().Count);
			}
			Shader.SetGlobalFloat(this.ROPE_INVERT, (float)(ropeSpool.isAntiRope ? 1 : 0));
			return;
		}
		this.ui_rope.gameObject.SetActive(false);
	}

	// Token: 0x06000791 RID: 1937 RVA: 0x0002ACCE File Offset: 0x00028ECE
	private void UpdateWebs()
	{
	}

	// Token: 0x06000792 RID: 1938 RVA: 0x0002ACD0 File Offset: 0x00028ED0
	private void UpdateThrow()
	{
		this.throwGO.SetActive(Character.localCharacter.refs.items.throwChargeLevel > 0f);
		if (Character.localCharacter.refs.items.throwChargeLevel > 0f)
		{
			float fillAmount = Mathf.Lerp(0.692f, 0.808f, Character.localCharacter.refs.items.throwChargeLevel);
			this.throwBar.fillAmount = fillAmount;
			this.throwBar.color = this.throwGradient.Evaluate(Character.localCharacter.refs.items.throwChargeLevel);
		}
	}

	// Token: 0x06000793 RID: 1939 RVA: 0x0002AD78 File Offset: 0x00028F78
	private void UpdateReticle()
	{
		this.reticleDefaultImage.color = ((this.character.data.sinceCanClimb < 0.05f) ? this.reticleColorHighlight : this.reticleColorDefault);
		if (Character.localCharacter.data.fullyPassedOut || Character.localCharacter.data.dead)
		{
			this.SetReticle(null);
			return;
		}
		if (this.reticleLock > 0f)
		{
			this.reticleLock -= Time.deltaTime;
			return;
		}
		if (Character.localCharacter.data.currentClimbHandle != null)
		{
			this.SetReticle(this.reticleSpike);
			return;
		}
		if (Character.localCharacter.data.isRopeClimbing)
		{
			this.SetReticle(this.reticleRope);
			return;
		}
		if (Character.localCharacter.data.sincePalJump < 0.5f)
		{
			this.SetReticle(this.reticleBoost);
			return;
		}
		if (Character.localCharacter.refs.items.throwChargeLevel > 0f)
		{
			this.SetReticle(this.reticleThrow);
			return;
		}
		if (Character.localCharacter.data.sincePressClimb < 0.1f && Character.localCharacter.refs.climbing.CanClimb())
		{
			this.SetReticle(this.reticleClimbTry);
			return;
		}
		if (Character.localCharacter.data.isClimbing)
		{
			if (Character.localCharacter.OutOfStamina())
			{
				this.SetReticle(this.reticleX);
				return;
			}
			this.SetReticle(this.reticleClimb);
			return;
		}
		else
		{
			if (Character.localCharacter.data.isReaching)
			{
				this.SetReticle(this.reticleReach);
				return;
			}
			if (Character.localCharacter.data.isVineClimbing)
			{
				this.SetReticle(this.reticleVine);
				return;
			}
			if (Character.localCharacter.data.isKicking)
			{
				this.SetReticle(this.reticleKick);
				return;
			}
			if (Character.localCharacter.data.currentItem && Character.localCharacter.data.currentItem.UIData.isShootable && Character.localCharacter.data.currentItem.CanUsePrimary())
			{
				this.SetReticle(this.reticleShoot);
				return;
			}
			if (!this.emoteWheel.gameObject.activeSelf)
			{
				this.SetReticle(this.reticleDefault);
				return;
			}
			this.SetReticle(null);
			return;
		}
	}

	// Token: 0x06000794 RID: 1940 RVA: 0x0002AFD0 File Offset: 0x000291D0
	public void ReticleLand()
	{
		RectTransform component = this.reticleDefault.GetComponent<RectTransform>();
		component.sizeDelta = new Vector2(40f, 10f);
		component.DOSizeDelta(new Vector2(10f, 10f), 0.33f, false).SetEase(Ease.InOutCubic);
	}

	// Token: 0x06000795 RID: 1941 RVA: 0x0002B01F File Offset: 0x0002921F
	public void Grasp()
	{
		this.SetReticle(this.reticleGrasp);
		this.reticleGrasp.GetComponent<Animator>().Play("Play", 0, 0f);
		this.reticleLock = 1f;
	}

	// Token: 0x06000796 RID: 1942 RVA: 0x0002B053 File Offset: 0x00029253
	public void ClimbJump()
	{
		this.SetReticle(this.reticleClimbJump);
		this.reticleLock = 0.5f;
	}

	// Token: 0x06000797 RID: 1943 RVA: 0x0002B06C File Offset: 0x0002926C
	private void SetReticle(GameObject activeReticle)
	{
		if (activeReticle == this.lastReticle && activeReticle != null)
		{
			return;
		}
		this.lastReticle = activeReticle;
		for (int i = 0; i < this.reticleList.Count; i++)
		{
			if (this.reticleList[i] != activeReticle)
			{
				this.reticleList[i].SetActive(false);
			}
		}
		if (activeReticle)
		{
			activeReticle.SetActive(true);
		}
	}

	// Token: 0x06000798 RID: 1944 RVA: 0x0002B0E4 File Offset: 0x000292E4
	private void InitReticleList()
	{
		this.reticleList.Add(this.reticleDefault);
		this.reticleList.Add(this.reticleRope);
		this.reticleList.Add(this.reticleSpike);
		this.reticleList.Add(this.reticleThrow);
		this.reticleList.Add(this.reticleReach);
		this.reticleList.Add(this.reticleX);
		this.reticleList.Add(this.reticleClimb);
		this.reticleList.Add(this.reticleClimbJump);
		this.reticleList.Add(this.reticleClimbTry);
		this.reticleList.Add(this.reticleGrasp);
		this.reticleList.Add(this.reticleVine);
		this.reticleList.Add(this.reticleBoost);
		this.reticleList.Add(this.reticleShoot);
		this.reticleList.Add(this.reticleKick);
	}

	// Token: 0x06000799 RID: 1945 RVA: 0x0002B1DF File Offset: 0x000293DF
	private void UpdateDebug()
	{
	}

	// Token: 0x0600079A RID: 1946 RVA: 0x0002B1E1 File Offset: 0x000293E1
	private IEnumerator ScreenshotRoutine(bool disableHud)
	{
		bool cacheEnabled = this.hudCanvas.enabled;
		if (disableHud)
		{
			this.hudCanvas.enabled = false;
		}
		yield return null;
		string text = "";
		if (Application.isEditor)
		{
			text = "Screenshots/";
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
		}
		string path = "Screenshot_" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + ".png";
		ScreenCapture.CaptureScreenshot(Path.Combine(text, path), 2);
		yield return null;
		this.hudCanvas.enabled = cacheEnabled;
		yield break;
	}

	// Token: 0x0600079B RID: 1947 RVA: 0x0002B1F8 File Offset: 0x000293F8
	public void AddStatusFX(CharacterAfflictions.STATUSTYPE type, float amount)
	{
		switch (type)
		{
		case CharacterAfflictions.STATUSTYPE.Injury:
			this.InjuryFX(amount);
			return;
		case CharacterAfflictions.STATUSTYPE.Hunger:
			this.HungerFX();
			return;
		case CharacterAfflictions.STATUSTYPE.Cold:
			this.ColdFX(amount);
			return;
		case CharacterAfflictions.STATUSTYPE.Poison:
			this.PoisonFX(amount);
			return;
		case CharacterAfflictions.STATUSTYPE.Curse:
			this.CurseFX(amount);
			return;
		case CharacterAfflictions.STATUSTYPE.Drowsy:
			this.DrowsyFX();
			return;
		case CharacterAfflictions.STATUSTYPE.Hot:
			this.HotFX(amount);
			return;
		case CharacterAfflictions.STATUSTYPE.Thorns:
			this.ThornsFX(amount);
			return;
		case CharacterAfflictions.STATUSTYPE.Spores:
			this.SporesFX(1f);
			return;
		case CharacterAfflictions.STATUSTYPE.Web:
			return;
		}
		this.InjuryFX(amount);
	}

	// Token: 0x0600079C RID: 1948 RVA: 0x0002B28E File Offset: 0x0002948E
	private void InjuryFX(float amount)
	{
		GamefeelHandler.instance.AddPerlinShake((amount + 1f) * 5f, 0.3f, 15f);
		this.injurySVFX.Play(amount);
	}

	// Token: 0x0600079D RID: 1949 RVA: 0x0002B2BD File Offset: 0x000294BD
	private void CurseFX(float amount)
	{
		GamefeelHandler.instance.AddPerlinShake((amount + 1f) * 30f, 0.3f, 15f);
		this.curseSVFX.Play(amount);
	}

	// Token: 0x0600079E RID: 1950 RVA: 0x0002B2EC File Offset: 0x000294EC
	private void HungerFX()
	{
	}

	// Token: 0x0600079F RID: 1951 RVA: 0x0002B2F0 File Offset: 0x000294F0
	private void DrowsyFX()
	{
		float num = 1f;
		GamefeelHandler.instance.AddPerlinShake(num * 5f, 0.3f, 15f);
		this.drowsyFX.Play(num);
	}

	// Token: 0x060007A0 RID: 1952 RVA: 0x0002B32A File Offset: 0x0002952A
	private void PoisonFX(float amount)
	{
		amount = 0.5f;
		GamefeelHandler.instance.AddPerlinShake(amount * 5f, 0.3f, 15f);
		this.poisonSVFX.Play(amount);
	}

	// Token: 0x060007A1 RID: 1953 RVA: 0x0002B35A File Offset: 0x0002955A
	private void ThornsFX(float amount)
	{
		amount = 0.5f;
		GamefeelHandler.instance.AddPerlinShake(amount * 5f, 0.3f, 15f);
		this.thornsSVFX.Play(amount);
	}

	// Token: 0x060007A2 RID: 1954 RVA: 0x0002B38A File Offset: 0x0002958A
	private void SporesFX(float amount)
	{
		GamefeelHandler.instance.AddPerlinShake(amount * 5f, 0.3f, 15f);
		this.sporesSVFX.Play(amount);
	}

	// Token: 0x060007A3 RID: 1955 RVA: 0x0002B3B3 File Offset: 0x000295B3
	private void ColdFX(float amount)
	{
		amount = 1f;
		GamefeelHandler.instance.AddPerlinShake(amount * 2f, 1f, 30f);
		this.PlayFXSequence(ref this.coldSequence, this.coldVolume, amount);
	}

	// Token: 0x060007A4 RID: 1956 RVA: 0x0002B3EA File Offset: 0x000295EA
	private void HotFX(float amount)
	{
		amount = 1f;
		GamefeelHandler.instance.AddPerlinShake(amount * 2f, 1f, 30f);
		this.hotSVFX.Play(amount);
	}

	// Token: 0x060007A5 RID: 1957 RVA: 0x0002B41C File Offset: 0x0002961C
	private void PlayFXSequence(ref Sequence sequence, Volume volume, float amount)
	{
		sequence.Kill(false);
		sequence = DOTween.Sequence();
		sequence.Append(DOTween.To(() => volume.weight, delegate(float x)
		{
			volume.weight = x;
		}, amount, 0.06f));
		sequence.AppendInterval(0.25f * amount);
		sequence.Append(DOTween.To(() => volume.weight, delegate(float x)
		{
			volume.weight = x;
		}, 0f, 0.45f));
	}

	// Token: 0x060007A6 RID: 1958 RVA: 0x0002B4B0 File Offset: 0x000296B0
	public void StartSugarRush()
	{
		float endValue = 1f;
		if (GUIManager.instance.photosensitivity)
		{
			endValue = 0.25f;
		}
		DOTween.To(() => this.sugarRushVolume.weight, delegate(float x)
		{
			this.sugarRushVolume.weight = x;
		}, endValue, 0.5f);
		GUIManager.instance.bar.AddRainbow();
	}

	// Token: 0x060007A7 RID: 1959 RVA: 0x0002B508 File Offset: 0x00029708
	public void EndSugarRush()
	{
		DOTween.To(() => this.sugarRushVolume.weight, delegate(float x)
		{
			this.sugarRushVolume.weight = x;
		}, 0f, 0.5f);
		GUIManager.instance.bar.RemoveRainbow();
	}

	// Token: 0x060007A8 RID: 1960 RVA: 0x0002B541 File Offset: 0x00029741
	public void StartEnergyDrink()
	{
		this.energySVFX.StartFX(0.15f);
	}

	// Token: 0x060007A9 RID: 1961 RVA: 0x0002B553 File Offset: 0x00029753
	public void EndEnergyDrink()
	{
		this.energySVFX.EndFX();
	}

	// Token: 0x060007AA RID: 1962 RVA: 0x0002B560 File Offset: 0x00029760
	private void HeatFX(float amount)
	{
		amount = 1f;
		this.heatSVFX.Play(amount);
	}

	// Token: 0x060007AB RID: 1963 RVA: 0x0002B575 File Offset: 0x00029775
	public void StartHeat()
	{
		this.heatSVFX.StartFX(0.5f);
	}

	// Token: 0x060007AC RID: 1964 RVA: 0x0002B587 File Offset: 0x00029787
	public void EndHeat()
	{
		this.heatSVFX.EndFX();
	}

	// Token: 0x060007AD RID: 1965 RVA: 0x0002B594 File Offset: 0x00029794
	public void StartSunscreen()
	{
		this.sunscreenSVFX.StartFX(0.5f);
	}

	// Token: 0x060007AE RID: 1966 RVA: 0x0002B5A6 File Offset: 0x000297A6
	public void EndSunscreen()
	{
		this.sunscreenSVFX.EndFX();
	}

	// Token: 0x060007AF RID: 1967 RVA: 0x0002B5B4 File Offset: 0x000297B4
	private void OnInteractChange()
	{
		if (this.currentInteractable.UnityObjectExists<IInteractible>())
		{
			this.currentInteractable.HoverExit();
		}
		this.currentInteractable = Interaction.instance.currentHovered;
		if (this.currentInteractable.UnityObjectExists<IInteractible>())
		{
			this.currentInteractable.HoverEnter();
		}
		this.RefreshInteractablePrompt();
	}

	// Token: 0x060007B0 RID: 1968 RVA: 0x0002B608 File Offset: 0x00029808
	public void RefreshInteractablePrompt()
	{
		if (this.currentInteractable.UnityObjectExists<IInteractible>())
		{
			this.interactPromptText.text = this.currentInteractable.GetInteractionText();
			this.interactName.SetActive(true);
			this.interactPromptPrimary.SetActive(true);
			this.interactPromptSecondary.SetActive(false);
			this.interactPromptHold.SetActive(false);
			if (this.currentInteractable is Item)
			{
				this.interactNameText.text = ((Item)this.currentInteractable).GetItemName(null);
			}
			else
			{
				CharacterInteractible characterInteractible = this.currentInteractable as CharacterInteractible;
				if (characterInteractible != null)
				{
					this.interactPromptPrimary.SetActive(characterInteractible.IsPrimaryInteractible(Character.localCharacter));
					this.interactName.SetActive(false);
					if (characterInteractible.IsSecondaryInteractible(Character.localCharacter))
					{
						this.interactPromptSecondary.SetActive(true);
						this.secondaryInteractPromptText.text = characterInteractible.GetSecondaryInteractionText();
					}
				}
				else
				{
					this.interactNameText.text = this.currentInteractable.GetName();
				}
			}
		}
		else
		{
			this.interactName.SetActive(false);
			this.interactPromptPrimary.SetActive(false);
			this.interactPromptSecondary.SetActive(false);
			this.interactPromptHold.SetActive(false);
		}
		if (Character.localCharacter && Character.localCharacter.data.climbingSpikeCount > 0 && Character.localCharacter.data.isClimbing)
		{
			this.interactPromptSecondary.SetActive(true);
			this.secondaryInteractPromptText.text = LocalizedText.GetText("SETPITON", true);
		}
	}

	// Token: 0x060007B1 RID: 1969 RVA: 0x0002B78F File Offset: 0x0002998F
	public void EnableBinocularOverlay()
	{
		this.sinceShowedBinocularOverlay = 0;
	}

	// Token: 0x060007B2 RID: 1970 RVA: 0x0002B798 File Offset: 0x00029998
	private void UpdateBinocularOverlay()
	{
		if (this.sinceShowedBinocularOverlay > 1)
		{
			this.binocularOverlay.enabled = false;
		}
		else
		{
			this.binocularOverlay.enabled = true;
		}
		this.sinceShowedBinocularOverlay++;
	}

	// Token: 0x060007B3 RID: 1971 RVA: 0x0002B7CB File Offset: 0x000299CB
	public void BlurBinoculars()
	{
	}

	// Token: 0x060007B4 RID: 1972 RVA: 0x0002B7D0 File Offset: 0x000299D0
	public void UpdateItems()
	{
		if (Character.observedCharacter == null)
		{
			return;
		}
		if (Character.observedCharacter == null || Character.observedCharacter.player == null)
		{
			for (int i = 0; i < this.items.Length; i++)
			{
				this.items[i].SetItem(null);
			}
			this.backpack.SetItem(null);
			this.UpdateItemPrompts();
			this.temporaryItem.gameObject.SetActive(false);
			return;
		}
		for (int j = 0; j < this.items.Length; j++)
		{
			if (j < Character.observedCharacter.player.itemSlots.Length)
			{
				this.items[j].SetItem(Character.observedCharacter.player.itemSlots[j]);
			}
		}
		this.backpack.SetItem(Character.observedCharacter.player.backpackSlot);
		if (!Character.observedCharacter.player.GetItemSlot(250).IsEmpty())
		{
			this.temporaryItem.gameObject.SetActive(true);
			this.temporaryItem.SetItem(Character.observedCharacter.player.GetItemSlot(250));
		}
		else
		{
			this.temporaryItem.gameObject.SetActive(false);
			this.temporaryItem.Clear();
		}
		this.UpdateItemPrompts();
		this.bar.ChangeBar();
	}

	// Token: 0x060007B5 RID: 1973 RVA: 0x0002B928 File Offset: 0x00029B28
	public void PlayDayNightText(int x)
	{
	}

	// Token: 0x060007B6 RID: 1974 RVA: 0x0002B92C File Offset: 0x00029B2C
	private void TestUpdateItemPrompts()
	{
		if (!Character.localCharacter || !Character.localCharacter.data.currentItem)
		{
			this.canUsePrimaryPrevious = false;
			this.canUseSecondaryPrevious = false;
			return;
		}
		bool flag = Character.localCharacter.data.currentItem.CanUsePrimary();
		bool flag2 = Character.localCharacter.data.currentItem.CanUseSecondary();
		if (flag != this.canUsePrimaryPrevious || flag2 != this.canUseSecondaryPrevious)
		{
			this.UpdateItemPrompts();
		}
		this.canUsePrimaryPrevious = flag;
		this.canUsePrimaryPrevious = flag2;
	}

	// Token: 0x060007B7 RID: 1975 RVA: 0x0002B9BC File Offset: 0x00029BBC
	public void UpdateItemPrompts()
	{
		if (Character.localCharacter != null && Character.localCharacter.data.currentItem)
		{
			Item currentItem = Character.localCharacter.data.currentItem;
			Item.ItemUIData uidata = currentItem.UIData;
			this.itemPromptMain.text = this.GetMainInteractPrompt(currentItem);
			this.itemPromptSecondary.text = this.GetSecondaryInteractPrompt(currentItem);
			this.itemPromptScroll.text = LocalizedText.GetText(uidata.scrollInteractPrompt, true);
			this.itemPromptMain.gameObject.SetActive(uidata.hasMainInteract && Character.localCharacter.data.currentItem.CanUsePrimary());
			this.itemPromptSecondary.gameObject.SetActive(uidata.hasSecondInteract && Character.localCharacter.data.currentItem.CanUseSecondary());
			this.itemPromptScroll.gameObject.SetActive(uidata.hasScrollingInteract);
			this.itemPromptDrop.gameObject.SetActive(uidata.canDrop);
			this.itemPromptThrow.gameObject.SetActive(uidata.canThrow);
			return;
		}
		this.itemPromptMain.gameObject.SetActive(false);
		this.itemPromptSecondary.gameObject.SetActive(false);
		this.itemPromptScroll.gameObject.SetActive(false);
		this.itemPromptDrop.gameObject.SetActive(false);
		this.itemPromptThrow.gameObject.SetActive(false);
	}

	// Token: 0x060007B8 RID: 1976 RVA: 0x0002BB3A File Offset: 0x00029D3A
	public void TheFogRises()
	{
		this.fogRises.SetActive(true);
		base.StartCoroutine(this.<TheFogRises>g__FogRisesRoutine|201_0());
	}

	// Token: 0x060007B9 RID: 1977 RVA: 0x0002BB55 File Offset: 0x00029D55
	public void TheLavaRises()
	{
		this.lavaRises.SetActive(true);
		base.StartCoroutine(this.<TheLavaRises>g__FogRisesRoutine|202_0());
	}

	// Token: 0x060007BA RID: 1978 RVA: 0x0002BB70 File Offset: 0x00029D70
	private string GetMainInteractPrompt(Item item)
	{
		return LocalizedText.GetText(item.UIData.mainInteractPrompt, true);
	}

	// Token: 0x060007BB RID: 1979 RVA: 0x0002BB83 File Offset: 0x00029D83
	public string GetSecondaryInteractPrompt(Item item)
	{
		return LocalizedText.GetText(item.UIData.secondaryInteractPrompt, true);
	}

	// Token: 0x060007BC RID: 1980 RVA: 0x0002BB96 File Offset: 0x00029D96
	public void StartNumb()
	{
		this.staminaCanvasGroup.DOFade(0f, 1f);
		this.mushroomsCanvasGroup.DOFade(1f, 1f);
		this.mushroomsCanvasGroup.gameObject.SetActive(true);
	}

	// Token: 0x060007BD RID: 1981 RVA: 0x0002BBD5 File Offset: 0x00029DD5
	public void StopNumb()
	{
		this.staminaCanvasGroup.DOFade(1f, 1f);
		this.mushroomsCanvasGroup.DOFade(0f, 1f).OnComplete(delegate
		{
			this.mushroomsCanvasGroup.gameObject.SetActive(false);
		});
	}

	// Token: 0x060007BE RID: 1982 RVA: 0x0002BC14 File Offset: 0x00029E14
	public void Quicksave()
	{
		this.quickSave.SetActive(true);
		base.Invoke("EndQuicksave", 2f);
	}

	// Token: 0x060007BF RID: 1983 RVA: 0x0002BC32 File Offset: 0x00029E32
	private void EndQuicksave()
	{
		this.quickSave.SetActive(false);
	}

	// Token: 0x060007C1 RID: 1985 RVA: 0x0002BC76 File Offset: 0x00029E76
	[CompilerGenerated]
	private IEnumerator <SetHeroTitle>g__HeroRoutine|140_0(string heroString)
	{
		this.heroCanvasObject.gameObject.SetActive(true);
		yield return null;
		string dayString = DayNightManager.instance.DayCountString();
		string timeOfDayString = DayNightManager.instance.TimeOfDayString();
		this.heroObject.gameObject.SetActive(true);
		this.heroImage.color = new Color(this.heroImage.color.r, this.heroImage.color.g, this.heroImage.color.b, 1f);
		this.heroShadowImage.color = new Color(this.heroShadowImage.color.r, this.heroShadowImage.color.g, this.heroShadowImage.color.b, 0.12f);
		this.heroDayText.text = "";
		this.heroTimeOfDayText.text = "";
		this.heroBG.color = new Color(0f, 0f, 0f, 0f);
		this.heroBG.DOFade(0.5f, 0.5f);
		int num;
		for (int i = 0; i < heroString.Length; i = num + 1)
		{
			this.heroText.text = heroString.Substring(0, i + 1);
			this.heroCamera.Render();
			yield return new WaitForSeconds(0.1f);
			num = i;
		}
		yield return new WaitForSeconds(0.5f);
		for (int i = 0; i < dayString.Length; i = num + 1)
		{
			this.heroDayText.text = dayString.Substring(0, i + 1);
			this.heroCamera.Render();
			yield return new WaitForSeconds(0.066f);
			num = i;
		}
		yield return new WaitForSeconds(0.5f);
		for (int i = 0; i < timeOfDayString.Length; i = num + 1)
		{
			this.heroTimeOfDayText.text = timeOfDayString.Substring(0, i + 1);
			this.heroCamera.Render();
			yield return new WaitForSeconds(0.066f);
			num = i;
		}
		yield return new WaitForSeconds(1.5f);
		this.heroImage.DOFade(0f, 2f);
		this.heroShadowImage.DOFade(0f, 1f);
		this.heroBG.DOFade(0f, 2f);
		yield return new WaitForSeconds(2f);
		this.heroObject.gameObject.SetActive(false);
		this.heroCanvasObject.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x060007C6 RID: 1990 RVA: 0x0002BCC2 File Offset: 0x00029EC2
	[CompilerGenerated]
	private IEnumerator <TheFogRises>g__FogRisesRoutine|201_0()
	{
		yield return new WaitForSeconds(4f);
		this.fogRises.SetActive(false);
		yield break;
	}

	// Token: 0x060007C7 RID: 1991 RVA: 0x0002BCD1 File Offset: 0x00029ED1
	[CompilerGenerated]
	private IEnumerator <TheLavaRises>g__FogRisesRoutine|202_0()
	{
		yield return new WaitForSeconds(4f);
		this.lavaRises.SetActive(false);
		yield break;
	}

	// Token: 0x0400074B RID: 1867
	public static GUIManager instance;

	// Token: 0x0400074C RID: 1868
	public Canvas hudCanvas;

	// Token: 0x0400074D RID: 1869
	public Canvas binocularOverlay;

	// Token: 0x0400074E RID: 1870
	public Canvas letterboxCanvas;

	// Token: 0x0400074F RID: 1871
	public BoardingPass boardingPass;

	// Token: 0x04000750 RID: 1872
	public StaminaBar bar;

	// Token: 0x04000751 RID: 1873
	public InventoryItemUI[] items;

	// Token: 0x04000752 RID: 1874
	public InventoryItemUI backpack;

	// Token: 0x04000753 RID: 1875
	public InventoryItemUI temporaryItem;

	// Token: 0x04000754 RID: 1876
	public CanvasGroup hudCanvasGroup;

	// Token: 0x04000755 RID: 1877
	public CanvasGroup staminaCanvasGroup;

	// Token: 0x04000756 RID: 1878
	public CanvasGroup mushroomsCanvasGroup;

	// Token: 0x04000757 RID: 1879
	public Sprite emptySprite;

	// Token: 0x04000758 RID: 1880
	public UI_Rope ui_rope;

	// Token: 0x04000759 RID: 1881
	public GameObject emoteWheel;

	// Token: 0x0400075A RID: 1882
	public BackpackWheel backpackWheel;

	// Token: 0x0400075B RID: 1883
	public UIPlayerNames playerNames;

	// Token: 0x0400075C RID: 1884
	public UI_UseItemProgressFriend friendUseItemProgressPrefab;

	// Token: 0x0400075D RID: 1885
	public Transform friendProgressTF;

	// Token: 0x0400075E RID: 1886
	public GameObject fogRises;

	// Token: 0x0400075F RID: 1887
	public GameObject lavaRises;

	// Token: 0x04000760 RID: 1888
	public LoadingScreen loadingScreenPrefab;

	// Token: 0x04000761 RID: 1889
	[FormerlySerializedAs("endgameCounter")]
	public EndgameCounter endgame;

	// Token: 0x04000762 RID: 1890
	public EndScreen endScreen;

	// Token: 0x04000763 RID: 1891
	[SerializeField]
	private GameObject pauseMenu;

	// Token: 0x04000764 RID: 1892
	public PauseMenuMainPage pauseMenuMainPage;

	// Token: 0x04000765 RID: 1893
	public List<UI_UseItemProgressFriend> friendUseItemProgressList = new List<UI_UseItemProgressFriend>();

	// Token: 0x04000766 RID: 1894
	private TextMeshProUGUI text;

	// Token: 0x04000768 RID: 1896
	public GameObject interactName;

	// Token: 0x04000769 RID: 1897
	public TextMeshProUGUI interactNameText;

	// Token: 0x0400076A RID: 1898
	public GameObject interactPromptPrimary;

	// Token: 0x0400076B RID: 1899
	public GameObject interactPromptSecondary;

	// Token: 0x0400076C RID: 1900
	public GameObject interactPromptHold;

	// Token: 0x0400076D RID: 1901
	public GameObject interactPromptLunge;

	// Token: 0x0400076E RID: 1902
	public TextMeshProUGUI interactPromptText;

	// Token: 0x0400076F RID: 1903
	public TextMeshProUGUI secondaryInteractPromptText;

	// Token: 0x04000770 RID: 1904
	public CanvasGroup strugglePrompt;

	// Token: 0x04000771 RID: 1905
	public TextMeshProUGUI itemPromptMain;

	// Token: 0x04000772 RID: 1906
	public TextMeshProUGUI itemPromptScroll;

	// Token: 0x04000773 RID: 1907
	public TextMeshProUGUI itemPromptSecondary;

	// Token: 0x04000774 RID: 1908
	public TextMeshProUGUI itemPromptDrop;

	// Token: 0x04000775 RID: 1909
	public TextMeshProUGUI itemPromptThrow;

	// Token: 0x04000776 RID: 1910
	public GameObject throwGO;

	// Token: 0x04000777 RID: 1911
	public Image throwBar;

	// Token: 0x04000778 RID: 1912
	public Gradient throwGradient;

	// Token: 0x04000779 RID: 1913
	public GameObject dyingBarObject;

	// Token: 0x0400077A RID: 1914
	public RectTransform dyingBarRect;

	// Token: 0x0400077B RID: 1915
	public Image dyingBarImage;

	// Token: 0x0400077C RID: 1916
	public Gradient dyingBarGradient;

	// Token: 0x0400077D RID: 1917
	public Animator dyingBarAnimator;

	// Token: 0x0400077E RID: 1918
	public GameObject dyingBarMushrooms;

	// Token: 0x0400077F RID: 1919
	public GameObject spectatingObject;

	// Token: 0x04000780 RID: 1920
	public GameObject spectatingInputs;

	// Token: 0x04000781 RID: 1921
	public TextMeshProUGUI spectatingNameText;

	// Token: 0x04000782 RID: 1922
	public Color spectatingNameColor;

	// Token: 0x04000783 RID: 1923
	public Color spectatingYourselfColor;

	// Token: 0x04000784 RID: 1924
	public GameObject heroObject;

	// Token: 0x04000785 RID: 1925
	public GameObject heroCanvasObject;

	// Token: 0x04000786 RID: 1926
	public Camera heroCamera;

	// Token: 0x04000787 RID: 1927
	public Image heroBG;

	// Token: 0x04000788 RID: 1928
	public RawImage heroImage;

	// Token: 0x04000789 RID: 1929
	public RawImage heroShadowImage;

	// Token: 0x0400078A RID: 1930
	public TextMeshProUGUI heroText;

	// Token: 0x0400078B RID: 1931
	public TextMeshProUGUI heroDayText;

	// Token: 0x0400078C RID: 1932
	public TextMeshProUGUI heroTimeOfDayText;

	// Token: 0x0400078D RID: 1933
	public AudioSource stingerSound;

	// Token: 0x0400078E RID: 1934
	public Volume blurVolume;

	// Token: 0x0400078F RID: 1935
	public Volume coldVolume;

	// Token: 0x04000790 RID: 1936
	public Volume sugarRushVolume;

	// Token: 0x04000791 RID: 1937
	public ScreenVFX injurySVFX;

	// Token: 0x04000792 RID: 1938
	public ScreenVFX coldSVFX;

	// Token: 0x04000793 RID: 1939
	public ScreenVFX poisonSVFX;

	// Token: 0x04000794 RID: 1940
	public ScreenVFX sugarRushSVFX;

	// Token: 0x04000795 RID: 1941
	public ScreenVFX hotSVFX;

	// Token: 0x04000796 RID: 1942
	public ScreenVFX energySVFX;

	// Token: 0x04000797 RID: 1943
	public ScreenVFX drowsyFX;

	// Token: 0x04000798 RID: 1944
	public ScreenVFX heatSVFX;

	// Token: 0x04000799 RID: 1945
	public ScreenVFX curseSVFX;

	// Token: 0x0400079A RID: 1946
	public ScreenVFX sunscreenSVFX;

	// Token: 0x0400079B RID: 1947
	public ScreenVFX thornsSVFX;

	// Token: 0x0400079C RID: 1948
	public ScreenVFX sporesSVFX;

	// Token: 0x0400079D RID: 1949
	public ScreenVFX sporesWarning;

	// Token: 0x0400079E RID: 1950
	private Character character;

	// Token: 0x0400079F RID: 1951
	public GameObject reticleDefault;

	// Token: 0x040007A0 RID: 1952
	public GameObject reticleX;

	// Token: 0x040007A1 RID: 1953
	public GameObject reticleClimb;

	// Token: 0x040007A2 RID: 1954
	public GameObject reticleClimbJump;

	// Token: 0x040007A3 RID: 1955
	public GameObject reticleThrow;

	// Token: 0x040007A4 RID: 1956
	public GameObject reticleReach;

	// Token: 0x040007A5 RID: 1957
	public GameObject reticleGrasp;

	// Token: 0x040007A6 RID: 1958
	public GameObject reticleSpike;

	// Token: 0x040007A7 RID: 1959
	public GameObject reticleRope;

	// Token: 0x040007A8 RID: 1960
	public GameObject reticleClimbTry;

	// Token: 0x040007A9 RID: 1961
	public GameObject reticleVine;

	// Token: 0x040007AA RID: 1962
	public GameObject reticleBoost;

	// Token: 0x040007AB RID: 1963
	public GameObject reticleShoot;

	// Token: 0x040007AC RID: 1964
	public GameObject reticleKick;

	// Token: 0x040007AD RID: 1965
	public Image reticleDefaultImage;

	// Token: 0x040007AE RID: 1966
	public Color reticleColorDefault;

	// Token: 0x040007AF RID: 1967
	public Color reticleColorHighlight;

	// Token: 0x040007B0 RID: 1968
	private Coroutine _heroRoutine;

	// Token: 0x040007B2 RID: 1970
	public BadgeManager mainBadgeManager;

	// Token: 0x040007B4 RID: 1972
	public GameObject quickSave;

	// Token: 0x040007B5 RID: 1973
	public bool photosensitivity;

	// Token: 0x040007B6 RID: 1974
	public bool colorblindness;

	// Token: 0x040007B7 RID: 1975
	private bool wasPitonClimbing;

	// Token: 0x040007BA RID: 1978
	private int lastBlockedInput;

	// Token: 0x040007BB RID: 1979
	private bool dead;

	// Token: 0x040007BC RID: 1980
	private Character currentSpecCharacter;

	// Token: 0x040007BD RID: 1981
	private int ROPE_INVERT = Shader.PropertyToID("Invert");

	// Token: 0x040007BE RID: 1982
	private float reticleLock;

	// Token: 0x040007BF RID: 1983
	private GameObject lastReticle;

	// Token: 0x040007C0 RID: 1984
	private List<GameObject> reticleList = new List<GameObject>();

	// Token: 0x040007C1 RID: 1985
	private Sequence injurySequence;

	// Token: 0x040007C2 RID: 1986
	private Sequence hungerSequence;

	// Token: 0x040007C3 RID: 1987
	private Sequence coldSequence;

	// Token: 0x040007C4 RID: 1988
	private Sequence poisonSequence;

	// Token: 0x040007C5 RID: 1989
	public int sinceShowedBinocularOverlay = 10;

	// Token: 0x040007C6 RID: 1990
	private bool canUsePrimaryPrevious;

	// Token: 0x040007C7 RID: 1991
	private bool canUseSecondaryPrevious;

	// Token: 0x0200045B RID: 1115
	// (Invoke) Token: 0x06001C24 RID: 7204
	public delegate void MenuWindowEvent(MenuWindow window);
}

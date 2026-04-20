using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Photon.Pun;
using Photon.Realtime;
using Unity.Multiplayer.Playmode;
using UnityEngine;
using UnityEngine.Serialization;
using Zorro.Core;
using Zorro.Core.CLI;

// Token: 0x02000072 RID: 114
[ConsoleClassCustomizer("Customization")]
public class CharacterCustomization : MonoBehaviour
{
	// Token: 0x1700006A RID: 106
	// (get) Token: 0x0600052D RID: 1325 RVA: 0x0001E8D4 File Offset: 0x0001CAD4
	public Color PlayerColor
	{
		get
		{
			CharacterCustomizationData customizationData = CharacterCustomization.GetCustomizationData(this._character.photonView.Owner);
			return Singleton<Customization>.Instance.skins[customizationData.currentSkin].color;
		}
	}

	// Token: 0x1700006B RID: 107
	// (get) Token: 0x0600052E RID: 1326 RVA: 0x0001E910 File Offset: 0x0001CB10
	public Vector3 PlayerColorAsVector
	{
		get
		{
			Color playerColor = this.PlayerColor;
			return new Vector3(playerColor.r, playerColor.g, playerColor.b);
		}
	}

	// Token: 0x0600052F RID: 1327 RVA: 0x0001E93B File Offset: 0x0001CB3B
	private void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
		this._character = base.GetComponent<Character>();
	}

	// Token: 0x06000530 RID: 1328 RVA: 0x0001E958 File Offset: 0x0001CB58
	public void Start()
	{
		if (this.view.IsMine && !this._character.isBot)
		{
			this.SetRandomIdle();
			this.HideChicken();
			InRoomState inRoomState = GameHandler.GetService<ConnectionService>().StateMachine.CurrentState as InRoomState;
			if (inRoomState != null && !inRoomState.hasLoadedCustomization)
			{
				inRoomState.hasLoadedCustomization = true;
				base.StartCoroutine(this.GetCosmeticsFromSteamRoutine());
				if (this._character.IsLocal)
				{
					this.refs.mainRenderer.updateWhenOffscreen = true;
				}
			}
		}
		Character character = this._character;
		character.reviveAction = (Action)Delegate.Combine(character.reviveAction, new Action(this.OnRevive));
		Character character2 = this._character;
		character2.UnPassOutAction = (Action)Delegate.Combine(character2.UnPassOutAction, new Action(this.OnRevive));
		Photon.Realtime.Player player = (this.overridePhotonPlayer != null) ? this.overridePhotonPlayer : this._character.photonView.Owner;
		PersistentPlayerDataService service = GameHandler.GetService<PersistentPlayerDataService>();
		service.SubscribeToPlayerDataChange(player, new Action<PersistentPlayerData>(this.OnPlayerDataChange));
		this.OnPlayerDataChange(service.GetPlayerData(player));
	}

	// Token: 0x06000531 RID: 1329 RVA: 0x0001EA72 File Offset: 0x0001CC72
	private IEnumerator GetCosmeticsFromSteamRoutine()
	{
		while (!AchievementManager.GotStats)
		{
			yield return null;
		}
		this.TryGetCosmeticsFromSteam();
		yield break;
	}

	// Token: 0x06000532 RID: 1330 RVA: 0x0001EA84 File Offset: 0x0001CC84
	private void TryGetCosmeticsFromSteam()
	{
		if (this.ignorePlayerCosmetics)
		{
			return;
		}
		if (CurrentPlayer.ReadOnlyTags().Contains("RandomCosmetics"))
		{
			this.RandomizeCosmetics();
			this.SetRandomSkinColor();
			return;
		}
		int num;
		if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.LoadedCosmeticsPreviously, out num))
		{
			if (num > 0)
			{
				int num2;
				if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.Cosmetic_Skin, out num2) && num2 != -1)
				{
					CharacterCustomization.SetCharacterSkinColor(num2);
				}
				else
				{
					this.SetRandomSkinColor();
				}
				int num3;
				if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.Cosmetic_Eyes, out num3) && num3 != -1)
				{
					CharacterCustomization.SetCharacterEyes(num3);
				}
				else
				{
					this.SetRandomEyes();
				}
				int num4;
				if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.Cosmetic_Mouth, out num4) && num4 != -1)
				{
					CharacterCustomization.SetCharacterMouth(num4);
				}
				else
				{
					this.SetRandomMouth();
				}
				int characterAccessory;
				if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.Cosmetic_Accessory, out characterAccessory) && num4 != -1)
				{
					CharacterCustomization.SetCharacterAccessory(characterAccessory);
				}
				else
				{
					this.SetRandomAccessory();
				}
				int num5;
				if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.Cosmetic_Outfit, out num5) && num5 != -1)
				{
					CharacterCustomization.SetCharacterOutfit(num5);
				}
				else
				{
					this.SetRandomOutfit();
				}
				int num6;
				if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.Cosmetic_Hat, out num6) && num6 != -1)
				{
					CharacterCustomization.SetCharacterHat(num6);
				}
				else
				{
					this.SetRandomHat();
				}
				int num7;
				if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.Cosmetic_Sash, out num7) && num7 != -1)
				{
					CharacterCustomization.SetCharacterSash(num7);
				}
				else
				{
					CharacterCustomization.SetCharacterSash(0);
				}
			}
			else
			{
				this.RandomizeCosmetics();
			}
			Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.LoadedCosmeticsPreviously, 1);
			return;
		}
		this.SetRandomSkinColor();
	}

	// Token: 0x06000533 RID: 1331 RVA: 0x0001EBDF File Offset: 0x0001CDDF
	[ConsoleCommand]
	public static void Randomize()
	{
		Character.localCharacter.refs.customization.RandomizeCosmetics();
	}

	// Token: 0x06000534 RID: 1332 RVA: 0x0001EBF5 File Offset: 0x0001CDF5
	public void RandomizeCosmetics()
	{
		this.SetRandomSkinColor();
		this.SetRandomEyes();
		this.SetRandomMouth();
		this.SetRandomAccessory();
		this.SetRandomOutfit();
		this.SetRandomHat();
	}

	// Token: 0x06000535 RID: 1333 RVA: 0x0001EC1C File Offset: 0x0001CE1C
	private void OnDestroy()
	{
		PersistentPlayerDataService service = GameHandler.GetService<PersistentPlayerDataService>();
		if (service != null)
		{
			service.UnsubscribeToPlayerDataChange(this._character.photonView.Owner, new Action<PersistentPlayerData>(this.OnPlayerDataChange));
		}
	}

	// Token: 0x06000536 RID: 1334 RVA: 0x0001EC54 File Offset: 0x0001CE54
	public void SetCustomizationForRef(CustomizationRefs refs)
	{
		CustomizationRefs customizationRefs = this.refs;
		this.refs = refs;
		PersistentPlayerDataService service = GameHandler.GetService<PersistentPlayerDataService>();
		service.SubscribeToPlayerDataChange(this._character.photonView.Owner, new Action<PersistentPlayerData>(this.OnPlayerDataChange));
		this.OnPlayerDataChange(service.GetPlayerData(this.view.Owner));
		this.refs = customizationRefs;
	}

	// Token: 0x06000537 RID: 1335 RVA: 0x0001ECB8 File Offset: 0x0001CEB8
	internal void OnPlayerDataChange(PersistentPlayerData playerData)
	{
		if (this.refs.PlayerRenderers[0] == null)
		{
			return;
		}
		if (this._character.data.isScoutmaster)
		{
			return;
		}
		if (this.ignorePlayerCosmetics)
		{
			return;
		}
		Debug.Log("On Player Data Change");
		int skinIndex = CharacterCustomization.GetSkinIndex(playerData);
		if (this.useDebugColor)
		{
			skinIndex = this.debugColorIndex;
		}
		Renderer[] array = this.refs.PlayerRenderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].material.SetColor(CharacterCustomization.SkinColor, Singleton<Customization>.Instance.skins[skinIndex].color);
		}
		array = this.refs.EyeRenderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].material.SetColor(CharacterCustomization.SkinColor, Singleton<Customization>.Instance.skins[skinIndex].color);
		}
		int fitIndex = CharacterCustomization.GetFitIndex(playerData);
		this.refs.mainRenderer.sharedMesh = Singleton<Customization>.Instance.fits[fitIndex].fitMesh;
		this.refs.mainRendererShadow.sharedMesh = Singleton<Customization>.Instance.fits[fitIndex].fitMesh;
		List<Material> list = new List<Material>();
		list.Add(this.refs.mainRenderer.materials[0]);
		list.Add(Singleton<Customization>.Instance.fits[fitIndex].fitMaterial);
		list.Add(Singleton<Customization>.Instance.fits[fitIndex].fitMaterialShoes);
		this.refs.mainRenderer.SetSharedMaterials(list);
		if (Singleton<Customization>.Instance.fits[fitIndex].noPants)
		{
			this.refs.skirt.gameObject.SetActive(false);
			this.refs.shortsShadow.gameObject.SetActive(false);
			this.refs.skirtShadow.gameObject.SetActive(false);
			this.refs.shorts.gameObject.SetActive(false);
		}
		else if (Singleton<Customization>.Instance.fits[fitIndex].isSkirt)
		{
			this.refs.skirt.gameObject.SetActive(true);
			this.refs.shortsShadow.gameObject.SetActive(false);
			this.refs.skirtShadow.gameObject.SetActive(true);
			this.refs.shorts.gameObject.SetActive(false);
			this.refs.skirt.sharedMaterial = Singleton<Customization>.Instance.fits[fitIndex].fitPantsMaterial;
		}
		else
		{
			this.refs.skirt.gameObject.SetActive(false);
			this.refs.shortsShadow.gameObject.SetActive(true);
			this.refs.skirtShadow.gameObject.SetActive(false);
			this.refs.shorts.gameObject.SetActive(true);
			this.refs.shorts.sharedMaterial = Singleton<Customization>.Instance.fits[fitIndex].fitPantsMaterial;
		}
		this.refs.playerHats[0].material = Singleton<Customization>.Instance.fits[fitIndex].fitHatMaterial;
		this.refs.playerHats[1].material = Singleton<Customization>.Instance.fits[fitIndex].fitHatMaterial;
		int eyesIndex = CharacterCustomization.GetEyesIndex(playerData);
		this.CurrentEyeTexture = Singleton<Customization>.Instance.eyes[eyesIndex].texture;
		array = this.refs.EyeRenderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].material.SetTexture(CharacterCustomization.MainTex, this.CurrentEyeTexture);
		}
		int mouthIndex = CharacterCustomization.GetMouthIndex(playerData);
		this.refs.mouthRenderer.material.SetTexture(CharacterCustomization.MainTex, Singleton<Customization>.Instance.mouths[mouthIndex].texture);
		int accessoryIndex = CharacterCustomization.GetAccessoryIndex(playerData);
		this.refs.accessoryRenderer.material.SetTexture(CharacterCustomization.MainTex, Singleton<Customization>.Instance.accessories[accessoryIndex].texture);
		this.refs.accessoryRenderer.material.renderQueue = (Singleton<Customization>.Instance.accessories[accessoryIndex].drawUnderEye ? 3007 : 3009);
		this.refs.accessoryEnabled = (!Singleton<Customization>.Instance.accessories[accessoryIndex].isThirdEye && !this._allRenderersHidden && !this._character.data.isSkeleton);
		this.refs.thirdEye.gameObject.SetActive(Singleton<Customization>.Instance.accessories[accessoryIndex].isThirdEye);
		int num = playerData.customizationData.currentHat;
		if (Singleton<Customization>.Instance.fits[fitIndex].overrideHat)
		{
			num = Singleton<Customization>.Instance.fits[fitIndex].overrideHatIndex;
		}
		MeshFilter meshFilter = null;
		for (int j = 0; j < this.refs.playerHats.Length; j++)
		{
			this.refs.playerHats[j].gameObject.SetActive(num == j);
			if (num == j)
			{
				meshFilter = this.refs.playerHats[j].GetComponent<MeshFilter>();
			}
		}
		if (!meshFilter)
		{
			meshFilter = this.refs.playerHats[0].GetComponent<MeshFilter>();
		}
		this.refs.hatShadowMeshFilter.sharedMesh = meshFilter.sharedMesh;
		this.refs.hatShadowMeshFilter.transform.SetPositionAndRotation(meshFilter.transform.position, meshFilter.transform.rotation);
		this.refs.hatShadowMeshFilter.transform.localScale = meshFilter.transform.localScale;
		List<Material> list2 = new List<Material>();
		list2.Add(this.refs.sashRenderer.materials[0]);
		int sashIndex = CharacterCustomization.GetSashIndex(playerData);
		list2.Add(this.refs.sashAscentMaterials[sashIndex]);
		this.refs.sashRenderer.SetMaterials(list2);
		if (this._character && this._character.refs.hideTheBody)
		{
			this._character.refs.hideTheBody.Refresh();
		}
	}

	// Token: 0x06000538 RID: 1336 RVA: 0x0001F2F4 File Offset: 0x0001D4F4
	public static int GetEyesIndex(PersistentPlayerData playerData)
	{
		int num = playerData.customizationData.currentEyes;
		if (num >= Singleton<Customization>.Instance.eyes.Length)
		{
			num = 0;
		}
		return num;
	}

	// Token: 0x06000539 RID: 1337 RVA: 0x0001F320 File Offset: 0x0001D520
	public static int GetSkinIndex(PersistentPlayerData playerData)
	{
		int num = playerData.customizationData.currentSkin;
		if (num >= Singleton<Customization>.Instance.skins.Length)
		{
			num = 0;
		}
		return num;
	}

	// Token: 0x0600053A RID: 1338 RVA: 0x0001F34C File Offset: 0x0001D54C
	public static int GetFitIndex(PersistentPlayerData playerData)
	{
		int num = playerData.customizationData.currentOutfit;
		if (num >= Singleton<Customization>.Instance.fits.Length)
		{
			num = 0;
		}
		return num;
	}

	// Token: 0x0600053B RID: 1339 RVA: 0x0001F378 File Offset: 0x0001D578
	public static int GetMouthIndex(PersistentPlayerData playerData)
	{
		int num = playerData.customizationData.currentMouth;
		if (num >= Singleton<Customization>.Instance.mouths.Length)
		{
			num = 0;
		}
		return num;
	}

	// Token: 0x0600053C RID: 1340 RVA: 0x0001F3A4 File Offset: 0x0001D5A4
	public static int GetAccessoryIndex(PersistentPlayerData playerData)
	{
		int num = playerData.customizationData.currentAccessory;
		if (num >= Singleton<Customization>.Instance.accessories.Length)
		{
			num = 0;
		}
		return num;
	}

	// Token: 0x0600053D RID: 1341 RVA: 0x0001F3D0 File Offset: 0x0001D5D0
	public static int GetSashIndex(PersistentPlayerData playerData)
	{
		int num = playerData.customizationData.currentSash;
		if (num >= Singleton<Customization>.Instance.sashes.Length)
		{
			num = Singleton<Customization>.Instance.sashes.Length - 1;
		}
		return num;
	}

	// Token: 0x0600053E RID: 1342 RVA: 0x0001F408 File Offset: 0x0001D608
	public static void SetCharacterSkinColor(int index)
	{
		if (index >= Singleton<Customization>.Instance.skins.Length)
		{
			index = 0;
		}
		CharacterCustomizationData customizationData = CharacterCustomization.GetCustomizationData(PhotonNetwork.LocalPlayer);
		customizationData.currentSkin = index;
		CharacterCustomization.SetCustomizationData(customizationData, PhotonNetwork.LocalPlayer);
		Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.Cosmetic_Skin, index);
		Debug.Log(string.Format("Set character color: {0}", index));
	}

	// Token: 0x0600053F RID: 1343 RVA: 0x0001F464 File Offset: 0x0001D664
	public static void SetCharacterEyes(int index)
	{
		if (index >= Singleton<Customization>.Instance.eyes.Length)
		{
			index = 0;
		}
		CharacterCustomizationData customizationData = CharacterCustomization.GetCustomizationData(PhotonNetwork.LocalPlayer);
		customizationData.currentEyes = index;
		CharacterCustomization.SetCustomizationData(customizationData, PhotonNetwork.LocalPlayer);
		Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.Cosmetic_Eyes, index);
		Debug.Log(string.Format("Set character eyes: {0}", index));
	}

	// Token: 0x06000540 RID: 1344 RVA: 0x0001F4C0 File Offset: 0x0001D6C0
	public static void SetCharacterMouth(int index)
	{
		if (index >= Singleton<Customization>.Instance.mouths.Length)
		{
			index = 0;
		}
		CharacterCustomizationData customizationData = CharacterCustomization.GetCustomizationData(PhotonNetwork.LocalPlayer);
		customizationData.currentMouth = index;
		CharacterCustomization.SetCustomizationData(customizationData, PhotonNetwork.LocalPlayer);
		Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.Cosmetic_Mouth, index);
		Debug.Log(string.Format("Setting Character Mouth: {0}", index));
	}

	// Token: 0x06000541 RID: 1345 RVA: 0x0001F51C File Offset: 0x0001D71C
	public static void SetCharacterAccessory(int index)
	{
		if (index >= Singleton<Customization>.Instance.accessories.Length)
		{
			index = 0;
		}
		CharacterCustomizationData customizationData = CharacterCustomization.GetCustomizationData(PhotonNetwork.LocalPlayer);
		customizationData.currentAccessory = index;
		CharacterCustomization.SetCustomizationData(customizationData, PhotonNetwork.LocalPlayer);
		Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.Cosmetic_Accessory, index);
		Debug.Log(string.Format("Setting Character Accessory: {0}", index));
	}

	// Token: 0x06000542 RID: 1346 RVA: 0x0001F578 File Offset: 0x0001D778
	public static void SetCharacterOutfit(int index)
	{
		if (index >= Singleton<Customization>.Instance.fits.Length)
		{
			index = 0;
		}
		CharacterCustomizationData customizationData = CharacterCustomization.GetCustomizationData(PhotonNetwork.LocalPlayer);
		customizationData.currentOutfit = index;
		CharacterCustomization.SetCustomizationData(customizationData, PhotonNetwork.LocalPlayer);
		Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.Cosmetic_Outfit, index);
		Debug.Log(string.Format("Setting Character outfit: {0}", index));
	}

	// Token: 0x06000543 RID: 1347 RVA: 0x0001F5D4 File Offset: 0x0001D7D4
	public static void SetCharacterHat(int index)
	{
		if (index >= Singleton<Customization>.Instance.hats.Length)
		{
			index = 0;
		}
		CharacterCustomizationData customizationData = CharacterCustomization.GetCustomizationData(PhotonNetwork.LocalPlayer);
		customizationData.currentHat = index;
		CharacterCustomization.SetCustomizationData(customizationData, PhotonNetwork.LocalPlayer);
		Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.Cosmetic_Hat, index);
		Debug.Log(string.Format("Setting Character Hat: {0}", index));
	}

	// Token: 0x06000544 RID: 1348 RVA: 0x0001F630 File Offset: 0x0001D830
	public static void SetCharacterSash(int index)
	{
		if (index >= Singleton<Customization>.Instance.sashes.Length)
		{
			index = Singleton<Customization>.Instance.sashes.Length - 1;
		}
		CharacterCustomizationData customizationData = CharacterCustomization.GetCustomizationData(PhotonNetwork.LocalPlayer);
		customizationData.currentSash = index;
		CharacterCustomization.SetCustomizationData(customizationData, PhotonNetwork.LocalPlayer);
		Singleton<AchievementManager>.Instance.SetSteamStat(STEAMSTATTYPE.Cosmetic_Sash, index);
		Debug.Log(string.Format("Setting Character Sash: {0}", index));
	}

	// Token: 0x06000545 RID: 1349 RVA: 0x0001F699 File Offset: 0x0001D899
	public void SetRandomSkinColor()
	{
		CharacterCustomization.SetCharacterSkinColor(Singleton<Customization>.Instance.GetRandomUnlockedIndex(Customization.Type.Skin));
	}

	// Token: 0x06000546 RID: 1350 RVA: 0x0001F6AB File Offset: 0x0001D8AB
	public void SetRandomEyes()
	{
		CharacterCustomization.SetCharacterEyes(Singleton<Customization>.Instance.GetRandomUnlockedIndex(Customization.Type.Eyes));
	}

	// Token: 0x06000547 RID: 1351 RVA: 0x0001F6BE File Offset: 0x0001D8BE
	public void SetRandomMouth()
	{
		CharacterCustomization.SetCharacterMouth(Singleton<Customization>.Instance.GetRandomUnlockedIndex(Customization.Type.Mouth));
	}

	// Token: 0x06000548 RID: 1352 RVA: 0x0001F6D1 File Offset: 0x0001D8D1
	public void SetRandomAccessory()
	{
		CharacterCustomization.SetCharacterAccessory(Singleton<Customization>.Instance.GetRandomUnlockedIndex(Customization.Type.Accessory));
	}

	// Token: 0x06000549 RID: 1353 RVA: 0x0001F6E4 File Offset: 0x0001D8E4
	public void SetRandomOutfit()
	{
		CharacterCustomization.SetCharacterOutfit(Singleton<Customization>.Instance.GetRandomUnlockedIndex(Customization.Type.Fit));
	}

	// Token: 0x0600054A RID: 1354 RVA: 0x0001F6F7 File Offset: 0x0001D8F7
	public void SetRandomHat()
	{
		CharacterCustomization.SetCharacterHat(Singleton<Customization>.Instance.GetRandomUnlockedIndex(Customization.Type.Hat));
	}

	// Token: 0x0600054B RID: 1355 RVA: 0x0001F70A File Offset: 0x0001D90A
	public void SetRandomIdle()
	{
		if (this.view.IsMine)
		{
			this.view.RPC("SetCharacterIdle_RPC", RpcTarget.AllBuffered, new object[]
			{
				Random.Range(0, this.maxIdles)
			});
		}
	}

	// Token: 0x0600054C RID: 1356 RVA: 0x0001F744 File Offset: 0x0001D944
	[PunRPC]
	public void CharacterDied()
	{
		for (int i = 0; i < this.refs.EyeRenderers.Length; i++)
		{
			this.refs.EyeRenderers[i].material.SetTexture(CharacterCustomization.MainTex, this.deadEyes);
			this.refs.EyeRenderers[i].material.SetInt(CharacterCustomization.Spin, 0);
		}
	}

	// Token: 0x0600054D RID: 1357 RVA: 0x0001F7A8 File Offset: 0x0001D9A8
	[PunRPC]
	public void CharacterPassedOut()
	{
		this.isPassedOut = true;
		if (this.forcePresetEyes)
		{
			return;
		}
		for (int i = 0; i < this.refs.EyeRenderers.Length; i++)
		{
			this.refs.EyeRenderers[i].material.SetTexture(CharacterCustomization.MainTex, this.passedOutEyes);
			this.refs.EyeRenderers[i].material.SetInt(CharacterCustomization.Spin, 1);
		}
	}

	// Token: 0x0600054E RID: 1358 RVA: 0x0001F81C File Offset: 0x0001DA1C
	public void BecomeChicken()
	{
		if (!this.isCannibalizable)
		{
			this.isCannibalizable = true;
			if (this.chickenTweener != null)
			{
				this.chickenTweener.Kill(false);
			}
			this.ShowChicken();
			this.chickenTweener = this.refs.chickenRenderer.material.DOFloat(1f, CharacterCustomization.Opacity, 1f).OnComplete(new TweenCallback(this.HideHuman));
			for (int i = 0; i < this.refs.AllRenderers.Length; i++)
			{
				for (int j = 0; j < this.refs.AllRenderers[i].materials.Length; j++)
				{
					this.refs.AllRenderers[i].materials[j].DOFloat(0f, CharacterCustomization.Opacity, 1f);
				}
			}
			this.chickenParticle.Play();
			this.refs.hatTransform.DOLocalMoveY(-4.66f, 1f, false);
		}
	}

	// Token: 0x0600054F RID: 1359 RVA: 0x0001F918 File Offset: 0x0001DB18
	public void BecomeHuman()
	{
		if (this.isCannibalizable)
		{
			this.isCannibalizable = false;
			if (this.chickenTweener != null)
			{
				this.chickenTweener.Kill(false);
			}
			this.chickenTweener = this.refs.chickenRenderer.material.DOFloat(0f, CharacterCustomization.Opacity, 1f).OnComplete(new TweenCallback(this.HideChicken));
			this.ShowHuman();
			for (int i = 0; i < this.refs.AllRenderers.Length; i++)
			{
				for (int j = 0; j < this.refs.AllRenderers[i].materials.Length; j++)
				{
					this.refs.AllRenderers[i].materials[j].DOFloat(1f, CharacterCustomization.Opacity, 1f);
				}
			}
			this.chickenParticle.Stop();
			this.refs.hatTransform.DOLocalMoveY(-3.98f, 1f, false);
		}
	}

	// Token: 0x06000550 RID: 1360 RVA: 0x0001FA13 File Offset: 0x0001DC13
	private void ShowChicken()
	{
		this.refs.chickenRenderer.gameObject.SetActive(true);
	}

	// Token: 0x06000551 RID: 1361 RVA: 0x0001FA2B File Offset: 0x0001DC2B
	private void HideChicken()
	{
		this.refs.chickenRenderer.gameObject.SetActive(false);
	}

	// Token: 0x06000552 RID: 1362 RVA: 0x0001FA44 File Offset: 0x0001DC44
	private void ShowHuman()
	{
		this.refs.mainRenderer.gameObject.SetActive(true);
		this.refs.sashRenderer.gameObject.SetActive(true);
		this.refs.shorts.gameObject.SetActive(true);
		this.refs.skirt.gameObject.SetActive(true);
	}

	// Token: 0x06000553 RID: 1363 RVA: 0x0001FAAC File Offset: 0x0001DCAC
	private void HideHuman()
	{
		this.refs.mainRenderer.gameObject.SetActive(false);
		this.refs.sashRenderer.gameObject.SetActive(false);
		this.refs.shorts.gameObject.SetActive(false);
		this.refs.skirt.gameObject.SetActive(false);
	}

	// Token: 0x06000554 RID: 1364 RVA: 0x0001FB14 File Offset: 0x0001DD14
	public void HideAllRenderers()
	{
		this._allRenderersHidden = true;
		for (int i = 0; i < this.refs.AllRenderers.Length; i++)
		{
			this.refs.AllRenderers[i].enabled = false;
		}
		this.refs.hatTransform.gameObject.SetActive(false);
	}

	// Token: 0x06000555 RID: 1365 RVA: 0x0001FB6C File Offset: 0x0001DD6C
	public void ShowAllRenderers()
	{
		this._allRenderersHidden = false;
		for (int i = 0; i < this.refs.AllRenderers.Length; i++)
		{
			this.refs.AllRenderers[i].enabled = true;
		}
		this.refs.hatTransform.gameObject.SetActive(true);
	}

	// Token: 0x06000556 RID: 1366 RVA: 0x0001FBC4 File Offset: 0x0001DDC4
	public void PulseStatus(Color c, float intensity = 1f)
	{
		for (int i = 0; i < this.refs.PlayerRenderers.Length; i++)
		{
			for (int j = 0; j < this.refs.PlayerRenderers[i].materials.Length; j++)
			{
				this.refs.PlayerRenderers[i].materials[j].SetColor(CharacterCustomization.StatusColor, c);
				this.refs.PlayerRenderers[i].materials[j].SetFloat(CharacterCustomization.StatusGlow, intensity);
				this.refs.PlayerRenderers[i].materials[j].DOFloat(0f, CharacterCustomization.StatusGlow, 0.5f);
			}
		}
	}

	// Token: 0x06000557 RID: 1367 RVA: 0x0001FC78 File Offset: 0x0001DE78
	[PunRPC]
	public void SetCharacterIdle_RPC(int index)
	{
		this.PlayerAnimator.SetFloat(CharacterCustomization.Idle, (float)index);
		Debug.Log(string.Format("Setting Character Idle: {0}", index));
	}

	// Token: 0x06000558 RID: 1368 RVA: 0x0001FCA1 File Offset: 0x0001DEA1
	private static CharacterCustomizationData GetCustomizationData(Photon.Realtime.Player player)
	{
		return GameHandler.GetService<PersistentPlayerDataService>().GetPlayerData(player).customizationData;
	}

	// Token: 0x06000559 RID: 1369 RVA: 0x0001FCB4 File Offset: 0x0001DEB4
	private static void SetCustomizationData(CharacterCustomizationData customizationData, Photon.Realtime.Player player)
	{
		PersistentPlayerDataService service = GameHandler.GetService<PersistentPlayerDataService>();
		PersistentPlayerData playerData = service.GetPlayerData(player);
		playerData.customizationData = customizationData;
		service.SetPlayerData(player, playerData);
	}

	// Token: 0x0600055A RID: 1370 RVA: 0x0001FCDC File Offset: 0x0001DEDC
	public void Update()
	{
		if (this._character.data.passedOut && !this.isPassedOut)
		{
			this.isPassedOut = true;
			if (this.view.IsMine)
			{
				this.view.RPC("CharacterPassedOut", RpcTarget.AllBuffered, Array.Empty<object>());
			}
		}
		if (this._character.data.dead && !this.isDead)
		{
			this.isDead = true;
			if (this.view.IsMine)
			{
				this.view.RPC("CharacterDied", RpcTarget.AllBuffered, Array.Empty<object>());
			}
		}
	}

	// Token: 0x0600055B RID: 1371 RVA: 0x0001FD71 File Offset: 0x0001DF71
	public void OnRevive()
	{
		if (this.view.IsMine)
		{
			this.view.RPC("OnRevive_RPC", RpcTarget.AllBuffered, Array.Empty<object>());
		}
	}

	// Token: 0x0600055C RID: 1372 RVA: 0x0001FD98 File Offset: 0x0001DF98
	[PunRPC]
	public void OnRevive_RPC()
	{
		Debug.Log("test dead");
		this.isDead = false;
		this.isPassedOut = false;
		if (this.forcePresetEyes)
		{
			return;
		}
		for (int i = 0; i < this.refs.EyeRenderers.Length; i++)
		{
			this.refs.EyeRenderers[i].material.SetTexture(CharacterCustomization.MainTex, this.CurrentEyeTexture);
			this.refs.EyeRenderers[i].material.SetInt(CharacterCustomization.Spin, 0);
		}
	}

	// Token: 0x04000580 RID: 1408
	private Character _character;

	// Token: 0x04000581 RID: 1409
	public CustomizationRefs refs;

	// Token: 0x04000582 RID: 1410
	public static readonly int MainTex = Shader.PropertyToID("_MainTex");

	// Token: 0x04000583 RID: 1411
	private static readonly int SkinColor = Shader.PropertyToID("_SkinColor");

	// Token: 0x04000584 RID: 1412
	private static readonly int Idle = Animator.StringToHash("Idle");

	// Token: 0x04000585 RID: 1413
	private static readonly int Spin = Shader.PropertyToID("_Spin");

	// Token: 0x04000586 RID: 1414
	private static readonly int VertexGhost = Shader.PropertyToID("_VertexGhost");

	// Token: 0x04000587 RID: 1415
	private static readonly int StatusColor = Shader.PropertyToID("_StatusColor");

	// Token: 0x04000588 RID: 1416
	private static readonly int StatusGlow = Shader.PropertyToID("_StatusGlow");

	// Token: 0x04000589 RID: 1417
	private static readonly int Opacity = Shader.PropertyToID("_Opacity");

	// Token: 0x0400058A RID: 1418
	public bool useDebugColor;

	// Token: 0x0400058B RID: 1419
	public int debugColorIndex;

	// Token: 0x0400058C RID: 1420
	public int maxIdles;

	// Token: 0x0400058D RID: 1421
	public Animator PlayerAnimator;

	// Token: 0x0400058E RID: 1422
	private PhotonView view;

	// Token: 0x0400058F RID: 1423
	public ParticleSystem chickenParticle;

	// Token: 0x04000590 RID: 1424
	public Texture passedOutEyes;

	// Token: 0x04000591 RID: 1425
	public bool ignorePlayerCosmetics;

	// Token: 0x04000592 RID: 1426
	private Texture CurrentEyeTexture;

	// Token: 0x04000593 RID: 1427
	[FormerlySerializedAs("diedTexture")]
	public Texture deadEyes;

	// Token: 0x04000594 RID: 1428
	public bool forcePresetEyes;

	// Token: 0x04000595 RID: 1429
	[FormerlySerializedAs("isDead")]
	public bool isPassedOut;

	// Token: 0x04000596 RID: 1430
	public bool isDead;

	// Token: 0x04000597 RID: 1431
	public Photon.Realtime.Player overridePhotonPlayer;

	// Token: 0x04000598 RID: 1432
	public bool isCannibalizable;

	// Token: 0x04000599 RID: 1433
	private Tweener chickenTweener;

	// Token: 0x0400059A RID: 1434
	private bool _allRenderersHidden;
}

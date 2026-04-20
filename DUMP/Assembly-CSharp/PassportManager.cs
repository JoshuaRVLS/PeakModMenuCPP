using System;
using System.Collections;
using DG.Tweening;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zorro.Core;
using Zorro.Core.CLI;

// Token: 0x02000157 RID: 343
public class PassportManager : MenuWindow
{
	// Token: 0x170000D4 RID: 212
	// (get) Token: 0x06000B4D RID: 2893 RVA: 0x0003CB31 File Offset: 0x0003AD31
	public override bool openOnStart
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000D5 RID: 213
	// (get) Token: 0x06000B4E RID: 2894 RVA: 0x0003CB34 File Offset: 0x0003AD34
	public override bool selectOnOpen
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170000D6 RID: 214
	// (get) Token: 0x06000B4F RID: 2895 RVA: 0x0003CB37 File Offset: 0x0003AD37
	public override Selectable objectToSelectOnOpen
	{
		get
		{
			return this.buttons[0].button;
		}
	}

	// Token: 0x170000D7 RID: 215
	// (get) Token: 0x06000B50 RID: 2896 RVA: 0x0003CB46 File Offset: 0x0003AD46
	public override bool closeOnPause
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170000D8 RID: 216
	// (get) Token: 0x06000B51 RID: 2897 RVA: 0x0003CB49 File Offset: 0x0003AD49
	public override bool closeOnUICancel
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170000D9 RID: 217
	// (get) Token: 0x06000B52 RID: 2898 RVA: 0x0003CB4C File Offset: 0x0003AD4C
	public override bool autoHideOnClose
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000B53 RID: 2899 RVA: 0x0003CB4F File Offset: 0x0003AD4F
	public void Awake()
	{
		PassportManager.instance = this;
		this.uiObject.SetActive(false);
	}

	// Token: 0x06000B54 RID: 2900 RVA: 0x0003CB63 File Offset: 0x0003AD63
	[ConsoleCommand]
	public static void TestAllCosmetics()
	{
		if (PassportManager.instance != null)
		{
			PassportManager.instance.testUnlockAll = true;
		}
	}

	// Token: 0x06000B55 RID: 2901 RVA: 0x0003CB80 File Offset: 0x0003AD80
	public static string GeneratePassportNumber(string name)
	{
		string arg = PassportManager.GenerateCountryCode(name);
		int num = PassportManager.GenerateNumericCode(name, 9);
		return string.Format("{0}{1:D7}", arg, num);
	}

	// Token: 0x06000B56 RID: 2902 RVA: 0x0003CBB0 File Offset: 0x0003ADB0
	private static string GenerateCountryCode(string name)
	{
		name = name.ToUpper().Replace(" ", "");
		if (name.Length < 2)
		{
			name += "XX";
		}
		return string.Format("{0}", name[0]);
	}

	// Token: 0x06000B57 RID: 2903 RVA: 0x0003CC00 File Offset: 0x0003AE00
	private static int GenerateNumericCode(string input, int length)
	{
		return Mathf.Abs(input.GetHashCode()) % (int)Mathf.Pow(10f, (float)length);
	}

	// Token: 0x06000B58 RID: 2904 RVA: 0x0003CC1B File Offset: 0x0003AE1B
	public void ToggleOpen()
	{
		if (!this.closing)
		{
			if (!base.isOpen)
			{
				this.Open();
				this.uiObject.SetActive(true);
				this.OpenTab(this.activeType);
				return;
			}
			base.Close();
		}
	}

	// Token: 0x06000B59 RID: 2905 RVA: 0x0003CC54 File Offset: 0x0003AE54
	protected override void Initialize()
	{
		string characterName = Character.localCharacter.characterName;
		PassportManager.passportNumberString = PassportManager.GeneratePassportNumber(characterName);
		this.nameText.text = characterName;
		this.passportNumberText.text = PassportManager.passportNumberString;
	}

	// Token: 0x06000B5A RID: 2906 RVA: 0x0003CC93 File Offset: 0x0003AE93
	protected override void OnClose()
	{
		base.StartCoroutine(this.CloseRoutine());
	}

	// Token: 0x06000B5B RID: 2907 RVA: 0x0003CCA2 File Offset: 0x0003AEA2
	private IEnumerator CloseRoutine()
	{
		this.closing = true;
		this.anim.Play("Close");
		this.CameraIn();
		yield return new WaitForSeconds(0.5f);
		this.uiObject.SetActive(false);
		this.closing = false;
		yield break;
	}

	// Token: 0x06000B5C RID: 2908 RVA: 0x0003CCB4 File Offset: 0x0003AEB4
	public void OpenTab(Customization.Type type)
	{
		this.activeType = type;
		int num = 0;
		for (int i = 0; i < this.tabs.Length; i++)
		{
			if (this.tabs[i].type == type)
			{
				num = i;
			}
			else
			{
				this.tabs[i].Close();
			}
		}
		this.tabs[num].Open();
		if (num == 4 || num == 6)
		{
			this.CameraOut();
		}
		else
		{
			this.CameraIn();
		}
		this.SetButtons(true);
		this.dummy.UpdateDummy();
	}

	// Token: 0x06000B5D RID: 2909 RVA: 0x0003CD34 File Offset: 0x0003AF34
	private void CameraIn()
	{
		this.dummyCamera.DOOrthoSize(0.6f, 0.2f);
		this.dummyCamera.transform.DOLocalMove(new Vector3(0f, 1.65f, 1f), 0.2f, false);
		if (this.camIn)
		{
			this.camIn.Play(default(Vector3));
		}
	}

	// Token: 0x06000B5E RID: 2910 RVA: 0x0003CDA4 File Offset: 0x0003AFA4
	private void CameraOut()
	{
		this.dummyCamera.DOOrthoSize(1.3f, 0.2f);
		this.dummyCamera.transform.DOLocalMove(new Vector3(0f, 1.05f, 1f), 0.2f, false);
		if (this.camOut)
		{
			this.camOut.Play(default(Vector3));
		}
	}

	// Token: 0x06000B5F RID: 2911 RVA: 0x0003CE14 File Offset: 0x0003B014
	public void SetButtons(bool fromTabChange = false)
	{
		int num = this.SetActiveButton(fromTabChange);
		if (fromTabChange)
		{
			this.page = num / this.buttonsPerPage;
		}
		CustomizationOption[] list = Singleton<Customization>.Instance.GetList(this.activeType);
		this.activeTabOptionCount = list.Length;
		this.pageNumNext.text = (this.page + 2).ToString();
		this.pageNumPrev.text = this.page.ToString();
		this.buttonNext.SetActive(this.activeTabOptionCount > (this.page + 1) * this.buttonsPerPage);
		this.buttonPrev.SetActive(this.activeTabOptionCount > this.buttonsPerPage && this.page > 0);
		for (int i = 0; i < this.buttonsPerPage; i++)
		{
			int num2 = i;
			int num3 = i + this.page * this.buttonsPerPage;
			if (num3 < list.Length)
			{
				CustomizationOption option = list[num3];
				this.buttons[num2].SetButton(option, num3);
			}
			else
			{
				this.buttons[num2].SetButton(null, -1);
			}
		}
	}

	// Token: 0x06000B60 RID: 2912 RVA: 0x0003CF24 File Offset: 0x0003B124
	private int SetActiveButton(bool fromTabChange = false)
	{
		PersistentPlayerData playerData = GameHandler.GetService<PersistentPlayerDataService>().GetPlayerData(PhotonNetwork.LocalPlayer);
		int num = playerData.customizationData.currentSkin;
		if (this.activeType == Customization.Type.Accessory)
		{
			num = playerData.customizationData.currentAccessory;
		}
		else if (this.activeType == Customization.Type.Eyes)
		{
			num = playerData.customizationData.currentEyes;
		}
		else if (this.activeType == Customization.Type.Mouth)
		{
			num = playerData.customizationData.currentMouth;
		}
		else if (this.activeType == Customization.Type.Fit)
		{
			num = playerData.customizationData.currentOutfit;
		}
		else if (this.activeType == Customization.Type.Hat)
		{
			num = playerData.customizationData.currentHat;
		}
		else if (this.activeType == Customization.Type.Sash)
		{
			num = playerData.customizationData.currentSash;
		}
		int num2 = this.page;
		if (fromTabChange)
		{
			num2 = num / this.buttonsPerPage;
		}
		for (int i = 0; i < this.buttons.Length; i++)
		{
			this.buttons[i].border.color = ((num == i + num2 * this.buttonsPerPage) ? this.activeBorderColor : this.inactiveBorderColor);
		}
		return num;
	}

	// Token: 0x06000B61 RID: 2913 RVA: 0x0003D030 File Offset: 0x0003B230
	public void SetOption(CustomizationOption option, int index)
	{
		if (option.type == Customization.Type.Skin)
		{
			CharacterCustomization.SetCharacterSkinColor(index);
		}
		else if (option.type == Customization.Type.Eyes)
		{
			CharacterCustomization.SetCharacterEyes(index);
		}
		else if (option.type == Customization.Type.Mouth)
		{
			CharacterCustomization.SetCharacterMouth(index);
		}
		else if (option.type == Customization.Type.Accessory)
		{
			CharacterCustomization.SetCharacterAccessory(index);
		}
		else if (option.type == Customization.Type.Fit)
		{
			CharacterCustomization.SetCharacterOutfit(index);
		}
		else if (option.type == Customization.Type.Hat)
		{
			CharacterCustomization.SetCharacterHat(index);
		}
		else if (option.type == Customization.Type.Sash)
		{
			CharacterCustomization.SetCharacterSash(index);
		}
		this.SetActiveButton(false);
		this.dummy.UpdateDummy();
	}

	// Token: 0x06000B62 RID: 2914 RVA: 0x0003D0CC File Offset: 0x0003B2CC
	public void PageNext()
	{
		this.page++;
		if (this.page > (this.activeTabOptionCount - 1) / this.buttonsPerPage)
		{
			this.page = (this.activeTabOptionCount - 1) / this.buttonsPerPage;
		}
		this.SetButtons(false);
	}

	// Token: 0x06000B63 RID: 2915 RVA: 0x0003D11A File Offset: 0x0003B31A
	public void PagePrev()
	{
		this.page--;
		if (this.page < 0)
		{
			this.page = 0;
		}
		this.SetButtons(false);
	}

	// Token: 0x04000A60 RID: 2656
	public static PassportManager instance;

	// Token: 0x04000A61 RID: 2657
	public Animator anim;

	// Token: 0x04000A62 RID: 2658
	public GameObject uiObject;

	// Token: 0x04000A63 RID: 2659
	public PassportTab[] tabs;

	// Token: 0x04000A64 RID: 2660
	public Customization.Type activeType;

	// Token: 0x04000A65 RID: 2661
	public PassportButton[] buttons;

	// Token: 0x04000A66 RID: 2662
	public PlayerCustomizationDummy dummy;

	// Token: 0x04000A67 RID: 2663
	public Camera dummyCamera;

	// Token: 0x04000A68 RID: 2664
	public TextMeshProUGUI nameText;

	// Token: 0x04000A69 RID: 2665
	public TextMeshProUGUI passportNumberText;

	// Token: 0x04000A6A RID: 2666
	private static string passportNumberString;

	// Token: 0x04000A6B RID: 2667
	public Color inactiveBorderColor;

	// Token: 0x04000A6C RID: 2668
	public Color activeBorderColor;

	// Token: 0x04000A6D RID: 2669
	public bool testUnlockAll;

	// Token: 0x04000A6E RID: 2670
	public SFX_Instance camIn;

	// Token: 0x04000A6F RID: 2671
	public SFX_Instance camOut;

	// Token: 0x04000A70 RID: 2672
	public GameObject buttonNext;

	// Token: 0x04000A71 RID: 2673
	public GameObject buttonPrev;

	// Token: 0x04000A72 RID: 2674
	public TextMeshProUGUI pageNumNext;

	// Token: 0x04000A73 RID: 2675
	public TextMeshProUGUI pageNumPrev;

	// Token: 0x04000A74 RID: 2676
	private bool closing;

	// Token: 0x04000A75 RID: 2677
	private int activeTabOptionCount;

	// Token: 0x04000A76 RID: 2678
	private int page;

	// Token: 0x04000A77 RID: 2679
	private int buttonsPerPage = 28;
}

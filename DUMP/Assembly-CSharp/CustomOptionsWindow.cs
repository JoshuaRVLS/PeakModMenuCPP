using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020001D9 RID: 473
public class CustomOptionsWindow : MenuWindow
{
	// Token: 0x1700010F RID: 271
	// (get) Token: 0x06000F01 RID: 3841 RVA: 0x0004AAA7 File Offset: 0x00048CA7
	public override bool selectOnOpen
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000110 RID: 272
	// (get) Token: 0x06000F02 RID: 3842 RVA: 0x0004AAAA File Offset: 0x00048CAA
	public override bool closeOnPause
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000111 RID: 273
	// (get) Token: 0x06000F03 RID: 3843 RVA: 0x0004AAAD File Offset: 0x00048CAD
	public override bool closeOnUICancel
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000112 RID: 274
	// (get) Token: 0x06000F04 RID: 3844 RVA: 0x0004AAB0 File Offset: 0x00048CB0
	public override Selectable objectToSelectOnOpen
	{
		get
		{
			return this.miniRunButton;
		}
	}

	// Token: 0x06000F05 RID: 3845 RVA: 0x0004AAB8 File Offset: 0x00048CB8
	protected override void Initialize()
	{
		this.closeButtonCustom.onClick.AddListener(new UnityAction(this.CloseCustomWindow));
		this.restoreDefaultsButton.onClick.AddListener(new UnityAction(this.RestoreDefaults));
		this.miniRunButton.onClick.AddListener(new UnityAction(this.ToggleMiniRunOptions));
		this.SpawnItemToggles();
	}

	// Token: 0x06000F06 RID: 3846 RVA: 0x0004AB1F File Offset: 0x00048D1F
	protected override void OnOpen()
	{
		base.OnOpen();
		this.ToggleMiniRunOptions();
	}

	// Token: 0x06000F07 RID: 3847 RVA: 0x0004AB2D File Offset: 0x00048D2D
	protected override void OnClose()
	{
		base.OnClose();
		this.OpenBoardingPass();
	}

	// Token: 0x06000F08 RID: 3848 RVA: 0x0004AB3C File Offset: 0x00048D3C
	private void SpawnItemToggles()
	{
		List<Item> modifiableItemPrefabs = RunSettings.ModifiableItemPrefabs;
		List<Item> list = new List<Item>();
		foreach (Item item in modifiableItemPrefabs)
		{
			if (item.itemTags.HasFlag(Item.ItemTags.Mystical))
			{
				list.Add(item);
			}
			else
			{
				Object.Instantiate<CustomOptionItemToggle>(this.customOptionItemTogglePrefab, this.spawnPoolList).Init(item);
			}
		}
		foreach (Item item2 in list)
		{
			Object.Instantiate<CustomOptionItemToggle>(this.customOptionItemTogglePrefab, this.mysticalSpawnPoolList).Init(item2);
		}
	}

	// Token: 0x06000F09 RID: 3849 RVA: 0x0004AC14 File Offset: 0x00048E14
	private void ToggleMiniRunOptions()
	{
		this.miniRunOptions.SetActive(RunSettings.GetValue(RunSettings.SETTINGTYPE.MiniRun, true) != RunSettings.GetDefaultValue(RunSettings.SETTINGTYPE.MiniRun));
	}

	// Token: 0x06000F0A RID: 3850 RVA: 0x0004AC3B File Offset: 0x00048E3B
	private void CloseCustomWindow()
	{
		base.Close();
	}

	// Token: 0x06000F0B RID: 3851 RVA: 0x0004AC43 File Offset: 0x00048E43
	private void OpenBoardingPass()
	{
		this.boardingPass.openingFromCustomWindow = true;
		this.boardingPass.Open();
	}

	// Token: 0x06000F0C RID: 3852 RVA: 0x0004AC5C File Offset: 0x00048E5C
	private void RestoreDefaults()
	{
		RunSettings.ResetAll();
		CustomOptionBase[] componentsInChildren = base.GetComponentsInChildren<CustomOptionBase>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].RestoreDefault();
		}
		this.ToggleMiniRunOptions();
	}

	// Token: 0x04000CB7 RID: 3255
	public Button closeButtonCustom;

	// Token: 0x04000CB8 RID: 3256
	public BoardingPass boardingPass;

	// Token: 0x04000CB9 RID: 3257
	public Transform spawnPoolList;

	// Token: 0x04000CBA RID: 3258
	public Transform mysticalSpawnPoolList;

	// Token: 0x04000CBB RID: 3259
	public CustomOptionItemToggle customOptionItemTogglePrefab;

	// Token: 0x04000CBC RID: 3260
	public Button restoreDefaultsButton;

	// Token: 0x04000CBD RID: 3261
	public Button miniRunButton;

	// Token: 0x04000CBE RID: 3262
	public GameObject miniRunOptions;
}

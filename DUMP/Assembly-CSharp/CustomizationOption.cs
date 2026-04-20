using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000091 RID: 145
[CreateAssetMenu(fileName = "CustomizationOption", menuName = "Scriptable Objects/CustomizationOption")]
public class CustomizationOption : ScriptableObject
{
	// Token: 0x17000078 RID: 120
	// (get) Token: 0x060005F0 RID: 1520 RVA: 0x000222DF File Offset: 0x000204DF
	public bool requiresSteamStat
	{
		get
		{
			return this.requiredSteamStat > STEAMSTATTYPE.NONE;
		}
	}

	// Token: 0x17000079 RID: 121
	// (get) Token: 0x060005F1 RID: 1521 RVA: 0x000222EA File Offset: 0x000204EA
	private bool IsAccessory
	{
		get
		{
			return this.type == Customization.Type.Accessory;
		}
	}

	// Token: 0x1700007A RID: 122
	// (get) Token: 0x060005F2 RID: 1522 RVA: 0x000222F6 File Offset: 0x000204F6
	private bool IsSkin
	{
		get
		{
			return this.type == Customization.Type.Skin;
		}
	}

	// Token: 0x1700007B RID: 123
	// (get) Token: 0x060005F3 RID: 1523 RVA: 0x00022301 File Offset: 0x00020501
	private bool IsFit
	{
		get
		{
			return this.type == Customization.Type.Fit;
		}
	}

	// Token: 0x1700007C RID: 124
	// (get) Token: 0x060005F4 RID: 1524 RVA: 0x0002230D File Offset: 0x0002050D
	public Material fitPantsMaterial
	{
		get
		{
			if (this.fitMaterialOverridePants != null)
			{
				return this.fitMaterialOverridePants;
			}
			return this.fitMaterial;
		}
	}

	// Token: 0x1700007D RID: 125
	// (get) Token: 0x060005F5 RID: 1525 RVA: 0x0002232A File Offset: 0x0002052A
	public Material fitHatMaterial
	{
		get
		{
			if (this.fitMaterialOverrideHat != null)
			{
				return this.fitMaterialOverrideHat;
			}
			return this.fitMaterial;
		}
	}

	// Token: 0x1700007E RID: 126
	// (get) Token: 0x060005F6 RID: 1526 RVA: 0x00022348 File Offset: 0x00020548
	public bool IsLocked
	{
		get
		{
			if (this.requiresAscent)
			{
				return Singleton<AchievementManager>.Instance.GetMaxAscent() < this.requiredAscent;
			}
			if (this.requiredAchievement == ACHIEVEMENTTYPE.NONE && this.customRequirement == CustomizationOption.CUSTOMREQUIREMENT.None)
			{
				return false;
			}
			if (this.customRequirement == CustomizationOption.CUSTOMREQUIREMENT.Goat)
			{
				return Singleton<AchievementManager>.Instance.GetMaxAscent() < 8;
			}
			if (this.customRequirement == CustomizationOption.CUSTOMREQUIREMENT.Crown)
			{
				return !Singleton<AchievementManager>.Instance.AllBaseAchievementsUnlocked();
			}
			return !Singleton<AchievementManager>.Instance.IsAchievementUnlocked(this.requiredAchievement);
		}
	}

	// Token: 0x040005E4 RID: 1508
	public Customization.Type type;

	// Token: 0x040005E5 RID: 1509
	public Texture texture;

	// Token: 0x040005E6 RID: 1510
	public ACHIEVEMENTTYPE requiredAchievement;

	// Token: 0x040005E7 RID: 1511
	public STEAMSTATTYPE requiredSteamStat;

	// Token: 0x040005E8 RID: 1512
	public int requiredSteamStatValue = 1;

	// Token: 0x040005E9 RID: 1513
	public bool requiresAscent;

	// Token: 0x040005EA RID: 1514
	public int requiredAscent;

	// Token: 0x040005EB RID: 1515
	public CustomizationOption.CUSTOMREQUIREMENT customRequirement;

	// Token: 0x040005EC RID: 1516
	public bool isBlank;

	// Token: 0x040005ED RID: 1517
	public bool testLocked;

	// Token: 0x040005EE RID: 1518
	public bool drawUnderEye;

	// Token: 0x040005EF RID: 1519
	public bool isThirdEye;

	// Token: 0x040005F0 RID: 1520
	[ColorUsage(true, false)]
	public Color color;

	// Token: 0x040005F1 RID: 1521
	public Mesh fitMesh;

	// Token: 0x040005F2 RID: 1522
	public Material fitMaterial;

	// Token: 0x040005F3 RID: 1523
	public Material fitMaterialShoes;

	// Token: 0x040005F4 RID: 1524
	public Material fitMaterialOverridePants;

	// Token: 0x040005F5 RID: 1525
	public Material fitMaterialOverrideHat;

	// Token: 0x040005F6 RID: 1526
	public bool isSkirt;

	// Token: 0x040005F7 RID: 1527
	public bool noPants;

	// Token: 0x040005F8 RID: 1528
	public bool overrideHat;

	// Token: 0x040005F9 RID: 1529
	public int overrideHatIndex;

	// Token: 0x02000440 RID: 1088
	public enum CUSTOMREQUIREMENT
	{
		// Token: 0x040018A3 RID: 6307
		None,
		// Token: 0x040018A4 RID: 6308
		Goat,
		// Token: 0x040018A5 RID: 6309
		Crown
	}
}

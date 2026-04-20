using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000090 RID: 144
public class Customization : Singleton<Customization>
{
	// Token: 0x060005EC RID: 1516 RVA: 0x00022144 File Offset: 0x00020344
	public List<CustomizationOption> TryGetUnlockedCosmetics(BadgeData badge)
	{
		bool flag = false;
		List<CustomizationOption> list = new List<CustomizationOption>();
		foreach (object obj in Enum.GetValues(typeof(Customization.Type)))
		{
			Customization.Type type = (Customization.Type)obj;
			foreach (CustomizationOption customizationOption in this.GetList(type))
			{
				if (!(customizationOption == null) && customizationOption.requiredAchievement != ACHIEVEMENTTYPE.NONE && customizationOption.requiredAchievement == badge.linkedAchievement)
				{
					if (customizationOption.type == Customization.Type.Fit)
					{
						if (flag)
						{
							goto IL_7A;
						}
						flag = true;
					}
					list.Add(customizationOption);
				}
				IL_7A:;
			}
		}
		return list;
	}

	// Token: 0x060005ED RID: 1517 RVA: 0x00022208 File Offset: 0x00020408
	public CustomizationOption[] GetList(Customization.Type type)
	{
		if (type <= Customization.Type.Eyes)
		{
			if (type == Customization.Type.Skin)
			{
				return this.skins;
			}
			if (type == Customization.Type.Accessory)
			{
				return this.accessories;
			}
			if (type == Customization.Type.Eyes)
			{
				return this.eyes;
			}
		}
		else if (type <= Customization.Type.Fit)
		{
			if (type == Customization.Type.Mouth)
			{
				return this.mouths;
			}
			if (type == Customization.Type.Fit)
			{
				return this.fits;
			}
		}
		else
		{
			if (type == Customization.Type.Hat)
			{
				return this.hats;
			}
			if (type == Customization.Type.Sash)
			{
				return this.sashes;
			}
		}
		return this.skins;
	}

	// Token: 0x060005EE RID: 1518 RVA: 0x00022280 File Offset: 0x00020480
	public int GetRandomUnlockedIndex(Customization.Type type)
	{
		CustomizationOption[] list = this.GetList(type);
		List<int> list2 = new List<int>();
		for (int i = 0; i < list.Length; i++)
		{
			if (!list[i].IsLocked)
			{
				list2.Add(i);
			}
		}
		if (list2.Count <= 0)
		{
			return 0;
		}
		return list2[Random.Range(0, list2.Count)];
	}

	// Token: 0x040005DB RID: 1499
	public CustomizationOption[] skins;

	// Token: 0x040005DC RID: 1500
	public CustomizationOption[] accessories;

	// Token: 0x040005DD RID: 1501
	public CustomizationOption[] eyes;

	// Token: 0x040005DE RID: 1502
	public CustomizationOption[] mouths;

	// Token: 0x040005DF RID: 1503
	public CustomizationOption[] fits;

	// Token: 0x040005E0 RID: 1504
	public CustomizationOption[] hats;

	// Token: 0x040005E1 RID: 1505
	public CustomizationOption[] sashes;

	// Token: 0x040005E2 RID: 1506
	public CustomizationOption goatHat;

	// Token: 0x040005E3 RID: 1507
	public CustomizationOption crownHat;

	// Token: 0x0200043F RID: 1087
	public enum Type
	{
		// Token: 0x0400189B RID: 6299
		Skin,
		// Token: 0x0400189C RID: 6300
		Accessory = 10,
		// Token: 0x0400189D RID: 6301
		Eyes = 20,
		// Token: 0x0400189E RID: 6302
		Mouth = 30,
		// Token: 0x0400189F RID: 6303
		Fit = 40,
		// Token: 0x040018A0 RID: 6304
		Hat = 50,
		// Token: 0x040018A1 RID: 6305
		Sash = 60
	}
}

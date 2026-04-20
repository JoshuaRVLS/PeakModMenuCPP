using System;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000128 RID: 296
public class MushroomManager : LevelGenStep
{
	// Token: 0x060009B0 RID: 2480 RVA: 0x00033910 File Offset: 0x00031B10
	public void Awake()
	{
		MushroomManager.instance = this;
	}

	// Token: 0x060009B1 RID: 2481 RVA: 0x00033918 File Offset: 0x00031B18
	private void GenerateEffectList()
	{
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		List<int> list3 = new List<int>();
		List<int> list4 = new List<int>();
		List<int> list5 = new List<int>();
		int num = this.minGoodEffects;
		int num2 = this.minBadEffects;
		for (int i = 0; i < 10; i++)
		{
			list.Add(i);
			if (Action_RandomMushroomEffect.GoodEffects.Contains(i))
			{
				list2.Add(i);
			}
			if (Action_RandomMushroomEffect.BadEffects.Contains(i))
			{
				list3.Add(i);
			}
		}
		while (list.Count > 0)
		{
			int item;
			if (num > 0)
			{
				int index = Random.Range(0, list2.Count);
				item = list2[index];
				num--;
			}
			else if (num2 > 0)
			{
				int index = Random.Range(0, list3.Count);
				item = list3[index];
				num2--;
			}
			else
			{
				int index = Random.Range(0, list.Count);
				item = list[index];
			}
			list4.Add(item);
			list5.Add(Random.Range(0, 4));
			list.Remove(item);
			list2.Remove(item);
			list3.Remove(item);
		}
		this.mushroomEffects = list4.ToArray();
		this.mushroomStamAmt = list5.ToArray();
	}

	// Token: 0x060009B2 RID: 2482 RVA: 0x00033A56 File Offset: 0x00031C56
	public override void Execute()
	{
		this.GenerateEffectList();
	}

	// Token: 0x060009B3 RID: 2483 RVA: 0x00033A5E File Offset: 0x00031C5E
	public override void Clear()
	{
	}

	// Token: 0x040008F4 RID: 2292
	public static MushroomManager instance;

	// Token: 0x040008F5 RID: 2293
	public const int MAX_MUSHROOM_EFFECTS = 10;

	// Token: 0x040008F6 RID: 2294
	public int[] mushroomEffects = new int[10];

	// Token: 0x040008F7 RID: 2295
	public int[] mushroomStamAmt = new int[10];

	// Token: 0x040008F8 RID: 2296
	public int minGoodEffects = 1;

	// Token: 0x040008F9 RID: 2297
	public int minBadEffects = 1;
}

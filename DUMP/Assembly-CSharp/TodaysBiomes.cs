using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200035F RID: 863
public class TodaysBiomes : MonoBehaviour
{
	// Token: 0x060016D6 RID: 5846 RVA: 0x00075434 File Offset: 0x00073634
	private void Start()
	{
		this.shoreIcon.SetActive(false);
		this.tropicsIcon.SetActive(false);
		this.alpineIcon.SetActive(false);
		this.mesaIcon.SetActive(false);
		this.kilnIcon.SetActive(false);
		base.StartCoroutine(this.<Start>g__TrySetBiomes|5_0());
	}

	// Token: 0x060016D7 RID: 5847 RVA: 0x0007548C File Offset: 0x0007368C
	private void SetBiomes(string biomes)
	{
		if (biomes.Contains('S'))
		{
			this.shoreIcon.SetActive(true);
		}
		if (biomes.Contains('T'))
		{
			this.tropicsIcon.SetActive(true);
		}
		if (biomes.Contains('A'))
		{
			this.alpineIcon.SetActive(true);
		}
		if (biomes.Contains('M'))
		{
			this.mesaIcon.SetActive(true);
		}
		if (biomes.Contains('K'))
		{
			this.kilnIcon.SetActive(true);
		}
	}

	// Token: 0x060016D9 RID: 5849 RVA: 0x0007550F File Offset: 0x0007370F
	[CompilerGenerated]
	private IEnumerator <Start>g__TrySetBiomes|5_0()
	{
		bool set = false;
		while (!set)
		{
			NextLevelService service = GameHandler.GetService<NextLevelService>();
			if (service.Data.IsSome)
			{
				string biomeID = SingletonAsset<MapBaker>.Instance.GetBiomeID(service.Data.Value.CurrentLevelIndex + NextLevelService.debugLevelIndexOffset);
				this.SetBiomes(biomeID);
				set = true;
			}
			else
			{
				yield return new WaitForSeconds(1f);
			}
		}
		yield break;
	}

	// Token: 0x04001542 RID: 5442
	public GameObject shoreIcon;

	// Token: 0x04001543 RID: 5443
	public GameObject tropicsIcon;

	// Token: 0x04001544 RID: 5444
	public GameObject alpineIcon;

	// Token: 0x04001545 RID: 5445
	public GameObject mesaIcon;

	// Token: 0x04001546 RID: 5446
	public GameObject kilnIcon;
}

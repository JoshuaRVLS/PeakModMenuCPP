using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000066 RID: 102
public class BiomeSelector : MonoBehaviour
{
	// Token: 0x060004E8 RID: 1256 RVA: 0x0001D480 File Offset: 0x0001B680
	public void Select(Biome.BiomeType biome)
	{
		bool flag = false;
		foreach (BiomeSelector.BiomeOption biomeOption in this.Biomes)
		{
			bool flag2 = biomeOption.biomeParent.biomeType == biome;
			biomeOption.biomeParent.gameObject.SetActive(flag2);
			if (flag2)
			{
				Debug.Log("Successfully found and enabled biome: " + biome.ToString());
				flag = true;
			}
		}
		if (!flag)
		{
			Debug.Log(string.Format("Couldn't find biome {0}, selecting at random...", biome));
			this.Select();
		}
	}

	// Token: 0x060004E9 RID: 1257 RVA: 0x0001D52C File Offset: 0x0001B72C
	public void Select()
	{
		BiomeSelector.BiomeOption biomeOption = this.Biomes.SelectRandomWeighted((BiomeSelector.BiomeOption biome) => biome.Weight);
		foreach (BiomeSelector.BiomeOption biomeOption2 in this.Biomes)
		{
			biomeOption2.biomeParent.gameObject.SetActive(false);
		}
		biomeOption.biomeParent.gameObject.SetActive(true);
	}

	// Token: 0x04000548 RID: 1352
	public List<BiomeSelector.BiomeOption> Biomes;

	// Token: 0x02000431 RID: 1073
	[Serializable]
	public class BiomeOption
	{
		// Token: 0x04001878 RID: 6264
		public Biome biomeParent;

		// Token: 0x04001879 RID: 6265
		public float Weight;
	}
}

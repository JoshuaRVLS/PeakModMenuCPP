using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000234 RID: 564
public class ChildMaterialSwapper : MonoBehaviour, IGenConfigStep
{
	// Token: 0x06001167 RID: 4455 RVA: 0x000579FE File Offset: 0x00055BFE
	public void RunStep()
	{
		this.chosenTheme = this.GetRandomTheme();
	}

	// Token: 0x06001168 RID: 4456 RVA: 0x00057A0C File Offset: 0x00055C0C
	private void Start()
	{
		if (this.forceDisable)
		{
			return;
		}
		if (this.chosenTheme != null)
		{
			this.ApplyRandomTheme();
		}
	}

	// Token: 0x06001169 RID: 4457 RVA: 0x00057A28 File Offset: 0x00055C28
	public void ApplyRandomTheme()
	{
		if (this.themes == null || this.themes.Count == 0)
		{
			Debug.LogError("No themes configured.");
			return;
		}
		if (this.chosenTheme == null)
		{
			Debug.LogError("Failed to choose a theme.");
			return;
		}
		this.BuildMaterialSlotLookup();
		foreach (MeshRenderer rend in base.GetComponentsInChildren<MeshRenderer>(true))
		{
			this.ReplaceMaterialsForRenderer(rend, this.chosenTheme);
		}
	}

	// Token: 0x0600116A RID: 4458 RVA: 0x00057A98 File Offset: 0x00055C98
	private void ReplaceMaterialsForRenderer(MeshRenderer rend, ThemeWithRarity chosenTheme)
	{
		if (rend == null || chosenTheme == null || chosenTheme.mats == null)
		{
			return;
		}
		Material[] sharedMaterials = rend.sharedMaterials;
		bool flag = false;
		for (int i = 0; i < sharedMaterials.Length; i++)
		{
			Material material = sharedMaterials[i];
			int num;
			if (!(material == null) && this._materialToSlot != null && this._materialToSlot.TryGetValue(material, out num))
			{
				if (num >= 0 && num < chosenTheme.mats.Length && chosenTheme.mats[num] != null)
				{
					if (sharedMaterials[i] != chosenTheme.mats[num])
					{
						sharedMaterials[i] = chosenTheme.mats[num];
						flag = true;
					}
				}
				else
				{
					Debug.LogWarning(string.Format("Chosen theme \"{0}\" doesn't have a material at slot {1}.", chosenTheme.name, num));
				}
			}
		}
		if (flag)
		{
			rend.sharedMaterials = sharedMaterials;
		}
	}

	// Token: 0x0600116B RID: 4459 RVA: 0x00057B68 File Offset: 0x00055D68
	private void BuildMaterialSlotLookup()
	{
		this._materialToSlot = new Dictionary<Material, int>();
		for (int i = 0; i < this.themes.Count; i++)
		{
			ThemeWithRarity themeWithRarity = this.themes[i];
			if (themeWithRarity != null && themeWithRarity.mats != null)
			{
				for (int j = 0; j < themeWithRarity.mats.Length; j++)
				{
					Material material = themeWithRarity.mats[j];
					if (!(material == null) && !this._materialToSlot.ContainsKey(material))
					{
						this._materialToSlot.Add(material, j);
					}
				}
			}
		}
	}

	// Token: 0x0600116C RID: 4460 RVA: 0x00057BF0 File Offset: 0x00055DF0
	private ThemeWithRarity GetRandomTheme()
	{
		float num = 0f;
		foreach (ThemeWithRarity themeWithRarity in this.themes)
		{
			if (themeWithRarity != null)
			{
				num += Mathf.Max(0f, themeWithRarity.rarity);
			}
		}
		if (num <= 0f)
		{
			return null;
		}
		float num2 = Random.Range(0f, num);
		float num3 = 0f;
		foreach (ThemeWithRarity themeWithRarity2 in this.themes)
		{
			if (themeWithRarity2 != null)
			{
				float num4 = Mathf.Max(0f, themeWithRarity2.rarity);
				num3 += num4;
				if (num2 <= num3)
				{
					return themeWithRarity2;
				}
			}
		}
		return null;
	}

	// Token: 0x04000F38 RID: 3896
	public List<ThemeWithRarity> themes = new List<ThemeWithRarity>();

	// Token: 0x04000F39 RID: 3897
	private Dictionary<Material, int> _materialToSlot;

	// Token: 0x04000F3A RID: 3898
	public bool forceDisable;

	// Token: 0x04000F3B RID: 3899
	public ThemeWithRarity chosenTheme;
}

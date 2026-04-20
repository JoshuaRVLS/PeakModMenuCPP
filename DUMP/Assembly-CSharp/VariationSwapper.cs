using System;
using System.Linq;
using UnityEngine;

// Token: 0x020001F7 RID: 503
public class VariationSwapper : MonoBehaviour, IGenConfigStep
{
	// Token: 0x06000FD9 RID: 4057 RVA: 0x0004DA30 File Offset: 0x0004BC30
	public void EnableRandom()
	{
		float maxInclusive = this.Variations.Sum((VariationSwapper.Variation variation) => variation.chance);
		float num = Random.Range(0f, maxInclusive);
		GameObject parent = this.Variations.First<VariationSwapper.Variation>().parent;
		float num2 = 0f;
		foreach (VariationSwapper.Variation variation2 in this.Variations)
		{
			num2 += variation2.chance;
			if (num < num2)
			{
				Debug.Log(string.Format("Found new: {0}", variation2.parent));
				parent = variation2.parent;
				break;
			}
		}
		if (parent != null)
		{
			VariationSwapper.Variation[] variations = this.Variations;
			for (int i = 0; i < variations.Length; i++)
			{
				variations[i].parent.SetActive(false);
			}
			parent.SetActive(true);
		}
	}

	// Token: 0x06000FDA RID: 4058 RVA: 0x0004DB16 File Offset: 0x0004BD16
	public void RunStep()
	{
		this.EnableRandom();
	}

	// Token: 0x04000D74 RID: 3444
	public VariationSwapper.Variation[] Variations;

	// Token: 0x020004EB RID: 1259
	[Serializable]
	public class Variation
	{
		// Token: 0x04001B9C RID: 7068
		public GameObject parent;

		// Token: 0x04001B9D RID: 7069
		public float chance = 1f;
	}
}

using System;
using UnityEngine;

// Token: 0x02000296 RID: 662
public class LevelGeneration : MonoBehaviour
{
	// Token: 0x06001317 RID: 4887 RVA: 0x00060AC4 File Offset: 0x0005ECC4
	public void Generate()
	{
		Debug.LogWarning("UH OHHHH!!! Doing nothing because we're not in Editor code!!", this);
	}

	// Token: 0x06001318 RID: 4888 RVA: 0x00060AD4 File Offset: 0x0005ECD4
	private void RandomizeBiomeVariants()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			BiomeVariant[] componentsInChildren = base.transform.GetChild(i).GetComponentsInChildren<BiomeVariant>(true);
			BiomeVariant[] array = componentsInChildren;
			for (int j = 0; j < array.Length; j++)
			{
				array[j].gameObject.SetActive(false);
			}
			if (componentsInChildren.Length != 0)
			{
				componentsInChildren[Random.Range(0, componentsInChildren.Length)].gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x06001319 RID: 4889 RVA: 0x00060B42 File Offset: 0x0005ED42
	private void Clear()
	{
		Object.FindFirstObjectByType<LightVolume>().SetSize();
		base.GetComponent<PropGrouper>().ClearAll();
	}

	// Token: 0x04001131 RID: 4401
	public int seed;

	// Token: 0x04001132 RID: 4402
	public bool updateLightmap = true;
}

using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000B0 RID: 176
public class MapGenerator : MonoBehaviour
{
	// Token: 0x060006B4 RID: 1716 RVA: 0x00026A10 File Offset: 0x00024C10
	public void GenerateAll()
	{
		if (this.seed != 0)
		{
			Debug.Log("Set Seed");
			Random.InitState(this.seed);
		}
		for (int i = 0; i < this.stages.Count; i++)
		{
			if (this.stages[i].gameObject.activeInHierarchy)
			{
				this.stages[i].Generate(0);
				Debug.Log(i.ToString() + " " + Random.state.GetHashCode().ToString());
			}
		}
	}

	// Token: 0x060006B5 RID: 1717 RVA: 0x00026AAC File Offset: 0x00024CAC
	public void ClearAll()
	{
		for (int i = 0; i < this.stages.Count; i++)
		{
			if (this.stages[i].gameObject.activeInHierarchy)
			{
				this.stages[i].ClearSpawnedObjects();
			}
		}
	}

	// Token: 0x040006B1 RID: 1713
	public int seed;

	// Token: 0x040006B2 RID: 1714
	public List<MapGenerationStage> stages;
}

using System;
using UnityEngine;

// Token: 0x02000252 RID: 594
public class DesertRockSpawner : LevelGenStep
{
	// Token: 0x060011F3 RID: 4595 RVA: 0x0005A50C File Offset: 0x0005870C
	public override void Clear()
	{
		this.GetRefs();
		for (int i = 0; i < this.enterences.childCount; i++)
		{
			Transform child = this.enterences.GetChild(i);
			for (int j = child.childCount - 1; j >= 0; j--)
			{
				Object.DestroyImmediate(child.GetChild(j).gameObject);
			}
		}
	}

	// Token: 0x060011F4 RID: 4596 RVA: 0x0005A568 File Offset: 0x00058768
	public override void Execute()
	{
		bool flag = Random.value < 0.5f;
		this.Clear();
		int num = Random.Range(0, this.enterences.childCount);
		for (int i = 0; i < this.enterences.childCount; i++)
		{
			Transform child = this.enterences.GetChild(i);
			if (i == num && flag)
			{
				HelperFunctions.InstantiatePrefab(this.enterenceObjects[Random.Range(0, this.enterenceObjects.Length)], child.position, child.rotation, child).transform.localScale = Vector3.one * 2f;
				this.inside.position = new Vector3(child.position.x, this.inside.position.y, child.position.z);
			}
			else
			{
				HelperFunctions.InstantiatePrefab(this.blockerObjects[Random.Range(0, this.blockerObjects.Length)], child.position, child.rotation, child).transform.localScale = Vector3.one * 2f;
			}
		}
	}

	// Token: 0x060011F5 RID: 4597 RVA: 0x0005A683 File Offset: 0x00058883
	private void GetRefs()
	{
		this.enterences = base.transform.Find("Enterences");
		this.inside = base.transform.Find("Inside");
	}

	// Token: 0x04000FFB RID: 4091
	public GameObject[] blockerObjects;

	// Token: 0x04000FFC RID: 4092
	public GameObject[] enterenceObjects;

	// Token: 0x04000FFD RID: 4093
	private Transform enterences;

	// Token: 0x04000FFE RID: 4094
	private Transform inside;
}

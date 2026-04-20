using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

// Token: 0x020000FB RID: 251
public class Action_SpawnGuidebookPage : ItemAction
{
	// Token: 0x060008AF RID: 2223 RVA: 0x0002FDE8 File Offset: 0x0002DFE8
	public override void RunAction()
	{
		if (base.character)
		{
			this.item.StartCoroutine(this.item.ConsumeDelayed(false));
			int index;
			GuidebookSpawnData itemToSpawn = this.PickGuidebookPage(out index);
			base.character.StartCoroutine(this.SpawnPageDelayed(itemToSpawn, index));
		}
	}

	// Token: 0x060008B0 RID: 2224 RVA: 0x0002FE37 File Offset: 0x0002E037
	public IEnumerator SpawnPageDelayed(GuidebookSpawnData itemToSpawn, int index)
	{
		Item itemToGrab = itemToSpawn.GetComponent<Item>();
		Character c = base.character;
		float timeout = 2f;
		while (this != null)
		{
			timeout -= Time.deltaTime;
			if (timeout <= 0f)
			{
				yield break;
			}
			yield return null;
		}
		Singleton<AchievementManager>.Instance.TriggerSeenGuidebookPage(index);
		GameUtils.instance.InstantiateAndGrab(itemToGrab, c, 0);
		yield break;
	}

	// Token: 0x060008B1 RID: 2225 RVA: 0x0002FE54 File Offset: 0x0002E054
	public GuidebookSpawnData PickGuidebookPage(out int indexChosen)
	{
		int nextPage = Singleton<AchievementManager>.Instance.GetNextPage();
		if (nextPage < 8)
		{
			indexChosen = nextPage;
			return this.possiblePages[indexChosen];
		}
		indexChosen = Random.Range(0, this.possiblePages.Count - 1);
		return this.possiblePages[indexChosen];
	}

	// Token: 0x04000844 RID: 2116
	public List<GuidebookSpawnData> possiblePages;
}

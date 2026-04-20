using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000312 RID: 786
public class PSB_ChildSpawners : PostSpawnBehavior
{
	// Token: 0x1700015D RID: 349
	// (get) Token: 0x0600150A RID: 5386 RVA: 0x0006A081 File Offset: 0x00068281
	protected override DeferredStepTiming DefaultTiming
	{
		get
		{
			return DeferredStepTiming.AfterCurrentStep;
		}
	}

	// Token: 0x0600150B RID: 5387 RVA: 0x0006A084 File Offset: 0x00068284
	public override void RunBehavior(IEnumerable<GameObject> spawned)
	{
		int num = 0;
		int num2 = 0;
		string text = string.Empty;
		foreach (GameObject gameObject in spawned)
		{
			num2++;
			if (gameObject == null)
			{
				num++;
			}
			else
			{
				text = gameObject.name;
				LevelGenStep[] componentsInChildren = gameObject.GetComponentsInChildren<LevelGenStep>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].Execute();
				}
			}
		}
		if (num > 0)
		{
			string arg = (text == string.Empty) ? "objects" : text;
			Debug.LogError(string.Format("Found {0} null {1} in our list of {2} total child spawners.", num, arg, num2));
		}
	}
}

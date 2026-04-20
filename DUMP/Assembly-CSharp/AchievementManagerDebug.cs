using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x02000047 RID: 71
public class AchievementManagerDebug : SerializedMonoBehaviour
{
	// Token: 0x0600043A RID: 1082 RVA: 0x0001AD87 File Offset: 0x00018F87
	private void Awake()
	{
		this.achievementManager = base.GetComponent<AchievementManager>();
	}

	// Token: 0x0600043B RID: 1083 RVA: 0x0001AD95 File Offset: 0x00018F95
	private void Update()
	{
		this.runBasedInts = this.achievementManager.runBasedValueData.runBasedInts;
	}

	// Token: 0x040004A7 RID: 1191
	private AchievementManager achievementManager;

	// Token: 0x040004A8 RID: 1192
	[SerializeField]
	public Dictionary<RUNBASEDVALUETYPE, int> runBasedInts = new Dictionary<RUNBASEDVALUETYPE, int>();
}

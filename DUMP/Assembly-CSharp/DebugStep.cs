using System;
using UnityEngine;

// Token: 0x02000250 RID: 592
public class DebugStep : MonoBehaviour
{
	// Token: 0x060011ED RID: 4589 RVA: 0x0005A48B File Offset: 0x0005868B
	private void FixedUpdate()
	{
		if (this.stepType == DebugStep.StepType.FixedUpdate)
		{
			Debug.Break();
		}
	}

	// Token: 0x060011EE RID: 4590 RVA: 0x0005A49B File Offset: 0x0005869B
	private void Update()
	{
		if (this.stepType == DebugStep.StepType.Update)
		{
			Debug.Break();
		}
	}

	// Token: 0x04000FF9 RID: 4089
	public DebugStep.StepType stepType;

	// Token: 0x02000503 RID: 1283
	public enum StepType
	{
		// Token: 0x04001BF5 RID: 7157
		Update,
		// Token: 0x04001BF6 RID: 7158
		FixedUpdate
	}
}

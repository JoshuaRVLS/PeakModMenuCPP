using System;
using System.Collections.Generic;

// Token: 0x0200029A RID: 666
public struct ExecuteDeferredStepList : IDeferredStep
{
	// Token: 0x0600131E RID: 4894 RVA: 0x00060B68 File Offset: 0x0005ED68
	public ExecuteDeferredStepList(List<IDeferredStep> steps)
	{
		this._steps = steps;
	}

	// Token: 0x0600131F RID: 4895 RVA: 0x00060B74 File Offset: 0x0005ED74
	public void DeferredGo()
	{
		foreach (IDeferredStep deferredStep in this._steps)
		{
			deferredStep.DeferredGo();
		}
	}

	// Token: 0x04001138 RID: 4408
	private List<IDeferredStep> _steps;
}

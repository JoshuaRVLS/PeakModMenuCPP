using System;
using UnityEngine;

// Token: 0x0200029C RID: 668
public abstract class LevelGenStep : MonoBehaviour, IMayHaveDeferredStep
{
	// Token: 0x17000140 RID: 320
	// (get) Token: 0x06001321 RID: 4897 RVA: 0x00060BCC File Offset: 0x0005EDCC
	public virtual DeferredStepTiming DeferredTiming
	{
		get
		{
			return DeferredStepTiming.None;
		}
	}

	// Token: 0x06001322 RID: 4898 RVA: 0x00060BCF File Offset: 0x0005EDCF
	public virtual IDeferredStep ConstructDeferred(IMayHaveDeferredStep _)
	{
		return null;
	}

	// Token: 0x06001323 RID: 4899 RVA: 0x00060BD2 File Offset: 0x0005EDD2
	public virtual void DeferredGo()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06001324 RID: 4900
	public abstract void Execute();

	// Token: 0x06001325 RID: 4901
	public abstract void Clear();
}

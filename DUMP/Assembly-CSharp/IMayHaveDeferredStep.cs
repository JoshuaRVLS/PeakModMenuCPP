using System;

// Token: 0x02000298 RID: 664
public interface IMayHaveDeferredStep
{
	// Token: 0x1700013F RID: 319
	// (get) Token: 0x0600131B RID: 4891
	DeferredStepTiming DeferredTiming { get; }

	// Token: 0x0600131C RID: 4892
	IDeferredStep ConstructDeferred(IMayHaveDeferredStep source);
}

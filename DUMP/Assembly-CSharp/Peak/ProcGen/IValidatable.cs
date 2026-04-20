using System;

namespace Peak.ProcGen
{
	// Token: 0x020003EC RID: 1004
	public interface IValidatable
	{
		// Token: 0x06001A91 RID: 6801
		ValidationState DoValidation();

		// Token: 0x170001C0 RID: 448
		// (get) Token: 0x06001A92 RID: 6802
		ValidationState ValidationState { get; }
	}
}

using System;

// Token: 0x02000021 RID: 33
public interface IInteractibleConstant : IInteractible
{
	// Token: 0x0600026A RID: 618
	bool IsConstantlyInteractable(Character interactor);

	// Token: 0x0600026B RID: 619
	float GetInteractTime(Character interactor);

	// Token: 0x0600026C RID: 620
	void Interact_CastFinished(Character interactor);

	// Token: 0x0600026D RID: 621
	void CancelCast(Character interactor);

	// Token: 0x0600026E RID: 622
	void ReleaseInteract(Character interactor);

	// Token: 0x17000030 RID: 48
	// (get) Token: 0x0600026F RID: 623
	bool holdOnFinish { get; }
}

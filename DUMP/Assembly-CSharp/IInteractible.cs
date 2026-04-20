using System;
using UnityEngine;

// Token: 0x02000020 RID: 32
public interface IInteractible
{
	// Token: 0x06000262 RID: 610
	bool IsInteractible(Character interactor);

	// Token: 0x06000263 RID: 611
	void Interact(Character interactor);

	// Token: 0x06000264 RID: 612
	void HoverEnter();

	// Token: 0x06000265 RID: 613
	void HoverExit();

	// Token: 0x06000266 RID: 614
	Vector3 Center();

	// Token: 0x06000267 RID: 615
	Transform GetTransform();

	// Token: 0x06000268 RID: 616
	string GetInteractionText();

	// Token: 0x06000269 RID: 617
	string GetName();
}

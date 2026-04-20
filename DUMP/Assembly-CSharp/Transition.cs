using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000364 RID: 868
public abstract class Transition : MonoBehaviour
{
	// Token: 0x060016F8 RID: 5880
	public abstract IEnumerator TransitionIn(float speed = 1f);

	// Token: 0x060016F9 RID: 5881
	public abstract IEnumerator TransitionOut(float speed = 1f);

	// Token: 0x04001565 RID: 5477
	public TransitionType transitionType;
}

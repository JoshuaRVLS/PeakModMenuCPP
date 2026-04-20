using System;
using UnityEngine;

// Token: 0x02000365 RID: 869
public class Transitions : MonoBehaviour
{
	// Token: 0x060016FB RID: 5883 RVA: 0x0007632C File Offset: 0x0007452C
	private void Awake()
	{
		Transitions.instance = this;
		this.transitions = base.GetComponentsInChildren<Transition>(true);
	}

	// Token: 0x060016FC RID: 5884 RVA: 0x00076344 File Offset: 0x00074544
	public void PlayTransition(TransitionType transitionType, Action action, float transitionInSpeed = 1f, float transitionOutSpeed = 1f)
	{
		Transitions.<>c__DisplayClass3_0 CS$<>8__locals1 = new Transitions.<>c__DisplayClass3_0();
		CS$<>8__locals1.transitionInSpeed = transitionInSpeed;
		CS$<>8__locals1.action = action;
		CS$<>8__locals1.transitionOutSpeed = transitionOutSpeed;
		CS$<>8__locals1.transition = this.GetTransition(transitionType);
		base.StartCoroutine(CS$<>8__locals1.<PlayTransition>g__IPlayTransition|0());
	}

	// Token: 0x060016FD RID: 5885 RVA: 0x00076388 File Offset: 0x00074588
	private Transition GetTransition(TransitionType transitionType)
	{
		for (int i = 0; i < this.transitions.Length; i++)
		{
			if (this.transitions[i].transitionType == transitionType)
			{
				return this.transitions[i];
			}
		}
		return null;
	}

	// Token: 0x04001566 RID: 5478
	private Transition[] transitions;

	// Token: 0x04001567 RID: 5479
	public static Transitions instance;
}

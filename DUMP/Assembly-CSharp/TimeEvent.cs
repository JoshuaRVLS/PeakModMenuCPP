using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200035E RID: 862
public class TimeEvent : MonoBehaviour
{
	// Token: 0x060016D3 RID: 5843 RVA: 0x000753C0 File Offset: 0x000735C0
	private void Update()
	{
		this.counter += Time.deltaTime;
		if (this.counter > this.rate)
		{
			if (!this.repeating)
			{
				base.enabled = false;
			}
			this.timeEvent.Invoke();
			this.counter = 0f;
		}
	}

	// Token: 0x060016D4 RID: 5844 RVA: 0x00075412 File Offset: 0x00073612
	private void OnEnable()
	{
		this.counter = 0f;
	}

	// Token: 0x0400153E RID: 5438
	private float counter;

	// Token: 0x0400153F RID: 5439
	public float rate = 2f;

	// Token: 0x04001540 RID: 5440
	public bool repeating;

	// Token: 0x04001541 RID: 5441
	public UnityEvent timeEvent;
}

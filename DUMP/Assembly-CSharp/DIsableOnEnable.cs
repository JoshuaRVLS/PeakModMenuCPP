using System;
using UnityEngine;

// Token: 0x02000259 RID: 601
public class DIsableOnEnable : MonoBehaviour
{
	// Token: 0x06001203 RID: 4611 RVA: 0x0005A7ED File Offset: 0x000589ED
	private void OnEnable()
	{
		this.objectToDisable.SetActive(false);
	}

	// Token: 0x06001204 RID: 4612 RVA: 0x0005A7FB File Offset: 0x000589FB
	private void OnDisable()
	{
		this.objectToDisable.SetActive(true);
	}

	// Token: 0x04001006 RID: 4102
	public GameObject objectToDisable;
}

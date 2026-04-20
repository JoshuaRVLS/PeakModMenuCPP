using System;
using UnityEngine;

// Token: 0x02000331 RID: 817
public class SetAnimatorBool : MonoBehaviour
{
	// Token: 0x060015C4 RID: 5572 RVA: 0x0006E7CC File Offset: 0x0006C9CC
	private void Update()
	{
		base.GetComponent<Animator>().SetBool(this.param, this.on);
	}

	// Token: 0x040013CF RID: 5071
	public string param = "Enabled";

	// Token: 0x040013D0 RID: 5072
	public bool on;
}

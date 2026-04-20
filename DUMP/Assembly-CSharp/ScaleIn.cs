using System;
using UnityEngine;

// Token: 0x0200032B RID: 811
public class ScaleIn : MonoBehaviour
{
	// Token: 0x06001587 RID: 5511 RVA: 0x0006CF11 File Offset: 0x0006B111
	private void Start()
	{
		this.targetScale = base.transform.localScale.x;
		base.transform.localScale = Vector3.zero;
	}

	// Token: 0x06001588 RID: 5512 RVA: 0x0006CF3C File Offset: 0x0006B13C
	private void Update()
	{
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, new Vector3(this.targetScale, this.targetScale, this.targetScale), Time.deltaTime * 5f);
		if (Mathf.Abs(this.targetScale - base.transform.localScale.x) < 0.05f)
		{
			Object.Destroy(this);
		}
	}

	// Token: 0x040013AA RID: 5034
	private float targetScale = 1f;
}

using System;
using UnityEngine;

// Token: 0x02000277 RID: 631
[DefaultExecutionOrder(-9999)]
[SelectionBase]
public class HandVisual : MonoBehaviour
{
	// Token: 0x0600127B RID: 4731 RVA: 0x0005C978 File Offset: 0x0005AB78
	private void Awake()
	{
		base.transform.GetChild(0).gameObject.SetActive(false);
	}
}

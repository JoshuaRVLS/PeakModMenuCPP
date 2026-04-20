using System;
using UnityEngine;

// Token: 0x020002BE RID: 702
public class NoParent : MonoBehaviour
{
	// Token: 0x060013C9 RID: 5065 RVA: 0x000647EC File Offset: 0x000629EC
	private void Start()
	{
		base.transform.parent = null;
	}
}

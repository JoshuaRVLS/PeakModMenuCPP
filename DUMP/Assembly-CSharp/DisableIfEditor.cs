using System;
using UnityEngine;

// Token: 0x0200009D RID: 157
public class DisableIfEditor : MonoBehaviour
{
	// Token: 0x06000620 RID: 1568 RVA: 0x00022F84 File Offset: 0x00021184
	private void Start()
	{
		if (Application.isEditor)
		{
			base.gameObject.SetActive(false);
		}
	}
}

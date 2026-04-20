using System;
using UnityEngine;

// Token: 0x02000216 RID: 534
public class Billboard : MonoBehaviour
{
	// Token: 0x06001063 RID: 4195 RVA: 0x0005177C File Offset: 0x0004F97C
	private void LateUpdate()
	{
		Vector3 a = MainCamera.instance.transform.position - base.transform.position;
		if (a.sqrMagnitude < 0.001f)
		{
			return;
		}
		base.transform.rotation = Quaternion.LookRotation(-a);
	}
}

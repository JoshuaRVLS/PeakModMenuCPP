using System;
using UnityEngine;

// Token: 0x02000270 RID: 624
[ExecuteInEditMode]
public class FollowSceneCam1 : MonoBehaviour
{
	// Token: 0x06001258 RID: 4696 RVA: 0x0005BE80 File Offset: 0x0005A080
	private void OnDrawGizmosSelected()
	{
		if (Camera.current != null)
		{
			base.transform.position = Camera.current.transform.position;
		}
	}
}

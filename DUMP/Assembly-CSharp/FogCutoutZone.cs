using System;
using UnityEngine;

// Token: 0x0200026C RID: 620
public class FogCutoutZone : MonoBehaviour
{
	// Token: 0x0600124A RID: 4682 RVA: 0x0005B9AC File Offset: 0x00059BAC
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(1f, 1f, 1f, this.amount);
		Gizmos.DrawWireSphere(base.transform.position, this.min);
		Gizmos.DrawWireSphere(base.transform.position, this.max);
		Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
		Gizmos.DrawCube(base.transform.position + Vector3.forward * this.transitionPoint, new Vector3(300f, 9999f, 0.1f));
	}

	// Token: 0x04001056 RID: 4182
	public float min = 10f;

	// Token: 0x04001057 RID: 4183
	public float max = 100f;

	// Token: 0x04001058 RID: 4184
	public float amount = 1f;

	// Token: 0x04001059 RID: 4185
	public float transitionPoint;
}

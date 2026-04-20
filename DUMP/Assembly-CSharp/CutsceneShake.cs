using System;
using UnityEngine;

// Token: 0x0200024C RID: 588
public class CutsceneShake : MonoBehaviour
{
	// Token: 0x060011D1 RID: 4561 RVA: 0x00059638 File Offset: 0x00057838
	private void Update()
	{
		base.transform.position = Vector3.Lerp(base.transform.position, this.follow.position, this.smooth * Time.deltaTime);
		base.transform.Translate(Vector3.right * (Random.Range(-this.shake, this.shake) * Time.deltaTime));
		base.transform.Translate(Vector3.up * (Random.Range(-this.shake, this.shake) * Time.deltaTime));
		base.transform.Translate(Vector3.forward * (Random.Range(-this.shake, this.shake) * Time.deltaTime));
	}

	// Token: 0x04000FC6 RID: 4038
	public Transform follow;

	// Token: 0x04000FC7 RID: 4039
	public float smooth = 0.5f;

	// Token: 0x04000FC8 RID: 4040
	public float shake;
}

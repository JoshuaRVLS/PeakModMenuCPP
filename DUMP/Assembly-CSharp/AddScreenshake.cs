using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000201 RID: 513
public class AddScreenshake : MonoBehaviour
{
	// Token: 0x06001007 RID: 4103 RVA: 0x0004E7DB File Offset: 0x0004C9DB
	private IEnumerator Start()
	{
		while (GamefeelHandler.instance == null)
		{
			yield return null;
		}
		while (GamefeelHandler.instance.setting == null)
		{
			yield return null;
		}
		if (!this.auto)
		{
			yield break;
		}
		this.Shake();
		yield break;
	}

	// Token: 0x06001008 RID: 4104 RVA: 0x0004E7EC File Offset: 0x0004C9EC
	public void Shake()
	{
		if (this.positional)
		{
			GamefeelHandler.instance.AddPerlinShakeProximity(base.transform.position, this.amount, this.duration, this.scale, this.range);
			return;
		}
		GamefeelHandler.instance.AddPerlinShake(this.amount, this.duration, this.scale);
	}

	// Token: 0x04000DA5 RID: 3493
	public float amount = 5f;

	// Token: 0x04000DA6 RID: 3494
	public float duration = 0.3f;

	// Token: 0x04000DA7 RID: 3495
	public float scale = 12f;

	// Token: 0x04000DA8 RID: 3496
	public bool auto;

	// Token: 0x04000DA9 RID: 3497
	public bool positional;

	// Token: 0x04000DAA RID: 3498
	public float range = 15f;
}

using System;
using UnityEngine;

// Token: 0x02000340 RID: 832
[ExecuteAlways]
public class SillyGrabber : MonoBehaviour
{
	// Token: 0x06001627 RID: 5671 RVA: 0x00070A20 File Offset: 0x0006EC20
	private void Update()
	{
		this.grabberArm.localScale = new Vector3(this.distance * this.mult, 1f, this.lengthToWidth.Evaluate(this.distance));
		this.grabberClaw.localScale = new Vector3(this.clawMult / this.distance, 1f, 1f / this.lengthToWidth.Evaluate(this.distance));
		this.grabberClaw2.localScale = new Vector3(this.clawMult / this.distance, 1f, 1f / this.lengthToWidth.Evaluate(this.distance));
		this.grabberClaw.localPosition = new Vector3(-22.3f, 0f, -this.distance * 0.1f);
		this.grabberClaw2.localPosition = new Vector3(-22.3f, 0f, this.distance * 0.1f);
	}

	// Token: 0x04001443 RID: 5187
	public float distance = 1f;

	// Token: 0x04001444 RID: 5188
	public float mult = 1f;

	// Token: 0x04001445 RID: 5189
	public float clawMult = 1f;

	// Token: 0x04001446 RID: 5190
	public AnimationCurve lengthToWidth;

	// Token: 0x04001447 RID: 5191
	public Transform grabberArm;

	// Token: 0x04001448 RID: 5192
	public Transform grabberClaw;

	// Token: 0x04001449 RID: 5193
	public Transform grabberClaw2;
}

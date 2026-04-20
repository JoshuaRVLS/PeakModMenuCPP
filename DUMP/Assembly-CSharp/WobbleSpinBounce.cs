using System;
using UnityEngine;

// Token: 0x020001FB RID: 507
public class WobbleSpinBounce : MonoBehaviour
{
	// Token: 0x06000FE7 RID: 4071 RVA: 0x0004DE44 File Offset: 0x0004C044
	private void Start()
	{
		if (this.target == null)
		{
			this.target = base.transform;
		}
		this.startPos = this.target.position;
		this.startRot = base.transform.eulerAngles;
	}

	// Token: 0x06000FE8 RID: 4072 RVA: 0x0004DE84 File Offset: 0x0004C084
	private void Update()
	{
		this.target.Rotate(this.rotateSpeed);
		if (this.bounceSize != Vector3.zero)
		{
			this.target.transform.position = this.startPos + new Vector3(Mathf.Sin(Time.time * this.bounceSpeed.x) * this.bounceSize.x, Mathf.Sin(Time.time * this.bounceSpeed.y) * this.bounceSize.y, Mathf.Sin(Time.time * this.bounceSpeed.z) * this.bounceSize.z);
		}
		if (this.wobbleAmount != Vector3.zero)
		{
			this.target.transform.eulerAngles = this.startRot + new Vector3(Mathf.Sin(Time.time * this.wobbleSpeed.x) * this.wobbleAmount.x, Mathf.Sin(Time.time * this.wobbleSpeed.y) * this.wobbleAmount.y, Mathf.Sin(Time.time * this.wobbleSpeed.z) * this.wobbleAmount.z);
		}
	}

	// Token: 0x04000D7A RID: 3450
	public Transform target;

	// Token: 0x04000D7B RID: 3451
	[Header("Rotate")]
	public Vector3 rotateSpeed;

	// Token: 0x04000D7C RID: 3452
	public Vector3 wobbleSpeed;

	// Token: 0x04000D7D RID: 3453
	public Vector3 wobbleAmount;

	// Token: 0x04000D7E RID: 3454
	[Header("Position")]
	public Vector3 bounceSize;

	// Token: 0x04000D7F RID: 3455
	public Vector3 bounceSpeed;

	// Token: 0x04000D80 RID: 3456
	private Vector3 startPos;

	// Token: 0x04000D81 RID: 3457
	private Vector3 startRot;
}

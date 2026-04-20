using System;
using UnityEngine;

// Token: 0x02000069 RID: 105
public class BotMoverRagdoll : MonoBehaviour
{
	// Token: 0x0600050B RID: 1291 RVA: 0x0001DF4E File Offset: 0x0001C14E
	private void Awake()
	{
		this.bot = base.GetComponent<Bot>();
		this.rig_g = base.GetComponent<Rigidbody>();
	}

	// Token: 0x0600050C RID: 1292 RVA: 0x0001DF68 File Offset: 0x0001C168
	private void Start()
	{
	}

	// Token: 0x0600050D RID: 1293 RVA: 0x0001DF6C File Offset: 0x0001C16C
	private void FixedUpdate()
	{
		float fixedDeltaTime = Time.fixedDeltaTime;
		this.rig_g.AddForce(base.transform.forward * (this.bot.MovementInput.y * (this.movementSpeed * fixedDeltaTime)), ForceMode.Acceleration);
		Vector3 up = Vector3.up;
		Vector3 lookDirection = this.bot.LookDirection;
		Vector3 b = Vector3.Cross(base.transform.up, up).normalized * Vector3.Angle(base.transform.up, up);
		Vector3 a = Vector3.Cross(base.transform.forward, lookDirection).normalized * Vector3.Angle(base.transform.forward, lookDirection);
		this.rig_g.angularVelocity = FRILerp.PLerp(this.rig_g.angularVelocity, (a + b) * this.rotSpring, this.rotDamp, fixedDeltaTime);
	}

	// Token: 0x04000564 RID: 1380
	private Bot bot;

	// Token: 0x04000565 RID: 1381
	public float movementSpeed;

	// Token: 0x04000566 RID: 1382
	private Rigidbody rig_g;

	// Token: 0x04000567 RID: 1383
	private Vector3 angularVel;

	// Token: 0x04000568 RID: 1384
	public float rotSpring = 15f;

	// Token: 0x04000569 RID: 1385
	public float rotDamp = 35f;
}

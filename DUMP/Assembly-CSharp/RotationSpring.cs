using System;
using UnityEngine;

// Token: 0x02000327 RID: 807
public class RotationSpring : MonoBehaviour
{
	// Token: 0x0600157B RID: 5499 RVA: 0x0006CC98 File Offset: 0x0006AE98
	private void Update()
	{
		Transform parent = base.transform.parent;
		Vector3 forward = parent.forward;
		Vector3 up = parent.up;
		Vector3 a = Vector3.Cross(base.transform.forward, forward).normalized * Vector3.Angle(base.transform.forward, forward);
		a += Vector3.Cross(base.transform.up, up).normalized * Vector3.Angle(base.transform.up, up);
		this.vel = FRILerp.Lerp(this.vel, a * this.spring, this.drag, true);
		base.transform.Rotate(this.vel * Time.deltaTime, Space.World);
	}

	// Token: 0x0600157C RID: 5500 RVA: 0x0006CD63 File Offset: 0x0006AF63
	public void AddForce(Vector3 force)
	{
		this.vel += force;
	}

	// Token: 0x040013A5 RID: 5029
	public float spring;

	// Token: 0x040013A6 RID: 5030
	public float drag;

	// Token: 0x040013A7 RID: 5031
	private Vector3 vel;
}
